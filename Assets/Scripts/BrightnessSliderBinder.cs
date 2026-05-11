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

        slider.onValueChanged.AddListener(OnValueChanged);
        if (ScreenBrightnessOverlay.Instance != null)
        {
            slider.value = ScreenBrightnessOverlay.Instance.GetBrightness();
        }
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
