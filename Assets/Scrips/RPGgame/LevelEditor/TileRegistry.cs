using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileRegistry", menuName = "LevelSystem/TileRegistry")]
public class TileRegistry : ScriptableObject
{
    [System.Serializable]
    public struct TileEntry
    {
        public int id;
        public GameObject prefab;
    }

    public List<TileEntry> entries;

    public GameObject GetPrefab(int id)
    {
        return entries.Find(e => e.id == id).prefab;
    }
}
