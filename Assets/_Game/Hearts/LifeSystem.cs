using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LifeSystem : MonoBehaviour
{
    [Header("Life Settings")]
    [SerializeField] private int maxLives = 3;
    private int currentLives;

    [Header("Heart UI")]
    [Tooltip("The Image components for the 3 hearts.")]
    [SerializeField] private Image[] heartImages; // Size = 3
    [SerializeField] private Sprite filledHeartSprite;
    [SerializeField] private Sprite unfilledHeartSprite;
    
    [Header("Professional Polish")]
    [SerializeField] private float heartPulseScale = 1.3f;
    [SerializeField] private float heartPulseDuration = 0.3f;

    [Header("UI Panels")]
    [SerializeField] private GameObject revivePanel;
    [SerializeField] private GameObject gameOverPanel;

    private bool isHandlingDeath = false;

    void Start()
    {
        currentLives = maxLives;
        UpdateHeartsUI();
    }

    public void OnPlayerOut()
    {
        if (isHandlingDeath) return;
        isHandlingDeath = true;

        if (currentLives > 0)
        {
            currentLives--;
            UpdateHeartsUI();
            
            // Pulse the heart that was just lost (index = currentLives)
            if (heartImages != null && currentLives < heartImages.Length)
            {
                StartCoroutine(PulseHeart(heartImages[currentLives].gameObject));
            }
        }

        if (currentLives > 0)
        {
            // 1st or 2nd elimination: Seamless restart
            StartCoroutine(SeamlessRestartSequence());
        }
        else
        {
            // 3rd elimination: Handle final death / Revive check
            HandleFinalDeath();
        }
    }

    private void UpdateHeartsUI()
    {
        if (heartImages == null || filledHeartSprite == null || unfilledHeartSprite == null) return;

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null) continue;
            
            // If index is less than currentLives, it's a filled heart
            heartImages[i].sprite = (i < currentLives) ? filledHeartSprite : unfilledHeartSprite;
        }
    }

    private IEnumerator PulseHeart(GameObject heart)
    {
        Vector3 originalScale = heart.transform.localScale;
        Vector3 targetScale = originalScale * heartPulseScale;

        float elapsed = 0f;
        while (elapsed < heartPulseDuration / 2)
        {
            heart.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / (heartPulseDuration / 2));
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < heartPulseDuration / 2)
        {
            heart.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / (heartPulseDuration / 2));
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        heart.transform.localScale = originalScale;
    }

    private IEnumerator SeamlessRestartSequence()
    {
        // Give a tiny moment for pulse and sound
        yield return new WaitForSeconds(0.1f);
        
        if (Player.Instance != null)
        {
            Player.Instance.RevivePlayer();
        }
        
        isHandlingDeath = false;
        Time.timeScale = 1f;
        // AudioListener.pause = false; // Removed to allow sounds to play correctly
    }

    private void HandleFinalDeath()
    {
        // Check ad availability
        bool adReady = RewardedAdController.Instance != null && RewardedAdController.Instance.IsRewardedAdReady();

        Time.timeScale = 0f;
        // AudioListener.pause = true; // Removed so the crash sound can still play even when game is paused/revive panel shown.
        // CarSoundController.Instance.effectsAudioSource.ignoreListenerPause is already true, but this global pause was muting everything.

        if (adReady && revivePanel != null)
        {
            revivePanel.SetActive(true);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnFinalLifeExhausted();
            }
        }
        else
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ShowActualGameOver();
            }
        }
        
        isHandlingDeath = false; // Reset for potential revive
    }

    public void ReviveFromAd()
    {
        currentLives = 1;
        UpdateHeartsUI();
        if (revivePanel != null) revivePanel.SetActive(false);
        
        if (Player.Instance != null)
        {
            Player.Instance.RevivePlayer();
        }
        
        Time.timeScale = 1f;
        // AudioListener.pause = false;
        isHandlingDeath = false;
    }
}
