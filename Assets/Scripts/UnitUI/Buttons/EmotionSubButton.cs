using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionSubButton : ButtonsUISubButtonBase
{
    public void Clicked()
    {
        NotifyOperation("show-emotion", "thumbsUp");
    }
}
