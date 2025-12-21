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
        SceneManager.LoadScene("Level" + PlayerPrefs.GetInt("SelectedLevel"));
    }
}
