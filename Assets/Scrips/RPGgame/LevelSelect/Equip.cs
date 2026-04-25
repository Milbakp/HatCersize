using UnityEngine;
using UnityEngine.SceneManagement;

public class Equip : MonoBehaviour
{
    public GameObject weaponEquipVisual;
    public void levelBack()
    {
        weaponEquipVisual.SetActive(false);
    }
    public void PlayLevel()
    {
        // Temporaily set the player prefs to load selected level.
        PlayerPrefs.SetInt("SelectedLevel", -1);
        if (PlayerPrefs.GetInt("SelectedLevel") == -1 || PlayerPrefs.GetInt("SelectedLevel") == -2)
        {
            SceneManager.LoadScene("TestLoadLevel");
            return;
        }
        SceneManager.LoadScene("Level" + PlayerPrefs.GetInt("SelectedLevel"));
    }
}
