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
        if (PlayerPrefs.GetInt("SelectedLevel") == -1)
        {
            SceneManager.LoadScene("TestLoadLevel");
            return;
        }
        SceneManager.LoadScene("Level" + PlayerPrefs.GetInt("SelectedLevel"));
    }
}
