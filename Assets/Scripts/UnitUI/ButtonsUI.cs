using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsUI : MonoBehaviour
{
    private BasicUnit target;

    public List<ButtonsUITopButton> topButtons;

    private bool active;

    public void SetVisibilityForce(bool visible)
    {
        if (!active)
        {
            topButtons.ForEach(i => i.NotifyActive(visible));
            this.gameObject.SetActive(visible);
        }
    }

    public void CloseSubButtons()
    {
        topButtons.ForEach(i => i.SetSubButtonActive(false));
    }

    private void _setInvisible()
    {
        SetVisibilityForce(false);
    }

    public void SetActive(bool active)
    {
        this.active = active;
        if(!active)
        {
            Target.UpdateButtonsDelay(0.1f);
        }
    }

    public BasicUnit Target
    {
        get => target;
        set
        {
            target = value;
            //Debug.Log("Set");
            topButtons.ForEach(button => button.Target = value);
        }
    }

    public void NotifyClickWithArgs(string operation, object args)
    {
        Target.NotifyOperation(operation, args);
    }

    public void NotifyClick(string operation)
    {
        NotifyClickWithArgs(operation, null);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
