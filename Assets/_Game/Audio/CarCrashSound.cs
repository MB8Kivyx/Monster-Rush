using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CarCrashSound : MonoBehaviour
{
    [Header("Crash Sound")]
    [SerializeField] private AudioClip crashSound;

    private AudioSource audioSource;
    private bool hasCrashed;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    // Call this when the car hits an obstacle
    public void PlayCrashSound()
    {
        if (hasCrashed) return;

        hasCrashed = true;
        audioSource.PlayOneShot(crashSound);
    }

    // Reset when gameplay restarts
    public void ResetCrashState()
    {
        hasCrashed = false;
    }
}
