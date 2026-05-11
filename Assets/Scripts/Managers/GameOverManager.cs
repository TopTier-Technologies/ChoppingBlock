using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text finalScoreText;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Text coinsEarnedText;

    void Start()
    {
        LoadAndDisplayScores();
        AudioManager.Instance?.PlayMusic(AudioManager.Instance.menuMusic);
    }

    void LoadAndDisplayScores()
    {
        int lastScore = UserDataStore.GetLastScore();
        int highScore = UserDataStore.GetHighScore();
        int coinsEarned = lastScore / 10;

        if (finalScoreText != null)
        {
            finalScoreText.text = lastScore.ToString();
        }

        if (highScoreText != null)
        {
            highScoreText.text = highScore.ToString();
        }

        if (coinsEarnedText != null)
        {
            coinsEarnedText.text = "+ " + coinsEarned + " coins";
        }

        Debug.Log("GameOver screen loaded - Score: " + lastScore + " | High Score: " + highScore);
    }

    public void TryAgain()
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.buttonClickSound);
        SceneManager.LoadScene("Gameplay");
    }

    public void NewGame()
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.buttonClickSound);
        // Reset score tracking for fresh start
        UserDataStore.SetLastScore(0);
        SceneManager.LoadScene("Gameplay");
    }

    public void GoToMainMenu()
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.buttonClickSound);
        SceneManager.LoadScene("MainMenu");
    }
}
