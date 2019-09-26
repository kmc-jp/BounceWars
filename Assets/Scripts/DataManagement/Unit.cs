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
    public float HP;
    public float MP;
}

//Type:
//0 for elf_sword, use sword cion
//1 for dragon_archer, use archer icon