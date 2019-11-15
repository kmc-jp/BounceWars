using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestOpAll : OperationHandlerBase
{
    GameObject obelisk;

    private void Awake()
    {
        obelisk = GameObject.Find("Obelisk");
    }

    public override void OnMessage(string operation, object args)
    {
        if (operation == "start-archer-mode")
        {
            //Debug.Log("Start archer mode");
            Invoke("_EnableArcherMode", 0.1f);
        }
        else if (operation == "start-fireball-mode")
        {
            //Debug.Log("Start fireball mode");
            Invoke("_EnableFireballMode", 0.1f);
        }
        else if (operation == "start-healingbuff-mode")
        {
            //Debug.Log("Start healing buff mode");
            Invoke("_EnableHealingBuffMode", 0.1f);
        }
    }

    private void _EnableFireballMode()
    {
        //Unit.DragMode = DragType.FIREBALL;
    }

    private void _EnableArcherMode()
    {
        //Unit.DragMode = DragType.ARCHER;
    }

    private void _EnableHealingBuffMode()
    {
        //Unit.DragMode = DragType.HEALING_BUFF;
        ExecuteEvents.Execute<IDragAndFireEventHandler>(obelisk, null, (x, y) => x.TurnOffDrag());
    }

    private void Update()
    {
        //not used
        //heal is processed in Simulator.cs
        if (Unit.DragMode == DragType.HEALING_BUFF)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == "Unit")
                    {
                        var target = hit.transform.gameObject;
                        var targetScript = target.GetComponent<UnitInfoTag>();
                        if (targetScript.owned)
                        {
                            int myid = this.Unit.unit.uuid;
                            int targetid = targetScript.uuid;
                            var request = new HealingBuffRequestCmd()
                            {
                                RequestorId = myid,
                                TargetId = targetid,
                            };
                            obelisk.GetComponent<Simulator>().commands.Add(request);
                        }
                    }
                }
            }
        }
    }
}
