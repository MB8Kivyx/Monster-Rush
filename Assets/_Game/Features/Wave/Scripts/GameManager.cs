using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
// using TMPro;


/// <summary>
/// The WaveGameManager class handles game state management such as game over, restarting the game, and initial UI interactions.
/// It controls the display of game over panels, manages game time scale, and handles scene reloading.
/// </summary>
public class WaveGameManager : MonoBehaviour
{

    
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private GameObject GameOverEffectPanel;
    [SerializeField] private GameObject touchToMoveTextObj;
    [SerializeField] private GameObject StartFadeInObj;
    
    
    void Awake()
    {
        Application.targetFrameRate = 60; // Set the target frame rate to 60 FPS
        Time.timeScale = 1.0f; // Ensure the game time scale is set to normal speed
        
        StartCoroutine(FadeInEffect()); // Start the fade-in effect at the beginning
    }

    private void Update()
    {
        // If the touchToMoveText is active and the player touches the screen, deactivate the text
        if (touchToMoveTextObj.activeSelf == false) return;
        if (Input.GetMouseButton(0))
        {
            touchToMoveTextObj.SetActive(false);
        }
    }

    // Coroutine to handle the fade-in effect at the start of the game
    private IEnumerator FadeInEffect()
    {
        StartFadeInObj.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        StartFadeInObj.SetActive(false);
    }
    
    // Method to be called when the game is over
    public void GameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    // Coroutine to handle the game over sequence
    private IEnumerator GameOverSequence()
    {
        GameOverEffectPanel.SetActive(true);
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(0.5f);
        GameOverPanel.SetActive(true);
    }

    // Method to restart the game by reloading the current scene
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
