using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager : MonoBehaviour
{
    public static void ChangeScene(string sceneName)
    {
        if (sceneName == "MainMenuScene")
        {
            // Destroy player and inventory if existing
            if (PlayerDataManager.Instance != null)
                Destroy(PlayerDataManager.Instance.gameObject);

            if (InventoryManager.Instance != null)
                Destroy(InventoryManager.Instance.gameObject);
        }

        SceneManager.LoadScene(sceneName);
    }
}
