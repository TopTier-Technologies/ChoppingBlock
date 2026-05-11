using UnityEngine;

[CreateAssetMenu(fileName = "NewBadFood", menuName = "Chopping Block/Bad Food Data")]
public class BadFoodData : ScriptableObject
{
    [Header("Basic Info")]
    public string foodName;
    public Sprite[] foodSprites; // Multiple sprites to choose from randomly
    
    [Header("Spawn Settings")]
    public float baseSpawnChance = 0.05f; // 5% chance at start
    public float maxSpawnChance = 0.25f; // Max 25% chance
    public int scoreToMaxSpawn = 500; // Reaches max spawn chance at this score
}