using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShootSubButton : ButtonsUISubButtonBase
{
    public void Clicked()
    {
        NotifyOperation("start-archer-mode", null);
    }
}
