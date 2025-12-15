using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButton : MonoBehaviour
{
    [Header("Write the name of your Main Menu scene here")]
    public string mainMenuSceneName = "MainMenu";

    // Call this from the UI button
    public void GoToMainMenu()
    {
        // Make sure timeScale is normal (in case game was paused)
        Time.timeScale = 1f;
        
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
