using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class MiddleLevelFish : Fish
{
    private bool isInCombat = false;
    private float chaseDuration = 10.0f;
    private float chaseTimer = 0.0f;
    private float attackCooldown = 2.0f;
    private float attackTimer = 0.0f;
    private float damageAmount = 10.0f;

    [SerializeField]
    private float disengageDistance = 200.0f;
    [SerializeField]
    private float fleeHealthThreshold = 20.0f; // Health threshold for fleeing
    [SerializeField]
    private float fleeRadius = 200.0f; // Radius within which to choose a flee point
    private bool isFleeing = false; // Flag to indicate if the fish is fleeing

    private const string AttackingLayer = "AttackingFish";
    private const string NormalLayer = "Fish";
    public float dropChance = .25f; // Example: 50% chance to drop an item
    public List<Item> possibleDrops;

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    protected override void Update()
    {
        base.Update(); // Call base class update

        if (isInCombat && !isFleeing)
        {
            chaseTimer -= Time.deltaTime;
            attackTimer -= Time.deltaTime;

            // Chase the player when in combat
            ChasePlayer();

            // Disengage from combat based on distance or if the chase timer runs out
            if (Vector3.Distance(transform.position, player.transform.position) > disengageDistance || chaseTimer <= 0)
            {
                DisengageCombat();
            }
        }

        // Check for fleeing behavior
        if (health <= maxHealth * (fleeHealthThreshold / 100.0f) && !isFleeing)
        {
            StartCoroutine(FleeFromPlayer());
        }
    }

    protected override void Move()
    {
        if (!isAIActive || isFleeing) return;
        base.Move(); // Call the base class movement logic

        if (isInCombat)
        {
            ChasePlayer();
        }
        else
        {
            base.Move(); // Call base Move method
        }
    }

    public override void OnAttacked(float damage)
    {
        base.OnAttacked(damage);
        isInCombat = true;
        chaseTimer = chaseDuration;
        attackTimer = attackCooldown;

        // Change the layer of the child object (e.g., "Body") to the attacking layer
        ChangeChildLayer("Body", "AttackingFish");
        Debug.Log(gameObject.name + " Attacked and entering combat.");
    }

    private void ChasePlayer()
    {
        if (player != null)
        {
            agent.SetDestination(player.transform.position);
            //Debug.Log(gameObject.name + " Chasing player to position: " + player.transform.position);
        }
        else
        {
            Debug.Log(gameObject.name + " Player object not found.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collider belongs to the player
        if (!collision.gameObject.CompareTag("Player")) return;

        Debug.Log(gameObject.name + " triggered with " + collision.gameObject.name);

        // Check if the fish is in combat and the attack timer has elapsed
        if (isInCombat && attackTimer <= 0)
        {
            Debug.Log(gameObject.name + " attempting to attack player.");

            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damageAmount);
                Debug.Log(gameObject.name + " dealt " + damageAmount + " damage to player.");
            }
            else
            {
                Debug.Log("PlayerStats component not found on " + collision.gameObject.name);
            }

            attackTimer = attackCooldown; // Reset attack timer after an attack
        }
        else
        {
            Debug.Log("Attack conditions not met. isInCombat: " + isInCombat + ", attackTimer: " + attackTimer);
        }
    }

    private void DisengageCombat()
    {
        isInCombat = false;
        // Revert the layer of the child object (e.g., "body") to the normal layer
        ChangeChildLayer("Body", "Fish");
        Debug.Log(gameObject.name + " Disengaging from combat.");
    }

    private void ChangeChildLayer(string childObjectName, string layerName)
    {
        Transform childObject = transform.Find(childObjectName);
        if (childObject != null)
        {
            childObject.gameObject.layer = LayerMask.NameToLayer(layerName);
            Debug.Log(gameObject.name + " changed the layer of " + childObjectName + " to " + layerName);
        }
        else
        {
            Debug.LogError("Child object named " + childObjectName + " not found in " + gameObject.name);
        }
    }

    private IEnumerator FleeFromPlayer()
    {
        isFleeing = true;
        isInCombat = false;

        // Calculate the direction away from the player
        Vector3 directionAwayFromPlayer = (transform.position - player.transform.position).normalized;
        Vector3 fleeCenter = transform.position + directionAwayFromPlayer * fleeRadius;

        // Filter points that are within the flee radius and in the opposite direction of the player
        var potentialPoints = destinationPoints.Where(point =>
            Vector3.Distance(point.position, transform.position) <= fleeRadius &&
            Vector3.Dot((point.position - transform.position).normalized, directionAwayFromPlayer) > 0
        ).ToList();

        // Choose a random point from the filtered list
        if (potentialPoints.Count > 0)
        {
            Transform chosenPoint = potentialPoints[Random.Range(0, potentialPoints.Count)];
            agent.SetDestination(chosenPoint.position);
        }
        else
        {
            Debug.LogError("No suitable flee point found.");
        }

        // Wait for the flee duration before stopping the fleeing behavior
        yield return new WaitForSeconds(3.0f); // Flee duration

        isFleeing = false;
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
    }

    private void DropItem()
    {
        if (Random.value <= dropChance && possibleDrops.Count > 0)
        {
            Item drop = possibleDrops[Random.Range(0, possibleDrops.Count)];
            SpawnItemDrop(drop);
        }
    }

}