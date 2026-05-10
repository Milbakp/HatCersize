using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if ENABLE_WINMD_SUPPORT
using Windows.Storage;
using Windows.Storage.Pickers;
using System;
using System.Threading.Tasks; // Added for Task support
#endif

public class CampaignManager : MonoBehaviour
{
    public Transform campaignSelectParent, levelSelectParent;
    private string levelStoragePath;
    public GameObject levelItemPrefab, levelSelectItemPrefab;
    public bool isLoadingLevelForCampaign = false; // Flag to determine if we're loading a level for campaign creation
    private string selectedLevelName; // Store the name of the level selected for campaign
    private SoundManager soundManager;
    private Dictionary<string, LevelData> loadedLevels = new Dictionary<string, LevelData>(); // Cache for loaded levels
    void Start()
    {
        levelStoragePath = Path.Combine(Application.streamingAssetsPath, "Levels");
        soundManager = FindAnyObjectByType<SoundManager>();
        LoadAllLevels();
    }

    void Update()
    {
        if (isLoadingLevelForCampaign)
        {
            isLoadingLevelForCampaign = false;// Reset the flag after loading
            createLevelItem(selectedLevelName);
        }
    }
    public async void loadLevelForCampaign(){
        #if ENABLE_WINMD_SUPPORT
        UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
        {
        try{
            // Initialize the Picker
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            openPicker.FileTypeFilter.Add(".json");

            // Open the picker and wait for the user to select a file
            StorageFile file = await openPicker.PickSingleFileAsync();
        
            if (file != null)
            {
                string json = await FileIO.ReadTextAsync(file);

                // Convert JSON back into your LevelData object
                LevelData data = JsonUtility.FromJson<LevelData>(json);
                if (data.fileType != "LevelData")
                {
                    Debug.LogError($"Wrong file type! Expected LevelData but found: {data.fileType}");
                    // Show a UI popup to the user here
                    return;
                }
                Debug.LogError("Level loaded successfully: " + file.Name);
                
                selectedLevelName = Path.GetFileNameWithoutExtension(file.Name);
                loadedLevels[selectedLevelName] = data; // Cache the loaded level data
                isLoadingLevelForCampaign = true;
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
    public void createLevelItem(string levelName)
    {
        GameObject levelItem = Instantiate(levelItemPrefab, campaignSelectParent);
        levelItem.GetComponent<ReorderItems>().SetLevelName(levelName);
        levelItem.transform.SetAsLastSibling(); // Ensure the new item is at the end of the list
    }

    public void createCampaign()
    {
        CampaignData newCampaign = new CampaignData();
        newCampaign.campaignTitle = "New Campaign";
        newCampaign.fileType = "CampaignData"; // Set the file type for identification when loading
        for (int i = 0; i < campaignSelectParent.childCount; i++)
        {
            string levelName = campaignSelectParent.GetChild(i).name;
            newCampaign.levels.Add(new LevelEntry(i + 1, levelName, loadedLevels[levelName]));
            Debug.LogError("Added level to campaign: " + levelName);
        }

        #if ENABLE_WINMD_SUPPORT
        UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
        {
            try 
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                
                // Ensure the extension matches exactly
                savePicker.FileTypeChoices.Add("JSON File", new List<string>() { ".json" });
                savePicker.SuggestedFileName = "NewCampaign";

                // This line requires the 'async' keyword in the method signature
                StorageFile file = await savePicker.PickSaveFileAsync();

                if (file != null)
                {
                    string json = JsonUtility.ToJson(newCampaign, true);
                    await FileIO.WriteTextAsync(file, json);
                    Debug.Log("Saved successfully: " + file.Path);
                }
                else
                {
                    Debug.Log("User cancelled the picker.");
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

    private List<GameObject> levelItems = new List<GameObject>();
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
            selectedLevelName = Path.GetFileNameWithoutExtension(filePath);
            loadedLevels[selectedLevelName] = data;
            
            Debug.Log($"Loaded Level: {Path.GetFileNameWithoutExtension(filePath)}");
            GameObject levelItem = Instantiate(levelSelectItemPrefab, levelSelectParent);
            levelItem.GetComponentInChildren<TMP_Text>().text = Path.GetFileNameWithoutExtension(filePath);
            levelItem.GetComponentInChildren<Button>().onClick.AddListener(() => {
                soundManager.PlayClickSound();
                createLevelItem(Path.GetFileNameWithoutExtension(filePath));
               
                Debug.Log("Button Pressed");
            });
            levelItems.Add(levelItem);
        }
    }
}
