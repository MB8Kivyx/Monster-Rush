using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; // For countdown text


public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameOverPanel;
    public GameObject revivePanel;
    public TextMeshProUGUI countdownText;

    public GameObject Panel;
    public GameObject gameOverwindow;
    public GameObject Look;

    private Coroutine countdownCoroutine;

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
        if (revivePanel != null) revivePanel.SetActive(false);
        Time.timeScale = 1f; // Game running
        AudioListener.pause = false; // Unmute all sounds

        if (BannerAdController.Instance != null)
        {
            BannerAdController.Instance.ShowBanner();
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0f; // PAUSE EVERYTHING (physics, animations, etc.)
        AudioListener.pause = true; // MUTE EVERYTHING

        // Robust check for revivePanel (including inactive objects)
        if (revivePanel == null)
        {
            // Try common names
            revivePanel = FindObjectInactive("RevivePanel");
            if (revivePanel == null) revivePanel = FindObjectInactive("Revive Panel");
            if (revivePanel == null) revivePanel = FindObjectInactive("ReviveWindow");
            
            // Try searching for ANY object with "Revive" in the name if still null
            if (revivePanel == null)
            {
                foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
                {
                    // Case-insensitive check for "Revive"
                    if (obj.name.ToLower().Contains("revive"))
                    {
                        revivePanel = obj;
                        Debug.Log("GameManager: Found potential Revive Panel: " + obj.name);
                        break;
                    }
                }
            }

            if (revivePanel == null) revivePanel = Look; 
        }

        if (countdownText == null && revivePanel != null)
        {
            countdownText = revivePanel.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (revivePanel != null)
        {
            revivePanel.SetActive(true);
            if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);
            countdownCoroutine = StartCoroutine(StartReviveCountdown());
        }
        else
        {
            Debug.LogWarning("GameManager: revivePanel reference missing! Showing GameOver directly.");
            ShowActualGameOver();
        }
    }

    private IEnumerator StartReviveCountdown()
    {
        int countdown = 5;
        while (countdown > 0)
        {
            if (countdownText != null)
                countdownText.text = countdown.ToString();
            
            yield return new WaitForSecondsRealtime(1f); // Use Realtime since timeScale is 0
            countdown--;
        }

        ShowActualGameOver();
    }

    public void ShowActualGameOver()
    {
        if (revivePanel != null) revivePanel.SetActive(false);
        
        if (gameOverPanel != null) 
        {
            gameOverPanel.SetActive(true);
            
            if (InterstitialAdController.Instance != null)
            {
                InterstitialAdController.Instance.OnGameOver();
            }
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

    public void OnWatchAdButtonClicked()
    {
        // call your rewarded ad script
        if (RewardedAdController.Instance != null && RewardedAdController.Instance.IsRewardedAdReady())
        {
            RewardedAdController.Instance.ShowRewardedAd(() => 
            {
                // On Reward Earned
                Revive();
            }, () => 
            {
                // On Ad Closed (without reward or failed)
                // We could keep the countdown or show GameOver
                // In many games, if ad fails, we just don't revive.
                Debug.Log("Ad closed without reward or failed.");
            });
        }
        else
        {
            Debug.LogWarning("Rewarded Ad not ready!");
            // Optionally show a message to user
        }
    }

    public void Revive()
    {
        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);
        
        if (revivePanel != null) revivePanel.SetActive(false);
        
        Time.timeScale = 1f;
        AudioListener.pause = false;

        // Reset player state
        if (Player.Instance != null)
        {
            Player.Instance.RevivePlayer();
        }

        Debug.Log("Player Revived!");
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
    // Helper to find inactive objects by name
    private GameObject FindObjectInactive(string name)
    {
        foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (obj.name == name) return obj;
        }
        return null;
    }
}