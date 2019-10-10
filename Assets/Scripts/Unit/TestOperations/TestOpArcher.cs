using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOpArcher : OperationHandlerBase
{
    public override void OnMessage(string operation, object args)
    {
        if (operation == "shoot-arrow")
        {
            // Start archer mode
        }
        else if (operation == "special_click")
        {
            var clickPos = (Vector3)args;
            // Instantiate arrow obj and send it to clickPos
        }
    }
}
