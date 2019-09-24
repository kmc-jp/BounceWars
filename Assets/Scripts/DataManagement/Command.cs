using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Command
{
    public bool processed;
    public int type;//0: move 1:set hp and mp
    public bool sent = true;
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
