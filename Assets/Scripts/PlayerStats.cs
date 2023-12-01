using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;


public class PlayerStats : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI oxygenText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI goldText;

    // Default PlayerStats
    [Header("Player Stats")]
    public float oxygen = 100f;
    public float health = 100f;
    private float maxY = 0f;
    public float moveSpeed = 5f;
    public float fireRate = 2f;
    public float viewDistance = 15f;
    public float maxHealth = 100f;
    public float damage = 10f;
    public float maxOxygen = 100f;

   /* private float baseMoveSpeed = 5f;
    private float baseFireRate = 2f;
    private float base6tance = 15f;
    private float baseMaxHealth = 100f;
    private float baseDamage = 10f;
    private float baseMaxOxygen = 100f;
    private float baseOxygen = 100f;
    private float baseHealth = 100f;*/

    [SerializeField]
    private Camera mainCamera;


    [Header("Inventory & Equipment")]
    //public Inventory playerInventory;

    public GameObject treasurePrefab; // Assign the Treasure prefab in the inspector
    private Vector3 treasureSpawnLocation; // Location where the treasure was destroyed

    private void Start()
    {
        // Subscribe to the OnGoldChanged event
        PlayerDataManager.Instance.OnGoldChanged += UpdateGoldUI;
        UpdateGoldUI(PlayerDataManager.Instance.playerData.gold); // Initial UI update

        // Set health and oxygen to max values when the player is first initialized
    }


    private void OnDestroy()
    {
        // Unsubscribe from the OnGoldChanged event
        PlayerDataManager.Instance.OnGoldChanged -= UpdateGoldUI;
    }

    private void Update()
    {
        if (transform.position.y < maxY)
        {
            oxygen -= Time.deltaTime;
            if (oxygen < 0)
            {
                oxygen = 0;
                health -= Time.deltaTime;
            }
        }
        else
        {
            oxygen += Time.deltaTime;
            if (oxygen > maxOxygen)
            {
                oxygen = maxOxygen;
            }
        }

        if (health <= 0)
        {
            health = 0;
            SceneManager.LoadScene("MainMenuScene");
        }

        oxygenText.text = $"Oxygen: {Mathf.Floor(oxygen)}";
        healthText.text = $"Health: {Mathf.Floor(health)}";
        //SetViewDistance(viewDistance);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Treasure")
        {
            // Generate a random amount of gold between 10 and 150
            int randomGoldAmount = Random.Range(10, 151);

            // Update gold through PlayerDataManager with the random amount
            PlayerDataManager.Instance.AddGold(randomGoldAmount);

            // Record the treasure's spawn location for possible respawn
            treasureSpawnLocation = collision.transform.position;

            // Destroy the treasure object
            Destroy(collision.gameObject);

            // Invoke the RespawnTreasure method after 5 seconds
            Invoke("RespawnTreasure", 5f);
        }
    }

    private void RespawnTreasure()
    {
        Instantiate(treasurePrefab, treasureSpawnLocation, Quaternion.identity);
    }

    // Update the gold UI text
    private void UpdateGoldUI(int gold)
    {
        goldText.text = $"Gold: {gold}";
    }

    public void TakeDamage(float damageAmount)
    {
        Debug.Log("Player taking damage: " + damageAmount);

        health -= damageAmount;
        healthText.text = $"Health: {Mathf.Floor(health)}";

        if (health <= 0)
        {
            health = 0;
            SceneManager.LoadScene("GameOverScene");
        }
    }

    public void UpdateUI()
    {
        oxygenText.text = $"Oxygen: {Mathf.Floor(oxygen)}";
        healthText.text = $"Health: {Mathf.Floor(health)}";
    }

    /*public void ResetToBaseStats()
    {
        moveSpeed = baseMoveSpeed;
        fireRate = baseFireRate;
        SetViewDistance(baseViewDistance);
        maxHealth = baseMaxHealth;
        damage = baseDamage;
        maxOxygen = baseMaxOxygen;
        oxygen = baseOxygen;
        health = baseHealth;
    }*/

    public void SetViewDistance(float newViewDistance)
    {
        if (mainCamera != null)
        {
            if (mainCamera.orthographic)
            {
                mainCamera.orthographicSize = newViewDistance;
            }
            else
            {
                // Log a warning or handle perspective cameras differently if needed
                Debug.LogWarning("Main Camera is not orthographic. View distance not set.");
            }
        }
        else
        {
            Debug.LogError("Main camera reference is null in PlayerStats.");
        }
    }

    private void OnEnable()
    {
        StartCoroutine(SetHealthToMaxAfterDelay(2f)); // Start the coroutine with a 10-second delay
    }

    private IEnumerator SetHealthToMaxAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        health = maxHealth;
        oxygen = maxOxygen;
        UpdateUI(); // Update the UI to reflect the change in health
    }

    public void SetHealthAndOxygenToMax()
    {
        health = maxHealth;
        oxygen = maxOxygen;
        UpdateUI();
    }


}
