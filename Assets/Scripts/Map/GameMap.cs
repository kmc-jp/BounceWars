using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameMap {
    public int id;
    public int mapType;
    public Tile[][] mapData;
    public Dictionary<GameObject, Tile> gameObjectTable;
}

[System.Serializable]
public class Tile {
    public int type;
    public int buildingType = 0;    //タイルの上にあるもの(森、石など)のタイプ(何もなければ0)
    public int itemType = 0;
    public double height = 0.0;
    public Vector3 position;
    public Vector2Int index;//index of mapData
}