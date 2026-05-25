using UnityEngine;
using UnityEngine.SceneManagement;

public class AreYouSurePopUp : MonoBehaviour
{
    public GameObject SurePopUp;
    void Start()
    {
        SurePopUp.SetActive(false);
    }

    public void activatePopUp()
    {
        SurePopUp.SetActive(true);
    }

    public void deactivatePopUp()
    {
        SurePopUp.SetActive(false);
    }
    public void returnToMenu()
    {
        TimerManager tm = FindAnyObjectByType<TimerManager>();
        if(tm.currentTimerState == TimerManager.TimerState.Off)
        {
            tm.startTimePanel.SetActive(false);
        }
        Time.timeScale = 1f;
        GameManager.Instance.SetGameState(GameManager.GameState.Menu);
        SceneManager.LoadScene("Menu");
    }
}
