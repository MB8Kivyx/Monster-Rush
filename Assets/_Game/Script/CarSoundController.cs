using System.Collections;
using UnityEngine;

public class CarSoundController : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource carAudioSource;
    public AudioClip engineStartClip; // The "Vroom" start sound
    public AudioClip engineLoopClip;  // The continuous "Hummm" sound (formerly carSoundClip)

    [Header("Engine Settings")]
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

    private bool isTouching = false;
    private bool isLooping = false; // Check if we are in the loop phase

    public static CarSoundController Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    void Start()
    {
        if (carAudioSource == null)
            carAudioSource = GetComponent<AudioSource>();

        if (carAudioSource == null)
            carAudioSource = gameObject.AddComponent<AudioSource>();

        StartCoroutine(PlayEngineSequence());
    }

    IEnumerator PlayEngineSequence()
    {
        // 1. Play Start Sound (if assigned)
        if (engineStartClip != null)
        {
            isLooping = false; // Pitch/Volume logic logic disabled/dampened during start
            carAudioSource.loop = false;
            carAudioSource.clip = engineStartClip;
            carAudioSource.volume = 1f; // Usually start sound is loud
            carAudioSource.pitch = 1f;  // Normal pitch for start
            carAudioSource.Play();

            // Wait until start sound finishes
            yield return new WaitForSeconds(engineStartClip.length);
        }

        // 2. Switch to Loop Sound
        if (engineLoopClip != null)
        {
            isLooping = true;
            carAudioSource.loop = true;
            carAudioSource.clip = engineLoopClip;
            carAudioSource.Play();
        }
    }

    private bool wasPausedByTimeScale = false;

    void Update()
    {
        if (carAudioSource == null) return;

        // 1. Handle Pause/Resume based on Time.timeScale
        if (Time.timeScale == 0)
        {
            if (!wasPausedByTimeScale)
            {
                if (carAudioSource.isPlaying) carAudioSource.Pause();
                wasPausedByTimeScale = true;
            }
            return; // Skip pitch/volume updates while paused
        }
        else if (wasPausedByTimeScale)
        {
            carAudioSource.UnPause();
            wasPausedByTimeScale = false;
        }

        // 2. Normal Input Handling
        isTouching = Input.GetMouseButton(0);

        // Only modulate engine sound if we are in the Looping phase
        if (isLooping)
        {
            HandleEngineSound();
        }
    }

    void HandleEngineSound()
    {
        if (carAudioSource == null) return;

        // Target Values
        float targetVolume = isTouching ? revVolume : idleVolume;
        float targetPitch = isTouching ? revPitch : idlePitch;

        // Smoothly Interpolate (Lerp) towards target
        carAudioSource.volume = Mathf.Lerp(carAudioSource.volume, targetVolume, Time.deltaTime * fadeSpeed);
        carAudioSource.pitch = Mathf.Lerp(carAudioSource.pitch, targetPitch, Time.deltaTime * fadeSpeed);

        // Ensure it stays playing unless paused
        if (!carAudioSource.isPlaying && carAudioSource.isActiveAndEnabled && !wasPausedByTimeScale)
        {
            carAudioSource.Play();
        }
    }
}
