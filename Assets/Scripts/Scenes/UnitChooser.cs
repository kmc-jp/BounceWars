using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitChooser : MonoBehaviour
{
    [SerializeField]
    public List<int> UnitTypes;

    public bool IsSelected;

    public Color ColorSelected = Color.green;
    public Color ColorUnselected = Color.gray;

    private void Awake()
    {
        SetSelected(IsSelected);
    }

    public void SetSelected(bool b)
    {
        if (b)
        {
            GetComponentInParent<UnitChooserManager>().UnselectAll();
        }
        IsSelected = b;
        transform.Find("ClickDetector").GetComponent<Image>().color = b ? ColorSelected : ColorUnselected;
    }

    public bool GetSelected()
    {
        return IsSelected;
    }
}
