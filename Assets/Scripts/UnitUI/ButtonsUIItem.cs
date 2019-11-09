using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsUIItem : MonoBehaviour, IButtonSignalHandler
{
    [SerializeField]
    List<Sprite> ItemSprites;

    public ButtonsUIItem()
    {
        ItemSprites = new List<Sprite>();
    }

    public void CloseButton()
    {
        this.enabled = false;
    }

    public void OpenButton()
    {
        this.enabled = true;
    }

    public void SetItemByIndex(int i)
    {
        if (i == -1)
        {
            // TODO: Unset sprite
        }
        GetComponent<Image>().sprite = ItemSprites[i];
    }
}
