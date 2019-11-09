using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndFire : MonoBehaviour, IDragAndFireEventHandler
{
    GameObject target;
    UnitInfoTag targetScript;
    Unit targetUnit;
    bool grabbing;
    Plane targetPlane = new Plane(Vector3.up, 0);
    Vector3 localOrigin;
    public Simulator simulator;

    private bool DragEnabled = true;

    // Update is called once per frame
    void Update()
    {
        if (!DragEnabled)
        {
            grabbing = false;
            target = null;
            targetScript = null;
            return;
        }
        if (target != null && Input.GetKeyDown(KeyCode.Escape)) // Tinaxd Press Esc to cancel dragging
        {
            target.GetComponent<BasicUnit>().NotifyDragEnd();
            grabbing = false;
            target = null;
            targetScript = null;
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Unit")
                {
                    target = hit.transform.gameObject;
                    targetScript = hit.transform.GetComponent<UnitInfoTag>();
                    grabbing = true;
                    targetPlane = new Plane(Vector3.up, target.transform.position);
                    float enter = 0;
                    targetPlane.Raycast(ray, out enter);
                    localOrigin = ray.GetPoint(enter);
                    targetUnit = simulator.GetUnit(targetScript.uuid);
                    if (targetUnit.owner != simulator.isClient)//0:host 1:client
                    {
                        grabbing = false;
                        target = null;
                        targetScript = null;
                    }
                    else // Tinaxd show DragUI
                    {
                        target.GetComponent<BasicUnit>().NotifyDragStart();
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (grabbing)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float enter = 0;
                targetPlane.Raycast(ray, out enter);
                Debug.Log(localOrigin - ray.GetPoint(enter));
                Vector3 vel = localOrigin - ray.GetPoint(enter);
                DragType dt = target.GetComponent<BasicUnit>().DragMode;
                switch (dt) 
                {
                    case DragType.NORMAL:
                        UnitMovedCmd c = new UnitMovedCmd();
                        c.sent = false;
                        c.vx = vel.x;
                        c.vz = vel.z;
                        c.uuid = targetUnit.uuid;
                        c.owner = simulator.isClient;

                        simulator.commands.Add(c);
                        break;

                    case DragType.ARCHER:
                        target.GetComponent<BasicUnit>().DragMode = DragType.NORMAL;
                        NewUnitCmd cmd = new NewUnitCmd
                        {
                            fromUnitId = target.GetComponent<BasicUnit>().unit.uuid,
                            velocity = new Vector3(vel.x*3, 0, vel.z*3),
                            unitType = 2, // Unit type Arrow
                        };

                        simulator.commands.Add(cmd);
                        break;

                    case DragType.FIREBALL:
                        target.GetComponent<BasicUnit>().DragMode = DragType.NORMAL;
                        NewUnitCmd cmd2 = new NewUnitCmd
                        {
                            fromUnitId = target.GetComponent<BasicUnit>().unit.uuid,
                            velocity = new Vector3(vel.x * 3, 0, vel.z * 3),
                            unitType = 3, // Unit type Fireball
                        };

                        simulator.commands.Add(cmd2);
                        break;
                }
                //targetScript.rg.AddForce(vel, ForceMode.VelocityChange);
                /*
                CommandData c = new CommandData();
                c.sent = false;
                c.when = manager.elapsedTime;
                c.vx = vel.x;
                c.vz = vel.z;
                c.x = targetScript.transform.position.x;
                c.z = targetScript.transform.position.z;

                targetScript.command = c;*/
                grabbing = false;
                target.GetComponent<BasicUnit>().NotifyDragEnd(); // Tinaxd disable DragUI
            }
        }
        // Tinaxd update DragUI
        if (grabbing)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var basicUnit = target.GetComponent<BasicUnit>();
            float enter = 0;
            targetPlane.Raycast(ray, out enter);
            var point = ray.GetPoint(enter);
            basicUnit.NotifyDragUpdate(point);
        }
    }

    public void TurnOnDrag()
    {
        DragEnabled = true;
        //Debug.Log("Drag detection enabled");
    }

    public void TurnOffDrag()
    {
        DragEnabled = false;
        //Debug.Log("Drag detection disabled");
    }
}
