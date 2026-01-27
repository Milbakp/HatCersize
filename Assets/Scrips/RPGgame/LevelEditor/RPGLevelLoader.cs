using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class RPGLevelLoader : MonoBehaviour
{
    public GameObject tile;
    private string savePath;
    public TileRegistry registry;
    public GameObject player;
    public GameObject destination;
    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "level.json");
        createLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public LevelData LoadLevel()
    {
        if (File.Exists(savePath))
        {
            // 1. Read the text from the file
            string json = File.ReadAllText(savePath);

            // 2. Convert the text back into a LevelData object
            LevelData data = JsonUtility.FromJson<LevelData>(json);
            return data;
        }
        else
        {
            Debug.LogError("Save file not found!");
            return null;
        }
    }
    public void createLevel()
    {
        LevelData newLevel = LoadLevel();
        if (newLevel == null)
        {
            Debug.LogError("Could not load level");
            return;
        }
        foreach(TileData td in newLevel.tiles)
        {
            GameObject prefab = registry.GetPrefab(td.tileID);
            if (prefab != null)
            {
                // Temporary fix for enemy spawn height
                if (prefab.CompareTag("Enemy")){
                    Instantiate(prefab, new Vector3(td.x, 3, td.z), Quaternion.identity);
                    continue;
                }
                Instantiate(prefab, new Vector3(td.x, 0, td.z), Quaternion.identity);
            }
        }
        player.transform.position = newLevel.playerStartPosition;
        destination.transform.position = newLevel.destinationPosition;
    }
}
