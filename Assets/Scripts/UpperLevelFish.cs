using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class UpperLevelFish : Fish
{
    [Header("Player Interaction")]
    public float fleeDistance = 10.0f;
    public float fleeRadius = 200.0f; // Radius within which to choose a flee point
    public float fleeDuration = 3.0f; // Duration for which the fish will flee
    private bool isFleeing = false;
    public float dropChance = .25f; // Example: 50% chance to drop an item
    public List<Item> possibleDrops;

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    protected override void Update()
    {
        base.Update(); // Call the base class update method
        CheckFleeCondition();
    }

    protected override void Move()
    {
        if (!isAIActive || isFleeing) return;
        base.Move(); // Call the base class movement logic
    }

    private void CheckFleeCondition()
    {
        if (isFleeing) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer < fleeDistance)
        {
            StartCoroutine(FleeFromPlayer());
        }
    }

    private IEnumerator FleeFromPlayer()
    {
        isFleeing = true;

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

        yield return new WaitForSeconds(fleeDuration);

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
