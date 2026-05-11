using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FoodSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject foodItemPrefab;
    [SerializeField] private float baseSpawnInterval = 1.5f;
    [SerializeField] private float minSpawnInterval = 0.1f;
    [SerializeField] private float spawnRangeX = 3f;
    [SerializeField] private float spawnY = -6f;
    
    [Header("Difficulty Progression")]
    [SerializeField] private int scorePerDifficultyIncrease = 50;
    [SerializeField] private float intervalDecreaseAmount = 0.05f;
    
    [Header("Bad Food Settings")]
    [SerializeField] private BadFoodData badFoodData;
    
    [Header("Physics Settings")]
    [SerializeField] private float minForce = 8f;
    [SerializeField] private float maxForce = 12f;
    [SerializeField] private float torqueAmount = 5f;

    private bool isSpawning = false;
    private float currentSpawnInterval;
    private int lastDifficultyScore = 0;
    private int currentScore = 0;

    void Start()
    {
        currentSpawnInterval = baseSpawnInterval;
        
        // Load bad food data if not assigned
        if (badFoodData == null)
        {
            badFoodData = Resources.Load<BadFoodData>("BadFoodData/BadFood");
        }
    }

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            currentSpawnInterval = baseSpawnInterval;
            lastDifficultyScore = 0;
            currentScore = 0;
            StartCoroutine(SpawnRoutine());
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    public void UpdateDifficulty(int score)
    {
        currentScore = score;
        
        // Check if we've crossed a difficulty threshold
        int difficultyLevel = score / scorePerDifficultyIncrease;
        int lastDifficultyLevel = lastDifficultyScore / scorePerDifficultyIncrease;
        
        if (difficultyLevel > lastDifficultyLevel)
        {
            // Increase difficulty
            currentSpawnInterval -= intervalDecreaseAmount;
            currentSpawnInterval = Mathf.Max(currentSpawnInterval, minSpawnInterval);
            
            lastDifficultyScore = score;
            
            Debug.Log($"Difficulty increased! New spawn interval: {currentSpawnInterval:F2}s");
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            SpawnFood();
            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }

   void SpawnFood()
{
    // Determine if we should spawn bad food
    bool shouldSpawnBadFood = ShouldSpawnBadFood();
    
    Debug.Log($"Should spawn bad food: {shouldSpawnBadFood} (Score: {currentScore})");
    
    if (shouldSpawnBadFood)
    {
        SpawnBadFood();
    }
    else
    {
        SpawnGoodFood();
    }
}
    
    bool ShouldSpawnBadFood()
{
    if (badFoodData == null)
    {
        Debug.LogWarning("badFoodData is NULL!");
        return false;
    }
    
    // Calculate current bad food spawn chance based on score
    float progress = Mathf.Clamp01((float)currentScore / badFoodData.scoreToMaxSpawn);
    float currentBadFoodChance = Mathf.Lerp(badFoodData.baseSpawnChance, badFoodData.maxSpawnChance, progress);
    
    float randomValue = Random.value;
    bool shouldSpawn = randomValue < currentBadFoodChance;
    
    Debug.Log($"Bad food chance: {currentBadFoodChance:F3} | Random: {randomValue:F3} | Spawn: {shouldSpawn}");
    
    // Random chance
    return shouldSpawn;
}
    
    void SpawnBadFood()
    {
        if (badFoodData == null || badFoodData.foodSprites.Length == 0)
        {
            Debug.LogWarning("Bad food data not set up properly!");
            SpawnGoodFood();
            return;
        }
        
        // Random spawn position
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);
        
        // Instantiate bad food
        GameObject food = Instantiate(foodItemPrefab, spawnPosition, Quaternion.identity);
        
        // Select random bad food sprite
        Sprite randomBadSprite = badFoodData.foodSprites[Random.Range(0, badFoodData.foodSprites.Length)];
        
        // Initialize as bad food
        FoodItem foodItem = food.GetComponent<FoodItem>();
        if (foodItem != null)
        {
            foodItem.InitializeAsBadFood(randomBadSprite);
        }
        
        // Add upward force
        Rigidbody2D rb = food.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float randomForce = Random.Range(minForce, maxForce);
            rb.AddForce(Vector2.up * randomForce, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-torqueAmount, torqueAmount));
        }
        
        Debug.Log("Spawned bad food!");
    }
    
    void SpawnGoodFood()
    {
        // Get unlocked foods
        List<FoodData> unlockedFoods = FoodManager.Instance.GetUnlockedFoods();
        
        if (unlockedFoods.Count == 0)
        {
            Debug.LogWarning("No unlocked foods available!");
            return;
        }

        // Random spawn position
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);

        // Random food from unlocked foods
        FoodData randomFood = unlockedFoods[Random.Range(0, unlockedFoods.Count)];
        
        // Instantiate food
        GameObject food = Instantiate(foodItemPrefab, spawnPosition, Quaternion.identity);
        
        // Initialize with food data
        FoodItem foodItem = food.GetComponent<FoodItem>();
        if (foodItem != null)
        {
            foodItem.Initialize(randomFood);
        }

        // Add upward force
        Rigidbody2D rb = food.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float randomForce = Random.Range(minForce, maxForce);
            rb.AddForce(Vector2.up * randomForce, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-torqueAmount, torqueAmount));
        }
    }
    
    public float GetCurrentSpawnInterval()
    {
        return currentSpawnInterval;
    }
}