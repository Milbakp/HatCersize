using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class RPGLevelLoader : MonoBehaviour
{
    public List<GameObject> Objects = new List<GameObject>();
    public GameObject tile;
    private string savePath;
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
            if(td.tileID == 1)
                Instantiate(tile, new Vector3(td.x, 0, td.z), Quaternion.identity);
            else if (td.tileID == 2)
                Instantiate(Objects[0], new Vector3(td.x, 0, td.z), Quaternion.identity);
            else if (td.tileID == 3)
                Instantiate(Objects[1], new Vector3(td.x, 0, td.z), Quaternion.identity);
        }
        Instantiate(newLevel.MapSize, new Vector3(newLevel.MapSize.transform.position.x, 0, newLevel.MapSize.transform.position.z), Quaternion.identity);
    }
}
