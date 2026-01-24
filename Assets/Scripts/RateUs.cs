using UnityEngine;
using UnityEngine.UI;

public class RateUs : MonoBehaviour
{
    private const string PLAY_STORE_URL = "https://play.google.com/store/apps/details?id=com.darkstarstudios.monsterrush";
    private const string GAMES_PLAYED_KEY = "GamesPlayedForRating";
    private const int RATE_THRESHOLD = 3; // Number of games before suggesting rating

    [SerializeField] private GameObject rateUsButton; // Reference to the Rate Us button if it should be hidden

    private void Start()
    {
        // For the Main Menu, we might want to hide the button until the threshold is met
        // or just have it always visible if the user explicitly wants to rate.
        // The user mentioned "not at first launch" and "after few levels".
        
        if (rateUsButton != null)
        {
            rateUsButton.SetActive(ShouldShowRateUs());
        }
    }

    /// <summary>
    /// Opens the Play Store page for the game.
    /// Call this from the Rate Us button's OnClick event.
    /// </summary>
    public void OpenPlayStore()
    {
        Application.OpenURL(PLAY_STORE_URL);
        
        // Optional: Mark that the user has been directed to the store
        PlayerPrefs.SetInt("HasClickedRateUs", 1);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Checks if the user has played enough to be asked for a rating.
    /// </summary>
    public bool ShouldShowRateUs()
    {
        // Don't show if they already clicked it
        if (PlayerPrefs.GetInt("HasClickedRateUs", 0) == 1)
        {
            return false;
        }

        int gamesPlayed = PlayerPrefs.GetInt(GAMES_PLAYED_KEY, 0);
        return gamesPlayed >= RATE_THRESHOLD;
    }

    /// <summary>
    /// Call this from the GameManager when a level/game ends to track progress.
    /// </summary>
    public static void IncrementGamesPlayed()
    {
        int currentCount = PlayerPrefs.GetInt(GAMES_PLAYED_KEY, 0);
        PlayerPrefs.SetInt(GAMES_PLAYED_KEY, currentCount + 1);
        PlayerPrefs.Save();
    }
}
