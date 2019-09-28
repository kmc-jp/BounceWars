using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Host to Client
public class UnitTimerCmd : Command
{
    public static readonly int MOVED = 0;
    public static readonly int HOST_LOCKDOWN = 1;
    public static readonly int CLIENT_LOCKDOWN = 2;

    public float time; // TODO
    public int timerType;

    public float penalty;

    public UnitTimerCmd()
    {
        type = 2;
    }
}
