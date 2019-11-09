using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitChooser : MonoBehaviour
{
    [SerializeField]
    public List<int> UnitTypes;

    private bool IsSelected;

    public void SetSelected(bool b)
    {
        if (b)
        {
            GetComponentInParent<UnitChooserManager>().UnselectAll();
        }
        IsSelected = b;
    }

    public bool GetSelected()
    {
        return IsSelected;
    }
}
