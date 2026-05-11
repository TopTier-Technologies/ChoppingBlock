using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class TrophyCaseManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text coinsText;
    [SerializeField] private Transform foodGridPanel;
    [SerializeField] private GameObject foodItemCardPrefab;
    [SerializeField] private GameObject foodDetailsPopup;
    [SerializeField] private Text notEnoughCoinsText;
    
    [Header("Details Popup")]
    [SerializeField] private Image detailsFoodIcon;
    [SerializeField] private Text detailsFoodName;
    [SerializeField] private Text funFactText;
    [SerializeField] private Text foodGroupText;
    [SerializeField] private Text caloriesText;
    [SerializeField] private Text seasonText;
    [SerializeField] private Text originText;
    [SerializeField] private Text nutrientsText;
    
    private FoodData currentSelectedFood;
    
    void Start()
    {
        UpdateCoinsDisplay();
        ShowAllFoods();
        AudioManager.Instance?.PlayMusic(AudioManager.Instance.menuMusic);
    }
    
    public void ShowAllFoods()
    {
        Debug.Log("ShowAllFoods called");
        
        // Clear existing food cards
        foreach (Transform child in foodGridPanel)
        {
            Destroy(child.gameObject);
        }
        
        if (FoodManager.Instance == null)
        {
            Debug.LogError("FoodManager.Instance is NULL!");
            return;
        }
        
        // Get ALL foods
        List<FoodData> allFoods = FoodManager.Instance.allFoods;
        
        Debug.Log($"Showing {allFoods.Count} foods in trophy case");
        
        if (foodItemCardPrefab == null)
        {
            Debug.LogError("foodItemCardPrefab is NULL!");
            return;
        }
        
        if (foodGridPanel == null)
        {
            Debug.LogError("foodGridPanel is NULL!");
            return;
        }
        
        // Create food cards
        foreach (FoodData food in allFoods)
        {
            Debug.Log($"Creating card for: {food.foodName}");
            CreateFoodCard(food);
        }
    }
    
    void CreateFoodCard(FoodData food)
{
    GameObject card = Instantiate(foodItemCardPrefab, foodGridPanel);
    
    // Get UI components with null checks
    Transform foodIconTransform = card.transform.Find("FoodIcon");
    Transform foodNameTransform = card.transform.Find("FoodNameText");
    Transform lockIconTransform = card.transform.Find("LockIcon");
    Transform unlockButtonTransform = card.transform.Find("UnlockButton");
    Transform viewDetailsButtonTransform = card.transform.Find("ViewDetailsButton");
    
    if (foodIconTransform == null)
    {
        Debug.LogError("FoodIcon not found in prefab!");
        return;
    }
    if (foodNameTransform == null)
    {
        Debug.LogError("FoodNameText not found in prefab!");
        return;
    }
    if (lockIconTransform == null)
    {
        Debug.LogError("LockIcon not found in prefab!");
        return;
    }
    if (unlockButtonTransform == null)
    {
        Debug.LogError("UnlockButton not found in prefab!");
        return;
    }
    if (viewDetailsButtonTransform == null)
    {
        Debug.LogError("ViewDetailsButton not found in prefab!");
        return;
    }
    
    Image foodIcon = foodIconTransform.GetComponent<Image>();
    Text foodNameText = foodNameTransform.GetComponent<Text>();
    GameObject lockIcon = lockIconTransform.gameObject;
    Button unlockButton = unlockButtonTransform.GetComponent<Button>();
    Button viewDetailsButton = viewDetailsButtonTransform.GetComponent<Button>();
    
    // Set food icon
    if (foodIcon != null && food.foodSprite != null)
    {
        foodIcon.sprite = food.foodSprite;
    }
    
    // Set food name
    if (foodNameText != null)
    {
        foodNameText.text = food.foodName;
    }
    
    // Configure based on unlock status
    if (food.isUnlocked)
    {
        // Unlocked - show normally
        lockIcon.SetActive(false);
        unlockButton.gameObject.SetActive(false);
        foodIcon.color = Color.white;
        
        // View details button
        viewDetailsButton.onClick.AddListener(() => ShowFoodDetails(food));
    }
    else
    {
        // Locked - show lock and unlock button
        lockIcon.SetActive(true);
        foodIcon.color = new Color(0.3f, 0.3f, 0.3f, 1f); // Darken
        
        if (food.coinCost > 0)
        {
            unlockButton.gameObject.SetActive(true);
            Text unlockButtonText = unlockButton.GetComponentInChildren<Text>();
            if (unlockButtonText != null)
            {
                unlockButtonText.text = $"Unlock: {food.coinCost} coins";
            }
            
            unlockButton.onClick.AddListener(() => TryUnlockFood(food));
        }
        else
        {
            unlockButton.gameObject.SetActive(false);
        }
        
        // Disable view details for locked foods
        viewDetailsButton.interactable = false;
    }
}
    
    void TryUnlockFood(FoodData food)
{
    Debug.Log($"Trying to unlock {food.foodName} for {food.coinCost} coins");
    
    bool success = FoodManager.Instance.TryUnlockWithCoins(food);
    
    if (success)
    {
        Debug.Log($"Successfully unlocked {food.foodName}!");
        UpdateCoinsDisplay();
        // Refresh the display
        ShowAllFoods();
    }
    else
    {
        Debug.Log($"Failed to unlock {food.foodName} - not enough coins!");
        ShowNotEnoughCoins();
    }
}
    
    void ShowFoodDetails(FoodData food)
    {
        currentSelectedFood = food;
        
        // Set details
        if (detailsFoodIcon != null && food.foodSprite != null)
        {
            detailsFoodIcon.sprite = food.foodSprite;
        }
        
        if (detailsFoodName != null)
        {
            detailsFoodName.text = food.foodName.ToUpper();
        }
        
        if (funFactText != null)
        {
            funFactText.text = $"{food.funFact}";
        }
        
        if (foodGroupText != null)
        {
            foodGroupText.text = $"{food.foodGroup}";
        }
        
        if (caloriesText != null)
        {
            caloriesText.text = $"{food.calories} (medium)";
        }
        
        if (seasonText != null)
        {
            seasonText.text = $"{food.season}";
        }
        
        if (originText != null)
        {
            originText.text = $"{food.origin}";
        }
        
        if (nutrientsText != null)
        {
            nutrientsText.text = $"{food.nutrients}";
        }
        
        // Show popup
        if (foodDetailsPopup != null)
        {
            foodDetailsPopup.SetActive(true);
        }
    }
    
    public void CloseFoodDetails()
    {
        if (foodDetailsPopup != null)
        {
            foodDetailsPopup.SetActive(false);
        }
    }
    
    void UpdateCoinsDisplay()
    {
        if (coinsText != null)
        {
            int coins = MainMenuManager.GetCoins();
            coinsText.text = coins.ToString();
        }
    }
    
    void ShowNotEnoughCoins()
    {
        if (notEnoughCoinsText != null)
        {
            notEnoughCoinsText.text = "Not enough coins!";
            notEnoughCoinsText.gameObject.SetActive(true);
            StartCoroutine(HideNotEnoughCoins());
        }
    }

    System.Collections.IEnumerator HideNotEnoughCoins()
    {
        yield return new WaitForSeconds(2f);
        if (notEnoughCoinsText != null)
        {
            notEnoughCoinsText.gameObject.SetActive(false);
        }
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
