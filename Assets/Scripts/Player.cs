using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public new Rigidbody2D rigidbody2D; // Rigidbody component
    public GameObject waterGrenadePrefab;
    private PlayerStats playerStats;

    [SerializeField]
    private float moveSpeed = 5f; // Regular movement speed

    [SerializeField]
    private float doubleClickTime = 0.2f; // Time window for double click, editable in Inspector

    [SerializeField]
    private float dashDistance = 5f; // Distance to dash on double click, editable in Inspector

    private float lastClickTimeW = 0f;
    private float lastClickTimeA = 0f;
    private float lastClickTimeS = 0f;
    private float lastClickTimeD = 0f;

    public GameObject projectilePrefab; // Drag your Projectile prefab here in the inspector
    private Camera mainCamera;

    [SerializeField]
    private float maxY = 1; // Set this to the y-coordinate of the water's surface

    private SpriteRenderer spriteRenderer; // SpriteRenderer component

    private float fireRate = .01f; // Time between shots in seconds
    private float nextFireTime = 0f; // Time when the player can fire next
    public static Player Instance;
    private EquipmentManager equipmentManager;
    public float throwForce = 10f;
    public OptionsMenuManager optionsMenuManager;


    void Awake()
    {
        // Ensure that EquipmentManager is assigned
        equipmentManager = GetComponent<EquipmentManager>();
        if (equipmentManager == null)
        {
            Debug.LogError("EquipmentManager component not found on the same GameObject");
        }

        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Update the equipment stats when a new scene is loaded
        if (equipmentManager != null)
        {
            equipmentManager.UpdateEquipmentStats();
        }
        else
        {
            Debug.LogError("EquipmentManager is not assigned in Player.");
        }

        StartCoroutine(RegularInventoryUpdateForFirstTwoSeconds());
        StartCoroutine(SetMaxHealthAndOxygenAfterDelay(1f));
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main; // Initialize main camera
        equipmentManager = GetComponent<EquipmentManager>();
        equipmentManager.UpdateEquipmentStats(); // Call this to initialize equipment stats

        playerStats = GetComponent<PlayerStats>(); // Initialize PlayerStats
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component not found on the player.");
        }

        InventoryManager.Instance.OnInventoryChanged += OnInventoryChanged;
        equipmentManager.UpdateEquipmentStats();

    }

    void Update()
    {
        // Ensure playerStats is not null
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component is null.");
            return;
        }

        // Regular movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(horizontalInput, verticalInput);
        rigidbody2D.velocity = movement * playerStats.moveSpeed;  // Use moveSpeed from PlayerStats

        // Get mouse position in world coordinates and calculate direction
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);

        // Make the player's "right" vector point toward the mouse cursor
        transform.right = direction;

        // Flip sprite based on the mouse position relative to the player
        Vector3 localScale = transform.localScale;
        if (mousePosition.x < transform.position.x)
        {
            localScale.y = -1f;
        }
        else
        {
            localScale.y = 1f;
        }
        transform.localScale = localScale;

        // Check for double click to dash
        CheckForDoubleClick(KeyCode.W, Vector2.up);
        CheckForDoubleClick(KeyCode.A, Vector2.left);
        CheckForDoubleClick(KeyCode.S, Vector2.down);
        CheckForDoubleClick(KeyCode.D, Vector2.right);

        // Prevent going above water surface
        if (transform.position.y >= maxY)
        {
            transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
            if (rigidbody2D.velocity.y > 0)
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
            }
        }

        // Shoot on left mouse click with fire rate control
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + playerStats.fireRate; // Update the next fire time
            Shoot(playerStats.damage);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) // Key for using Health Stim
        {
            UseHighestVersionItem(ItemType.HealthStim);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) // Key for using OxygenStim
        {
            UseHighestVersionItem(ItemType.OxygenStim);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) // Key for using Water Grenade
        {
            UseHighestVersionItem(ItemType.WaterGrenade);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsMenuManager != null)
            {
                optionsMenuManager.TogglePause();
            }
            else
            {
                Debug.LogError("OptionsMenuManager not assigned in Player script.");
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Treasure")
        {
            // Generate a random amount of gold between 10 and 150
            int randomGoldAmount = Random.Range(10, 151);

            // Add the random amount of gold to the player's total gold
            PlayerDataManager.Instance.AddGold(randomGoldAmount);

            // Destroy the treasure object
            Destroy(collision.gameObject);
        }
    }


    void Shoot(float damage)
    {
        // Debug.Log("Shooting with damage: " + damage);
        // Offset the projectile position slightly in the direction the player is facing
        Vector3 spawnPosition = transform.position + (transform.right * 1f);
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.Euler(0, 0, angle));
        if (projectile)
        {
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript)
            {
                projectileScript.SetDirection(transform.right, damage);

                // Destroy the projectile after 5 seconds
                Destroy(projectile, 5f);
            }
            else
            {
                Debug.LogError("Projectile script not found on the instantiated object.");
            }

            AudioManagerMainScene.instance.PlayShootSFX();
        }
        else
        {
            Debug.LogError("Projectile could not be instantiated.");
        }
    }


    private void CheckForDoubleClick(KeyCode key, Vector2 direction)
    {
        float lastClickTime = 0;

        // Get the appropriate last click time based on the key
        switch (key)
        {
            case KeyCode.W: lastClickTime = lastClickTimeW; break;
            case KeyCode.A: lastClickTime = lastClickTimeA; break;
            case KeyCode.S: lastClickTime = lastClickTimeS; break;
            case KeyCode.D: lastClickTime = lastClickTimeD; break;
        }

        // Check for double click
        if (Input.GetKeyDown(key))
        {
            if (Time.time - lastClickTime < doubleClickTime)
            {
                // Perform dash
                rigidbody2D.position += direction * dashDistance;
            }

            // Update last click time
            lastClickTime = Time.time;
        }

        // Update the appropriate last click time based on the key
        switch (key)
        {
            case KeyCode.W: lastClickTimeW = lastClickTime; break;
            case KeyCode.A: lastClickTimeA = lastClickTime; break;
            case KeyCode.S: lastClickTimeS = lastClickTime; break;
            case KeyCode.D: lastClickTimeD = lastClickTime; break;
        }
    }

    private void UseHighestVersionItem(ItemType itemType)
    {
        Item highestVersionItem = GetHighestVersionItem(itemType);
        if (highestVersionItem != null)
        {
            ApplyItemEffects(highestVersionItem);
            // Reduce item count or remove from inventory after use
            InventoryManager.Instance.Remove(highestVersionItem);
        }
    }

    private Item GetHighestVersionItem(ItemType itemType)
    {
        Item highestVersionItem = null;
        int highestLevel = 0;

        foreach (Item item in InventoryManager.Instance.Items)
        {
            if (item.itemType == itemType && item.maxLv > highestLevel)
            {
                highestLevel = item.maxLv;
                highestVersionItem = item;
            }
        }

        return highestVersionItem;
    }

    private void ApplyItemEffects(Item item)
    {
        var playerStats = GetComponent<PlayerStats>(); // Get the PlayerStats component
        if (playerStats != null)
        {
            switch (item.itemType)
            {
                case ItemType.HealthStim:
                    playerStats.health += item.healthValue;
                    break;
                case ItemType.OxygenStim:
                    playerStats.oxygen += item.oxygenValue;
                    break;
                case ItemType.WaterGrenade:
                    Item waterGrenadeItem = GetHighestVersionItem(ItemType.WaterGrenade);
                    if (waterGrenadeItem != null)
                    {
                        ThrowGrenade(waterGrenadeItem);
                    }
                    break;
            }
        }

        playerStats.UpdateUI(); // Update the UI to reflect changes
    }

    private void CreateWaterGrenade(Item item)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        GameObject grenade = Instantiate(waterGrenadePrefab, mousePosition, Quaternion.identity);
        WaterGrenade grenadeScript = grenade.GetComponent<WaterGrenade>();
        if (grenadeScript)
        {
            grenadeScript.damage = item.damageValue;
            // Additional configuration as needed
        }
    }

    void OnDestroy()
    {
        // Unsubscribe when the object is destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Unsubscribe from the OnInventoryChanged event
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= OnInventoryChanged;
        }
    }

    public void OnInventoryChanged()
    {
        equipmentManager.UpdateEquipmentStats();
    }

    private IEnumerator RegularInventoryUpdateForFirstTwoSeconds()
    {
        float elapsedTime = 0f;
        float updateDuration = 2f; // Set the duration for the coroutine to run

        while (elapsedTime < updateDuration)
        {
            if (equipmentManager != null)
            {
                equipmentManager.UpdateEquipmentStats();
            }
            yield return new WaitForSeconds(1f); // Wait for 1 second before the next update
            elapsedTime += 1f; // Increment the elapsed time
        }
    }

    private void ThrowGrenade(Item grenadeItem)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        GameObject grenade = Instantiate(waterGrenadePrefab, transform.position, Quaternion.identity);
        WaterGrenade grenadeScript = grenade.GetComponent<WaterGrenade>();
        if (grenadeScript != null)
        {
            grenadeScript.damage = grenadeItem.damageValue;
            StartCoroutine(MoveGrenadeToPosition(grenade, mousePosition));
        }
    }

    private IEnumerator MoveGrenadeToPosition(GameObject grenade, Vector3 targetPosition)
    {
        float timeToReachTarget = 0.5f; // Adjust as needed for speed
        float elapsedTime = 0;

        Vector3 startPosition = grenade.transform.position;

        while (elapsedTime < timeToReachTarget)
        {
            grenade.transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / timeToReachTarget));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        grenade.transform.position = targetPosition;
        grenade.GetComponent<WaterGrenade>().Explode(); // Explode immediately upon reaching the target
    }

    private IEnumerator SetMaxHealthAndOxygenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (playerStats != null)
        {
            playerStats.health = playerStats.maxHealth;
            playerStats.oxygen = playerStats.maxOxygen;
            playerStats.UpdateUI();
        }
    }

}