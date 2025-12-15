using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// The BackgroundColorManager class manages the background color of the main camera in a 2D game.
/// It uses a singleton pattern to ensure only one instance exists. The background color's hue changes 
/// periodically based on a random initial value and increments with each update. This provides a dynamic 
/// and visually appealing background effect.
/// 
/// Key functionalities:
/// - Ensures a singleton instance.
/// - Initializes with a random hue value.
/// - Updates and applies the background color based on the hue value.
/// - Increments the hue value and wraps it around if it exceeds 1.
/// </summary>
public class BackgroundColorManager : MonoBehaviour {
    
    public static BackgroundColorManager Instance; // Singleton instance of BackgroundColorManager
    
    private float currentHueValue; // Stores the current hue value for background color
    private Camera cachedCamera; // Cached reference to avoid repeated Camera.main lookups


    private void Awake()
    {
        // Ensure Singleton pattern, destroy if an instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        cachedCamera = Camera.main;
    }


    private void Start()
    {
        // Initialize the hue with a random value and update the background color
        currentHueValue = GetRandomHueValue();
        
        ApplyBackgroundColor();
    }
    
    // Generates a random hue value between 0 and 1
    private float GetRandomHueValue()
    {
        return Random.Range(0, 10) / 10.0f;
    }
    
    
    public void ApplyBackgroundColor()
    {
        UpdateHueValue();
        SetBackgroundColor();
    }
    
    // Increments the hue value and wraps around if it exceeds 1
    private void UpdateHueValue()
    {
        currentHueValue = Mathf.Repeat(currentHueValue + 0.1f, 1f);
    }
    
    // Updates the camera's background color based on the current hue value
    private void SetBackgroundColor()
    {
        if (cachedCamera == null)
        {
            cachedCamera = Camera.main;
            if (cachedCamera == null)
            {
                return;
            }
        }

        cachedCamera.backgroundColor = Color.HSVToRGB(currentHueValue, 0.6f, 0.8f);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
