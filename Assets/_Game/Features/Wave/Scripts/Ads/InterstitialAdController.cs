using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class InterstitialAdController : MonoBehaviour
{
    public static InterstitialAdController Instance { get; private set; }

    private InterstitialAd interstitialAd;
    private int gameOverCount = 0;
    private const int GAMES_PER_AD = 3; // Show ad every 3 game overs

    [Header("Ad Unit IDs")]
    [SerializeField] private string androidAdUnitId = "ca-app-pub-3025488325095617/2522613098";
#pragma warning disable 0414
    [SerializeField] private string iosAdUnitId = "ca-app-pub-3025488325095617/3381572657";
#pragma warning restore 0414

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadInterstitialAd();
    }

    private void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        string adUnitId = GetAdUnitId();

        AdRequest request = new AdRequest();

        InterstitialAd.Load(adUnitId, request, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Interstitial ad failed to load: " + error?.GetMessage());
                return;
            }

            interstitialAd = ad;
            Debug.Log("Interstitial ad loaded.");

            // Subscribe to full screen content events
            interstitialAd.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Interstitial ad opened.");
            };

            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial ad closed. Reloading...");
                LoadInterstitialAd();
            };

            interstitialAd.OnAdFullScreenContentFailed += (AdError adError) =>
            {
                Debug.LogError("Interstitial ad failed to show: " + adError.GetMessage());
                LoadInterstitialAd(); // Try to load again if it failed
            };
        });
    }

    /// <summary>
    /// Call this method when the player dies/game over.
    /// It tracks game overs and shows ad every 3rd game over.
    /// </summary>
    public void OnGameOver(Action onAdFinished = null)
    {
        gameOverCount++;
        Debug.Log($"GameOver Count: {gameOverCount}/{GAMES_PER_AD}");

        if (gameOverCount >= GAMES_PER_AD)
        {
            ShowInterstitialAd();
            gameOverCount = 0; // Reset counter
        }
        else
        {
            // If we don't show an ad, we should probably still trigger the callback immediately? 
            // In this specific game flow, it seems strictly for display, but good practice.
             onAdFinished?.Invoke();
        }
    }

    private void ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("Interstitial ad is not ready yet.");
            LoadInterstitialAd(); // Try loading again for next time
        }
    }

    private string GetAdUnitId()
    {
#if UNITY_ANDROID
        return androidAdUnitId;
#elif UNITY_IOS
        return iosAdUnitId;
#else
        return "unexpected_platform";
#endif
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            interstitialAd?.Destroy();
            Instance = null;
        }
    }
}
