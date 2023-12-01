using UnityEngine;
using UnityEngine.UI;

public class GoldenGunController : MonoBehaviour
{
    public GameObject teleportButton; // Drag your 'Teleport' button here
    public static GoldenGunController Instance;

    private void Start()
    {
        // Initially hide the teleport button
        teleportButton.SetActive(false);

        // Add listener to the button
        teleportButton.GetComponent<Button>().onClick.AddListener(TeleportPlayer);
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
            // Show the teleport button when the player collides with the GoldenGun
            teleportButton.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Hide the button when the player leaves the GoldenGun area
            teleportButton.SetActive(false);
        }
    }

    private void TeleportPlayer()
    {
        // Assuming the player has a tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Teleport the player to the specified coordinates
            player.transform.position = new Vector3(1000f, -1000f, 0f);
        }

        // Optionally, hide the button after teleporting
        teleportButton.SetActive(false);
    }
}
