using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOpArcher : OperationHandlerBase
{
    private bool archerMode;
    public bool ArcherMode 
    {
        get => archerMode;
    }

    private Simulator simulator;

    public override void OnMessage(string operation, object args)
    {
        if (operation == "start-archer-mode")
        {
            Debug.Log("Start archer mode");
            Invoke("_EnableArcherMode", 0.1f);
        }
    }

    private void _EnableArcherMode()
    {
        archerMode = true;
        // TODO: Change mouse cursor
    }

    private void Awake()
    {
        simulator = GameObject.Find("Obelisk").GetComponent<Simulator>();
    }

    private void Update()
    {
        if (archerMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit))
                {
                    var targetPlane = new Plane(Vector3.up, this.transform.position);
                    float enter = 0;
                    targetPlane.Raycast(ray, out enter);
                    var localOrigin = ray.GetPoint(enter);
                    //Debug.Log("Arrow sent: " + transform.position + " -> " + localOrigin);
                    SendArrow(localOrigin);
                    archerMode = false;
                }
            }
        }
    }

    private void SendArrow(Vector3 destination)
    {
        NewUnitCmd cmd = new NewUnitCmd
        {
            fromUnitId = GetComponent<BasicUnit>().unit.uuid,
            to = destination
        };

        simulator.commands.Add(cmd);
    }
}
