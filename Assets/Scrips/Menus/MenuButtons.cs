using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    private TimerManager timerManager;
    public void Start()
    {
        timerManager = FindAnyObjectByType<TimerManager>();
        if(PlayerPrefs.GetInt("playerHealth") == 0)
        {
            Reset();
        }
    }
    public void PlayButton()
    {
        SceneManager.LoadScene("LevelSelectMenu");
        Debug.Log("PlayButton");
    }

    public void AboutButton()
    {
        SceneManager.LoadScene("AboutScene");
        Debug.Log("AboutButton");
    }
    public void shopButton()
    {
        SceneManager.LoadScene("Shop");
    }

    public void SettingsButton()
    {
        SceneManager.LoadScene("SettingsScene");
        Debug.Log("SettingsButton");
    }

    public void EditorButton()
    {
        SceneManager.LoadScene("LevelEditor");
        Debug.Log("EditorButton");
    }
    public void Reset()
    {
        PlayerPrefs.SetInt("playerHealth", 20);
        PlayerPrefs.SetInt("playerAttack", 1);
        PlayerPrefs.SetInt("coins", 0);
        PlayerPrefs.SetInt("weaponType", 1);
        PlayerPrefs.SetInt("specialType", 1);
    }

    public void LevelEditorButton()
    {
        SceneManager.LoadScene("LevelEditor");
        Debug.Log("LevelEditorButton");
    }

    public void QuitButton()
    {
        if (timerManager.currentTimerState == TimerManager.TimerState.On)
        {
            Debug.Log("Timer is still running. You can not retrive gpx data if you quit now.");
            timerManager.quittingMenu();
            return;
        }
        BLEManager.Instance?.bleConnect?.Disconnect();
        SoundManager.Instance.StopBGM();
        Application.Quit();
        Debug.Log("QuitButton");
    }
    public void PlayTestLevel()
    {
        SceneManager.LoadScene("TestLoadLevel");
    }

    public void CampaignButton()
    {
        SceneManager.LoadScene("MakeCampaign");
    }
}
