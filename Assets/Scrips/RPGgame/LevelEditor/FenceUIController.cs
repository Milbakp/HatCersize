using UnityEngine;
using UnityEngine.UI; // Required for handling UI components
using TMPro;          // Optional: Add this if you want to display the length text
using System;

public class FenceUIController : MonoBehaviour
{
    [Header("References")]
    public FenceGenerator fenceGenerator;
    public Slider lengthSlider;
    
    [Header("Optional UI Elements")]
    public TextMeshProUGUI lengthTextDisplay; 
    public GameObject sliderUI;
    private LevelEditorManager levelEditorManager;

    void Start()
    {
        if (fenceGenerator == null || lengthSlider == null)
        {
            Debug.LogError("Please assign the Fence Generator and Slider references in the Inspector!");
            return;
        }
        sliderUI.SetActive(false);
        levelEditorManager = FindAnyObjectByType<LevelEditorManager>();
        levelEditorManager.Preview += deactivateSlider;
        // 1. Initialize the slider values based on your generator settings
        lengthSlider.minValue = 2f;    // Minimum fence length (e.g., at least 1 panel)
        lengthSlider.maxValue = 50f;   // Maximum fence length allowed for the player
        lengthSlider.value = fenceGenerator.fenceLength; // Match the current starting setup

        // 2. Add a listener via code so it updates every frame the slider moves
        lengthSlider.onValueChanged.AddListener(OnSliderValueChanged);

        // Run an initial update to sync text labels
        UpdateTextDisplay(lengthSlider.value);
    }

    // This method triggers automatically whenever the player drags the slider handle
    void OnSliderValueChanged(float newValue)
    {
        // Update the value on the generator
        fenceGenerator.fenceLength = newValue;

        // Force the fence to recreate itself at runtime
        fenceGenerator.GenerateFence();

        // Optional: Update the screen text
        UpdateTextDisplay(newValue);
    }

    void UpdateTextDisplay(float value)
    {
        if (lengthTextDisplay != null)
        {
            // Displays the length rounded to one decimal place (e.g., "Length: 12.4m")
            lengthTextDisplay.text = $"{value:F1}m";
        }
    }

    // Clean up our UI listener when this object is destroyed to prevent memory leaks
    void OnDestroy()
    {
        if (lengthSlider != null)
        {
            lengthSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }

    public void deactivateSlider()
    {
        sliderUI.SetActive(false);
    }
}