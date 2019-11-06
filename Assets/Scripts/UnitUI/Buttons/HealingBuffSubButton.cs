using UnityEngine;

public class HealingBuffSubButton : ButtonsUISubButtonBase
{
    public void Clicked()
    {
        NotifyOperation("start-healingbuff-mode", null);
    }
}
