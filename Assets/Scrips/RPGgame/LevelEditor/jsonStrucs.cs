using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData
{
    public int tileWidth = 5;
    public int tileHeight = 5;
    public int x;
    public int z;
    public int tileID; // e.g., 1 = Grass, 2 = Wall
}
[System.Serializable]
public class aboveObjects
{
    public int x;
    public int z;
    public int ID;
}

[System.Serializable]
public class LevelData
{
    public List<TileData> tiles = new List<TileData>();
    public int MapSize;
}