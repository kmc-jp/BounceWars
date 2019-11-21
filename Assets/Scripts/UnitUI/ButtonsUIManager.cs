using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonsUIManager : MonoBehaviour, IButtonSignalHandler
{
    public BasicUnit basicunit;

    [SerializeField]
    private DragType DefaultDragType = DragType.NORMAL;

    [SerializeField]
    private Vector2 Offset = new Vector2(-50, 25);

    [SerializeField]
    private List<GameObject> children = default;
    public List<GameObject> Children
    {
        get => children;
    }

    public bool disabled = false;

    private void Awake()
    {
    }

    public void CloseAll()
    {
        children.ForEach(c => 
            ExecuteEvents.Execute<IButtonSignalHandler>(c, null, (x, y) => x.CloseButton())
        );
    }

    public void OpenAll()
    {
        if (disabled)
            return;
        children.ForEach(c =>
            c.SetActive(true)
        );
    }

    public DragType CurrentDragType
    {
        get
        {
            var attackButton = transform.Find("AttackButton");
            if (attackButton == null)
                return DefaultDragType;
            return attackButton.GetComponent<ButtonsUIToggleButton>().CurrentDragType;
        }
    }

    public void CloseButton()
    {
        CloseAll();
    }

    public void OpenButton()
    {
        OpenAll();
    }
}
