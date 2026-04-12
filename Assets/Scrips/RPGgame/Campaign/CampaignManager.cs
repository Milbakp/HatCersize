using UnityEngine;
using System.Collections.Generic;
using System.IO;

#if ENABLE_WINMD_SUPPORT
using Windows.Storage;
using Windows.Storage.Pickers;
using System;
using System.Threading.Tasks; // Added for Task support
#endif

public class CampaignManager : MonoBehaviour
{
    public Transform levelSelectParent;
    public GameObject levelItemPrefab;
    public bool isLoadingLevelForCampaign = false; // Flag to determine if we're loading a level for campaign creation
    private string selectedLevelName; // Store the name of the level selected for campaign
    private Dictionary<string, LevelData> loadedLevels = new Dictionary<string, LevelData>(); // Cache for loaded levels
    void Start()
    {
        
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
        GameObject levelItem = Instantiate(levelItemPrefab, levelSelectParent);
        levelItem.GetComponent<ReorderItems>().SetLevelName(levelName);
        levelItem.transform.SetAsLastSibling(); // Ensure the new item is at the end of the list
    }

    public void createCampaign()
    {
        CampaignData newCampaign = new CampaignData();
        newCampaign.campaignTitle = "New Campaign";
        for (int i = 0; i < levelSelectParent.childCount; i++)
        {
            string levelName = levelSelectParent.GetChild(i).name;
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
}
