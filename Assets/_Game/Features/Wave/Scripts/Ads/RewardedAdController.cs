using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class RewardedAdController : MonoBehaviour
{
    public static RewardedAdController Instance { get; private set; }

    private RewardedAd rewardedAd;
    private bool isLoading = false;

    [Header("Ad Unit IDs")]
    [SerializeField] private string androidAdUnitId = "ca-app-pub-3025488325095617/3882968906";
    [SerializeField] private string iosAdUnitId = "ca-app-pub-3025488325095617/5168130413";


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadRewardedAd();
    }

    public void LoadRewardedAd()
    {
        if (isLoading || (rewardedAd != null && rewardedAd.CanShowAd()))
            return;

        isLoading = true;

        // Clean up old ad if exists
        rewardedAd?.Destroy();
        rewardedAd = null;

        string adUnitId = GetAdUnitId();

        var request = new AdRequest();

        // NEW API (v22+): RewardedAd.Load() is static!
        RewardedAd.Load(adUnitId, request, AdLoadCallback);
    }

    private void AdLoadCallback(RewardedAd ad, LoadAdError error)
    {
        if (error != null || ad == null)
        {
            Debug.LogError("Rewarded ad failed to load: " + error);
            isLoading = false;
            // Retry after 10 seconds
            Invoke(nameof(LoadRewardedAd), 10f);
            return;
        }

        Debug.Log("Rewarded ad loaded successfully!");

        rewardedAd = ad;
        isLoading = false;

        // Subscribe to events
        rewardedAd.OnAdFullScreenContentOpened += () => Debug.Log("Rewarded ad opened");
        rewardedAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad closed/skipped");
            LoadRewardedAd(); // Auto reload
        };
        rewardedAd.OnAdFullScreenContentFailed += (AdError err) =>
        {
            Debug.LogError("Rewarded ad failed to show: " + err.GetMessage());
            LoadRewardedAd();
        };
        rewardedAd.OnAdPaid += (AdValue value) => Debug.Log($"Ad paid: {value.Value} {value.CurrencyCode}");
    }

    public void ShowRewardedAd(Action onRewardEarned, Action onAdClosed = null)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"Player earned reward: {reward.Amount} {reward.Type}");
                onRewardEarned?.Invoke();
            });

        }
        else
        {
            Debug.Log("Rewarded ad not ready.");
            onAdClosed?.Invoke();
        }
    }

    // This is the KEY method for hiding the button!
    public bool IsRewardedAdReady()
    {
        return rewardedAd != null && rewardedAd.CanShowAd();
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
            rewardedAd?.Destroy();
            Instance = null;
        }
    }
}