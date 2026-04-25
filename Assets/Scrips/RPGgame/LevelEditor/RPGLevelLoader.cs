using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RPGLevelLoader : MonoBehaviour
{
    public GameObject tile;
    private string savePath;
    public TileRegistry registry;
    public GameObject player;
    public GameObject destination;
    public RPGLevelManager levelManager;
    public CharacterController controller;
    private GameManager gameManager;

    public void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        Debug.LogError("Current Game Mode in RPGLevelLoader Awake: " + gameManager.CurrentMode);
    }

    void Start()
    {
        //savePath = Path.Combine(Application.persistentDataPath, "level.json");
        //savePath = PlayerPrefs.GetString("PlayerMadeLevelPath", "");
        if(gameManager.CurrentMode == GameManager.GameMode.CustomLevel)
        {
            LevelData loadedLevel = LoadLevel();
            createLevel(loadedLevel);
        }
        else if(gameManager.CurrentMode == GameManager.GameMode.Campaign)
        {
            createCampaignLevel();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public LevelData LoadLevel()
    {
        return GameManager.Instance.LevelToLoad; // Get the level data from GameManager
    }
    public void createLevel(LevelData levelData)
    {
        LevelData newLevel = levelData;
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
                Instantiate(prefab, new Vector3(td.x, td.y, td.z), Quaternion.Euler(0, td.rotationY, 0));
            }
        }

        controller.enabled = false; 
        player.transform.position =  new Vector3(newLevel.playerStartPosition.x, player.transform.position.y, newLevel.playerStartPosition.z);
        player.transform.rotation = Quaternion.Euler(0, newLevel.playerRotationY, 0);
        controller.enabled = true;
        
        destination.transform.position = newLevel.destinationPosition;
        destination.transform.rotation = Quaternion.Euler(0, newLevel.destinationRotationY, 0);

        // Getting initial enemy count
        GameObject[]enemiesArray = GameObject.FindGameObjectsWithTag("Enemy");
        int numOfEnemies = enemiesArray.Length;
        Debug.LogError("Number of Enemies at Start: " + numOfEnemies);
        levelManager.numOfEnemies = numOfEnemies;
    }

    public void createCampaignLevel()
    {
        foreach(LevelEntry entry in gameManager.CampaignToLoad.levels)
        {
            if(entry.order == gameManager.CurrentCampaignLevelIndex + 1) // Load the next level in the campaign
            {
                createLevel(entry.levelData);
                break;
            }
        }
    }
}
