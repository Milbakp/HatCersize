using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class UiButtons : MonoBehaviour
{
    public GameManager gameManager;
    private RPGLevelManager levelManager;
    public GameObject objectNextLevelButton;
    public void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        levelManager = FindAnyObjectByType<RPGLevelManager>();
        if(gameManager.CurrentMode != GameManager.GameMode.Campaign)
        {
            objectNextLevelButton.SetActive(false);
        }
        else
        {
            objectNextLevelButton.SetActive(true);
        }
    }
    void Start()
    {
        // Moved this code to RPGLevelLoader's Start method to ensure the game state is set before the level loads
        // GameManager.Instance.SetGameState(GameManager.GameState.InGame);
        // Debug.LogError("CurrentState: " + GameManager.Instance.CurrentState);
    }
    public void tryAgainButton()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void returnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
    public void returnToLevelSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelectMenu");
    }

    public void nextLevelButton()
    {
        if(gameManager.CurrentMode == GameManager.GameMode.Campaign)
        {
            Time.timeScale = 1f;
            gameManager.CurrentCampaignLevelIndex++;
            if (nextLevelExists())
            {
                SceneManager.LoadScene("TestLoadLevel");
            }
            else
            {
                Debug.LogError("No more levels in campaign");
            }
        }
        else
        {
            Debug.LogError("Loaded single level. Not campaign.");
        }
    }

    private bool nextLevelExists()
    {
        if(gameManager.CampaignToLoad == null)
        {
            Debug.LogError("CampaignToLoad is null in GameManager");
            return false;
        }
        foreach(LevelEntry entry in gameManager.CampaignToLoad.levels)
        {
            if(entry.order == gameManager.CurrentCampaignLevelIndex + 1) // Check if the next level in the campaign exists
            {
                return true;
            }
        }
        return false;
    }

    public void resumeGame()
    {
        levelManager.ResumeGame();
    }
}
