using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BoatController : MonoBehaviour
{
    public GameObject changeSceneButton; // Drag your 'Change Scenes' button here
    public static BoatController Instance;

    private void Start()
    {
        // Initially hide the change scene button
        changeSceneButton.SetActive(false);

        // Add listener to the button
        changeSceneButton.GetComponent<Button>().onClick.AddListener(ChangeToPlayerHouseScene);
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Show the change scene button when the player collides with the boat
            changeSceneButton.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Optionally, hide the button when the player leaves the boat area
            changeSceneButton.SetActive(false);
        }
    }

    private void ChangeToPlayerHouseScene()
    {
        // Change to the PlayerHouse scene
        SceneManager.LoadScene("PlayerHouse");
    }
}
