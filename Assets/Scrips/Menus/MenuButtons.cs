using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public void Start()
    {
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
        PlayerPrefs.SetInt("playerHealth", 5);
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
        BLEManager.Instance?.bleConnect?.Disconnect();
        SoundManager.Instance.StopBGM();
        Application.Quit();
        Debug.Log("QuitButton");
    }
}
