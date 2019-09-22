using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitUpdateCmd : Command
{
    public float when;
    public float x;
    public float z;
    public float vx;
    public float vz;

    public float hp;
    public float mp;

    public int uuid;
    public int owner;//1 or 0
}
