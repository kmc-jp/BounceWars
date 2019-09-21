using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollisionInfo
{
    public Unit me;
    public Unit other;
    public float vx1;//me
    public float vz1;//me
    public float vx2;//other
    public float vz2;//other
}
