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
            enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        Tile t = MapBehaviour.instance.GetTile(transform.position);
        if (t == null) return;
        bool shown = t.buildingType != 1||spotted;
        if (shown!=shown1)
        {
            shown1 = shown;
            for (int i = 0; i < visibleObjects.Count; i++)
            {
                //visibleObjects[i].SetActive(shown);
            }
        }
    }
    public bool spotted = false;
    bool shown1 = true;
}
