using UnityEngine;

[CreateAssetMenu(fileName = "NewFood", menuName = "Chopping Block/Food Data")]
public class FoodData : ScriptableObject
{
    [Header("Basic Info")]
    public string foodName;
    public Sprite foodSprite;
    public int pointValue = 10;
    [Header("Unlock Requirements")]
    public bool isUnlocked = false;
    public int requiredScore = 0;
    public int coinCost = 0;
    
    [Header("Trophy Case Info")]
    public string season; 
    public string foodGroup; 
    public string funFact;
    public int calories;
    public string origin;
    public string nutrients;
    
    [Header("Prefab")]
    public GameObject foodPrefab;
}