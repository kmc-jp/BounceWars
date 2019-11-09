using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonsUIManager : MonoBehaviour
{
    public BasicUnit basicunit;

    [SerializeField]
    private DragType DefaultDragType = DragType.NORMAL;

    private bool CursorOn = false;
    private bool CloseRequestFromBasicUnit = false;

    [SerializeField]
    private Vector2 Offset = new Vector2(10, 8);

    public void CloseAll()
    {
        ExecuteEvents.ExecuteHierarchy<IButtonSignalHandler>(this.gameObject, null, (x, y) => x.CloseButton());
        CloseRequestFromBasicUnit = false;
        CursorOn = false;
    }

    public void OpenAll()
    {
        ExecuteEvents.ExecuteHierarchy<IButtonSignalHandler>(this.gameObject, null, (x, y) => x.OpenButton());
        CloseRequestFromBasicUnit = false;
        CursorOn = true;
    }

    public void CloseRequest()
    {
        if (!CursorOn)
            CloseAll();
        else
            CloseRequestFromBasicUnit = true;
    }

    public void InternalCloseRequest()
    {
        CursorOn = false;
        if (CloseRequestFromBasicUnit)
            CloseAll();
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
}
