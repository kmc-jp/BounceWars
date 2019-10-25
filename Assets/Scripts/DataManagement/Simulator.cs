using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Simulator : MonoBehaviour
{
    public float time;
    public List<GameObject> prefabs;

    List<UnitInfoTag> instances = new List<UnitInfoTag>();
    [HideInInspector]
    public List<Unit> units = new List<Unit>();
    [HideInInspector]
    public int isClient = 0;
    [HideInInspector]
    public List<Command> commands = new List<Command>();
    public bool isCommandProcessingDone = false;
    public GameSetCmd cmdGameSet = null;

    [HideInInspector]
    public List<Command> unitTimerRequests = new List<Command>();

    public List<MapPhysicsMaterial> mapPhysicsMaterials;

    public MapBehaviour mapBehaviour;
    GameMap gameMap;

    void SimulateCollision(List<Unit> targets)
    {
        //Debug.Log(targets[0].vx);
        List<bool> clean = new List<bool>();

        for (int i = 0; i < targets.Count; i++)
        {
            clean.Add(true);
        }
        for (int i = 0; i < targets.Count; i++)
        {
            targets[i].vx1 = targets[i].vx;
            targets[i].vz1 = targets[i].vz;
        }
        List<CollisionInfo> infos = new List<CollisionInfo>();
        for (int i = 0; i < targets.Count; i++)
        {
            for (int j = i; j < targets.Count; j++)
            {
                if (i == j) continue;
                Unit u1 = targets[i];
                Unit u2 = targets[j];
                float d = UnitDistance1(u1, u2);
                float dx = u2.x1 - u1.x1;
                dx /= d;
                float dz = u2.z1 - u1.z1;
                dz /= d;

                if (0 < d && d < 1)
                {
                    CollisionInfo collision = new CollisionInfo();
                    collision.me = u1;
                    collision.other = u2;
                    collision.vx1 = u1.vx;
                    collision.vz1 = u1.vz;
                    collision.vx2 = u2.vx;
                    collision.vz2 = u2.vz;
                    infos.Add(collision);
                    float rvx = u2.vx - u1.vx;
                    float rvz = u2.vz - u1.vz;
                    float sizeVertical = dx * rvx + dz * rvz;//hiroaki strength of impulse

                    collision.normalVelocity = sizeVertical;
                    if (u1.type == 2) // tinaxd u1 is arrow
                    {
                        u1.vx1 = u1.vx + dx * 0.95f * sizeVertical;
                        u1.vz1 = u1.vz + dz * 0.95f * sizeVertical;
                        u1.HP = 0;
                        u2.vx1 = u2.vx + dx * 0.05f * sizeVertical;
                        u2.vz1 = u2.vz + dz * 0.05f * sizeVertical;
                    }
                    else if (u2.type == 2) // tinaxd u2 is arrow
                    {
                        u1.vx1 = u1.vx + dx * 0.05f * sizeVertical;
                        u1.vz1 = u1.vz + dz * 0.05f * sizeVertical;
                        u2.vx1 = u2.vx + dx * 0.95f * sizeVertical;
                        u2.vz1 = u2.vz + dz * 0.95f * sizeVertical;
                        u2.HP = 0;
                    }
                    else // neither u1 nor u2 is arrow
                    {
                        u1.vx1 = u1.vx + dx * sizeVertical;
                        u1.vz1 = u1.vz + dz * sizeVertical;
                        u2.vx1 = u2.vx - dx * sizeVertical;
                        u2.vz1 = u2.vz - dz * sizeVertical;
                    }
                    //Debug.Log(string.Format("{0}:({1},{2}),({3},{4})({5})",Time.time,i,j,rvx,rvz,sizeVertical));
                    //Debug.Log(string.Format("{0} i={1}:({2},{3})",Time.time,i, u1.vx1, u1.vz1));
                    clean[i] = false;
                    clean[j] = false;
                }
            }
        }

        for (int i = 0; i < targets.Count; i++)
        {
            Unit u = targets[i];
            Vector2 pos = new Vector2(u.x1, u.z1);

            Tile t = mapBehaviour.GetTile(new Vector3(u.x1, 0, u.z1));
            if (t==null||t.buildingType == 0)
            {
                continue;
            }
            Vector2 tPos = new Vector2(t.position.x, t.position.z);
            Vector2 diff = pos - tPos;
            if (diff.sqrMagnitude < 1)
            {
                clean[i] = false;
                Vector2 normal = diff.normalized;
                u.vx1 = u.vx1 - u.vx1 * normal.x * normal.x*2;
                u.vz1 = u.vz1 - u.vz1 * normal.y * normal.y*2;
            }
        }
        for (int i = 0; i < infos.Count; i++)
        {
            CollisionInfo collision = infos[i];
            UnitInfoTag unitInfoTag = FindInstance(collision.me.uuid);
            unitInfoTag.basicUnit.CollisionEvent(collision);
        }
        float E = 0;
        float px = 0;
        float pz = 0;
        for (int i = 0; i < targets.Count; i++)
        {
            Unit curUnit = targets[i];
            curUnit.vx = curUnit.vx1;
            curUnit.vz = curUnit.vz1;
            E += curUnit.vx * curUnit.vx + curUnit.vz * curUnit.vz;
            px += curUnit.vx;
            pz += curUnit.vz;

            //死亡判定
            if (isClient == 0 && !curUnit.isDead)
            {
                // if HP is zero, and Unit is stopped
                if (curUnit.HP == 0 && curUnit.vx < 0.05 && curUnit.vz < 0.05)
                {
                    curUnit.isDead = true;
                    checkGameSet();
                }
                else if (isOutOfBounds(curUnit))     //ユニットが範囲外に出たときの死亡判定
                {
                    curUnit.isDead = true;
                    checkGameSet();
                }
            }
        }

        //        Debug.Log(targets[0].vx);
        //Debug.Log(string.Format("E={0},p=({1},{2})", E, px, pz));

        for (int i = 0; i < targets.Count; i++)
        {
            if (clean[i])
            {
                ApplyIntegral(targets[i]);
            }
            else
            {
                SimulateIntegral(targets[i], Time.deltaTime);
                ApplyIntegral(targets[i]);
            }
        }

        //        Debug.Log(targets[0].vx);
    }

    private bool isOutOfBounds(Unit curUnit)
    {
        int xnum = 20;
        int ynum = 12;
        if(curUnit.x < -0.5 - 10 || curUnit.x > 2*xnum + 0.5 - 10 || curUnit.z < -1.3 || curUnit.z > 1.3 * 1.1547*ynum + 1.3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //ゲームが決着したかどうかの判定
    void checkGameSet()
    {
        bool isHostAlive = false, isClientAlive = false;
        //それぞれのチームで生存しているユニットがいるかどうかを調べる
        for(int i = 0; i < units.Count; i++)
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
            sendGameSetCmd(false);
            openResultScene(false);
        }
        else if (!isClientAlive)
        {
            sendGameSetCmd(true);
            openResultScene(true);
        }
    }

    //HostからClientにゲームが決着したことを知らせるコマンドを送る
    private void sendGameSetCmd(bool didHostWin)
    {
        cmdGameSet = new GameSetCmd(didHostWin);
        Thread.Sleep(500);      //送り終わるまで待つ
    }

    public void openResultScene(bool didHostWin)
    {
        IntersceneBehaviour.SetWinner(didHostWin);
        SceneManager.LoadScene("Result");
    }


    float UnitDistance1(Unit u1, Unit u2)
    {
        //Debug.Log(Mathf.Sqrt((u1.x1 - u2.x1) * (u1.x1 - u2.x1) + (u1.z1 - u2.z1) * (u1.z1 - u2.z1)));
        return Mathf.Sqrt((u1.x1 - u2.x1) * (u1.x1 - u2.x1) + (u1.z1 - u2.z1) * (u1.z1 - u2.z1));
    }

    void SimulateIntegral(Unit u, float dt)
    {
        float v = Mathf.Sqrt(u.vx * u.vx + u.vz * u.vz);
        if (v > 0)
        {
            Tile tile = mapBehaviour.GetTile(new Vector3(u.x, 0, u.z));
            float friction = 1;
            Debug.Log(tile);
            if (tile != null)
            {
                friction = mapPhysicsMaterials[tile.type].friction;
            }
            float fx = -u.vx / v * friction;
            float fz = -u.vz / v * friction;
            float fxdt = fx * dt;
            float fzdt = fz * dt;
            float vx1 = u.vx + fxdt;
            float vz1 = u.vz + fzdt;
            if (vx1 * u.vx > 0)
            {
                u.vx = vx1;
                u.vz = vz1;
            }
            else
            {
                u.vx = 0;
                u.vz = 0;
            }
        }
        u.x1 += u.vx * dt;
        u.z1 += u.vz * dt;
    }
    void ApplyIntegral(Unit u)
    {
        u.x = u.x1;
        u.z = u.z1;
    }
    // Start is called before the first frame update
    void Start()
    {
        gameMap = mapBehaviour.map;
        if (isClient > 0)
        {
            return;
        }
        for (int n = -1; n < 2; n += 2)
        {
            for (int i = 0; i < 5; i++) // Tinaxd reduced the number of units
            {
                Unit u = new Unit();
                u.x = n * 10;
                u.z = i * 1.5f;
                u.x1 = u.x;
                u.z1 = u.z;
                //Schin set unit type
                u.type = Random.Range(0, 2);
                u.uuid = Random.Range(int.MinValue, int.MaxValue);
                // Tinaxd set HP/MP here
                u.HP = 50;   // TODO
                u.MP = 100;  // TODO
                if (n == -1)
                {
                    u.owner = 0;
                }
                else
                {
                    u.owner = 1;
                }
                units.Add(u);

                /*
                GameObject g = Instantiate(prefabs[0]);
                UnitInfoTag tag = g.GetComponent<UnitInfoTag>();
                tag.Apply(u);
                instances.Add(tag);*/
            }
        }
        UpdateInstances();
    }

    void UpdateInstances()
    {
        for (int i = 0; i < units.Count; i++)
        {
            bool found = false;
            for (int j = 0; j < instances.Count; j++)
            {
                if (units[i].uuid == instances[j].uuid)
                {
                    instances[j].Apply(units[i]);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                GameObject g = Instantiate(prefabs[units[i].type]); // Tinaxd
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
        ProcessMyCommand();
        for (int i = 0; i < units.Count; i++)
        {
            SimulateIntegral(units[i], Time.deltaTime);
        }
        SimulateCollision(units);
        UpdateInstances();
    }

    void ProcessMyCommand()
    {
        for (int i = 0; i < commands.Count; i++)
        {
            Command c1 = commands[i];
            if (c1.processed)
            {
                continue;
            }
            if (c1 is UnitMovedCmd)
            {
                UnitMovedCmd c = (UnitMovedCmd)c1;
                var basicUnit = GetBasicUnit(c.uuid);
                //Debug.Log(c.vx);
                if (!basicUnit.MovementLocked && !basicUnit.Locked) // lockdown and waittime check
                {
                    Unit u = GetUnit(c.uuid);
                    u.vx = c.vx;
                    u.vz = c.vz;

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
                        basicUnit.MarkMoved();
                        // Lockdown ends
                        foreach (var instance in instances)
                        {
                            var bu = instance.basicUnit;
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
                if (isClient > 0)
                    c.processed = true;
            }
            if (c1 is NewUnitCmd)
            {
                NewUnitCmd c = (NewUnitCmd)c1;
                CreateArrow(GetUnit(c.fromUnitId), c.to);
                c.processed = true;
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
    UnitInfoTag FindInstance(int uuid)
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
        if(cmdGameSet != null)
        {
            cs.Add(cmdGameSet);     //add GameSetCmd
            cmdGameSet = null;
        }
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
        UnitInfoTag tag = GetUnitInfoTag(uuid);
        if (tag != null)
            return tag.basicUnit;
        return null;
    }

    // Tinaxd Used by host only 
    private void CreateArrow(Unit fromUnit, Vector3 to)
    {
        Debug.Log("Creating arrow");
        Unit u = new Unit();
        // Set velocity
        var vel = new Vector3(to.x - fromUnit.x, 0, to.z - fromUnit.z).normalized;
        u.vx = vel.x * 30;
        u.vz = vel.z * 30;
        u.vx1 = u.vx;
        u.vz1 = u.vz;
        //Debug.Log("Arrow sent: (" + fromUnit.x + ", 0, " + fromUnit.z + ") -> (" + to.x + ", 0, " + to.z + ")");
        //Debug.Log("Velocity: " + vel);
        // Set position
        u.x = fromUnit.x + vel.x;
        u.z = fromUnit.z + vel.z;
        u.x1 = u.x;
        u.z1 = u.z;
        u.HP = 0.1f; // TODO
        u.MP = 0;    //

        u.type = 2; // Set unit type to "arrow"
        u.uuid = Random.Range(int.MinValue, int.MaxValue);
        u.owner = fromUnit.owner;
        units.Add(u);
    }
}
