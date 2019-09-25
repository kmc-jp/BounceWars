using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Client to Host
public class UnitMovedCmd : Command
{
    public float when;
 //   public float x;
 //   public float z;
    public float vx;//is it velocity? idk
    public float vz;

 //   public float hp;
 //   public float mp;

    public int uuid;
    public int owner;//1 or 0
}
