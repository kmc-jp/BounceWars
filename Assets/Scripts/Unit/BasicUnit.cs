using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Original author: Tinaxd
// Attach this script to all units
public class BasicUnit : MonoBehaviour
{
    public Unit unit;

    public bool isDead = false;
    protected float hp;
    protected float mp;

    protected UnitUI unitUI;

    public GameObject damagePopup;
    public GameObject canvas;
    public GameObject buttonsUI;
    public GameObject buttons;

    public float buttonsUICloseDelay = 0.3f;

    private GameObject myButtons;

    public bool MouseOn;

    public Tile currentTile;

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

    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //HP = 50;
        //MP = 100;
        // bind ButtonsUI OBJ Schin
        buttonsUI = GameObject.Find("ButtonsUIObj");

        myButtons = Instantiate(buttons, new Vector3(0, 0, 0), Quaternion.identity);
        myButtons.transform.SetParent(buttonsUI.transform);
        myButtons.GetComponent<ButtonsUI>().Target = this;
        myButtons.GetComponent<ButtonsUI>().UpdateActive();

        // Tinaxd register units to CountDownBar
        countDownBar.RegisterUnit(this, CountDownIconPath);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        currentTile = MapBehaviour.instance.GetTile(transform.position);
        HP = unit.HP;
        MP = unit.MP;

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
        if (!Locked)
            myButtons.GetComponent<ButtonsUI>().UpdateActive();
    }

    private void OnMouseExit()
    {
        MouseOn = false;
        myButtons.GetComponent<ButtonsUI>().UpdateActive();
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
        unitUI.ShowEmotion(emotionName, length);
    }
    public void CollisionEvent(CollisionInfo info)
    {
        Debug.Log("CollisionEvent");

        Debug.Log("Attacked!");
        float damage =Mathf.Abs(info.normalVelocity);
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
        myButtons.GetComponent<ButtonsUI>().UpdateActive();
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
    }

    public DragType DragMode = DragType.NORMAL;
}

public enum DragType
{
    NORMAL,
    ARCHER,
    FIREBALL,
}