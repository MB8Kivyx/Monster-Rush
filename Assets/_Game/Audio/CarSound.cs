using UnityEngine;

public class CarSound : MonoBehaviour
{
    public AudioSource engineSource;

    [Header("Sound Settings")]
    public float minPitch = 0.8f;
    public float maxPitch = 2.0f;
    public float minVolume = 0.3f;
    public float maxVolume = 1.0f;

    [Header("Player")]
    public Rigidbody2D playerRB;

    private bool isGameOver = false;

    void Start()
    {
        if (engineSource == null) engineSource = GetComponent<AudioSource>(); // Failure safety
        
        if (engineSource != null)
        {
            engineSource.loop = true;
            engineSource.pitch = minPitch;
            engineSource.volume = minVolume;
            engineSource.Play();
        }
    }

    private bool wasPausedByTimeScale = false;

    void Update()
    {
        if (engineSource == null) return;

        // 1. Handle Pause/Resume based on Time.timeScale
        if (Time.timeScale == 0)
        {
            if (!wasPausedByTimeScale)
            {
                if (engineSource.isPlaying) engineSource.Pause();
                wasPausedByTimeScale = true;
            }
            return; // Skip pitch/volume updates while paused
        }
        else if (wasPausedByTimeScale)
        {
            engineSource.UnPause();
            wasPausedByTimeScale = false;
        }

        if (isGameOver) return;

        // Get Speed from Player Script directly (since RB velocity might be 0 due to manual movement)
        float speed = 0f;
        if (Player.Instance != null)
        {
            speed = Player.Instance.CurrentSpeed;
        }
        else if (playerRB != null)
        {
            // Fallback
            speed = playerRB.linearVelocity.y;
        }

        // Normalizing speed (assuming max speed around 20-30 now)
        // You can adjust the divider '20f' to tune when pitch hits max
        float t = Mathf.Clamp01(Mathf.Abs(speed) / 20f); 

        float targetPitch = Mathf.Lerp(minPitch, maxPitch, t);
        float targetVolume = Mathf.Lerp(minVolume, maxVolume, t);

        engineSource.pitch = Mathf.Lerp(engineSource.pitch, targetPitch, Time.deltaTime * 3f);
        engineSource.volume = Mathf.Lerp(engineSource.volume, targetVolume, Time.deltaTime * 3f);
    }

    public void PlayerOut()
    {
        isGameOver = true;
        if (engineSource != null) engineSource.Stop();
    }

    public void PauseSoundByUser()
    {
        if (engineSource != null && engineSource.isPlaying)
        {
            engineSource.Pause();
        }
    }

    public void ResumeSoundByUser()
    {
        if (!isGameOver && engineSource != null)
        {
            engineSource.UnPause();
        }
    }

    // 🔥 NEW METHOD TO RESUME SOUND
    public void ResumeSound()
    {
        isGameOver = false;

        if (engineSource != null)
        {
            // If it was paused, UnPause; otherwise Play
            engineSource.UnPause();
            if (!engineSource.isPlaying)
            {
                engineSource.Play();
            }
        }
    }
}
