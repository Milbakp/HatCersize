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
    public GameObject TimerMenuPanel, timerDisplay;
    private bool PanelIsVisible;
    public TMP_Text toggleButtonText, displayText;
    public static TimerManager Instance { get; private set; }
    TimerState currentTimerState = TimerState.Off;
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
        UpdateCanvasVisibility();
    }

    private void OnSceneUnloaded(Scene scene)
    {
        UpdateCanvasVisibility();
    }

    private void UpdateCanvasVisibility()
    {
        if (TimerMenuPanel == null)
        {
            Debug.LogError("TimerMenuPanel is not assigned!");
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

        TimerMenuPanel.SetActive(!shouldHide);
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

    public void increaseTimerDuration()
    {
        increasingTimer = true;
        timerDuration += 60;
        StartCoroutine(continuouslyAdjustTimer());

    }
    public void decreaseTimerDuration()
    {
        decreasingTimer = true;
        timerDuration -= 60;
        StartCoroutine(continuouslyAdjustTimer());
    }
    
    IEnumerator continuouslyAdjustTimer()
    {
        yield return new WaitForSeconds(1f);
        while (increasingTimer)
        {
            timerDuration += 60;
            yield return new WaitForSeconds(0.2f);
        }
        while (decreasingTimer)
        {
            timerDuration -= 60;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void stopCoroutines()
    {
        increasingTimer = false;
        decreasingTimer = false;
        StopAllCoroutines();
    }
}
