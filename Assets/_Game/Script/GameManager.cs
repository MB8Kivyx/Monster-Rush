using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; 
using UnityEngine.UI; // For heart images


public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameOverPanel;
    public GameObject revivePanel;
    public TextMeshProUGUI countdownText;

    public GameObject Panel;
    public GameObject gameOverwindow;
    public GameObject Look;

    [Header("Life System Component")]
    [SerializeField] private LifeSystem lifeSystem;

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
        
        if (lifeSystem == null) lifeSystem = FindFirstObjectByType<LifeSystem>();
        
        Time.timeScale = 1f; // Game running
        AudioListener.pause = false; // Unmute all sounds

        // Track that a game session has started
        RateUs.IncrementGamesPlayed();

        if (BannerAdController.Instance != null)
        {
            BannerAdController.Instance.ShowBanner();
        }
    }


    public void OnPlayerEliminated()
    {
        if (lifeSystem == null) lifeSystem = FindFirstObjectByType<LifeSystem>();

        if (lifeSystem != null)
        {
            lifeSystem.OnPlayerOut();
        }
        else
        {
            Debug.LogError("GameManager: LifeSystem reference missing!");
            ShowActualGameOver();
        }
    }

    public void OnFinalLifeExhausted()
    {
        if (countdownText == null && revivePanel != null)
            countdownText = revivePanel.GetComponentInChildren<TextMeshProUGUI>();

        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);
        countdownCoroutine = StartCoroutine(StartReviveCountdown());
    }

    private IEnumerator StartReviveCountdown()
    {
        int countdown = 5;
        while (countdown > 0)
        {
            if (countdownText != null)
                countdownText.text = countdown.ToString();
            
            yield return new WaitForSecondsRealtime(1f);
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
    }

    public void OnWatchAdButtonClicked()
    {
        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);
        
        if (RewardedAdController.Instance != null && RewardedAdController.Instance.IsRewardedAdReady())
        {
            RewardedAdController.Instance.ShowRewardedAd(() => 
            {
                Revive();
            }, () => 
            {
                ShowActualGameOver();
            });
        }
        else
        {
            ShowActualGameOver();
        }
    }

    public void CancelRevive()
    {
        // Stop the countdown
        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);
        
        // Show game over panel immediately
        ShowActualGameOver();
        
        Debug.Log("Revive cancelled by user.");
    }

    public void Revive()
    {
        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);
        
        if (revivePanel != null) revivePanel.SetActive(false);
        
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (lifeSystem != null)
        {
            lifeSystem.ReviveFromAd();
        }
        else if (Player.Instance != null)
        {
            Player.Instance.RevivePlayer();
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

    // Helper to find inactive objects by name (used sparingly)
    private GameObject FindObjectInactive(string name)
    {
        foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (obj.name == name) return obj;
        }
        return null;
    }
}