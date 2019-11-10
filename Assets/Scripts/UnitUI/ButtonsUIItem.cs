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
        gameObject.SetActive(false);
    }

    public void OpenButton()
    {
        gameObject.SetActive(true);
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
