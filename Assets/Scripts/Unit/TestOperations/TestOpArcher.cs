using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOpArcher : OperationHandlerBase
{
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
        //Unit.DragMode = DragType.ARCHER;
    }

    private void Update()
    {
    }
}
