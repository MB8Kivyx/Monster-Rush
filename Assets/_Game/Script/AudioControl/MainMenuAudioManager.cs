using UnityEngine;

public class MainMenuAudioManager : MonoBehaviour
{
    [Header("Main Menu Music Settings")]
    [Tooltip("Drag your music file here in the Inspector")]
    public AudioClip menuMusic;
    
    [Tooltip("Volume of the music (0 to 1)")]
    [Range(0f, 1f)]
    public float musicVolume = 1.0f;

    [Tooltip("Should the music loop?")]
    public bool loopMusic = true;

    [Header("Internal References")]
    public AudioSource audioSource;

    void Start()
    {
        // Check if we have an AudioSource, if not, add one
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        // Sync with global sound setting
        bool isSoundOn = PlayerPrefs.GetInt("IsSoundOn", 1) == 1;
        if (isSoundOn)
        {
            PlayMusic();
        }
    }

    public void PlayMusic()
    {
        // Double check global setting
        if (PlayerPrefs.GetInt("IsSoundOn", 1) == 0) return;

        if (menuMusic != null)
        {
            audioSource.clip = menuMusic;
            audioSource.volume = musicVolume;
            audioSource.loop = loopMusic;
            audioSource.playOnAwake = true;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("MainMenuAudioManager: No AudioClip assigned! Please drag a sound file into the 'Menu Music' slot in the Inspector.");
        }
    }

    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    // Helper to change music at runtime if needed
    public void SetMusic(AudioClip newClip)
    {
        menuMusic = newClip;
        PlayMusic();
    }
}
