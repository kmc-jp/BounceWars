using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockdownHandler : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            var cmd = new UnitTimerCmd();
            cmd.time = Time.time;
            if (GetComponent<Simulator>().isClient == 0)
            {
                cmd.timerType = 1; // HOST LOCKDOWN
            }
            else
            {
                cmd.timerType = 2; // CLIENT LOCKDOWN
            }
            GetComponent<Simulator>().commands.Add(cmd);
            GetComponent<Simulator>().unitTimerRequests.Add(cmd);
        }
    }
}
