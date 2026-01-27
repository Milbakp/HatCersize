using UnityEngine;

public class RPGLevelManager : MonoBehaviour
{
    public enum LevelState
    {
        Playing,    // Normal gameplay
        Paused,     // Game is paused
        Completed   // Level finished
    }
    public LevelState CurrentLevelState { get; private set; } = LevelState.Playing;
    private LevelState previousLevelState;
    [SerializeField] PlayerHealth playerHealth;
    public GameObject gameOverScreen;
    public GameObject levelCompletedScreen;
    public GameObject pauseScreen;
    public int numOfEnemies;
    public GameObject Door;
    public bool levelCompleted;
    public AudioSource aud;
    public AudioClip doorOpenSound;
    void Start()
    {
        Time.timeScale = 1f;
        gameOverScreen.SetActive(false);
        levelCompletedScreen.SetActive(false);
        levelCompleted = false;
        GPXMovementTracker tracker = FindAnyObjectByType<GPXMovementTracker>();
        if (tracker != null)
        {
            tracker.ResetTracking();
        }
        BLEManager.Instance.bleConnect.UpdateSensorStateOnBLE("start");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth.health <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
            gameOverScreen.SetActive(true);
        }
        if (numOfEnemies == 0)
        {
            numOfEnemies--;
            aud.PlayOneShot(doorOpenSound);
            Door.SetActive(false);
        }
        // Pause/Resume game on Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentLevelState == LevelState.Playing)
            {
                PauseGame();
            }
            else if (CurrentLevelState == LevelState.Paused)
            {
                ResumeGame();
            }
        }
    }

    public void PauseGame()
    {
        if (CurrentLevelState != LevelState.Playing)
            return;

        pauseScreen.SetActive(true);
        previousLevelState = CurrentLevelState;
        CurrentLevelState = LevelState.Paused;
        Cursor.visible = true; // Show cursor
        Cursor.lockState = CursorLockMode.None; // Unlock cursor

        Time.timeScale = 0f; // Stop time when paused
        if (BLEManager.Instance != null && BLEManager.Instance.bleConnect != null)
        {
            BLEManager.Instance.bleConnect.UpdateSensorStateOnBLE("stop"); // Pause sensors
        }
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        pauseScreen.SetActive(false);
        Cursor.visible = false; // Hide cursor
        Cursor.lockState = CursorLockMode.None; // Ensure consistent state
        CurrentLevelState = previousLevelState;

        Time.timeScale = 1f; // Resume time
        if (BLEManager.Instance != null && BLEManager.Instance.bleConnect != null)
        {
            BLEManager.Instance.bleConnect.UpdateSensorStateOnBLE("start"); // Resume sensors
        }
        Debug.Log("Game Resumed");
    }

    public void LevelCompleted()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        levelCompletedScreen.SetActive(true);
        PlayerPrefs.SetInt("coins", playerHealth.coins + PlayerPrefs.GetInt("coins"));
    }
}
