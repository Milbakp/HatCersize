using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

#if ENABLE_WINMD_SUPPORT
using Windows.Storage;
using Windows.Storage.Pickers;
using System;
using System.Threading.Tasks; // Added for Task support
#endif

public class RPGLevelSelect : MonoBehaviour
{
    public int levelIndex;
    public GameObject weaponEquipVisual;
    public GameManager gameManager;
    public SoundManager soundManager;
    public GameObject levelScrollView, campaignScrollView;
    public GameObject levelSelectItemPrefab, campaignSelectItemPrefab;
    public Transform levelSelectParent, campaignSelectParent;
    private string levelStoragePath, campaignStoragePath;
    private List<GameObject> levelItems = new List<GameObject>();
    private List<GameObject> CampaignItems = new List<GameObject>();
    private bool importingLevel = false;
    private bool importingCampaign = false;
    private string jsonData, jsonFileName;
    public TMP_Text viewText, switchTextButtonText;
    void Start(){
        levelStoragePath = Path.Combine(Application.streamingAssetsPath, "Levels");
        campaignStoragePath = Path.Combine(Application.streamingAssetsPath, "Campaigns");
        gameManager = FindAnyObjectByType<GameManager>();
        soundManager = FindAnyObjectByType<SoundManager>();
        LoadAllLevels();
        LoadAllCampaigns();
        levelScrollView.SetActive(true);
        campaignScrollView.SetActive(false);
        viewText.text = "Levels";
        switchTextButtonText.text = "View Campaigns";
    }
    void Update()
    {
        if (importingLevel)
        {
            importingLevel = false;
            SaveDataWithCustomName(jsonData, jsonFileName, levelStoragePath);
        }
        if (importingCampaign)
        {
            importingCampaign = false;
            SaveDataWithCustomName(jsonData, jsonFileName, campaignStoragePath);
        }
    }
    public void LoadAllLevels()
    {
        //string path = Path.Combine(Application.streamingAssetsPath, "Levels");

        if (!Directory.Exists(levelStoragePath))
        {
            Debug.LogWarning("Level folder not found at: " + levelStoragePath);
            return;
        }

        // Get all files ending in .json
        string[] fileEntries = Directory.GetFiles(levelStoragePath, "*.json");

        //allLevels.Clear();
        foreach (GameObject item in levelItems)
        {
            Destroy(item);
        }
        levelItems.Clear();

        foreach (string filePath in fileEntries)
        {
            string jsonContent = File.ReadAllText(filePath);
            LevelData data = JsonUtility.FromJson<LevelData>(jsonContent);
            // Ignoring files that aren't LevelData (e.g., CampaignData)
            if (data.fileType != "LevelData")
            {
                continue;
            }
            
            Debug.Log($"Loaded Level: {Path.GetFileNameWithoutExtension(filePath)}");
            GameObject levelItem = Instantiate(levelSelectItemPrefab, levelSelectParent);
            levelItem.GetComponentInChildren<TMP_Text>().text = Path.GetFileNameWithoutExtension(filePath);
            levelItem.GetComponentInChildren<Button>().onClick.AddListener(() => {
                soundManager.PlayClickSound();
                gameManager.LevelToLoad = data; // Store the loaded level data in GameManager
                gameManager.setGameMode(GameManager.GameMode.CustomLevel); // Set the game mode to CustomLevel
                weaponEquipVisual.SetActive(true);
                Debug.Log("Button Pressed");
            });
            levelItems.Add(levelItem);
        }
        
        // Trigger your UI display logic here
    }
    public void LoadAllCampaigns()
    {
        if (!Directory.Exists(campaignStoragePath))
        {
            Debug.LogWarning("Campaign folder not found at: " + campaignStoragePath);
            return;
        }

        // Get all files ending in .json
        string[] fileEntries = Directory.GetFiles(campaignStoragePath, "*.json");

        foreach (GameObject item in CampaignItems)
        {
            Destroy(item);
        }
        CampaignItems.Clear();

        foreach (string filePath in fileEntries)
        {
            string jsonContent = File.ReadAllText(filePath);
            CampaignData data = JsonUtility.FromJson<CampaignData>(jsonContent);
            // Ignoring files that aren't CampaignData (e.g., LevelData)
            if (data.fileType != "CampaignData")
            {
                continue;
            }
            
            Debug.Log($"Loaded Campaign: {Path.GetFileNameWithoutExtension(filePath)}");
            GameObject campaignItem = Instantiate(campaignSelectItemPrefab, campaignSelectParent);
            campaignItem.GetComponentInChildren<TMP_Text>().text = Path.GetFileNameWithoutExtension(filePath);
            campaignItem.GetComponentInChildren<Button>().onClick.AddListener(() => {
                soundManager.PlayClickSound();
                gameManager.CampaignToLoad = data; // Store the loaded campaign data in GameManager
                gameManager.setGameMode(GameManager.GameMode.Campaign); // Set the game mode to CustomLevel
                weaponEquipVisual.SetActive(true);
                gameManager.CurrentCampaignLevelIndex = 0; // Reset campaign level index when loading a new campaign
                Debug.Log("Button Pressed");
            });
            CampaignItems.Add(campaignItem);
        }
        
        // Trigger your UI display logic here
    }

    public async void importCampaignOrLevel()
    {
        #if ENABLE_WINMD_SUPPORT
        UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
        {
        try{
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            // Filter for the file types you want to show
            openPicker.FileTypeFilter.Add(".json");

            // Open the picker and wait for the user to select a file
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null )
            {
                string newFileName = file.Name;
                string jsonContent = await FileIO.ReadTextAsync(file);
                jsonData = jsonContent;
                jsonFileName = newFileName;
                LevelData levelData = JsonUtility.FromJson<LevelData>(jsonContent);
                CampaignData campaignData = JsonUtility.FromJson<CampaignData>(jsonContent);
                if(isLevelFile(levelData))
                {
                    importingLevel = true;
                    Debug.LogError($"Loading Level: {newFileName}");
                }else if(isCampaignFile(campaignData))
                {
                    Debug.LogError($"Loading Campaign: {newFileName}");
                    importingCampaign = true;
                }
                else
                {
                    Debug.LogError($"Wrong file type! Expected LevelData or CampaignData but found: {jsonContent}");
                    // Show a UI popup to the user here
                    return;
                }
            }
            else
            {
                Debug.Log("Load operation cancelled.");
            }
        }
        catch (Exception ex)
            {
                Debug.LogError("UWP Picker Exception: " + ex.Message);
            }
        }, true);
    #else
        Debug.LogError("This function only works on UWP builds!");
    #endif
    }
    public bool isLevelFile(LevelData data)
    {
        if (data.fileType != "LevelData")
        {
            return false;
        }
        return true;
    }
    public bool isCampaignFile(CampaignData data)
    {
        if (data.fileType != "CampaignData")
        {
            return false;
        }
        return true;
    }

    public void refreshList()
    {
        LoadAllLevels();
        LoadAllCampaigns();
    }

    public void SaveDataWithCustomName(string jsonContent, string chosenName, string storagePath)
    {
        // Ensure the name ends with .json
        if (!chosenName.EndsWith(".json")) 
        {
            chosenName += ".json";
        }

        // Combine the folder path with your new filename
        string destinationPath = Path.Combine(storagePath, chosenName);

        // This creates the file and writes the text in one go
        File.WriteAllText(destinationPath, jsonContent);
        refreshList();
        Debug.LogError($"File saved successfully at: {destinationPath}");
    }

    public void switchScrollview()
    {
        soundManager.PlayClickSound();
        if(levelScrollView.activeSelf)
        {
            levelScrollView.SetActive(false);
            campaignScrollView.SetActive(true);
            viewText.text = "Campaigns";
            switchTextButtonText.text = "View Levels";
        }
        else
        {
            levelScrollView.SetActive(true);
            campaignScrollView.SetActive(false);
            viewText.text = "Levels";
            switchTextButtonText.text = "View Campaigns";
        }
    }
}
