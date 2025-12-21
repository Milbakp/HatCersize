using UnityEngine;
using UnityEngine.SceneManagement;

public class RPGLevelSelect : MonoBehaviour
{
    public int levelIndex;
    public GameObject weaponEquipVisual;
    
    public void SelectLevel()
    {
        // SceneManager.LoadScene("Level" + levelIndex);
        PlayerPrefs.SetInt("SelectedLevel", levelIndex);
        weaponEquipVisual.SetActive(true);
    }
}
