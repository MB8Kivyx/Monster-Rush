using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;

public class AdMobInitializer : MonoBehaviour
{
    public static AdMobInitializer Instance { get; private set; }

    [Header("AdMob App IDs")]
    [SerializeField] private string androidAppId = "ca-app-pub-3025488325095617~2532159878";
#pragma warning disable 0414
    [SerializeField] private string iosAppId = "ca-app-pub-3025488325095617~2260062676";
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
        // Initialize UMP (User Messaging Platform) for GDPR consent
        ConsentRequestParameters request = new ConsentRequestParameters
        {
            TagForUnderAgeOfConsent = false,
        };

        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }

    private void OnConsentInfoUpdated(FormError error)
    {
        if (error != null)
        {
            Debug.LogError("Consent info update failed: " + error.Message);
            InitializeAdMob(); // fallback to initialize AdMob even if consent fails
            return;
        }

        if (ConsentInformation.IsConsentFormAvailable())
        {
            LoadConsentForm();
        }
        else
        {
            InitializeAdMob();
        }
    }

    private void LoadConsentForm()
    {
        ConsentForm.Load((ConsentForm form, FormError error) =>
        {
            if (error != null)
            {
                Debug.LogError("Consent form load failed: " + error.Message);
                InitializeAdMob();
                return;
            }

            if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
            {
                form.Show((FormError showError) =>
                {
                    if (showError != null)
                    {
                        Debug.LogError("Consent form show failed: " + showError.Message);
                    }

                    InitializeAdMob();
                });
            }
            else
            {
                InitializeAdMob();
            }
        });
    }

    // Public entry point used by GameManager or other systems
    public void InitializeAds()
    {
        InitializeAdMob();
    }

    private void InitializeAdMob()
    {
        string appId = GetAppId();

        Debug.Log("Initializing AdMob with App ID: " + appId);

        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            Debug.Log("AdMob initialization complete.");

            // Optional: Log individual adapter statuses
            foreach (var adapter in initStatus.getAdapterStatusMap())
            {
                Debug.Log($"Adapter: {adapter.Key}, State: {adapter.Value.InitializationState}, Description: {adapter.Value.Description}");
            }
        });
    }

    private string GetAppId()
    {
#if UNITY_ANDROID
        return androidAppId;
#elif UNITY_IOS
        return iosAppId;
#else
        return "unexpected_platform";
#endif
    }

    // Convenience wrappers so existing GameManager calls compile
    public void ShowRewarded(System.Action onRewardedComplete)
    {
        if (RewardedAdController.Instance != null)
        {
            RewardedAdController.Instance.ShowRewardedAd(onRewardedComplete);
        }
        else
        {
            Debug.LogWarning("AdMobInitializer: RewardedAdController.Instance is null, cannot show rewarded ad.");
        }
    }

    public void ShowInterstitial()
    {
        if (InterstitialAdController.Instance != null)
        {
            // Let the interstitial controller handle frequency logic
            InterstitialAdController.Instance.OnGameOver();
        }
        else
        {
            Debug.LogWarning("AdMobInitializer: InterstitialAdController.Instance is null, cannot show interstitial ad.");
        }
    }
}
