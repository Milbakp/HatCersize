using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class TimerManager : MonoBehaviour
{
    public enum TimerState{
        Off, // Timer is not active
        On // Timer is active and counting down
    }
    public GameObject toggleButton, TimerMenuPanel, timerDisplay, quittingMenuPanel;
    private bool PanelIsVisible;
    public TMP_Text toggleButtonText, displayText, durationText;
    public static TimerManager Instance { get; private set; }
    public TimerState currentTimerState = TimerState.Off;
    public float timer, timerDuration = 60f; // Default timer duration in minutes
    public bool increasingTimer = false, decreasingTimer = false;
    void Awake()
    {
         if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        PanelIsVisible = TimerMenuPanel.activeSelf;
        timerDisplay.SetActive(false);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        toggleButtonText.text = "Timer \t Menu";
        displayText.text = $"{timer:F1}";
        durationText.text = $"{ Mathf.FloorToInt(timerDuration/ 60):F1} Minutes";
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTimerState == TimerState.On)
        {
            timer += Time.deltaTime;
            DisplayTime(timer);
            if (timer >= timerDuration)
            {
                deactivateTimer();
            }
        }
    }
    public void timerButton()
    {
     activateTimer();   
    }
    public void activateTimer()
    {
        timerDisplay.SetActive(true);
        currentTimerState = TimerState.On;
    }
    public void deactivateTimer()
    {
        timer = 0;
        currentTimerState = TimerState.Off;
        timerDisplay.SetActive(false);
    }
    void DisplayTime(float timeToDisplay)
    {
        // Calculate minutes and seconds
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        // Format the string as 00:00
        displayText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded; // Prevent leaks
    }
     private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TimerMenuPanel.SetActive(false); // Hide the timer menu panel when a new scene is loaded
        UpdateCanvasVisibility();
    }

    private void OnSceneUnloaded(Scene scene)
    {
        UpdateCanvasVisibility();
    }

    private void UpdateCanvasVisibility()
    {
        if (toggleButton == null)
        {
            Debug.LogError("toggleButton is not assigned!");
            return;
        }

        // Check all loaded scenes
        bool shouldHide = false;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.isLoaded && (scene.name == "LevelEditor" || scene.name == "DefaultLevel" || scene.name=="CustomLevel"))
            {
                shouldHide = true;
                break;
            }
        }

        toggleButton.SetActive(!shouldHide);
    }
    public void TooglePanel()
    {
        PanelIsVisible = !PanelIsVisible;
        TimerMenuPanel.SetActive(PanelIsVisible);
        if (PanelIsVisible)
        {
            toggleButtonText.text = "Back";
        }
        else
        {
            toggleButtonText.text = "Timer \t Menu";
        }
    }
    // Functions for adjusting timer duration
    public void increaseTimerDuration()
    {
        increasingTimer = true;
        timerDuration += 60;
        durationText.text = $"{ Mathf.FloorToInt(timerDuration/ 60):F1} Minutes";
        StartCoroutine(continuouslyAdjustTimer());

    }
    public void decreaseTimerDuration()
    {
        if(timerDuration <= 0)
        {
            return; // Can't decrease below 0
        }
        decreasingTimer = true;
        timerDuration -= 60;
        durationText.text = $"{ Mathf.FloorToInt(timerDuration/ 60):F1} Minutes";
        StartCoroutine(continuouslyAdjustTimer());
    }
    
    IEnumerator continuouslyAdjustTimer()
    {
        yield return new WaitForSeconds(1f);
        while (increasingTimer)
        {
            timerDuration += 60;
            durationText.text = $"{ Mathf.FloorToInt(timerDuration/ 60):F1} Minutes";
            yield return new WaitForSeconds(0.2f);
        }
        while (decreasingTimer)
        {
            timerDuration -= 60;
            durationText.text = $"{ Mathf.FloorToInt(timerDuration/ 60):F1} Minutes";
            yield return new WaitForSeconds(0.2f);
        }
    }
    public void stopCoroutines()
    {
        increasingTimer = false;
        decreasingTimer = false;
        StopAllCoroutines();
    }
    // Functions for quitting menu
    public void quittingMenu()
    {
        quittingMenuPanel.SetActive(quittingMenuPanel.activeSelf ? false : true);
    }
    public void confirmQuit()
    {
        deactivateTimer();
        BLEManager.Instance?.bleConnect?.Disconnect();
        SoundManager.Instance.StopBGM();
        Application.Quit();
    }
}
