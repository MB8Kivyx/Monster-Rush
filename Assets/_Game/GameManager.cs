using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // For TextMeshPro (install if needed: Window > Package Manager > TextMeshPro)

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameOverPanel;

    public GameObject Panel;
    public GameObject gameOverwindow;
    public TextMeshProUGUI scoreText; // Drag ScoreText here
    public TextMeshProUGUI gameOverText; // Drag GameOverText here (optional)

    private int score = 0; // Your game score

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        Time.timeScale = 1f; // Game running
        UpdateScoreUI();
    }

    public void GameOver()
    {
        Time.timeScale = 0f; // PAUSE EVERYTHING (physics, animations, etc.)
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        // Optional: Flash text or shake (add later)
        if (gameOverText != null) gameOverText.text = "GAME OVER\nFinal Score: " + score;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload current scene
    }

    // Call this from your score system (e.g., collect coin: score++, UpdateScoreUI())
    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
    }
}