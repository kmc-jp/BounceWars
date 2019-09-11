using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class ButtonsUISubButtonBase : MonoBehaviour
{
    public ButtonsUITopButton TopButton;

    protected void NotifyOperation(string operation, object args)
    {
        TopButton.GetComponentInParent<ButtonsUI>().NotifyClickWithArgs(operation, args);
    }

    protected void CloseButtonsUI()
    {
        TopButton.GetComponentInParent<ButtonsUI>().SetVisibilityForce(false);
    }
}
