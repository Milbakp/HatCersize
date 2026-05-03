using UnityEngine;
using UnityEngine.UI;

public class SetWeaponButton : MonoBehaviour
{
    public int weaponType;
    private Button button;
    private SoundManager soundManager;
    
    public void Start()
    {
        button = GetComponent<Button>();
        soundManager = FindAnyObjectByType<SoundManager>();
    }
    public void Update()
    {
        if(PlayerPrefs.GetInt("weaponType") != weaponType)
        {
            ResetButtonColor();
        }
    }
    public void setWeaponType()
    {
        soundManager.PlayClickSound();
        PlayerPrefs.SetInt("weaponType", weaponType);
        SetButtonColorGreen();
    }

    // Probably not the best way to use player prefs but it works.
    public void SetButtonColorGreen() {
        ColorBlock cb = button.colors;
        cb.normalColor = new Color(0f, 1f, 0f);
        button.colors = cb;
    }

    public void ResetButtonColor() {
        ColorBlock cb = button.colors;
        cb.normalColor = new Color(1f, 1f, 1f);
        button.colors = cb;
    }
}
