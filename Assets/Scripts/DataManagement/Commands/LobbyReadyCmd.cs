using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyReadyCmd : Command
{
    public int isReady = 0;
    public LobbyReadyCmd()
    {
        isReady = 0;
        type = 101;
    }
    public LobbyReadyCmd(int r)
    {
        isReady = r;
        type = 101;
    }
}
