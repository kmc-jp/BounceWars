using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetCmd : Command
{
    public bool isHostWin;

    public GameSetCmd(bool iHW)
    {
        isHostWin = iHW;
    }
}
