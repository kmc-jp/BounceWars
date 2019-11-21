using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitChooserManager : MonoBehaviour
{
    public List<UnitChooser> groups;
    private void Start()
    {
        Debug.Log(gameObject.name);
        groups[0].SetSelected(true);
    }
    public void UnselectAll()
    {
        foreach (var group in groups)
        {
            group.SetSelected(false);
        }
    }

    public List<int> GetSelectedUnitTypes()
    {
        Debug.Log("Unit selected");
        for (int i = 0; i < groups.Count; i++)
        {
            if (groups[i].GetSelected())
            {
                return groups[i].UnitTypes;
            }
        }
        return null;
    }
}
