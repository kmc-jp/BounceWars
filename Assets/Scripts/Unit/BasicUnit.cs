using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private void Awake()
    {
        unitUI = transform.Find("UnitUI").gameObject.GetComponent<UnitUI>();
        //Debug.Log(transform.Find("UnitUI").gameObject.GetComponent<UnitUI>());

        unitUI.HPMax = 50;
        unitUI.MPMax = 100;
        HP = 50;
        MP = 100;
    }

    // Start is called before the first frame update
    void Start()
    {
        myButtons = Instantiate(buttons, new Vector3(0, 0, 0), Quaternion.identity);
        myButtons.transform.SetParent(buttonsUI.transform);
        myButtons.GetComponent<ButtonsUI>().Target = this;
        myButtons.GetComponent<ButtonsUI>().SetVisibilityForce(false);
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
