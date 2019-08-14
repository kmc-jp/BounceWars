using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Command
{
    public bool processed;
    public int type;
    public bool sent = true;
    public float when;
    public float x;
    public float z;
    public float vx;
    public float vz;
    public int uuid;
    public int owner;//1 or 0
}
