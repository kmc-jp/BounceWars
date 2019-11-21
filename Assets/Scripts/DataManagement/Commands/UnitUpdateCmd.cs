using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Host to Client
public class UnitUpdateCmd : Command
{
    public List<Unit> units;
    public float time;
    public UnitUpdateCmd()
    {
        type = -1;
        sent = false;
    }
}
