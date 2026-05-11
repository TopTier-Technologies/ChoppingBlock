using UnityEngine;

public class FoodItem : MonoBehaviour
{
    [Header("Food Properties")]
    [SerializeField] private FoodData foodData;
    [SerializeField] private bool isBadFood = false;
    
    [Header("Effects")]
    [SerializeField] private GameObject tapEffectPrefab;
    [SerializeField] private AudioClip tapSound;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool hasBeenTapped = false;

    void Awake()
    {
        // Get components in Awake instead of Start so they're ready earlier
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (foodData != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = foodData.foodSprite;
        }
    }

    public void Initialize(FoodData data)
    {
        // Ensure spriteRenderer is available
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        foodData = data;
        isBadFood = false;
        
        if (spriteRenderer != null && foodData != null)
        {
            spriteRenderer.sprite = foodData.foodSprite;
        }
    }
    
    public void InitializeAsBadFood(Sprite badSprite)
    {
        // Ensure spriteRenderer is available
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        isBadFood = true;
        foodData = null;
        
        if (spriteRenderer != null && badSprite != null)
        {
            spriteRenderer.sprite = badSprite;
            Debug.Log($"Bad food sprite set: {badSprite.name}");
        }
        else
        {
            if (spriteRenderer == null) Debug.LogError("SpriteRenderer is NULL!");
            if (badSprite == null) Debug.LogError("Bad sprite is NULL!");
        }
    }

    public void OnTapped()
    {
        if (hasBeenTapped) return;
        
        hasBeenTapped = true;

        // Play tap effect
        if (tapEffectPrefab != null)
        {
            Instantiate(tapEffectPrefab, transform.position, Quaternion.identity);
        }

        // Play sound
        if (tapSound != null)
        {
            AudioSource.PlayClipAtPoint(tapSound, transform.position);
        }

        // Notify game manager
        if (GameManager.Instance != null)
        {
            if (isBadFood)
            {
                GameManager.Instance.OnBadFoodTapped();
            }
            else
            {
                GameManager.Instance.OnFoodTapped(this);
            }
        }

        // Destroy the food item
        Destroy(gameObject);
    }

    public string GetFoodName()
    {
        return foodData != null ? foodData.foodName : "Bad Food";
    }

    public int GetPoints()
    {
        return foodData != null ? foodData.pointValue : 0;
    }

    public FoodData GetFoodData()
    {
        return foodData;
    }
    
    public bool IsBadFood()
    {
        return isBadFood;
    }

    void OnBecameInvisible()
    {
        // If food falls off screen without being tapped
        if (!hasBeenTapped)
        {
            if (GameManager.Instance != null)
            {
                // Don't lose lives for missing bad food - that's good!
                if (!isBadFood)
                {
                    GameManager.Instance.OnFoodMissed(this);
                }
            }
            Destroy(gameObject);
        }
    }
}