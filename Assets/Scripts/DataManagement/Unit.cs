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
    public int buff;
}

//Type:(Legacy after Tinaxd use this for arrow)
//0 for elf_sword, use sword cion
//1 for dragon_archer, use archer icon
//2 for fireball
// Schin added UnitType uniform representation
public sealed class UnitType
{
    public static readonly int TYPE_CHESS = 0;
    public static readonly int TYPE_ARCHER = 1;
    public static readonly int TYPE_ARROW = 2;
    public static readonly int TYPE_FIREBALL = 3;
    public static readonly int TYPE_UNDEFINED = -1;
    public static readonly int TYPE_ITEM_HEAL = 5;
    public static readonly int TYPE_ITEM_MPOT = 6;
    public static readonly int TYPE_ITEM_LPOT = 7;
    public static bool isItem(int type) {
        return type == TYPE_ITEM_HEAL || type == TYPE_ITEM_MPOT || type == TYPE_ITEM_LPOT;
    }
}

// Tinaxd added BuffFlag
public sealed class BuffFlag
{
    public static readonly int BUFF_HEALING = 1;
    //2
    //4
    //8
    //16
    //2^n
}