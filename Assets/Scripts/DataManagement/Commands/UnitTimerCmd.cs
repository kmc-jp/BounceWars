using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Host to Client
public class UnitTimerCmd : Command
{
    public static readonly int MOVED = 0;
    public static readonly int LOCKDOWN = 1;

    public float time; // TODO
    public int timerType;

    public float penalty;

    public UnitTimerCmd()
    {
        type = 2;
    }
}
