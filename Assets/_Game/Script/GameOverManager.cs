using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [Header("Game Over Panel")]
    public GameObject gameOverPanel;

    private bool isGameOver = false;

    void Start()
    {
        // Start par panel hide rahe
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        // Game freeze
        Time.timeScale = 0f;

        // Panel show
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
}
