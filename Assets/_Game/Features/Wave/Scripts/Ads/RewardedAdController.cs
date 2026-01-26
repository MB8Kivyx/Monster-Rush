using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class RewardedAdController : MonoBehaviour
{
    public static RewardedAdController Instance { get; private set; }

    private RewardedAd rewardedAd;
    private bool isLoading = false;

    [Header("Ad Unit IDs")]
    [SerializeField] private string androidAdUnitId = "ca-app-pub-3025488325095617/3265645280";
#pragma warning disable 0414
    [SerializeField] private string iosAdUnitId = "ca-app-pub-3025488325095617/2068490987";
#pragma warning restore 0414


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

    private Action _onRewardEarned;
    private Action _onAdClosed;
    private bool _rewardEarned;

    public void ShowRewardedAd(Action onRewardEarned, Action onAdClosed = null)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            _onRewardEarned = onRewardEarned;
            _onAdClosed = onAdClosed;
            _rewardEarned = false;

            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"Player earned reward: {reward.Amount} {reward.Type}");
                _rewardEarned = true;
                _onRewardEarned?.Invoke();
                _onRewardEarned = null; // Prevent double trigger
            });
        }
        else
        {
            Debug.Log("Rewarded ad not ready.");
            onAdClosed?.Invoke();
        }
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
            
            // If reward wasn't earned (user closed early or skip), trigger failure callback
            if (!_rewardEarned)
            {
                _onAdClosed?.Invoke();
            }
            
            _onRewardEarned = null;
            _onAdClosed = null;
            
            LoadRewardedAd(); // Auto reload
        };

        rewardedAd.OnAdFullScreenContentFailed += (AdError err) =>
        {
            Debug.LogError("Rewarded ad failed to show: " + err.GetMessage());
            
            _onAdClosed?.Invoke();
            _onRewardEarned = null;
            _onAdClosed = null;
            
            LoadRewardedAd();
        };
        
        rewardedAd.OnAdPaid += (AdValue value) => Debug.Log($"Ad paid: {value.Value} {value.CurrencyCode}");
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