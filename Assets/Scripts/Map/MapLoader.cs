using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LitJson;

public class MapLoader
{
    public static GameMap loadMap(string fileName)
    {
        TextAsset textasset = new TextAsset(); //テキストファイルのデータを取得するインスタンスを作成
        textasset = Resources.Load(fileName, typeof(TextAsset)) as TextAsset; //Resourcesフォルダから対象テキストを取得
        string mapJson = textasset.text; //テキスト全体をstring型で入れる変数を用意して入れる
        GameMap map = JsonMapper.ToObject<GameMap>(mapJson);
        generateTiles(map);
        return map;
    }

    public static void generateTiles(GameMap map)
    {
        // CubeプレハブをGameObject型で取得
        GameObject[] tiles = {
                (GameObject)Resources.Load ("Tiles/Tile_Grass"),  //0
                (GameObject)Resources.Load ("Tiles/Tile_Sea"),    //1
                (GameObject)Resources.Load ("Tiles/Tile_Sand"),   //2
                (GameObject)Resources.Load ("Tiles/Tile_Brick"),  //3
                (GameObject)Resources.Load ("Tiles/Tile_Clay"),   //4
                (GameObject)Resources.Load ("Tiles/Tile_Lava"),   //5
                (GameObject)Resources.Load ("Tiles/Tile_Rock") }; //6
        GameObject[] buildings = {
                null,
                (GameObject)Resources.Load ("Buildings/Building_Forest"),
                (GameObject)Resources.Load ("Buildings/Building_Stones_1"),
                (GameObject)Resources.Load ("Buildings/Building_Stones_2"),
                (GameObject)Resources.Load ("Buildings/Building_Stones_3"),};
        // Cubeプレハブを元に、インスタンスを生成、
        //Object.Instantiate(obj, new Vector3(0.0f,0.0f,0.0f), Quaternion.identity);
        Dictionary<GameObject, Tile> gameObjectTable = new Dictionary<GameObject, Tile>();

        for (int q = 0; q < map.mapData.Length; q++)
        {
            for (int p = 0; p < map.mapData[q].Length; p++)
            {
                float offset = 0;
                if (q % 2 == 0) offset = 1;
                Tile curTile = map.mapData[q][p];
                int type = curTile.type;

                Vector3 position = new Vector3(p * 2 + offset - 10, -0.5f + (float)curTile.height, q * 1.5f * 1.1547f);
                GameObject mapGenObj = GameObject.Find("Obelisk");
                GameObject g = UnityEngine.Object.Instantiate(tiles[type], position, Quaternion.Euler(-90, 0, 0), mapGenObj.transform);
                GameObject curBuilding = buildings[curTile.buildingType];
                gameObjectTable.Add(g, curTile);
                curTile.position = position;
                curTile.index = new Vector2Int(q, p);
                if (curBuilding != null)
                {
                    Vector3 position1 = new Vector3(p * 2 + offset - 10, -0.5f + (float)curTile.height, q * 1.5f * 1.1547f);
                    UnityEngine.Object.Instantiate(curBuilding, position1, new Quaternion(0, 0, 0, 0), mapGenObj.transform);

                }
            }
        }
        map.gameObjectTable = gameObjectTable;
    }
}
