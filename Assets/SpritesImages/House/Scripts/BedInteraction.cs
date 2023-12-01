using UnityEngine;

public class BedInteraction : MonoBehaviour
{
    public GameObject savePrompt; // Assign this in the Unity editor

    public void OnBedClicked()
    {
        // Show the save prompt when the bed is clicked
        savePrompt.SetActive(true);
        Debug.Log("Bed Clicked - Showing Save Prompt");
    }

    public void OnSaveConfirmed()
    {
        // Call the save method from PlayerDataManager or your save system
        PlayerDataManager.Instance.SaveGame();
        savePrompt.SetActive(false); // Hide the prompt after saving
        Debug.Log("Game Saved");
    }

    public void OnSaveCancelled()
    {
        // Hide the prompt without saving
        savePrompt.SetActive(false);
        Debug.Log("Save Cancelled");
    }
}
