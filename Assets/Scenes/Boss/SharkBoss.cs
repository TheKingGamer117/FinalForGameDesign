using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class SharkBoss: Fish
{
    private bool isBloodlusted = false;
    private float bloodlustDuration = 20.0f;
    private float bloodlustTimer = 0.0f;
    private bool isInFrenzy = false;
    private float frenzyDamageMultiplier = 2.0f; // Multiplier for damage during frenzy
    private float frenzyAttackSpeedMultiplier = 2.0f; // Multiplier for attack speed during frenzy
    public int damage;
    public float attackRange = 20.0f;
    private bool isAttacking = false;
    public float dropChance = .25f; // Example: 50% chance to drop an item
    public List<Item> possibleDrops;
    private bool isCharging = false;
    private float chargeSpeed = 40.0f; // Adjust as needed
    private Vector3 chargeTarget;
    private float chargeDuration = 3.0f; // Duration of the charge
    private float chargeTimer;
    private float normalSpeed = 10.0f;

    protected override void Start()
    {
        base.Start();
        normalSpeed = agent.speed;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    protected override void Update()
    {
        base.Update(); // Call the base class update method

        if (isBloodlusted)
        {
            bloodlustTimer -= Time.deltaTime;
            if (bloodlustTimer <= 0)
            {
                isBloodlusted = false;
                isInFrenzy = false; // Reset frenzy state
            }
        }

        // Check for frenzy state when health is low
        if (health <= maxHealth * 0.2 && !isInFrenzy)
        {
            isInFrenzy = true;
            // Optionally adjust attack speed and damage here
        }

        if (CanChargeAtPlayer())
        {
            StartChargeAttack();
        }

        if (isCharging)
        {
            ChargeTowardsTarget();
        }

        CheckForPlayerAndAttack();
    }

    protected override void Move()
    {
        if (!isAIActive) return;
        if (isBloodlusted || isInFrenzy)
        {
            ChasePlayer();
        }
        else
        {
            base.Move();
        }
    }

    public override void OnAttacked(float damage)
    {
        base.OnAttacked(damage);
        isBloodlusted = true;
        bloodlustTimer = bloodlustDuration;
        if (health <= maxHealth * 0.2)
        {
            isInFrenzy = true;
        }
    }

    private void ChasePlayer()
    {
        if (player != null)
        {
            agent.SetDestination(player.transform.position);
        }
    }

    // Override attack method to apply frenzy effects
    protected void AttackPlayer(PlayerStats playerStats)
    {
        if (playerStats != null)
        {
            float attackDamage = isInFrenzy ? damage * frenzyDamageMultiplier : damage;
            playerStats.TakeDamage(attackDamage);
        }
    }

    private void CheckForPlayerAndAttack()
    {
        if (player != null && isAIActive && !isAttacking)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= attackRange || isBloodlusted)
            {
                StartCoroutine(AttackSequence());
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        agent.SetDestination(player.transform.position);
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;

        MoveTowardsPlayer();

        // Wait for a moment while the fish moves towards the player
        yield return new WaitForSeconds(1); // Adjust this duration as needed

        // Define someAttackProximity according to your game mechanics
        float someAttackProximity = 2.0f; // Example value

        // Check if the fish is close enough to attack
        if (Vector3.Distance(transform.position, player.transform.position) <= someAttackProximity)
        {
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                AttackPlayer(playerStats); // Deal damage to the player
            }
        }

        isAttacking = false;
    }

    protected override void Die()
    {
        // Spawn the dead fish prefab
        if (deadFishPrefab != null)
        {
            Instantiate(deadFishPrefab, transform.position, transform.rotation);
        }

        // Handle item drop
        DropItem();

        // Notify the spawner to respawn
        spawner?.RespawnFish(gameObject);

        // Destroy the fish object
        Destroy(gameObject);

        StartCoroutine(DelayedTeleport(5.0f));
    }

    private void DropItem()
    {
        if (Random.value <= dropChance && possibleDrops.Count > 0)
        {
            Item drop = possibleDrops[Random.Range(0, possibleDrops.Count)];
            GameObject droppedItem = SpawnItemDrop(drop); // Spawn the item and get its GameObject

            // Reset the scale of the dropped item to its original scale
            if (droppedItem != null)
            {
                droppedItem.transform.localScale = Vector3.one; // This sets the scale to 1,1,1
            }
        }
    }
    private bool CanChargeAtPlayer()
    {
        return !isInFrenzy && Vector3.Distance(transform.position, player.transform.position) < 30.0f;
    }

    private void StartChargeAttack()
    {
        isCharging = true;
        chargeTarget = player.transform.position; // Target the player's last known position
        chargeTimer = chargeDuration;
        agent.speed = chargeSpeed; // Temporarily increase speed for the charge
    }

    private void ChargeTowardsTarget()
    {
        chargeTimer -= Time.deltaTime;
        if (chargeTimer > 0)
        {
            agent.SetDestination(chargeTarget);
        }
        else
        {
            isCharging = false;
            agent.speed = normalSpeed; // Reset to normal speed after charge
        }
    }

    private IEnumerator DelayedTeleport(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Set the player's position to the specified coordinates
            player.transform.position = new Vector3(-430.8f, -0.027f, 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }

    private void HandleCollision(GameObject collidedObject)
    {
        if (collidedObject.CompareTag("Player"))
        {
            PlayerStats playerStats = collidedObject.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }
        }
    }
}