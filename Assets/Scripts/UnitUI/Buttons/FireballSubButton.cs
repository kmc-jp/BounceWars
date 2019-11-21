using UnityEngine;

public class FireballSubButton : ButtonsUISubButtonBase
{
    public void Clicked()
    {
        NotifyOperation("start-fireball-mode", null);
    }
}
