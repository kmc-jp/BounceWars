using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyStartgameCmd : Command
{
    public string mapName;
    public List<Unit> hostUnits;
    public List<Unit> clientUnits;

    public LobbyStartgameCmd(string mapname)
    {
        //set type
        type = 105;
        mapName = mapname;
        hostUnits = new List<Unit>();
        clientUnits = new List<Unit>();
    }
    //other settings
}
