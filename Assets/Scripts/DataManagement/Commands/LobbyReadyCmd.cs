using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyReadyCmd : Command
{
    public int isReady = 0;
    public string opponentName;
    public List<int> unitTypes;

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
    public LobbyReadyCmd(int r, string name)
    {
        isReady = r;
        opponentName = name;
        type = 101;
    }

    public LobbyReadyCmd(int r, string name, List<int> unittypes)
    {
        isReady = r;
        opponentName = name;
        unitTypes = unittypes;
    }
}
