using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ModeSelectManager : MonoBehaviour
{
    [Header("Coming Soon Popup")]
    [SerializeField] private GameObject comingSoonPopup;
    [SerializeField] private Text comingSoonText;

    void Start()
    {
        AudioManager.Instance?.PlayMusic(AudioManager.Instance.menuMusic);

        if (comingSoonPopup != null)
        {
            comingSoonPopup.SetActive(false);
        }
    }

    public void PlayClassic()
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.buttonClickSound);
        SceneManager.LoadScene("Gameplay");
    }

    public void PlayCountdown()
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.buttonClickSound);
        ShowComingSoon("Countdown Mode is coming soon!");
    }

    public void PlayQuiz()
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.buttonClickSound);
        ShowComingSoon("Quiz Mode is coming soon!");
    }

    public void GoBack()
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.buttonClickSound);
        SceneManager.LoadScene("MainMenu");
    }

    public void CloseComingSoon()
    {
        if (comingSoonPopup != null)
        {
            comingSoonPopup.SetActive(false);
        }
    }

    void ShowComingSoon(string message)
    {
        if (comingSoonPopup != null)
        {
            if (comingSoonText != null)
            {
                comingSoonText.text = message;
            }
            comingSoonPopup.SetActive(true);
        }
        else
        {
            Debug.Log(message);
        }
    }
}
