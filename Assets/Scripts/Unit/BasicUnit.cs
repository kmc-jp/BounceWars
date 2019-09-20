using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Original author: Tinaxd
// Attach this script to all units
public class BasicUnit : MonoBehaviour
{
    private float hp;
    private float mp;

    private UnitUI unitUI;

    public GameObject damagePopup;
    public GameObject canvas;
    public GameObject buttonsUI;
    public GameObject buttons;

    public float buttonsUICloseDelay = 0.3f;

    private GameObject myButtons;

    private bool mouseOn;


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

    public float LockdownStartTime;
    public float LastMoveTime;


    public float HP
    {
        get => hp;
        set
        {
            this.hp = value;
            unitUI.HP = value;
        }
    }

    public float MP
    {
        get => mp;
        set
        {
            this.mp = value;
            unitUI.MP = value;
        }
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

    private void Awake()
    {
        unitUI = transform.Find("UnitUI").gameObject.GetComponent<UnitUI>();
        //Debug.Log(transform.Find("UnitUI").gameObject.GetComponent<UnitUI>());

        unitUI.HPMax = 50;
        unitUI.MPMax = 100;
        HP = 50;
        MP = 100;

        WaitTime = 0;

        var iconTest = new string[] { "archer", "fire", "gear", "scout", "shield" };
        var iconTestIndex = Random.Range(0, 5);
        CountDownIconPath = "test/" + iconTest[iconTestIndex];

        // bind CountDownUI OBJ Tinaxd
        countDownBar = GameObject.Find("CountDownUIObj").GetComponentInChildren<CountDownBar>();

    }

    // Start is called before the first frame update
    void Start()
    {
        // bind ButtonsUI OBJ Schin
        buttonsUI = GameObject.Find("ButtonsUIObj");

        myButtons = Instantiate(buttons, new Vector3(0, 0, 0), Quaternion.identity);
        myButtons.transform.SetParent(buttonsUI.transform);
        myButtons.GetComponent<ButtonsUI>().Target = this;
        myButtons.GetComponent<ButtonsUI>().SetVisibilityForce(false);
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;
        if (Locked)
        {
            WaitTime -= delta;
        }

        // Lockdown
        if (!LockdownPenalty && Input.GetKeyDown(KeyCode.L))
            MarkLockdown();

        if (LockdownPenalty)
        {
            UpdateWaitTimeText();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Attacked!");
        float damage = Random.Range(5, 25);
        HP = HP - damage;
        PopupDamage(damage);
    }


    private void PopupDamage(float damage)
    {
        Debug.Log("HP: " + HP);

        GameObject popup = Instantiate(damagePopup, Vector3.zero, Quaternion.identity);
        popup.transform.SetParent(canvas.transform);
        popup.GetComponent<DamagePopup>().Unit = this.gameObject;
        popup.GetComponent<DamagePopup>().Canvas = canvas.GetComponent<Canvas>();
        popup.GetComponent<DamagePopup>().Text = (-damage).ToString();
    }

    private void OnMouseEnter()
    {
        mouseOn = true;
        if (!Locked)
            myButtons.GetComponent<ButtonsUI>().SetVisibilityForce(true);
    }

    private void OnMouseExit()
    {
        mouseOn = false;
        Invoke("DisableButtonsUI", buttonsUICloseDelay);
    }

    public void UpdateButtonsDelay(float delay)
    {
        Invoke("UpdateButtons", delay);
    }

    private void UpdateButtons()
    {
        if (!mouseOn)
            Invoke("DisableButtonsUI", buttonsUICloseDelay);
    }

    private void DisableButtonsUI()
    {
        myButtons.GetComponent<ButtonsUI>().SetVisibilityForce(false);
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
    }

    // Emotion Icon Tinaxd
    public void ShowEmotion(string emotionName, float length)
    {
        unitUI.ShowEmotion(emotionName, length);
    }
}