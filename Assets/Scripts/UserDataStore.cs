using UnityEngine;

public static class UserDataStore
{
    // Keys for PlayerPrefs
    private const string LAST_SCORE_KEY = "LastScore";
    private const string HIGH_SCORE_KEY = "HighScore";
    private const string USER_COINS_KEY = "UserCoins";
    private const string PLAYER_NAME_KEY = "PlayerName";
    private const string AGE_RANGE_KEY = "AgeRange";
    private const string GRAPHICS_QUALITY_KEY = "GraphicsQuality";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SfxVolume";
    private const string BRIGHTNESS_KEY = "Brightness";
    private const string IS_MUTED_KEY = "IsMuted";
    private const string SELECTED_BACKGROUND_KEY = "SelectedBackground";
    private const string FOOD_UNLOCKED_PREFIX = "Food_Unlocked_";

    // Score Management
    public static void SetLastScore(int score)
    {
        PlayerPrefs.SetInt(LAST_SCORE_KEY, score);
        PlayerPrefs.Save();
    }

    public static int GetLastScore()
    {
        return PlayerPrefs.GetInt(LAST_SCORE_KEY, 0);
    }

    public static bool TrySetHighScore(int score)
    {
        int currentHighScore = GetHighScore();
        
        if (score > currentHighScore)
        {
            SetHighScore(score);
            return true;
        }
        
        return false;
    }

    public static void SetHighScore(int score)
    {
        PlayerPrefs.SetInt(HIGH_SCORE_KEY, score);
        PlayerPrefs.Save();
    }

    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
    }

    // Coins Management
    public static int GetCoins(int defaultValue = 25)
    {
        return PlayerPrefs.GetInt(USER_COINS_KEY, defaultValue);
    }

    public static void SetCoins(int amount)
    {
        PlayerPrefs.SetInt(USER_COINS_KEY, amount);
        PlayerPrefs.Save();
    }

    public static void AddCoins(int amount)
    {
        int currentCoins = GetCoins();
        PlayerPrefs.SetInt(USER_COINS_KEY, currentCoins + amount);
        PlayerPrefs.Save();
    }

    public static bool TrySpendCoins(int amount)
    {
        int currentCoins = GetCoins();
        
        if (currentCoins >= amount)
        {
            SetCoins(currentCoins - amount);
            return true;
        }
        
        return false;
    }

    // Player Profile
    public static void SetPlayerName(string name)
    {
        PlayerPrefs.SetString(PLAYER_NAME_KEY, name);
        PlayerPrefs.Save();
    }

    public static string GetPlayerName(string defaultValue = "Player")
    {
        return PlayerPrefs.GetString(PLAYER_NAME_KEY, defaultValue);
    }

    public static void SetAgeRangeIndex(int index)
    {
        PlayerPrefs.SetInt(AGE_RANGE_KEY, index);
        PlayerPrefs.Save();
    }

    public static int GetAgeRangeIndex(int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(AGE_RANGE_KEY, defaultValue);
    }

    // Graphics Settings
    public static void SetGraphicsQualityIndex(int index)
    {
        PlayerPrefs.SetInt(GRAPHICS_QUALITY_KEY, index);
        PlayerPrefs.Save();
        QualitySettings.SetQualityLevel(index);
    }

    public static int GetGraphicsQualityIndex(int defaultValue = 2)
    {
        return PlayerPrefs.GetInt(GRAPHICS_QUALITY_KEY, defaultValue);
    }

    // Audio Settings
    public static void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    public static float GetMusicVolume(float defaultValue = 0.7f)
    {
        return PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, defaultValue);
    }

    public static void SetSfxVolume(float volume)
    {
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    public static float GetSfxVolume(float defaultValue = 0.7f)
    {
        return PlayerPrefs.GetFloat(SFX_VOLUME_KEY, defaultValue);
    }

    public static void SetMuted(bool muted)
    {
        PlayerPrefs.SetInt(IS_MUTED_KEY, muted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool GetMuted()
    {
        return PlayerPrefs.GetInt(IS_MUTED_KEY, 0) == 1;
    }

    // Brightness
    public static void SetBrightness(float brightness)
    {
        PlayerPrefs.SetFloat(BRIGHTNESS_KEY, brightness);
        PlayerPrefs.Save();
    }

    public static float GetBrightness(float defaultValue = 1.0f)
    {
        return PlayerPrefs.GetFloat(BRIGHTNESS_KEY, defaultValue);
    }

    // Background Selection
    public static void SetSelectedBackground(int index)
    {
        PlayerPrefs.SetInt(SELECTED_BACKGROUND_KEY, index);
        PlayerPrefs.Save();
    }

    public static int GetSelectedBackground(int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(SELECTED_BACKGROUND_KEY, defaultValue);
    }

    // Food Unlock Status
    public static void SetFoodUnlocked(string foodName, bool unlocked)
    {
        PlayerPrefs.SetInt(FOOD_UNLOCKED_PREFIX + foodName, unlocked ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool GetFoodUnlocked(string foodName)
    {
        return PlayerPrefs.GetInt(FOOD_UNLOCKED_PREFIX + foodName, 0) == 1;
    }

    // Overload for FoodManager compatibility
    public static bool GetFoodUnlocked(string foodName, bool defaultValue)
    {
        return PlayerPrefs.GetInt(FOOD_UNLOCKED_PREFIX + foodName, defaultValue ? 1 : 0) == 1;
    }

    // Reset All Data
    public static void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}