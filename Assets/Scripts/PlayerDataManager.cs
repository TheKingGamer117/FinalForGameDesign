using System.IO;
using UnityEngine;
using System; // Required for the Action delegate

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;
    private string saveFilePath;
    public PlayerData playerData; // Reference to PlayerData class defined in another script
    // Event to notify when gold changes
    public event Action<int> OnGoldChanged;

    [Serializable]
    public class PlayerData
    {
        public int gold;
        public int resolutionIndex;
        public bool vsyncEnabled;
        public bool fullscreenEnabled;
        public float masterVolume;
        public float musicVolume;
        public float sfxVolume;
        // Other fields...
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // This ensures persistence across scenes

            // Your existing initialization code...
            saveFilePath = Application.persistentDataPath + "/savefile.json";
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Destroy any duplicate instances
        }
    }

    // Method to add gold and trigger the event
    public void AddGold(int amount)
    {
        playerData.gold += amount;
        OnGoldChanged?.Invoke(playerData.gold); // Trigger the event
       // Debug.Log("[PlayerDataManager] AddGold called with amount: " + amount + ". Current gold: " + (playerData != null ? playerData.gold.ToString() : "PlayerData is null"));

    }

    // Method to subtract gold (e.g., when purchasing items) and trigger the event
    public void UseGold(int amount)
    {
        if (amount <= playerData.gold)
        {
            playerData.gold -= amount;
            OnGoldChanged?.Invoke(playerData.gold); // Trigger the event
        }
        else
        {
           // Debug.LogError("Not enough gold to complete this transaction.");
        }
      //  Debug.Log("[PlayerDataManager] useGold called with amount: " + amount + ". Current gold: " + (playerData != null ? playerData.gold.ToString() : "PlayerData is null"));

    }

    public void SaveGame()
    {
        string jsonData = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(saveFilePath, jsonData);
       // Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string jsonData = File.ReadAllText(saveFilePath);
            playerData = JsonUtility.FromJson<PlayerData>(jsonData);
           // Debug.Log("Game Loaded");
        }
    }

    public void SaveSettings(int resolutionIndex, bool vsyncStatus /*, other parameters */)
    {
        // Assign current settings to playerData
        playerData.resolutionIndex = resolutionIndex;
        playerData.vsyncEnabled = QualitySettings.vSyncCount == 1;
        // Assign other settings...

        SaveGame(); // Call existing SaveGame method
    }


    public void LoadSettings()
    {
        PlayerDataManager.PlayerData data = PlayerDataManager.Instance.playerData;
        LoadGame(); // Load data

        // Apply settings from playerData
        // Set resolution, vsync, etc., based on playerData values
    }
}
