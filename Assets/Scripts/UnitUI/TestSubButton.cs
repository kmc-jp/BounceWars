using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSubButton : ButtonsUISubButtonBase
{
    public void OnClick()
    {
        NotifyOperation("CLICKED!!", null);
    }
}
