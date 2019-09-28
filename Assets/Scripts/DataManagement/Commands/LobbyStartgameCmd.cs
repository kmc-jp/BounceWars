using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyStartgameCmd : Command
{
    public int mapID;
    public List<Unit> hostUnits;
    public List<Unit> clientUnits;

    public LobbyStartgameCmd()
    {
        mapID = 0;
        hostUnits = new List<Unit>();
        clientUnits = new List<Unit>();
    }
    //other settings
}
