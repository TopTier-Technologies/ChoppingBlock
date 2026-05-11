using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    [SerializeField] private int score = 0;
    [SerializeField] private int lives = 3;
    [SerializeField] private bool isPlaying = false;

    [Header("UI References")]
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject[] lifeIcons;

    [Header("Spawner")]
    [SerializeField] private FoodSpawner foodSpawner;

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
    }

    void Start()
    {
        UpdateUI();

        Invoke("StartGameWithCountdown", 0.5f);
    }

    public void StartGameWithCountdown()
{
    // This is called by the Start button
    Debug.Log("StartGameWithCountdown called!");
    
    // It triggers the countdown instead of starting immediately
    CountdownManager countdownManager = Object.FindFirstObjectByType<CountdownManager>();
    
    if (countdownManager != null)
    {
        Debug.Log("CountdownManager found, starting countdown!");
        countdownManager.StartCountdown();
    }
    else
    {
        // Fallback if no countdown manager
     Debug.LogError("CountdownManager NOT FOUND! Starting game immediately.");
        StartGame();
    }
}

public void StartGame()
{
    // ountdownManager after countdown finishes
    isPlaying = true;
    score = 0;
    lives = 3;
    UpdateUI();
    
    if (foodSpawner != null)
    {
        foodSpawner.StartSpawning();
    }
    
    // Play gameplay music
    AudioManager.Instance?.PlayMusic(AudioManager.Instance.gameplayMusic);
}

    public void OnFoodTapped(FoodItem food)
    {
         if (!isPlaying) 
    {
        Debug.Log("OnFoodTapped called but game is not playing!");
        return;
    }

    //Add score
    score += food.GetPoints();
    Debug.Log("Score increased to: " + score);
    UpdateUI();
     // Check for unlocks 
    FoodManager.Instance.CheckScoreUnlocks(score);
    }

    public void OnFoodMissed(FoodItem food)
    {
        if (!isPlaying) return;

        //Lose a life
        lives--;
        UpdateUI();

        if (lives <= 0)
        {
            GameOver();
        }
    }

    void UpdateUI()
    {
        //Update score
    if (scoreText != null)
    {
        scoreText.text = score.ToString();
        Debug.Log("Score UI updated to: " + score);
    }
    else
    {
        Debug.LogError("ScoreText is NULL! Not assigned in Inspector.");
    }

    //Update lives
    for (int i = 0; i < lifeIcons.Length; i++)
    {
        if (i < lives)
        {
            lifeIcons[i].SetActive(true);
        }
        else
        {
            lifeIcons[i].SetActive(false);
        }
    }
    }

    void GameOver()
{
    isPlaying = false;

    if (foodSpawner != null)
    {
        foodSpawner.StopSpawning();
    }

    // Play gentle game over sound (kid-friendly, not scary)
    AudioManager.Instance?.PlaySFX(AudioManager.Instance.gameOverSound);

    // Award coins based on score (1 coin per 10 points)
    int coinsEarned = score / 10;
    MainMenuManager.AddCoins(coinsEarned);

    // Save score and high score for GameOver screen
    UserDataStore.SetLastScore(score);
    if (UserDataStore.TrySetHighScore(score))
    {
        Debug.Log("New High Score: " + score);
    }

    Debug.Log("Game Over! Final Score: " + score + " | Coins Earned: " + coinsEarned);

    // Load game over screen
    SceneManager.LoadScene("GameOver");
}
public void OnBadFoodTapped()
{
    if (!isPlaying) return;
    
    Debug.Log("Bad food tapped! Game Over!");
    
    // Immediate game over
    GameOver();
}
    public bool IsPlaying()
    {
        return isPlaying;
    }
}
