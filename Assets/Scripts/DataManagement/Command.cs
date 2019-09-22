using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Command
{
    public bool processed;
    public int type;//0: move 1:set hp and mp
    public bool sent = true;
}
