using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Assign the generic Panel object (the parent of your Pause Menu) here.")]
    public GameObject pauseMenuUI;


    void Start()
    {
        // Ensure the pause menu is hidden at start
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    // Call this method from your Pause Button
    public void OnPauseButtonPressed()
    {
        
        // Show the Pause UI
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);

        // Tell Game Manager to freeze functionality and MUTE AUDIO
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseGame();
        }
    }

    // Call this method from your Resume Button
    public void OnResumeButtonPressed()
    {

        // Hide the Pause UI
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);

        // Tell Game Manager to unfreeze and UNMUTE AUDIO
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
    }

    public void OnHomeButtonPressed()
    {
        // Ensure time is running before leaving
        Time.timeScale = 1f;
        AudioListener.pause = false; 
        
        // Load Main Menu (Assuming index 0 or name "MainMenu")
        // UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
