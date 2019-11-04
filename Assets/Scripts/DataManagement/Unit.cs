using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Unit
{
    public float x;
    public float z;
    public float x1;
    public float z1;
    public float vx;
    public float vz;
    public float vx1;
    public float vz1;
    public int uuid;
    public int type;
    public int owner;
    public float HP = 999;
    public float MP;
    public bool isDead = false;
}


//Type:(Legacy after Tinaxd use this for arrow)
//0 for elf_sword, use sword cion
//1 for dragon_archer, use archer icon
// Schin added UnitType uniform representation
public sealed class UnitType
{
    public static readonly int TYPE_CHESS = 0;
    public static readonly int TYPE_ARROW = 1;
    public static readonly int TYPE_UNDEFINED = -1;
}