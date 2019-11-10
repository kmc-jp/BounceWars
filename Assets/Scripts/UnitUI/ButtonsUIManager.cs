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

    private List<GameObject> children;

    public bool disabled = false;

    private void Awake()
    {
        children = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i).gameObject);
        }
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
            c.GetComponent<IButtonSignalHandler>().OpenButton()
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

    private void LateUpdate()
    {
        transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, basicunit.transform.position) + Offset;
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
