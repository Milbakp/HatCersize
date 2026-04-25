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
    public GameObject levelSelectItemPrefab;
    public Transform levelSelectParent;
    private string levelStoragePath;
    private List<GameObject> levelItems = new List<GameObject>();
    private bool importingLevel = false;
    private string jsonData, jsonFileName;
    void Start(){
        levelStoragePath = Path.Combine(Application.streamingAssetsPath, "Levels");
        gameManager = FindAnyObjectByType<GameManager>();
        soundManager = FindAnyObjectByType<SoundManager>();
        LoadAllLevels();
    }
    void Update()
    {
        if (importingLevel)
        {
            importingLevel = false;
            SaveDataWithCustomName(jsonData, jsonFileName);
        }
    }
    
    public void SelectLevel()
    {
        // SceneManager.LoadScene("Level" + levelIndex);
        soundManager.PlayClickSound();
        PlayerPrefs.SetInt("SelectedLevel", levelIndex);
        if(levelIndex == -1)
        {
            playerMadeLevel();
            gameManager.setGameMode(GameManager.GameMode.CustomLevel); // Set the game mode to CustomLevel
        }
        else if(levelIndex == -2)
        {
            loadCampaign();
            gameManager.setGameMode(GameManager.GameMode.Campaign); // Set the game mode to Campaign
        }
        else
        {
            gameManager.setGameMode(GameManager.GameMode.CustomLevel);
        }
        weaponEquipVisual.SetActive(true);
    }

    public async void playerMadeLevel(){
        #if ENABLE_WINMD_SUPPORT
        UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
        {
        try{
            // 1. Initialize the Picker
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            // 2. Filter for the file types you want to show
            openPicker.FileTypeFilter.Add(".json");

            // 3. Open the picker and wait for the user to select a file
            StorageFile file = await openPicker.PickSingleFileAsync();
        
            if (file != null)
            {
                // 4. Read the file content
                string json = await FileIO.ReadTextAsync(file);

                // 5. Convert JSON back into your LevelData object
                LevelData data = JsonUtility.FromJson<LevelData>(json);

                Debug.Log("Level loaded successfully: " + file.Name);
                
                gameManager.LevelToLoad = data; // Store the loaded level data in GameManager
                gameManager.setGameMode(GameManager.GameMode.CustomLevel); // Set the game mode to CustomLevel
            }
            else
            {
                Debug.Log("Load operation cancelled.");
            }
        }
        catch (Exception ex)
            {
                // This will tell you the EXACT error (e.g., Access Denied or Threading error)
                Debug.LogError("UWP Picker Exception: " + ex.Message);
            }
        }, true);
    #else
        Debug.LogError("This function only works on UWP builds!");
    #endif
    }

    public async void loadCampaign(){
        #if ENABLE_WINMD_SUPPORT
        UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
        {
        try{
            // 1. Initialize the Picker
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            // 2. Filter for the file types you want to show
            openPicker.FileTypeFilter.Add(".json");

            // 3. Open the picker and wait for the user to select a file
            StorageFile file = await openPicker.PickSingleFileAsync();
        
            if (file != null)
            {
                // 4. Read the file content
                string json = await FileIO.ReadTextAsync(file);

                // 5. Convert JSON back into your LevelData object
                CampaignData data = JsonUtility.FromJson<CampaignData>(json);

                Debug.LogError("Campaign loaded successfully: " + file.Name);
                
                gameManager.CampaignToLoad = data; // Store the loaded campaign data in GameManager
                gameManager.setGameMode(GameManager.GameMode.Campaign); // Set the game mode to Campaign
            }
            else
            {
                Debug.LogError("Load operation cancelled.");
            }
        }
        catch (Exception ex)
            {
                // This will tell you the EXACT error (e.g., Access Denied or Threading error)
                Debug.LogError("UWP Picker Exception: " + ex.Message);
            }
        }, true);
    #else
        Debug.LogError("This function only works on UWP builds!");
    #endif
    weaponEquipVisual.SetActive(true);
    gameManager.CurrentCampaignLevelIndex = 0; // Reset campaign level index when loading a new campaign
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
            
            //allLevels.Add(data);
            Debug.Log($"Loaded Level: {Path.GetFileNameWithoutExtension(filePath)}");
            GameObject levelItem = Instantiate(levelSelectItemPrefab, levelSelectParent);
            levelItem.GetComponentInChildren<TMP_Text>().text = Path.GetFileNameWithoutExtension(filePath);
            levelItem.GetComponentInChildren<Button>().onClick.AddListener(() => {
                gameManager.LevelToLoad = data; // Store the loaded level data in GameManager
                gameManager.setGameMode(GameManager.GameMode.CustomLevel); // Set the game mode to CustomLevel
                weaponEquipVisual.SetActive(true);
                Debug.Log("Button Pressed");
            });
            levelItems.Add(levelItem);
        }
        
        // Trigger your UI display logic here
    }

    public async void importLevel()
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
            //StorageFolder localFolder = await StorageFolder.GetFolderFromPathAsync(levelStoragePath);
            string newFileName = file.Name;

            // CreationCollisionOption.ReplaceExisting handles overwriting
            //StorageFile newFile = await localFolder.CreateFileAsync(newFileName, CreationCollisionOption.ReplaceExisting);
        
            if (file != null)
            {
                string json = await FileIO.ReadTextAsync(file);
                jsonData = json;
                jsonFileName = newFileName;
                importingLevel = true;
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

    public void refreshLevelList()
    {
        LoadAllLevels();
    }

    public void SaveDataWithCustomName(string jsonContent, string chosenName)
    {
        // Ensure the name ends with .json
        if (!chosenName.EndsWith(".json")) 
        {
            chosenName += ".json";
        }

        // Combine the folder path with your new filename
        string destinationPath = Path.Combine(levelStoragePath, chosenName);

        // This creates the file and writes the text in one go
        File.WriteAllText(destinationPath, jsonContent);
    }

}
