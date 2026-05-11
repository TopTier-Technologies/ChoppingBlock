using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    private AudioSource musicSource;
    private AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;

    [Header("SFX Clips")]
    public AudioClip tapSound;
    public AudioClip unlockSound;
    public AudioClip gameOverSound;
    public AudioClip buttonClickSound;

    [Header("Volume Settings")]
    [SerializeField] private float musicVolume = 0.4f;
    [SerializeField] private float sfxVolume = 0.6f;
    [SerializeField] private bool isMuted = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Create AudioSource components
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        // Configure sources for kid-friendly audio
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        sfxSource.playOnAwake = false;

        LoadVolumeSettings();
        Debug.Log("AudioManager initialized - Kid-friendly audio system ready");
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
            Debug.Log($"Playing SFX: {clip.name}");
        }
        else if (clip == null)
        {
            Debug.LogWarning("AudioManager: Attempted to play null SFX clip");
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip != null && musicSource != null)
        {
            // Don't restart if same music is already playing
            if (musicSource.clip == clip && musicSource.isPlaying)
            {
                return;
            }

            musicSource.clip = clip;
            musicSource.volume = musicVolume;
            musicSource.Play();
            Debug.Log($"Playing Music: {clip.name}");
        }
        else if (clip == null)
        {
            Debug.LogWarning("AudioManager: Attempted to play null music clip");
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
        SaveVolumeSettings();
        Debug.Log($"Music volume set to: {musicVolume}");
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        SaveVolumeSettings();
        Debug.Log($"SFX volume set to: {sfxVolume}");
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        AudioListener.volume = isMuted ? 0f : 1f;
        UserDataStore.SetMuted(isMuted);
        Debug.Log($"Audio muted: {isMuted}");
    }

    void LoadVolumeSettings()
    {
        // Load volume settings from PlayerPrefs with defaults
        musicVolume = UserDataStore.GetMusicVolume(0.4f);
        sfxVolume = UserDataStore.GetSfxVolume(0.6f);
        isMuted = UserDataStore.GetMuted();

        // Apply settings
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
        AudioListener.volume = isMuted ? 0f : 1f;

        Debug.Log($"Loaded audio settings - Music: {musicVolume}, SFX: {sfxVolume}, Muted: {isMuted}");
    }

    void SaveVolumeSettings()
    {
        UserDataStore.SetMusicVolume(musicVolume);
        UserDataStore.SetSfxVolume(sfxVolume);
        Debug.Log("Audio settings saved");
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    public bool IsMuted()
    {
        return isMuted;
    }
}
