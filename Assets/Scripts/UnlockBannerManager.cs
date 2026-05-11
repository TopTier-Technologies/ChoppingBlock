using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UnlockBannerManager : MonoBehaviour
{
    public static UnlockBannerManager Instance;
    
    [Header("UI References")]
    [SerializeField] private GameObject bannerPanel;
    [SerializeField] private Text bannerText;
    [SerializeField] private Image foodIcon;
    [SerializeField] private float displayDuration = 3f;
    
    private Queue<FoodData> unlockQueue = new Queue<FoodData>();
    private bool isShowingBanner = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        if (bannerPanel != null)
        {
            bannerPanel.SetActive(false);
        }
    }
    
    public void ShowUnlockBanner(FoodData food)
    {
        unlockQueue.Enqueue(food);
        
        if (!isShowingBanner)
        {
            StartCoroutine(ShowBannerRoutine());
        }
    }
    
    IEnumerator ShowBannerRoutine()
    {
        isShowingBanner = true;
        
        while (unlockQueue.Count > 0)
        {
            FoodData food = unlockQueue.Dequeue();
            
            // Update banner UI
            if (bannerText != null)
            {
                bannerText.text = $"New Food Unlocked!\n{food.foodName}\nCheck it out in the Trophy Case!";
            }
            
            if (foodIcon != null && food.foodSprite != null)
            {
                foodIcon.sprite = food.foodSprite;
            }
            
            // Play celebratory unlock sound
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.unlockSound);

            // Show banner
            if (bannerPanel != null)
            {
                bannerPanel.SetActive(true);
            }
            
            // Wait
            yield return new WaitForSeconds(displayDuration);
            
            // Hide banner
            if (bannerPanel != null)
            {
                bannerPanel.SetActive(false);
            }
            
            yield return new WaitForSeconds(0.5f);
        }
        
        isShowingBanner = false;
    }
}