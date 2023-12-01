using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Only if you're using TextMeshPro components
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    public GameObject optionsMenuPanel;
    public TMP_Dropdown resolutionDropdown; // Change to Dropdown if you're not using TextMeshPro
    public Slider audioSlider;
    public TMP_InputField fpsLimitInput; // Change to InputField if you're not using TextMeshPro

    private Resolution[] availableResolutions;

    private void Start()
    {
        // Set default resolution to 1920x1080
        Screen.SetResolution(1920, 1080, Screen.fullScreen);

        // Populate the resolution dropdown
        availableResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> resolutionOptions = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            string option = availableResolutions[i].width + "x" + availableResolutions[i].height;
            resolutionOptions.Add(option);

            // Update the condition to check for 1920x1080 resolution
            if (availableResolutions[i].width == 1920 &&
                availableResolutions[i].height == 1080)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Ensure the SetResolution method is called whenever the dropdown value changes
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainGameSceneOcean");
    }

    public void OpenOptionsMenu()
    {
        optionsMenuPanel.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        optionsMenuPanel.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = availableResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetVolume(float volume)
    {
        // You'll need to integrate this with Unity's audio system, e.g., AudioMixer
        Debug.Log("Volume set to: " + volume);
    }

    public void SetFPSLimit()
    {
        int fps;
        if (int.TryParse(fpsLimitInput.text, out fps))
        {
            Application.targetFrameRate = fps;
        }
        else
        {
            Debug.LogError("Invalid FPS value entered.");
        }
    }

    public void OnFPSInputSelected(string currentText)
    {
        // This will be executed when the fpsLimitInput field is selected.
        // currentText will contain the current text of the input field.
        Debug.Log("FPS Input Field selected. Current value: " + currentText);
        // You can add any additional actions or feedback you want here.
    }

    public void OnFPSInputDeselected(string currentText)
    {
        // This will be executed when the fpsLimitInput field is deselected.
        // currentText will contain the current text of the input field.
        Debug.Log("FPS Input Field deselected. Current value: " + currentText);
        // You can add any additional actions or feedback you want here.
    }
}
