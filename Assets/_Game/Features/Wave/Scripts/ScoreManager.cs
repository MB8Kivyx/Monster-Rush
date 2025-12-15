using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// The ScoreManager class is responsible for managing the player's score, including updating the current score and the best score.
/// It handles score incrementation, display of current and best scores, and saving the best score using PlayerPrefs.
/// </summary>
public class ScoreManager : MonoBehaviour {

    public static ScoreManager Instance { get; private set; } // Singleton instance of ScoreManager


    private int currentScore = 0;
    [SerializeField] private Text currentScoreText;
    [SerializeField] private Text bestScoreText;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate ScoreManager detected. Destroying redundant instance.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Initialize the score display
        currentScoreText.text = "0";
        bestScoreText.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
    }

    // Adds one to the current score and updates the UI
    public void IncrementScore()
    {
        currentScore++;
        currentScoreText.text = currentScore.ToString();

        // Update best score if the current score exceeds it
        if (currentScore > PlayerPrefs.GetInt("BestScore", 0))
        {
            PlayerPrefs.SetInt("BestScore", currentScore);
            PlayerPrefs.Save();
            bestScoreText.text = currentScore.ToString();
        }
    }

    // Returns the current score
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
