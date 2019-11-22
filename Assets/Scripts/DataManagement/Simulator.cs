using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Simulator : MonoBehaviour
{
    public float time;
    //public List<GameObject> prefabs;

    [HideInInspector]
    public List<UnitInfoTag> instances = new List<UnitInfoTag>();
    [HideInInspector]
    public List<Unit> units = new List<Unit>();
    [HideInInspector]
    public int isClient = 0;
    [HideInInspector]
    public List<Command> commands = new List<Command>();
    public bool isCommandProcessingDone = false;
    //public GameSetCmd cmdGameSet = null;

    [HideInInspector]
    public List<Command> unitTimerRequests = new List<Command>();

    public List<MapPhysicsMaterial> mapPhysicsMaterials;

    public MapBehaviour mapBehaviour;
    GameMap gameMap;

    // Used only in Host
    public int HealingBuffMaxDistance;

    public AutoBow autoBow;

    void SimulateCollision(List<Unit> targets)
    {
        List<bool> clean = new List<bool>();
        List<CollisionInfo> infos = new List<CollisionInfo>();
        //Simulate Collision:manipulate only veloity and position
        PhysicsSimulator.SimulateCollision(targets, mapBehaviour, this, clean, infos);

        for (int i = 0; i < targets.Count; i++)
        {
            if (clean[i])
            {
                PhysicsSimulator.ApplyIntegral(targets[i]);
            }
            else
            {
                PhysicsSimulator.SimulateIntegral(targets[i], Time.deltaTime,mapBehaviour);
                PhysicsSimulator.ApplyIntegral(targets[i]);
            }
        }
        for (int i = 0; i < infos.Count; i++)
        {
            CollisionInfo collision = infos[i];
            // Stop healing buff
            infos[i].me.buff &= ~BuffFlag.BUFF_HEALING;
            infos[i].other.buff &= ~BuffFlag.BUFF_HEALING;
            UnitInfoTag unitInfoTag = FindInstance(collision.me.uuid);
            if (unitInfoTag != null)
                unitInfoTag.CollisionEvent(collision);
        }
        List<Unit> remains = new List<Unit>();
        for (int i = 0; i < targets.Count; i++)
        {
            Unit curUnit = targets[i];
            //死亡判定
            if (isClient == 0 && !curUnit.isDead)
            {
                if (curUnit.HP<= 0)
                {
                    curUnit.isDead = true;
                }
                else if (isOutOfBounds(curUnit))     //ユニットが範囲外に出たときの死亡判定
                {
                    curUnit.isDead = true;
                }
            }
            if (!curUnit.isDead)
            {
                remains.Add(curUnit);
            }
        }
        units = remains;
        checkGameSet();
    }

    public bool isOutOfBounds(Unit curUnit)
    {
        return mapBehaviour.GetTile(new Vector3(curUnit.x, 0, curUnit.z)) == null;
        /*
        int xnum = 20;
        int ynum = 12;
        if (curUnit.x < -0.5 - 10 || curUnit.x > 2 * xnum + 0.5 - 10 || curUnit.z < -1.3 || curUnit.z > 1.3 * 1.1547 * ynum + 1.3)
        {
            return true;
        }
        else
        {
            return false;
        }*/
    }

    //ゲームが決着したかどうかの判定
    public void checkGameSet()
    {
        if (isClient == 1) // Host only
            return;
        bool isHostAlive = false, isClientAlive = false;
        //それぞれのチームで生存しているユニットがいるかどうかを調べる
        for (int i = 0; i < units.Count; i++)
        {
            if (!units[i].isDead)
            {
                if (units[i].owner == 0) isHostAlive = true;
                if (units[i].owner == 1) isClientAlive = true;
                if (isHostAlive && isClientAlive) break;
            }
        }
        if (!isHostAlive)
        {
            openResultScene(false);
        }
        else if (!isClientAlive)
        {
            openResultScene(true);
        }
    }

    //HostからClientにゲームが決着したことを知らせるコマンドを送る
    private void sendGameSetCmd(bool didHostWin)
    {
        //cmdGameSet = new GameSetCmd(didHostWin);
        //Thread.Sleep(500);      //送り終わるまで待つ
    }

    public void openResultScene(bool didHostWin)
    {
        IntersceneBehaviour.SetWinner(didHostWin);
        Debug.Log("game set");
        if (isClient == 0)
            SceneManager.LoadScene("ResultHost");
        else
            SceneManager.LoadScene("ResultClient");
    }




    // Used by Host
    public List<UnitSpec> UnitSpecs;
    static public List<int> InitialUnitTypes; // 0-4: Host's units 5-9: Client's units
    static public List<Unit> InitialItems = new List<Unit>();   //MapLoaderによって配置されたアイテム

    // Start is called before the first frame update
    void Start()
    {
        autoBow = GetComponent<AutoBow>();
        if (InitialUnitTypes == null) // For debug
        {
            InitialUnitTypes = new List<int>
            {
                0, 0, 0, 1, 1, 0, 0, 0, 1, 1
            };
        }
        gameMap = mapBehaviour.map;
        if (isClient > 0)
        {
            return;
        }
        //Debug.Log("Initial Unit Types");
        /*for (int i = 0; i < InitialUnitTypes.Count; i++)
        {
            Debug.Log(InitialUnitTypes[i]);
        }*/
        //Debug.Log("Initiating units...");
        for (int n = -1; n < 2; n += 2)
        {
            for (int i = 0; i < 5; i++) // Tinaxd reduced the number of units
            {
                CreateUnitFromHost(n == -1 ? 0 : 1, InitialUnitTypes[i + (n == 1 ? 5 : 0)], Vector2.zero, new Vector2(n * 10, i * 1.5f));
            }
        }

        /*for (int i = 0; i < InitialItems.Count; i++)    //marot
        {
            Debug.Log("TestTest");
            CreateUnitFromHost(-1, InitialItems[i].type, Vector2.zero, new Vector2(InitialItems[i].x, InitialItems[i].z));
        }*/

        UpdateInstances();

        gameStatusUI.SetAlly(isClient + 1);
    }

    void UpdateInstances()
    {
        List<UnitInfoTag> remains=new List<UnitInfoTag>();
        for (int i = 0; i < instances.Count; i++)
        {
            if (GetUnit(instances[i].uuid) == null)
            {
                if (instances[i].basicUnit != null && !instances[i].basicUnit.isDead)
                    instances[i].basicUnit.isDead = true;
                Destroy(instances[i].gameObject);
            }
            else
            {
                remains.Add(instances[i]);
            }
        }
        instances = remains;
        for (int i = 0; i < units.Count; i++)
        {
            bool found = false;
            for (int j = 0; j < instances.Count; j++)
            {
                if (units[i].uuid == instances[j].uuid)
                {
                    instances[j].Apply(units[i]);
                    found = true;
                    //Schin update CD ready emotion
                    //this process has moved to inside Update() of BasicUnit
                    break;
                }
            }
            if (!found)
            {
                var prefab = (GameObject)Resources.Load(UnitTypeIndexMapper.map[units[i].type]);
                GameObject g = Instantiate(prefab);
                units[i].HP = g.GetComponent<UnitSpec>().MaxHP;
                units[i].MP = g.GetComponent<UnitSpec>().MaxMP;
                UnitInfoTag tag = g.GetComponent<UnitInfoTag>();
                tag.sim = this;
                tag.Apply(units[i]);
                tag.InitializeBasicUnit(units[i]);
                tag.SetOwned(units[i].owner == isClient);
                instances.Add(tag);
            }
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        time += Time.deltaTime;
        bool doRegen = false;
        if (isClient == 0 && time > RegenTimer)
        {
            RegenTimer = Mathf.Ceil(time);
            doRegen = true;
        }
        ProcessMyCommand();
        for (int i = 0; i < units.Count; i++)
        {
            PhysicsSimulator.SimulateIntegral(units[i], Time.deltaTime,mapBehaviour);
            if (doRegen)
            {
                units[i].MP = Mathf.Min(units[i].MP + 1, 50);
                TryProcessingHealingBuff(units[i]);
            }
        }
        autoBow.ProcessArrow(Time.deltaTime);
        SimulateCollision(units);
        UpdateInstances();
        UpdateGameTimeUI();
    }

    private float RegenTimer = 0;

    void ProcessMyCommand()
    {
        for (int i = 0; i < commands.Count; i++)
        {
            Command c1 = commands[i];
            if (c1 == null)
            {
                continue;
            }
            if (c1.processed)
            {
                continue;
            }
            if (c1 is GameSetCmd)
            {
                GameSetCmd c = (GameSetCmd)c1;
                openResultScene(c.isHostWin);
            }
            if (c1 is UnitMovedCmd)
            {
                UnitMovedCmd c = (UnitMovedCmd)c1;
                var basicUnit = GetBasicUnit(c.uuid);
                //Debug.Log(c.vx);
                if (basicUnit!=null&&!basicUnit.MovementLocked && !basicUnit.Locked) // lockdown and waittime check
                {
                    Unit u = GetUnit(c.uuid);
                    u.vx = c.vx;
                    u.vz = c.vz;
                    //schin Remove "ready to roll" emotion when ready.
                    //      this doesn't check if it's really moved for client
                    basicUnit.ExpireEmotion(EmotionType.CD_READY);

                    // Tinaxd update CountdownUI (Host only)
                    // Generate a UnitTimerCommand
                    if (isClient == 0)
                    {
                        UnitTimerCmd utc = new UnitTimerCmd();
                        utc.penalty = basicUnit.WaitTimePenaltyTime;
                        utc.timerType = UnitTimerCmd.MOVED;
                        utc.uuid = c.uuid;
                        unitTimerRequests.Add(utc);
                        commands.Add(utc);
                    }
                }
                else
                {

                }
                c.processed = true;
            }
            if (c1 is UnitUpdateCmd)
            {
                UnitUpdateCmd c = (UnitUpdateCmd)c1;
                //Debug.Log(c.vx);
                if (isClient > 0)
                {
                    //TODO if units.isDeadあり
                    //UnitDied()
                    units = c.units;
                }
                c.processed = true;
                c.sent = true;
            }
            if (c1 is UnitTimerCmd)
            {
                UnitTimerCmd c = (UnitTimerCmd)c1;
                switch (c.timerType)
                {
                    case 0: // MOVED
                        var basicUnit = GetBasicUnit(c.uuid);
                        if (basicUnit != null) basicUnit.MarkMoved();
                        // Lockdown ends
                        foreach (var instance in instances)
                        {
                            var bu = instance.basicUnit;
                            if (bu == null) continue;
                            if (bu.Owned != basicUnit.Owned)
                            {
                                bu.MovementLocked = false;
                            }
                        }
                        break;
                    case 1: // HOST LOCKDOWN
                        foreach (var instance in instances)
                        {
                            var bu = instance.basicUnit;
                            if (bu == null) continue;
                            if (isClient > 0)
                            {
                                if (bu.Owned)
                                    bu.MovementLocked = true;
                                else
                                    bu.MarkLockdown();
                            }
                            else
                            {
                                if (bu.Owned)
                                    bu.MarkLockdown();
                                else
                                    bu.MovementLocked = true;
                            }
                        }
                        break;
                    case 2: // CLIENT LOCKDOWN
                        foreach (var instance in instances)
                        {
                            var bu = instance.basicUnit;
                            if (bu == null) continue;
                            if (isClient > 0)
                            {
                                if (bu.Owned)
                                    bu.MarkLockdown();
                                else
                                    bu.MovementLocked = true;
                            }
                            else
                            {
                                if (bu.Owned)
                                    bu.MovementLocked = true;
                                else
                                    bu.MarkLockdown();
                            }
                        }
                        break;
                }
                //TODO if c is GameSetCmd
                //SceneManager.LoadScene("Result");
                //clientBattleScene, isHostWin = GameSetCmd.isHostWin
                if (isClient > 0 && (!AutoPlay.isOffline))
                {
                    c.processed = true;
                }
            }
            if (c1 is NewUnitCmd)
            {
                //Debug.Log(GetUnit(((NewUnitCmd)c1).fromUnitId).owner);
                NewUnitCmd c = (NewUnitCmd)c1;
                switch (c.unitType)
                {
                    case 2: // Arrow
                        {
                            Unit u = GetUnit(c.fromUnitId);
                            if (u == null) break;
                            if (u.projectileReload <= 0)
                            {
                                u.projectileReload = 7;
                                CreateArrow(GetUnit(c.fromUnitId), c.velocity);
                            }
                        }
                        break;
                    case 3: // Fireball
                        {
                            var u = GetUnit(c.fromUnitId);
                            if (u.MP > 25)
                            {
                                u.MP -= 25;
                                CreateFireball(u, c.velocity);
                            }
                        }
                        break;
                }
                c.processed = true;
            }

            if (c1 is HealingBuffRequestCmd)
            {
                var c = (HealingBuffRequestCmd)c1;
                if (isClient == 0)
                {
                    var requestor = GetUnit(c.RequestorId);
                    var target = GetUnit(c.TargetId);
                    if (PhysicsSimulator.UnitDistance1(requestor, target) < HealingBuffMaxDistance)
                    {
                        target.buff |= BuffFlag.BUFF_HEALING;
                    }
                    c.processed = true;
                }
                //GetBasicUnit(c.RequestorId).DragMode = DragType.NORMAL;
                //UnityEngine.EventSystems.ExecuteEvents.Execute<IDragAndFireEventHandler>(this.gameObject, null, (x, y) => x.TurnOnDrag());
            }
        }
        List<Command> remains = new List<Command>();
        for (int i = 0; i < commands.Count; i++)
        {
            if (commands[i].sent) continue;
            remains.Add(commands[i]);
        }
        commands = remains;
    }
    public UnitInfoTag FindInstance(int uuid)
    {
        for (int i = 0; i < instances.Count; i++)
        {
            if (instances[i].uuid == uuid)
            {
                return instances[i];
            }
        }
        return null;
    }
    public void SetCommandsSent()
    {
        for (int i = 0; i < commands.Count; i++)
        {
            Command c = commands[i];
            if (c != null)
                c.sent = true;
        }
    }
    public Unit GetUnit(int uuid)
    {
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].uuid == uuid)
            {
                return units[i];
            }
        }
        return null;
    }
    public void OnGUI()
    {
        GUILayout.Box(time.ToString("F2"));
    }
    public List<Command> GetCommandsFromHost()
    {
        List<Command> cs = new List<Command>();
        //if(cmdGameSet != null)
        //{
        //    cs.Add(cmdGameSet);     //add GameSetCmd
        //    cmdGameSet = null;
        //}
        //unitupdatecmd
        UnitUpdateCmd unitUpdateCmd = new UnitUpdateCmd();
        unitUpdateCmd.units = units;
        cs.Add((Command)unitUpdateCmd);
        return cs;
    }

    public UnitInfoTag GetUnitInfoTag(int uuid)
    {
        for (int i = 0; i < instances.Count; i++)
        {
            if (instances[i].uuid == uuid)
            {
                return instances[i];
            }
        }
        return null;
    }

    public BasicUnit GetBasicUnit(int uuid)
    {
        UnitInfoTag info = GetUnitInfoTag(uuid);
        if (info != null)
            return info.basicUnit;
        return null;
    }
    void CreateUnitFromHost(int owner, int type, Vector2 velocity, Vector2 position)//should be called only for initialization from host
    {
//        Debug.Log("Creating an item");
        Unit u = new Unit();
        // Set velocity
        u.vx = velocity.x;
        u.vz = velocity.y;
        u.vx1 = u.vx;
        u.vz1 = u.vz;
        // Set position
        u.x = position.x;
        u.z = position.y;
        u.x1 = u.x;
        u.z1 = u.z;

        u.type = type; // Set unit type
        u.uuid = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        u.owner = owner;
        units.Add(u);
        if (isClient != 0)
        {
            //add command
        }
    }
    // Tinaxd Used by host only 
    private void CreateArrow(Unit fromUnit, Vector3 velocity)
    {
//        Debug.Log("Creating arrow");
        Unit u = new Unit();
        // Set velocity
        u.vx = velocity.x;
        u.vz = velocity.z;
        u.vx1 = u.vx;
        u.vz1 = u.vz;
        //Debug.Log("Arrow sent: (" + fromUnit.x + ", 0, " + fromUnit.z + ") -> (" + to.x + ", 0, " + to.z + ")");
        //Debug.Log("Velocity: " + vel);
        // Set position
        u.x = fromUnit.x + velocity.normalized.x;
        u.z = fromUnit.z + velocity.normalized.z;
        u.x1 = u.x;
        u.z1 = u.z;

        u.type = UnitType.TYPE_ARROW; // Set unit type to "arrow"
        u.uuid = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        u.owner = fromUnit.owner;
        units.Add(u);
    }

    private void CreateFireball(Unit fromUnit, Vector3 velocity)
    {
        Debug.Log("Creating fireball");
        Unit u = new Unit();
        // Set velocity
        u.vx = velocity.x;
        u.vz = velocity.z;
        u.vx1 = u.vx;
        u.vz1 = u.vz;
        //Debug.Log("Arrow sent: (" + fromUnit.x + ", 0, " + fromUnit.z + ") -> (" + to.x + ", 0, " + to.z + ")");
        //Debug.Log("Velocity: " + vel);
        // Set position
        u.x = fromUnit.x + velocity.normalized.x;
        u.z = fromUnit.z + velocity.normalized.z;
        u.x1 = u.x;
        u.z1 = u.z;

        u.type = UnitType.TYPE_FIREBALL; // Set unit type to "arrow"
        u.uuid = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        u.owner = fromUnit.owner;
        units.Add(u);
    }

    private void TryProcessingHealingBuff(Unit u)
    {
        if ((u.buff & BuffFlag.BUFF_HEALING) != 0)
        {
            u.HP += 2;
            var max = GetBasicUnit(u.uuid).MaxHP;
            if (u.HP > max)
            {
                u.HP = max;
                u.buff &= ~BuffFlag.BUFF_HEALING;
            }
        }
    }
    private void Awake()
    {
        PhysicsSimulator.mapPhysicsMaterials = mapPhysicsMaterials;
        gameStatusUI = GameObject.Find("ScreenUIObj").GetComponentInChildren<GameStatusUI>();
    }

    private GameStatusUI gameStatusUI;

    private void UpdateGameTimeUI()
    {
        gameStatusUI.UpdateGameTimeText(this.time);
    }
}

public sealed class UnitTypeIndexMapper
{
    public static readonly Dictionary<int, string> map = new Dictionary<int, string>
    {
        [UnitType.TYPE_CHESS] = "Units/Swordsman",
        [UnitType.TYPE_ARCHER] = "Units/Archer",
        [UnitType.TYPE_ARROW] = "Units/NonPlayer/ArcherArrow",
        [UnitType.TYPE_FIREBALL] = "Units/NonPlayer/fireball",
        [UnitType.TYPE_ITEM_HEAL] = "Units/NonPlayer/Heal",
        [UnitType.TYPE_ITEM_MPOT] = "Units/NonPlayer/MPot",
        [UnitType.TYPE_ITEM_LPOT] = "Units/NonPlayer/LPot",
    };
}