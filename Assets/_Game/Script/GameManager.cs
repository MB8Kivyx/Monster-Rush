using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameOverPanel;

    public GameObject Panel;
    public GameObject gameOverwindow;
    public GameObject Look;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }



    void Start()
    {
        Debug.Log("GameManager: Start called. Resetting Time.timeScale to 1.");
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        Time.timeScale = 1f; // Game running
        AudioListener.pause = false; // Unmute all sounds
    }

    public void GameOver()
    {
        Time.timeScale = 0f; // PAUSE EVERYTHING (physics, animations, etc.)
        AudioListener.pause = true; // MUTE EVERYTHING

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
        AudioListener.pause = false; // Ensure sound is back on
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload current scene
    }

    // --- PAUSE SYSTEM ---
    public void PauseGame()
    {
        Time.timeScale = 0f; // Freeze Game
        AudioListener.pause = true; // Mute Audio
        
        Debug.Log("Game Paused: Audio Muted.");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Unfreeze Game
        AudioListener.pause = false; // Unmute Audio

        Debug.Log("Game Resumed: Audio Playing.");
    }

    // Call this from your score system (e.g., collect coin: score++, UpdateScoreUI())
    // REMOVED: Conflicting score logic. Use ScoreManager.Instance.IncrementScore() instead.
}