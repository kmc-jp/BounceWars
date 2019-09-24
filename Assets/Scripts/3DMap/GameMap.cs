using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameMap {
    public int id;
    public int mapType;
    public Tile[][] mapData;
}

[System.Serializable]
public class Tile {
    public int type;
    public int buildingType = 0;    //タイルの上にあるもの(森、石など)のタイプ(何もなければ0)
    public double height = 0.0;
}