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
}