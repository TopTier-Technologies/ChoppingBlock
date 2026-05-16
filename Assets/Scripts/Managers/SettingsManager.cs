using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio Controls")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Text muteButtonText;

    [Header("Graphics Controls")]
    [SerializeField] private Dropdown graphicsDropdown;
    [SerializeField] private Button graphicsButton;
    [SerializeField] private Text graphicsButtonText;

    [Header("Optional Controls")]
    [SerializeField] private GameObject languageDropdownObject;
    [SerializeField] private Text languageLabelText;

    [Header("Attribution Text")]
    [SerializeField] private Text attributionText;

    private const string ATTRIBUTION = "Music by Kevin MacLeod (incompetech.com)\n" +
                                       "Licensed under Creative Commons: By Attribution 4.0\n" +
                                       "Sound effects from Kenney.nl (CC0)";

    void Start()
    {
        ResolveSceneReferences();
        LoadCurrentSettings();
        HideUnsupportedLanguageControl();

        if (attributionText != null)
        {
            attributionText.text = ATTRIBUTION;
        }

        Debug.Log("SettingsManager initialized");
    }

    void LoadCurrentSettings()
    {
        ScreenBrightnessOverlay.EnsureExists();

        if (AudioManager.Instance != null)
        {
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            }

            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = AudioManager.Instance.GetSFXVolume();
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            }

            UpdateMuteButtonText();
        }

        if (graphicsDropdown != null)
        {
            int savedGraphicsIndex = UserDataStore.GetGraphicsQualityIndex(0);
            graphicsDropdown.value = savedGraphicsIndex;
            graphicsDropdown.onValueChanged.AddListener(OnGraphicsChanged);
            ApplyGraphicsIndex(savedGraphicsIndex);
        }
        else
        {
            int savedGraphicsIndex = UserDataStore.GetGraphicsQualityIndex(0);
            ApplyGraphicsIndex(savedGraphicsIndex);
            UpdateGraphicsButtonText(savedGraphicsIndex);

            if (graphicsButton != null)
            {
                graphicsButton.onClick.RemoveListener(CycleGraphicsQuality);
                graphicsButton.onClick.AddListener(CycleGraphicsQuality);
            }
        }
    }

    void ResolveSceneReferences()
    {
        if (graphicsButton == null)
        {
            var graphicsObject = GameObject.Find("GraphicsDropdown");
            if (graphicsObject != null)
            {
                graphicsButton = graphicsObject.GetComponent<Button>();
                if (graphicsButton == null)
                {
                    graphicsButton = graphicsObject.AddComponent<Button>();
                    var image = graphicsObject.GetComponent<Image>();
                    if (image != null)
                    {
                        graphicsButton.targetGraphic = image;
                    }
                }

                if (graphicsButtonText == null)
                {
                    graphicsButtonText = graphicsObject.GetComponentInChildren<Text>(true);
                    if (graphicsButtonText == null)
                    {
                        graphicsButtonText = CreateButtonValueText(graphicsObject.transform, "GraphicsValueText");
                    }
                }
            }
        }

        if (languageDropdownObject == null)
        {
            languageDropdownObject = GameObject.Find("LanguageDropdown");
        }

        if (languageLabelText == null)
        {
            var languageLabelObject = GameObject.Find("LanguageText");
            if (languageLabelObject != null)
            {
                languageLabelText = languageLabelObject.GetComponent<Text>();
            }
        }

        if (attributionText == null)
        {
            var attributionObject = GameObject.Find("AttributionText");
            if (attributionObject != null)
            {
                attributionText = attributionObject.GetComponent<Text>();
            }
        }
    }

    Text CreateButtonValueText(Transform parent, string objectName)
    {
        var textObject = new GameObject(objectName);
        textObject.transform.SetParent(parent, false);

        var rect = textObject.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var text = textObject.AddComponent<Text>();
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(0.298f, 0.145f, 0.0078f, 1f);
        text.fontSize = 20;
        text.fontStyle = FontStyle.Bold;
        text.raycastTarget = false;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        return text;
    }

    void HideUnsupportedLanguageControl()
    {
        if (languageDropdownObject != null)
        {
            languageDropdownObject.SetActive(false);
        }

        if (languageLabelText != null)
        {
            languageLabelText.gameObject.SetActive(false);
        }
    }

    public void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance?.SetMusicVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
    }

    public void ToggleMute()
    {
        AudioManager.Instance?.ToggleMute();
        UpdateMuteButtonText();
    }

    public void OnGraphicsChanged(int index)
    {
        // 0 = Standard, 1 = Low Performance
        UserDataStore.SetGraphicsQualityIndex(index);
        ApplyGraphicsIndex(index);
        Debug.Log("Graphics quality set to: " + (index == 0 ? "Standard" : "Low Performance"));
    }

    void ApplyGraphicsIndex(int index)
    {
        QualitySettings.SetQualityLevel(index == 0 ? QualitySettings.names.Length - 1 : 0);
    }

    public void CycleGraphicsQuality()
    {
        int currentIndex = UserDataStore.GetGraphicsQualityIndex(0);
        int nextIndex = currentIndex == 0 ? 1 : 0;
        UserDataStore.SetGraphicsQualityIndex(nextIndex);
        ApplyGraphicsIndex(nextIndex);
        UpdateGraphicsButtonText(nextIndex);
    }

    void UpdateGraphicsButtonText(int index)
    {
        if (graphicsButtonText != null)
        {
            graphicsButtonText.text = index == 0 ? "Standard" : "Low";
        }
    }

    void UpdateMuteButtonText()
    {
        if (muteButtonText != null && AudioManager.Instance != null)
        {
            muteButtonText.text = AudioManager.Instance.IsMuted() ? "Unmute Sound" : "Mute Sound";
        }
    }

    public void GoHome()
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.buttonClickSound);
        SceneManager.LoadScene("MainMenu");
    }
}
