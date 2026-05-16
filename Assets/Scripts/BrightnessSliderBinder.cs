using UnityEngine;
using UnityEngine.UI;

public class BrightnessSliderBinder : MonoBehaviour
{
    [SerializeField] private Slider slider;

    void Awake()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
    }

    void OnEnable()
    {
        if (slider == null)
        {
            return;
        }

        ScreenBrightnessOverlay.EnsureExists();
        slider.onValueChanged.AddListener(OnValueChanged);
        slider.SetValueWithoutNotify(ScreenBrightnessOverlay.Instance != null
            ? ScreenBrightnessOverlay.Instance.GetBrightness()
            : UserDataStore.GetBrightness(1f));
    }

    void OnDisable()
    {
        if (slider == null)
        {
            return;
        }

        slider.onValueChanged.RemoveListener(OnValueChanged);
    }

    void OnValueChanged(float value)
    {
        if (ScreenBrightnessOverlay.Instance != null)
        {
            ScreenBrightnessOverlay.Instance.SetBrightness(value);
        }
    }
}
