using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideUnit : MonoBehaviour
{
    public List<GameObject> visibleObjects;
    public UnitInfoTag unit;
    private void Start()
    {
        if (unit.owned)
        {
            //enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        bool shown = MapBehaviour.instance.GetTile(transform.position).buildingType != 1;
        if (shown!=shown1)
        {
            shown1 = shown;
            for (int i = 0; i < visibleObjects.Count; i++)
            {
                visibleObjects[i].SetActive(shown);
            }
        }
    }
    bool shown1 = true;
}
