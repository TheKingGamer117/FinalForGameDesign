using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For UI elements
using TMPro;

public class OptionsMenuManager : MonoBehaviour
{
    public AudioManagerMainScene audioManager;
    public GameObject optionsMenuCanvas;

    // UI Elements
    public TMP_Dropdown resolutionDropdown;
    public Toggle vsyncToggle;
    public Toggle fullscreenToggle;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private Resolution[] basicResolutions = new Resolution[]
    {
        new Resolution { width = 1920, height = 1080 }, // Full HD
        new Resolution { width = 1280, height = 720 },  // HD
        new Resolution { width = 1024, height = 768 },  // XGA
        // Add more resolutions as needed
    };

    void Start()
    {
        InitializeResolutionDropdown();
        LoadSettings();
        PlayerDataManager.Instance.LoadSettings();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void InitializeResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        // Define a list of basic resolutions
        basicResolutions = new Resolution[]
        {
        new Resolution { width = 1920, height = 1080 }, // Full HD
        new Resolution { width = 1280, height = 720 },  // HD
        new Resolution { width = 1024, height = 768 },  // XGA
                                                        // Add more resolutions as needed
        };

        int currentResolutionIndex = 0;
        for (int i = 0; i < basicResolutions.Length; i++)
        {
            Resolution res = basicResolutions[i];
            options.Add(res.width + " x " + res.height);

            if (res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }


    public void SetVSync(bool isVSync)
    {
        Debug.Log("SetVSync called with value: " + isVSync);
        QualitySettings.vSyncCount = isVSync ? 1 : 0;
        // Update playerData and save
        PlayerDataManager.Instance.playerData.vsyncEnabled = isVSync;
        // Assuming resolutionIndex is stored or accessible in this context
        int resolutionIndex = resolutionDropdown.value;
        PlayerDataManager.Instance.SaveSettings(resolutionIndex, isVSync);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetMasterVolume(float volume)
    {
        // Assuming MasterVolume controls both music and SFX
        audioManager.SetMusicVolume(volume);
        audioManager.SetSFXVolume(volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioManager.SetMusicVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioManager.SetSFXVolume(volume);
    }

    public void LoadSettings()
    {
        // Correctly reference PlayerData from PlayerDataManager
        PlayerDataManager.PlayerData data = PlayerDataManager.Instance.playerData;
        PlayerDataManager.Instance.LoadGame();

        // Apply settings from playerData
        SetResolution(data.resolutionIndex);
        SetVSync(data.vsyncEnabled);
        SetFullscreen(data.fullscreenEnabled);

        // Update UI elements
        resolutionDropdown.value = data.resolutionIndex;
        vsyncToggle.isOn = data.vsyncEnabled;
        fullscreenToggle.isOn = data.fullscreenEnabled;

        // Synchronize audio settings with AudioManager
        audioManager.SetMusicVolume(data.musicVolume);
        audioManager.SetSFXVolume(data.sfxVolume);

        // Update UI sliders
        masterVolumeSlider.value = data.masterVolume;
        musicVolumeSlider.value = data.musicVolume;
        sfxVolumeSlider.value = data.sfxVolume;
    }




    public void TogglePause()
    {
        bool isPaused = Time.timeScale == 0;
        Time.timeScale = isPaused ? 1 : 0; // Pause or unpause the game
        optionsMenuCanvas.SetActive(!isPaused); // Show or hide the options menu
    }

    public void SetResolution(int resolutionIndex)
    {
        // Set the resolution based on the selected index from predefined resolutions
        Resolution selectedResolution = basicResolutions[resolutionIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);

        // Save the selected resolution index and the current vsync status
        bool currentVsyncStatus = QualitySettings.vSyncCount == 1;
        PlayerDataManager.Instance.SaveSettings(resolutionIndex, currentVsyncStatus);
    }
}
