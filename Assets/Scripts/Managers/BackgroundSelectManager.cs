using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the Background Customization screen.
/// Player browses available backgrounds with left/right arrows and selects one.
/// Selection is saved locally via UserDataStore.
/// </summary>
public class BackgroundSelectManager : MonoBehaviour
{
    [Header("Background Options")]
    [SerializeField] private Sprite[] backgroundSprites;
    [SerializeField] private string[] backgroundNames;

    [Header("UI References")]
    [SerializeField] private Image previewImage;
    [SerializeField] private Text backgroundNameText;
    [SerializeField] private Text pageIndicatorText;
    [SerializeField] private Button leftArrowButton;
    [SerializeField] private Button rightArrowButton;
    [SerializeField] private Text feedbackText;

    private int currentIndex = 0;

    void Start()
    {
        currentIndex = UserDataStore.GetSelectedBackground(0);
        currentIndex = Mathf.Clamp(currentIndex, 0, Mathf.Max(0, GetBackgroundCount() - 1));

        ShowBackground(currentIndex);

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);

        AudioManager.Instance?.PlayMusic(AudioManager.Instance.menuMusic);
    }

    int GetBackgroundCount()
    {
        return backgroundSprites != null ? backgroundSprites.Length : 0;
    }

    public void NavigateLeft()
    {
        if (GetBackgroundCount() == 0) return;

        currentIndex--;
        if (currentIndex < 0)
            currentIndex = GetBackgroundCount() - 1;

        ShowBackground(currentIndex);
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.buttonClickSound);
    }

    public void NavigateRight()
    {
        if (GetBackgroundCount() == 0) return;

        currentIndex++;
        if (currentIndex >= GetBackgroundCount())
            currentIndex = 0;

        ShowBackground(currentIndex);
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.buttonClickSound);
    }

    public void SelectBackground()
    {
        if (GetBackgroundCount() == 0) return;

        UserDataStore.SetSelectedBackground(currentIndex);
        Debug.Log("BackgroundSelectManager: Background selected — index: " + currentIndex);

        AudioManager.Instance?.PlaySFX(AudioManager.Instance.buttonClickSound);
        ShowFeedback("Background selected!");
    }

    public void GoBack()
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.buttonClickSound);
        SceneManager.LoadScene("MainMenu");
    }

    void ShowBackground(int index)
    {
        if (GetBackgroundCount() == 0)
        {
            Debug.LogWarning("BackgroundSelectManager: No background sprites assigned.");
            return;
        }

        if (previewImage != null && backgroundSprites[index] != null)
            previewImage.sprite = backgroundSprites[index];

        if (backgroundNameText != null)
        {
            string name = (backgroundNames != null && index < backgroundNames.Length)
                ? backgroundNames[index]
                : "Background " + (index + 1);
            backgroundNameText.text = name;
        }

        if (pageIndicatorText != null)
            pageIndicatorText.text = (index + 1) + " / " + GetBackgroundCount();

        if (leftArrowButton != null)
            leftArrowButton.interactable = GetBackgroundCount() > 1;

        if (rightArrowButton != null)
            rightArrowButton.interactable = GetBackgroundCount() > 1;

        Debug.Log("BackgroundSelectManager: Showing background " + index);
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
