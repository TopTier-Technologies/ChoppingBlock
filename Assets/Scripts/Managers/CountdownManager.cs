using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountdownManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text countdownText;
    [SerializeField] private GameObject countdownPanel; // Optional background panel
    
    [Header("Countdown Settings")]
    [SerializeField] private int countdownSeconds = 3;
    [SerializeField] private string goText = "GO!";
    [SerializeField] private float goTextDuration = 0.5f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip countdownBeep;
    [SerializeField] private AudioClip goSound;
    
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = GameManager.Instance;
    }
    
    public void StartCountdown()
    {
        StartCoroutine(CountdownRoutine());
    }
    
    IEnumerator CountdownRoutine()
    {
        // Show countdown UI
        if (countdownText != null)
        {
            Debug.Log("Activating countdown text");
            countdownText.gameObject.SetActive(true);
        }
        else
    {
        Debug.LogError("CountdownText is NULL!");
    }
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(true);
        }
        
        // Countdown from 3, 2, 1...
        for (int i = countdownSeconds; i > 0; i--)
{
    if (countdownText != null)
    {
        countdownText.text = i.ToString();
        
        // Animate scale
        StartCoroutine(ScaleText(countdownText.transform));
    }
    
    // Play beep sound
    if (countdownBeep != null)
    {
        AudioSource.PlayClipAtPoint(countdownBeep, Camera.main.transform.position);
    }
    else
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.buttonClickSound);
    }
    
    yield return new WaitForSeconds(1f);
}
        
        // Show "GO!"
        if (countdownText != null)
        {
            countdownText.text = goText;
        }
        
        // Play go sound
        if (goSound != null)
        {
            AudioSource.PlayClipAtPoint(goSound, Camera.main.transform.position);
        }
        else
        {
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.buttonClickSound);
        }
        
        yield return new WaitForSeconds(goTextDuration);
        
        // Hide countdown UI
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
        
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(false);
        }
        
        // Start the actual game
        if (gameManager != null)
        {
            gameManager.StartGame();
        }
    }
    IEnumerator ScaleText(Transform textTransform)
{
    textTransform.localScale = Vector3.one * 2f;
    
    // Shrink to normal over 0.3 seconds
    float elapsed = 0f;
    float duration = 0.3f;
    
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float scale = Mathf.Lerp(2f, 1f, elapsed / duration);
        textTransform.localScale = Vector3.one * scale;
        yield return null;
    }
    
    textTransform.localScale = Vector3.one;
}
}