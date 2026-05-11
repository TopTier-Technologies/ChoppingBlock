using UnityEngine;

public class SettingsToggleVisual : MonoBehaviour
{
    [SerializeField] private RectTransform toggleRect;
    [SerializeField] private RectTransform knobRect;
    [SerializeField] private bool mutedMovesRight = true;
    [SerializeField] private bool computeRightFromGeometry = true;
    [SerializeField] private Vector2 leftPosition;
    [SerializeField] private Vector2 rightPosition;

    void Awake()
    {
        if (toggleRect == null)
        {
            toggleRect = GetComponent<RectTransform>();
        }
    }

    void Start()
    {
        CachePositions();
        UpdateFromAudio();
    }

    void CachePositions()
    {
        if (toggleRect == null || knobRect == null)
        {
            return;
        }

        leftPosition = knobRect.anchoredPosition;

        if (computeRightFromGeometry)
        {
            float leftInset = knobRect.anchoredPosition.x - toggleRect.anchoredPosition.x;
            float rightX = toggleRect.anchoredPosition.x
                + (toggleRect.sizeDelta.x - knobRect.sizeDelta.x - leftInset);
            rightPosition = new Vector2(rightX, knobRect.anchoredPosition.y);
        }
    }

    public void UpdateFromAudio()
    {
        if (knobRect == null)
        {
            return;
        }

        bool muted = AudioManager.Instance != null && AudioManager.Instance.IsMuted();
        bool moveRight = mutedMovesRight ? muted : !muted;
        knobRect.anchoredPosition = moveRight ? rightPosition : leftPosition;
    }
}
