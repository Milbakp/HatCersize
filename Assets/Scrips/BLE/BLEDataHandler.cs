using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BLEDataHandler : MonoBehaviour
{
    // Events for step data and turn state
    public event Action<float> OnSpeedUpdated; // Update speed for character movement
    public event Action<int> OnTurnStateUpdated; // Update turning for character rotation
    public event Action OnStepReceived; // Update step count

    private float stepLength => GPXCoordinate.StepLength; // Average step length in meters, used for calcuate speed

    public void Initialize(BLEConnect bleConnect)
    {
        // Subscribe to events from BleConnect
        bleConnect.OnGameStepDataUpdated += OnGameStepDataReceived;
        bleConnect.OnTurnStateUpdated += OnTurnStateReceived;
        bleConnect.OnMapCoordinateReceived += OnMapCoordinateReceived;
        bleConnect.OnPauseRequested += OnPauseRequestReceived;
        bleConnect.OnScreenshotRequested += OnScreenshotRequestReceived;
        bleConnect.OnAttackRequested += OnAttackReceived;
        bleConnect.OnSpecialRequested += OnSpecialReceived; 
        bleConnect.OnInteractRequested += OnInteractReceived;

        Debug.Log("Data handler initialized");
    }

    private void OnGameStepDataReceived(string stepData)
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.InGame)
        {
            Debug.Log("BLEDataHandler: Ignoring step data since not InGame state.");
            return;
        }

        try
        {
            // Check if stepData is null or empty
            if (string.IsNullOrEmpty(stepData))
            {
                Debug.Log("Data handler: no step data to process");
                OnSpeedUpdated?.Invoke(0f); // Notify subscribers with zero speed
                return;
            }

            //Debug.Log($"Data handler received step data: {stepData}");

            if (!int.TryParse(stepData, out int timeMs))
            {
                Debug.LogWarning($"Failed to parse time from GameStepData. Data: {stepData}");
                OnSpeedUpdated?.Invoke(0f); // Notify subscribers with zero speed
                return;
            }

            //Debug.Log($"Data handler received step data: {timeMs} ms");
            // Calculate speed using step length and step interval
            float speed = stepLength / (timeMs / 1000f); // speed in meters per second

            //Debug.Log($"Step interval: {timeMs} ms, Speed: {speed} m/s");
            // Notify subscribers of the updated speed
            OnSpeedUpdated?.Invoke(speed);

            OnStepReceived?.Invoke();

        }
        catch (Exception ex)
        {
            Debug.Log($"Exception in OnGameStepDataReceived: {ex.Message}");
        }
    }

    private void OnTurnStateReceived(int newTurnState)
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.InGame)
        {
            Debug.Log("BLEDataHandler: Ignoring turn state data since not InGame state");
            return;
        }
        Debug.Log($"BLEDataHandler: Received turn state: {newTurnState}");
        OnTurnStateUpdated?.Invoke(newTurnState); // Notify subscribers of the updated turn state
    }

    private void OnMapCoordinateReceived(string coordinateData)
    {
        try
        {
            Debug.Log($"Coordinates received: {coordinateData}");
            string[] coordinates = coordinateData.Split(',');
            if (coordinates.Length != 2 || !double.TryParse(coordinates[0], out double latitude) ||
                !double.TryParse(coordinates[1], out double longitude))
            {
                Debug.LogWarning($"Invalid coordinates: {coordinateData}");
                return;
            }

            GPXCoordinate.SaveCoordinate(latitude, longitude);
            Debug.Log($"Saved coordinates: Lat={latitude}, Lon={longitude}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in coordinates: {e.Message}");
        }
    }

    private void OnPauseRequestReceived()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.InGame)
        {
            Debug.Log("BLEDataHandler: Ignoring pause request since not InGame state");
            return;
        }

        RPGLevelManager levelManager = FindAnyObjectByType<RPGLevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("BLEDataHandler: LevelManager not found for pause request");
            return;
        }

        if (levelManager.CurrentLevelState == RPGLevelManager.LevelState.Playing)
        {
            levelManager.PauseGame();
            Debug.Log("BLEDataHandler: Paused game via pause command");
        }
        else if (levelManager.CurrentLevelState == RPGLevelManager.LevelState.Paused)
        {
            levelManager.ResumeGame();
            Debug.Log("BLEDataHandler: Resumed game via pause command");
        }
    }

    private void OnScreenshotRequestReceived()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.InGame)
        {
            Debug.Log("BLEDataHandler: Ignoring screenshot request since not InGame state");
            return;
        }
        ScreenshotManager.Instance.StartCoroutine(ScreenshotManager.Instance.TakeScreenshotWithExif());
        Debug.Log("Data handler called screenshot");
    }
    // Luqman Added
    private void OnAttackReceived()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.InGame)
        {
            Debug.Log("BLEDataHandler: Ignoring turn state data since not InGame state");
            return;
        }
        Debug.Log($"BLEDataHandler: Received attack command");
        
        Attack attack = FindAnyObjectByType<Attack>();
        if (attack == null)
        {
            Debug.LogError("BLEDataHandler: attack not found for attack request");
            return;
        }
        attack.callAttack();
    }

    private void OnSpecialReceived()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.InGame)
        {
            Debug.Log("BLEDataHandler: Ignoring special command since not InGame state");
            return;
        }

        Special special = FindAnyObjectByType<Special>();
        if (special == null)
        {
            Debug.LogError("BLEDataHandler: special not found for special request");
            return;
        }
        special.specialCall();
    }

    private void OnInteractReceived()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.InGame)
        {
            Debug.Log("BLEDataHandler: Ignoring interact command since not InGame state");
            return;
        }
        
        Interact interact = FindAnyObjectByType<Interact>();
        if (interact == null)
        {
            Debug.LogError("BLEDataHandler: interact not found for interact request");
            return;
        }
        interact.interact();
    }

}
