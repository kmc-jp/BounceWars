using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapBehaviour : MonoBehaviour
{
    public string mapname = "Map_Grassland";
    //public string jsonData = "{\"id\": 1, \"mapType\": 3, \"mapData\":[[{\"type\": 1}, {\"type\": 1}, {\"type\": 1}], [{\"type\": 1}, {\"type\": 1}, {\"type\": 1}], [{\"type\": 1}, {\"type\": 1}, {\"type\": 1}]]}";
    //public string jsonData = "{\"id\": 1, \"mapType\": 3}";
    public GameMap map;
    
    // Start is called before the first frame update
    void Awake()
    {
        map=MapLoader.loadMap(mapname);
    }

    public Tile GetTile(Vector3 position)
    {
        Ray ray = new Ray();
        Vector3 origin = position;
        origin.y = 10;
        ray.origin = origin;
        ray.direction = Vector3.down;
        RaycastHit hit;
        Tile t=null;
        if(Physics.Raycast(ray,out hit, 20f,LayerMask.GetMask("Map"))){
            map.gameObjectTable.TryGetValue(hit.transform.gameObject, out t);
        }
        return t;
    }
}
