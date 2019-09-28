using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapBehaviour : MonoBehaviour
{
    public string mapname = "Map_Grassland";
    //public string jsonData = "{\"id\": 1, \"mapType\": 3, \"mapData\":[[{\"type\": 1}, {\"type\": 1}, {\"type\": 1}], [{\"type\": 1}, {\"type\": 1}, {\"type\": 1}], [{\"type\": 1}, {\"type\": 1}, {\"type\": 1}]]}";
    //public string jsonData = "{\"id\": 1, \"mapType\": 3}";
    GameMap map;

    // Start is called before the first frame update
    void Start()
    {
        MapLoader.loadMap(mapname);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
