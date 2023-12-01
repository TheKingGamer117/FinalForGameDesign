using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManagerMainScene : MonoBehaviour
{
    public static AudioManagerMainScene instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip gameMusicMainSceneOcean;
    public AudioClip gameMusicPlayerHouse;
    public AudioClip gameMusic;
    public AudioClip shootSFX;
    public AudioClip gameMusicMainMenu;

    [Range(0f, 1f)] public float musicVolume = 1f;  // Set default music volume to max
    [Range(0f, 1f)] public float sfxVolume = 1f;    // Set default SFX volume to max

    void Start()
    {
        // Apply the default volume settings
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
        PlayGameMusic();
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep this object alive across scenes
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void PlayGameMusic()
    {
        musicSource.clip = gameMusic;
        musicSource.Play();
    }

    public void PlayShootSFX()
    {
        sfxSource.PlayOneShot(shootSFX);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = musicVolume;
        // Update the PlayerData
        PlayerDataManager.Instance.playerData.musicVolume = volume;

        // Call SaveSettings with the required parameters
        int resolutionIndex = PlayerDataManager.Instance.playerData.resolutionIndex;
        bool vsyncStatus = PlayerDataManager.Instance.playerData.vsyncEnabled;
        PlayerDataManager.Instance.SaveSettings(resolutionIndex, vsyncStatus);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = sfxVolume;
        // Update the PlayerData
        PlayerDataManager.Instance.playerData.sfxVolume = volume;

        // Call SaveSettings with the required parameters
        int resolutionIndex = PlayerDataManager.Instance.playerData.resolutionIndex;
        bool vsyncStatus = PlayerDataManager.Instance.playerData.vsyncEnabled;
        PlayerDataManager.Instance.SaveSettings(resolutionIndex, vsyncStatus);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainGameSceneOcean":
                PlayMusic(gameMusicMainSceneOcean);
                break;
            case "PlayerHouse":
                PlayMusic(gameMusicPlayerHouse);
                break;
            case "MainMenuScene": // Add case for MainMenuScene
                PlayMusic(gameMusicMainMenu);
                break;
            default:
                // Optional: Handle default case
                break;
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }


}
