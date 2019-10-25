using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOpHandler : OperationHandlerBase
{
    public override void OnMessage(string operation, object args)
    {
        Debug.Log("Operation: " + operation + " (" + args + ")");
    }
}
