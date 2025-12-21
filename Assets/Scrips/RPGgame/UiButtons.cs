using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class UiButtons : MonoBehaviour
{
    // private int bluetoothToggle;
    // public GameObject bluetoothCanvas;
    void Start()
    {
        // bluetoothToggle = 0;
        GameManager.Instance.SetGameState(GameManager.GameState.InGame);
        Debug.LogError("CurrentState: " + GameManager.Instance.CurrentState);

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
    // public void bluetoothButton()
    // {
    //     Debug.LogError("Bluetooth Button Pressed");
    //     if(bluetoothToggle == 0)
    //     {
    //         bluetoothCanvas.SetActive(true);
    //         Cursor.lockState = CursorLockMode.None;
    //         Cursor.visible = true;
    //         bluetoothToggle = 1;
    //     }
    //     else
    //     {
    //         bluetoothCanvas.SetActive(false);
    //         Cursor.lockState = CursorLockMode.Locked;
    //         Cursor.visible = false;
    //         bluetoothToggle = 0;
    //     }
    // }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(BeginCountdown());
        }
    }
    private IEnumerator BeginCountdown()
    {
        Debug.LogError("Bluetooth Sensors enabled");
        yield return new WaitForSeconds(2);
        BLEManager.Instance.bleConnect.UpdateSensorStateOnBLE("start");
    }
}
