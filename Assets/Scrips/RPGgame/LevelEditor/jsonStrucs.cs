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
    public float rotationY;
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
    public string fileType;
    public List<TileData> tiles = new List<TileData>();
    // Player info
    public Vector3 playerStartPosition;
    public float playerRotationY;
    // Endpoint info
    public Vector3 destinationPosition;
    public float destinationRotationY;
}

[System.Serializable]
public class LevelEntry
{
    public int order;
    public string name;
    public LevelData levelData;

    public LevelEntry(int order, string name, LevelData levelData)
    {
        this.order = order;
        this.name = name;
        this.levelData = levelData;
    }
}

[System.Serializable]
public class CampaignData
{
    public string fileType;
    public string campaignTitle;
    public List<LevelEntry> levels = new List<LevelEntry>(); 
}