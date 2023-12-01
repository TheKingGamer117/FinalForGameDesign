using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryCanvasManager : MonoBehaviour
{
    public static InventoryCanvasManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe when the object is destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenuScene")
        {
            gameObject.SetActive(false); // Hide InventoryCanvas in MainMenuScene
        }
        else
        {
            gameObject.SetActive(true); // Show InventoryCanvas in other scenes
        }
    }

    // Additional methods for InventoryCanvasManager...
}
