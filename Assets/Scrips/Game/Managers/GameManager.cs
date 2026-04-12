using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        Menu, // User is in the menu, Bluetooth data should be ignored
        InGame, // User is in a game level, Bluetooth data should control the character
        //Paused // Game is paused, Bluetooth data should not affect movement
    }

    public enum GameMode
    {
        Campaign,
        CustomLevel
    }

    public GameState CurrentState { get; private set; } = GameState.Menu;
    public GameMode CurrentMode { get; private set; } = GameMode.CustomLevel;
    public string CurrentLevelName { get; private set; }
    public string CurrentCustomLevelPath { get; set; }
    public LevelData LevelToLoad { get; set; }
    public CampaignData CampaignToLoad { get; set; }
    public int CurrentCampaignLevelIndex { get; set; } = 0; // Track the current level index in the campaign

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        if (!SoundManager.Instance.IsBGMPlaying)
        {
            SoundManager.Instance.PlayBGM();
        }
    }

    public void SetGameState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log($"Game state changed to: {newState}");
    }

    public void SetCurrentLevelName(string levelName)
    {
        CurrentLevelName = levelName;
        Debug.Log($"GameManager: Set CurrentLevelName to {levelName}");
    }

    public void ClearCurrentLevelName()
    {
        CurrentLevelName = null;
        Debug.Log("GameManager: Cleared CurrentLevelName");
    }

    public void ClearCurrentCustomLevelPath()
    {
        CurrentCustomLevelPath = null;
        Debug.Log("GameManager: Cleared CurrentCustomLevelPath");
    }

    // Luqman Code
    public void setGameMode(GameMode mode)
    {
        CurrentMode = mode;
        Debug.LogError($"Game mode set to: {mode}");
    }
}