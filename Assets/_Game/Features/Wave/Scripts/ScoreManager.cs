using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;


public class ScoreManager : MonoBehaviour {

    public static ScoreManager Instance { get; private set; } // Singleton instance of ScoreManager


    private int currentScore = 0;
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI gameOverCurrentScoreText; // New: For Game Over panel
    [SerializeField] private TextMeshProUGUI bestScoreText; // Keeping this if used elsewhere, but user wants MainMenu highscore separately


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate ScoreManager detected. Destroying redundant instance.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Reset current score to 0 when starting a new game
        currentScore = 0;

        // Initialize the score display
        if (currentScoreText != null)
        {
            currentScoreText.text = "0";
        }
        
        // Initialize Game Over score text
        if (gameOverCurrentScoreText != null)
        {
            gameOverCurrentScoreText.text = "0";
        }
        else
        {
            // Fallback
            var foundText = GameObject.Find("CurrentScoreText"); 
            if (foundText != null)
            {
                 currentScoreText = foundText.GetComponent<TextMeshProUGUI>();
                 if (currentScoreText != null) currentScoreText.text = "0";
            }
            else
            {
                 Debug.LogError("ScoreManager: currentScoreText is not assigned and could not be found!", this);
            }
        }
        
        if (bestScoreText != null)
        {
            bestScoreText.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
        }
        else
        {
            Debug.LogError("ScoreManager: bestScoreText is not assigned in the Inspector!", this);
        }
    }

    // Adds one to the current score and updates the UI
    public void IncrementScore()
    {
        currentScore++;
        
        // Update current score text
        if (currentScoreText != null)
        {
            currentScoreText.text = currentScore.ToString();
        }
        
        // Update Game Over score text
        if (gameOverCurrentScoreText != null)
        {
            gameOverCurrentScoreText.text = currentScore.ToString();
        }
        else
        {
            Debug.LogWarning("ScoreManager: Cannot update current score - currentScoreText is null! Current score: " + currentScore, this);
        }
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);
        
        if (currentScore > bestScore)
        {
            PlayerPrefs.SetInt("BestScore", currentScore);
            PlayerPrefs.Save();
            bestScore = currentScore; 
        }
        
        if (bestScoreText != null)
        {
            bestScoreText.text = bestScore.ToString();
        }
    }
    
    public int GetBestScore()
    {
        return PlayerPrefs.GetInt("BestScore", 0);
    }

    public int GetScore()
    {
        return currentScore;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }


}
