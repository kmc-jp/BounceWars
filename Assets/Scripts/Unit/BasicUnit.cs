using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Original author: Tinaxd
// Attach this script to all units
public class BasicUnit : MonoBehaviour
{
    BuffParticleManager buffParticleManager;

    public UnitInfoTag infoTag;
    public Unit unit;

    private bool _isDead = false;
    protected float hp;
    protected float mp;

    protected UnitUI unitUI;

    public GameObject damagePopup;
    public GameObject canvas;

    public float buttonsUICloseDelay = 0.3f;

    [SerializeField]
    private GameObject myButtons;

    public bool MouseOn;

    public Tile currentTile;

    private Simulator simulator;

    // Tinaxd countdown timer
    private float waitTime = 0;
    public float WaitTime
    {
        get => waitTime;
        set
        {
            waitTime = value;
            UpdateWaitTimeText();
        }
    }

    public bool Locked
    {
        get => WaitTime > 0;
    }

    private CountDownBar countDownBar;

    // relative to the resources directory
    public string CountDownIconPath;

    public float BaseCountDownTime = 8;

    public bool Moved = false;
    public bool LockdownPenalty = false;

    public float LockdownPenaltyTime
    {
        get => 1.5f * (LastMoveTime - LockdownStartTime);
    }

    public float WaitTimePenaltyTime
    {
        get => LockdownPenalty ? LockdownPenaltyTime : 0;
    }

    public float LockdownStartTime;
    public float LastMoveTime;

    public bool Owned;

    public virtual float HP
    {
        get => hp;
        set
        {
            this.hp = value;
            this.unit.HP = value;
            unitUI.HP = value;
        }
    }

    public virtual float MP
    {
        get => mp;
        set
        {
            this.mp = value;
            this.unit.MP = value;
            unitUI.MP = value;
        }
    }

    public virtual float MaxHP
    {
        get => unitUI.HPMax;
        set => unitUI.HPMax = value;
    }

    public virtual float MaxMP
    {
        get => unitUI.MPMax;
        set => unitUI.MPMax = value;
    }

    private void UpdateWaitTimeText()
    {
        if (!LockdownPenalty)
        {
            unitUI.WaitTimeText = string.Format("{0:0.#}s", WaitTime);
            unitUI.WaitTimeColor = Color.black;
        }
        else
        {
            unitUI.WaitTimeText = string.Format("{0:0.#}s [LOCKDOWN +{1:0.0}s]", WaitTime, Time.time - LockdownStartTime);
            unitUI.WaitTimeColor = Color.red;
        }
    }

    protected virtual void Awake()
    {
        unitUI = transform.Find("UnitUI").gameObject.GetComponent<UnitUI>();
        //Debug.Log(transform.Find("UnitUI").gameObject.GetComponent<UnitUI>());

        //unitUI.HPMax = 50;
        //unitUI.MPMax = 100;

        WaitTime = 0;

        var iconNames = new string[] { "sword", "archer" };
        var unitType = unit.type;
        CountDownIconPath = "test/" + iconNames[unitType];

        // bind CountDownUI OBJ Tinaxd
        countDownBar = GameObject.Find("ScreenUIObj").GetComponentInChildren<CountDownBar>();

        simulator = GameObject.Find("Obelisk").GetComponent<Simulator>();

        myButtons.GetComponent<ButtonsUIManager>().basicunit = this;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        buffParticleManager = GetComponent<BuffParticleManager>();
        //HP = 50;
        //MP = 100;

        // Tinaxd register units to CountDownBar
        countDownBar.RegisterUnit(this, CountDownIconPath);

        if (unit.owner != simulator.isClient)
        {
            myButtons.GetComponent<ButtonsUIManager>().disabled = true;
            myButtons.GetComponent<ButtonsUIManager>().CloseAll();
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        currentTile = MapBehaviour.instance.GetTile(transform.position);
        HP = unit.HP;
        MP = unit.MP;
        if (!unit.isDead
          && unit.owner == simulator.isClient
          && !Locked)
        {
            ShowEmotion(EmotionType.CD_READY, 5.0f);
        }

        float delta = Time.deltaTime;
        if (Locked)
        {
            WaitTime -= delta;
        }

        if (LockdownPenalty)
        {
            UpdateWaitTimeText();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        /*
        Debug.Log("Attacked!");
        float damage = Random.Range(5, 25);
        HP = HP - damage;
        PopupDamage(damage);*/
    }


    private void PopupDamage(float damage)
    {
        Debug.Log("HP: " + HP);

        GameObject popup = Instantiate(damagePopup);
        popup.transform.SetParent(canvas.transform);
        popup.transform.localPosition = new Vector3(0, 1, 0);
        popup.GetComponent<DamagePopup>().Unit = this.gameObject;
        popup.GetComponent<DamagePopup>().Canvas = canvas.GetComponent<Canvas>();
        popup.GetComponent<DamagePopup>().Text = (-damage).ToString();
    }

    private void OnMouseEnter()
    {
        MouseOn = true;
    }

    private void OnMouseExit()
    {
        MouseOn = false;
    }

    public void NotifyOperation(string operation, object args)
    {
        var opHandler = GetComponent<OperationHandlerBase>();
        opHandler.OnMessage(operation, args);
    }

    // Tinaxd Call this method after moving this unit
    public void MarkMoved()
    {
        if (!Locked)
        {
            Moved = true;
            LastMoveTime = Time.time;
            WaitTime += BaseCountDownTime;
            if (LockdownPenalty)
            {
                WaitTime += LockdownPenaltyTime;
                //Debug.Log("Lockdown penalty! New Time: " + WaitTime + " (penalty: " + LockdownPenaltyTime + ")");
            }
        }
    }

    public void MarkLockdown()
    {
        LockdownStartTime = Time.time;
        LockdownPenalty = true;
    }

    public bool MovementLocked = false;

    /* // Tinaxd removed method
    public void UseCountDown(bool b)
    {
        if (b)
        {
            countDownBar.RegisterUnit(this, CountDownIconPath);
        }
        else
        {
            countDownBar.RemoveUnit(this);
        }
        unitUI.WaitTimeEnabled = b;
    }*/

    // Emotion Icon Tinaxd
    public void ShowEmotion(string emotionName, float length)
    {
        if (unitUI != null) // Tinaxd some units do not have unitUI
        {
            if (!unit.isDead)
                unitUI.ShowEmotion(emotionName, length);
        }
    }
    public void ExpireEmotion(string emotionName)//schin hide specific emotion
    {
        if (unitUI != null) // Tinaxd some units do not have unitUI
        {
            if (!unit.isDead)
                unitUI.ExpireEmotion(emotionName);
        }
    }
    public bool HasEmotion(string emotionName)// schin check is Unit is showing some emotion
    {
        if (unitUI != null) // Tinaxd some units do not have unitUI
        {
            if (!unit.isDead)
                return unitUI.HasEmotion(emotionName);
            return false;
        }
        return false;
    }
    public virtual void CollisionEvent(CollisionInfo info)
    {
        //        Debug.Log("CollisionEvent");

        //        Debug.Log("Attacked!");
        float damage = 10;

        if (UnitType.isItem(info.other.type))
        {
            AddBuff(0);
            UpdateBuff(GetBuffs());
            return;
        }
        HP = HP - damage;

        if (HP < 0) HP = 0;
        //TODO check UI update

        PopupDamage(damage);
    }

    // Tinaxd DragUI
    public void NotifyDragStart()
    {
        unitUI.DragUI.ShowDragUI(true);
        // close ButtonsUI
        MouseOn = false;
        myButtons.GetComponent<ButtonsUIManager>().CloseAll();
    }

    public void NotifyDragUpdate(Vector3 worldPos)
    {
        float radius = Vector3.Distance(transform.position, worldPos);
        var dir = worldPos - transform.position;
        unitUI.DragUI.UpdateDragUI(dir);
    }

    public void NotifyDragEnd()
    {
        unitUI.DragUI.ShowDragUI(false);
        // open ButtonsUI
        myButtons.GetComponent<ButtonsUIManager>().OpenAll();
    }

    public DragType DragMode
    {
        get
        {
            return myButtons.GetComponent<ButtonsUIManager>().CurrentDragType;
        }
    }

    // Tinaxd added buff related operations
    public int GetBuffs()
    {
        return unit.buff;
    }

    public void AddBuff(int buffType)
    {
        unit.buff |= (1 << buffType);
    }

    public void RemoveBuff(int buffType)
    {
        unit.buff &= ~(1 << buffType);
    }
    public void RemoveBuffAll()
    {
        unit.buff = 0;
    }

    public void UpdateBuff(int buffs)
    {
        Debug.Log("UpdateBuff");
        unit.buff = buffs;
        buffParticleManager.UpdateParticles(buffs);
    }

    public bool isDead
    {
        get => _isDead;
        set
        {
            _isDead = value;
            if (value) // If Dead
            {
                if (myButtons != null) // If unit has buttons
                    Destroy(myButtons);
            }
        }
    }
}

public enum DragType
{
    NORMAL,
    ARCHER,
    FIREBALL,
    HEALING_BUFF,
}