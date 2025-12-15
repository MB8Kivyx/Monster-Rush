using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    [Header("Write the name of your Game Scene here")]
    public string gameSceneName = "GameScene";

    // Call this from the UI Start Button
    public void StartGame()
    {
        // Ensure time scale is normal
        Time.timeScale = 1f;

        SceneManager.LoadScene(gameSceneName);
    }
}
