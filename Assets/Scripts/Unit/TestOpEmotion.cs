using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOpEmotion : OperationHandlerBase
{
    public override void OnMessage(string operation, object args)
    {
        if (operation.Equals("show-emotion"))
        {
            Unit.ShowEmotion((string)args, 1.0f);
        }
    }
}
