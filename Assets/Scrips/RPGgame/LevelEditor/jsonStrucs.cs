using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData
{
    public int tileWidth = 5;
    public int tileHeight = 5;
    public float x;
    public float y;
    public float z;
    public int tileID;
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
    public Vector3 playerStartPosition;
    public Vector3 destinationPosition;
}