using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameOverPanel;

    public GameObject Panel;
    public GameObject gameOverwindow;
    public GameObject Look;


    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        Time.timeScale = 1f; // Game running
    }

    public void GameOver()
    {
        Time.timeScale = 0f; // PAUSE EVERYTHING (physics, animations, etc.)
        if (gameOverPanel != null) 
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            // Fallback: Try to find it
            Debug.LogWarning("GameManager: gameOverPanel reference missing! Trying to find it...");
            var panel = GameObject.Find("GameOverPanel");
            if (panel != null)
            {
                panel.SetActive(true);
                gameOverPanel = panel; // Cache it
            }
            else
            {
                Debug.LogError("GameManager: Could not find GameOverPanel object!");
            }
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload current scene
    }

    // Call this from your score system (e.g., collect coin: score++, UpdateScoreUI())
    // REMOVED: Conflicting score logic. Use ScoreManager.Instance.IncrementScore() instead.
}