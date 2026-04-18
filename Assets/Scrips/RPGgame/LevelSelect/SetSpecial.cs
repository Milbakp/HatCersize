using UnityEngine;
using UnityEngine.UI;

public class SetSpecial : MonoBehaviour
{
    public int specialType;
    private Button button;
    private SoundManager soundManager;
    public void Start()
    {
        button = GetComponent<Button>();
        soundManager = FindObjectOfType<SoundManager>();
    }
    public void Update()
    {
        if(PlayerPrefs.GetInt("specialType") != specialType)
        {
            ResetButtonColor();
        }
    }
    // Probably not the best way to use player prefs but it works.
    public void setSpecialType()
    {
        soundManager.PlayClickSound();
        PlayerPrefs.SetInt("specialType", specialType);
        SetButtonColorGreen();
    }

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
