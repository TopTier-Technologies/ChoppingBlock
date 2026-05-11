using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the Account/Profile screen.
/// Lets the player set their display name and age range, and shows their points.
/// All data is saved locally via UserDataStore (PlayerPrefs).
/// </summary>
public class AccountManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private InputField nameInputField;
    [SerializeField] private Dropdown ageRangeDropdown;
    [SerializeField] private Text pointsText;
    [SerializeField] private Text feedbackText;

    // Age range options must match dropdown order in the scene
    // 0 = "8-9", 1 = "10-11", 2 = "12+"
    private static readonly string[] AgeRangeLabels = { "8-9", "10-11", "12+" };

    void Start()
    {
        LoadProfile();
        UpdatePointsDisplay();

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        // Play menu music
        AudioManager.Instance?.PlayMusic(AudioManager.Instance.menuMusic);
    }

    void LoadProfile()
    {
        string savedName = UserDataStore.GetPlayerName("Player");
        int savedAgeIndex = UserDataStore.GetAgeRangeIndex(0);

        if (nameInputField != null)
            nameInputField.text = savedName;

        if (ageRangeDropdown != null)
        {
            ageRangeDropdown.value = savedAgeIndex;
            ageRangeDropdown.RefreshShownValue();
        }

        Debug.Log("AccountManager: Profile loaded — Name: " + savedName + " | Age range: " + AgeRangeLabels[savedAgeIndex]);
    }

    void UpdatePointsDisplay()
    {
        if (pointsText != null)
        {
            int highScore = UserDataStore.GetHighScore();
            pointsText.text = "Best Score: " + highScore.ToString();
        }
    }

    public void SaveProfile()
    {
        string playerName = "Player";
        if (nameInputField != null && !string.IsNullOrWhiteSpace(nameInputField.text))
            playerName = nameInputField.text.Trim();

        int ageRangeIndex = 0;
        if (ageRangeDropdown != null)
            ageRangeIndex = ageRangeDropdown.value;

        UserDataStore.SetPlayerName(playerName);
        UserDataStore.SetAgeRangeIndex(ageRangeIndex);

        Debug.Log("AccountManager: Profile saved — Name: " + playerName + " | Age range: " + AgeRangeLabels[ageRangeIndex]);

        AudioManager.Instance?.PlaySFX(AudioManager.Instance.buttonClickSound);
        ShowFeedback("Saved!");
    }

    public void GoBack()
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.buttonClickSound);
        SceneManager.LoadScene("MainMenu");
    }

    void ShowFeedback(string message)
    {
        if (feedbackText == null) return;

        feedbackText.text = message;
        feedbackText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideFeedback));
        Invoke(nameof(HideFeedback), 2f);
    }

    void HideFeedback()
    {
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }
}
