using UnityEngine;
using UnityEngine.SceneManagement;

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
    void Start(){
        gameManager = FindAnyObjectByType<GameManager>();
        soundManager = FindAnyObjectByType<SoundManager>();
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
        // string loadPath = EditorUtility.OpenFilePanel("Load Level", "", "json");
        // if (string.IsNullOrEmpty(loadPath))
        // {
        //     Debug.LogError("No file selected!");
        //     return;
        // }
        // PlayerPrefs.SetString("PlayerMadeLevelPath", loadPath);
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


}
