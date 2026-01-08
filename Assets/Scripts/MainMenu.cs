using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreText;

    private void Start()
    {
        if (highScoreText != null)
        {
            int bestScore = PlayerPrefs.GetInt("BestScore", 0);
            highScoreText.text = bestScore.ToString(); // Or "High Score: " + bestScore
        }

        if (BannerAdController.Instance != null)
        {
            BannerAdController.Instance.ShowBanner();
        }
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("wave");
    }

    public void ExitGame()
{
    #if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
    #else
    Application.Quit();
    #endif
}
}




