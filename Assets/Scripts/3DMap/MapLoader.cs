using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LitJson;

public class MapLoader
{
    public static GameMap loadMap(string fileName){
        TextAsset textasset = new TextAsset(); //テキストファイルのデータを取得するインスタンスを作成
        textasset = Resources.Load(fileName, typeof(TextAsset) )as TextAsset; //Resourcesフォルダから対象テキストを取得
        string mapJson = textasset.text; //テキスト全体をstring型で入れる変数を用意して入れる
        GameMap map = JsonMapper.ToObject<GameMap>(mapJson);
        generateTiles(map);
        return map;
    }

    public static void generateTiles(GameMap map){
        // CubeプレハブをGameObject型で取得
        GameObject[] tiles = {
                (GameObject)Resources.Load ("Tile_Grass"),
                (GameObject)Resources.Load ("Tile_Sea"),
                (GameObject)Resources.Load ("Tile_Sand"),
                (GameObject)Resources.Load ("Tile_Brick"),
                (GameObject)Resources.Load ("Tile_Clay"),
                (GameObject)Resources.Load ("Tile_Lava") };
        GameObject[] buildings = {
                null,
                (GameObject)Resources.Load ("Building_Forest"),
                (GameObject)Resources.Load ("Building_Stones_1"),
                (GameObject)Resources.Load ("Building_Stones_2"),
                (GameObject)Resources.Load ("Building_Stones_3"),};
        // Cubeプレハブを元に、インスタンスを生成、
        //Object.Instantiate(obj, new Vector3(0.0f,0.0f,0.0f), Quaternion.identity);

        for(int q = 0; q < map.mapData.Length; q++){
            for(int p = 0; p < map.mapData[q].Length; p++){
                float offset = 0;
                if(q % 2 == 0) offset = 1;
                Tile curTile = map.mapData[q][p];
                int type = curTile.type;
                GameObject mapGenObj = GameObject.Find("Obelisk");
                UnityEngine.Object.Instantiate(tiles[type], new Vector3(p*2 + offset - 10,-0.5f + (float)curTile.height,q*1.5f*1.1547f), Quaternion.Euler(-90,0,0), mapGenObj.transform);
                GameObject curBuilding = buildings[curTile.buildingType];
                if(curBuilding != null)
                    UnityEngine.Object.Instantiate(curBuilding, new Vector3(p*2 + offset - 10,-0.5f + (float)curTile.height,q*1.5f*1.1547f), new Quaternion(0,0,0,0), mapGenObj.transform);
                
            }
        }
    }
}
