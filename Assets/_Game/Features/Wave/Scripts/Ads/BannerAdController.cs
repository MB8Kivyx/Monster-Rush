using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class BannerAdController : MonoBehaviour
{
    public static BannerAdController Instance { get; private set; }

    private BannerView _bannerView;

    [Header("Ad Unit IDs")]
    [SerializeField] private string androidAdUnitId = "ca-app-pub-3025488325095617/4886226012";
#pragma warning disable 0414
    [SerializeField] private string iosAdUnitId = "ca-app-pub-3025488325095617/8035133537";
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
    }

    private void Start()
    {
        // Automatically load banner on start? Or wait for explicit call?
        // User said "banner ads on my main menu scene and game scene".
        // Effectively always visible.
        RequestBanner();
    }

    public void RequestBanner()
    {
        // If we already have a banner, destroy it to load a new one.
        if (_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
        }

        string adUnitId = GetAdUnitId();
        
        // Create a 320x50 banner at the bottom of the screen.
        // Or use AdSize.Banner which adjusts to screen width.
        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        _bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Bottom);

        AdRequest request = new AdRequest();
        _bannerView.LoadAd(request);
        Debug.Log("Requested Banner Ad.");
    }

    public void ShowBanner()
    {
        if (_bannerView != null)
        {
            _bannerView.Show();
        }
        else
        {
            RequestBanner();
        }
    }

    public void HideBanner()
    {
        if (_bannerView != null)
        {
            _bannerView.Hide();
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
        if (_bannerView != null)
        {
            _bannerView.Destroy();
        }
    }
}
