using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("User Info")]
    [SerializeField] private int userCoins = 25;
    [SerializeField] private Text coinsText;

    void Start()
    {
        // Load user coins from saved data later
        LoadUserData();
        UpdateCoinsDisplay();

        // Play menu music
        AudioManager.Instance?.PlayMusic(AudioManager.Instance.menuMusic);
    }

    public void PlayGame()
    {
        // Load the gameplay scene
        SceneManager.LoadScene("Gameplay");
    }

    public void OpenAccount()
    {
        SceneManager.LoadScene("Account");
    }

    public void OpenBackgroundSelect()
    {
        SceneManager.LoadScene("BackgroundSelect");
    }

    public void OpenTrophyCase()
    {
        SceneManager.LoadScene("TrophyCase");
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void ToggleSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleMute();
        }
        else
        {
            Debug.LogWarning("AudioManager not found - cannot toggle sound");
        }
    }

    void UpdateCoinsDisplay()
    {
        if (coinsText != null)
        {
            coinsText.text = userCoins.ToString();
        }
    }

    void LoadUserData()
    {
        // Load coins from PlayerPrefs (saved data)
        userCoins = UserDataStore.GetCoins(25);
        UserDataStore.SetCoins(userCoins);
    }

    public static void AddCoins(int amount)
    {
        UserDataStore.AddCoins(amount);
    }

    public static int GetCoins()
    {
        return UserDataStore.GetCoins(25);
    }
}
