using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetCmd : Command
{
    public int isHostWin;
    public GameSetCmd()
    {

    }
    public GameSetCmd(int iHW)
    {
        isHostWin = iHW;
    }
}
