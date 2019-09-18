using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class OperationHandlerBase : MonoBehaviour
{
    abstract public void OnMessage(string operation, object args);

    protected BasicUnit Unit
    {
        get => GetComponent<BasicUnit>();
    }
}
