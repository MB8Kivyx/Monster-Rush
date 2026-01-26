using System.Collections;
using UnityEngine;

public class CarSoundController : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource carAudioSource;
    public AudioClip engineClip; // Single engine sound (assign Jeep_sound or Sound 01 here)

    [Header("Engine Pitch/Volume Settings")]
    [Tooltip("Volume when NOT touching (Idle)")]
    [Range(0f, 1f)] public float idleVolume = 0.3f;

    [Tooltip("Volume when TOUCHING (Revving)")]
    [Range(0f, 1f)] public float revVolume = 1.0f;

    [Tooltip("Pitch when NOT touching (Idle)")]
    [Range(0.5f, 3f)] public float idlePitch = 0.8f;

    [Tooltip("Pitch when TOUCHING (Revving)")]
    [Range(0.5f, 3f)] public float revPitch = 1.5f;

    [Tooltip("How fast the sound changes.")]
    public float fadeSpeed = 3.0f;

    [Header("Global Setting")]
    [Tooltip("Turn sound on/off globally. Syncs with PlayerPrefs.")]
    public bool isSoundOn = true;

    [Header("Engine State (Read-only)")]
    public bool isLooping = false; 
    public float currentNormalizedSpeed = 0f;

    private bool wasPausedByTimeScale = false;
    public static CarSoundController Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        isSoundOn = PlayerPrefs.GetInt("IsSoundOn", 1) == 1;
    }

    void Start()
    {
        if (carAudioSource == null)
            carAudioSource = GetComponent<AudioSource>();

        if (carAudioSource == null)
            carAudioSource = gameObject.AddComponent<AudioSource>();

        carAudioSource.playOnAwake = false;

        if (isSoundOn)
        {
            StartEngineLoop();
        }
    }

    private void StartEngineLoop()
    {
        if (engineClip != null)
        {
            isLooping = true;
            carAudioSource.loop = true;
            carAudioSource.clip = engineClip;
            carAudioSource.volume = idleVolume;
            carAudioSource.pitch = idlePitch;
            carAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("CarSoundController: No engineClip assigned in Inspector!");
        }
    }

    void Update()
    {
        if (carAudioSource == null) return;

        UpdateSoundToggleState();

        if (HandlePauseState()) return;

        if (isSoundOn)
        {
            SyncWithPlayerSpeed();
            ApplyAudioModulation();
        }
    }

    private void UpdateSoundToggleState()
    {
        // sync with the inspector/PlayerPrefs
        if (isSoundOn && !carAudioSource.isPlaying && !wasPausedByTimeScale && Time.timeScale > 0)
        {
            if (isLooping) carAudioSource.Play();
            else StartEngineLoop();
        }
        else if (!isSoundOn && carAudioSource.isPlaying)
        {
            carAudioSource.Stop();
        }
    }

    private bool HandlePauseState()
    {
        if (Time.timeScale == 0)
        {
            if (!wasPausedByTimeScale)
            {
                if (carAudioSource.isPlaying) carAudioSource.Pause();
                wasPausedByTimeScale = true;
            }
            return true; 
        }
        else if (wasPausedByTimeScale)
        {
            if (isSoundOn) carAudioSource.UnPause();
            wasPausedByTimeScale = false;
        }
        return false;
    }

    private void SyncWithPlayerSpeed()
    {
        if (Player.Instance != null)
        {
            // The Player script now handles the gradual change of speed.
            // We just follow that value.
            currentNormalizedSpeed = Player.Instance.NormalizedSpeed;
        }
        else
        {
            // Fallback to input if Player instance is missing
            float target = Input.GetMouseButton(0) ? 1f : 0f;
            currentNormalizedSpeed = Mathf.MoveTowards(currentNormalizedSpeed, target, Time.deltaTime * 0.5f);
        }
    }

    private void ApplyAudioModulation()
    {
        // Target values based on normalized speed
        float targetVolume = Mathf.Lerp(idleVolume, revVolume, currentNormalizedSpeed);
        float targetPitch = Mathf.Lerp(idlePitch, revPitch, currentNormalizedSpeed);

        // Smoothly interpolate audio changes (fadeSpeed acts as additional dampening for realism)
        carAudioSource.volume = Mathf.Lerp(carAudioSource.volume, targetVolume, Time.deltaTime * fadeSpeed);
        carAudioSource.pitch = Mathf.Lerp(carAudioSource.pitch, targetPitch, Time.deltaTime * fadeSpeed);
    }

    public void ToggleSoundExternally(bool isOn)
    {
        isSoundOn = isOn;
        PlayerPrefs.SetInt("IsSoundOn", isOn ? 1 : 0);
        PlayerPrefs.Save();
        
        if (!isOn) carAudioSource.Stop();
        else if (!carAudioSource.isPlaying) StartEngineLoop();
    }
}
