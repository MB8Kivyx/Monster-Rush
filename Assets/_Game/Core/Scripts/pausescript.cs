using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;   // optional: agar pause menu UI dikhana ho
    public Button pauseButton;
    public Button resumeButton;

    void Start()
    {
        // Button listeners set karo
        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        // Initially resume state
        ResumeGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; // ⏸️ sab movement / physics / updates ruk jaayenge
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);  // optional pause menu dikhana ho to
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // ▶️ game resume
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
    }
}




