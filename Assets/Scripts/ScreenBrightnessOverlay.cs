using UnityEngine;
using UnityEngine.UI;

public class ScreenBrightnessOverlay : MonoBehaviour
{
    public static ScreenBrightnessOverlay Instance;

    [SerializeField] private float brightness = 1f;

    private Image overlayImage;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        CreateOverlay();
        LoadBrightness();
        ApplyBrightness();
    }

    void CreateOverlay()
    {
        var canvasGO = new GameObject("BrightnessOverlayCanvas");
        canvasGO.transform.SetParent(transform, false);

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        var imageGO = new GameObject("BrightnessOverlay");
        imageGO.transform.SetParent(canvasGO.transform, false);
        overlayImage = imageGO.AddComponent<Image>();
        overlayImage.color = new Color(0f, 0f, 0f, 0f);
        overlayImage.raycastTarget = false;

        var rect = overlayImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    void LoadBrightness()
    {
        brightness = UserDataStore.GetBrightness(1f);
        brightness = Mathf.Clamp01(brightness);
    }

    void SaveBrightness()
    {
        UserDataStore.SetBrightness(brightness);
    }

    void ApplyBrightness()
    {
        if (overlayImage == null)
        {
            return;
        }

        overlayImage.color = new Color(0f, 0f, 0f, 1f - brightness);
    }

    public void SetBrightness(float value)
    {
        brightness = Mathf.Clamp01(value);
        ApplyBrightness();
        SaveBrightness();
    }

    public float GetBrightness()
    {
        return brightness;
    }
}
