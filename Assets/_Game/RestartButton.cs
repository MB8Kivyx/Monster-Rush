using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    // This function is called by the UI Button
    public void Restart()
    {
        // Reload the currently active scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // Optional: Quit Game
    public void QuitGame()
    {
        Application.Quit();
    }
}
