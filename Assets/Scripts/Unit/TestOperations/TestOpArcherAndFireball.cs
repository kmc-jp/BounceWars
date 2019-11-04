using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOpArcherAndFireball : OperationHandlerBase
{
    public override void OnMessage(string operation, object args)
    {
        if (operation == "start-archer-mode")
        {
            Debug.Log("Start archer mode");
            Invoke("_EnableArcherMode", 0.1f);
        }
        else if (operation == "start-fireball-mode")
        {
            Debug.Log("Start fireball mode");
            Invoke("_EnableFireballMode", 0.1f);
        }
    }

    private void _EnableFireballMode()
    {
        Unit.DragMode = DragType.FIREBALL;
    }

    private void _EnableArcherMode()
    {
        Unit.DragMode = DragType.ARCHER;
    }

    private void Update()
    {
    }
}
