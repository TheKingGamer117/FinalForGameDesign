using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class FishSpawner : MonoBehaviour
{
    public GameObject[] upperLevelFishPrefabs;
    public GameObject[] middleLevelFishPrefabs;
    public GameObject[] lowerLevelFishPrefabs;

    public Transform[] upperLevelSpawnPoints;
    public Transform[] middleLevelSpawnPoints;
    public Transform[] lowerLevelSpawnPoints;

    public float yOffset = 1f;  // Y-offset for each tier of fish

    void Start()
    {
        SpawnFish(upperLevelFishPrefabs, upperLevelSpawnPoints, 0);
        SpawnFish(middleLevelFishPrefabs, middleLevelSpawnPoints, -yOffset);
        SpawnFish(lowerLevelFishPrefabs, lowerLevelSpawnPoints, -2 * yOffset);
    }

    void SpawnFish(GameObject[] fishPrefabs, Transform[] spawnPoints, float yLevelOffset)
    {
        foreach (GameObject fishPrefab in fishPrefabs)
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                int packSize = fishPrefab.GetComponent<Fish>().packSize;
                for (int j = 0; j < packSize; j++)
                {
                    Vector3 spawnPosition = GetValidSpawnPosition(spawnPoint.position, yLevelOffset, CalculateDetectionRadius(fishPrefab));
                    GameObject fishInstance = Instantiate(fishPrefab, spawnPosition, Quaternion.identity);

                    // Assign the spawner to the spawned fish
                    Fish fishScript = fishInstance.GetComponent<Fish>();
                    if (fishScript != null)
                    {
                        fishScript.spawner = this;
                    }
                    else
                    {
                        Debug.LogError("Fish script not found on spawned fish prefab");
                    }
                }
            }
        }
        // Debug.Log("Fish spawning completed in " + (Time.time - startTime) + " seconds.");
    }

    float CalculateDetectionRadius(GameObject fishPrefab)
    {
        Collider collider = fishPrefab.GetComponentInChildren<Collider>();
        if (collider != null)
        {
            return collider.bounds.extents.magnitude; // Use the largest extent of the collider
        }
        return 1.0f; // Default value if no collider is found
    }

    Vector3 GetValidSpawnPosition(Vector3 basePosition, float yLevelOffset, float detectionRadius)
    {
        float startTime = Time.time;
        Vector3 spawnPosition = basePosition;
        bool positionFound = false;
        int maxAttempts = 100;
        int attempts = 0;

        while (!positionFound && attempts < maxAttempts)
        {
            float randomX = Random.Range(-500f, 500f);
            float randomY = Random.Range(-500f, 500f);
            Vector3 randomPosition = basePosition + new Vector3(randomX, randomY + yLevelOffset, 0);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPosition, out hit, detectionRadius, NavMesh.AllAreas))
            {
                Vector3 directionToCenter = (Vector3.zero - hit.position).normalized; // Assuming Vector3.zero is a safe point
                if (!Physics.Raycast(hit.position, directionToCenter, detectionRadius))
                {
                    spawnPosition = hit.position;
                    positionFound = true;
                }
            }

            attempts++;
        }

        if (!positionFound)
        {
            Debug.LogWarning("Failed to find a valid spawn position after " + maxAttempts + " attempts.");
        }
        else
        {
            // Debug.Log("Spawn position found in " + (Time.time - startTime) + " seconds.");
        }

        return spawnPosition;
    }

    public void RespawnFish(GameObject deadFish)
    {
        GameObject[] fishArray = DetermineFishArray(deadFish);
        if (fishArray == null)
        {
            Debug.LogError("Unable to determine fish array for respawn.");
            return;
        }

        GameObject fishPrefab = fishArray[Random.Range(0, fishArray.Length)];
        Transform spawnPoint = SelectRandomSpawnPoint(fishPrefab);
        Instantiate(fishPrefab, spawnPoint.position, Quaternion.identity);
    }

    private GameObject[] DetermineFishArray(GameObject deadFish)
    {
        string deadFishName = deadFish.name.Replace("(Clone)", "").Trim();

        foreach (var fishPrefab in upperLevelFishPrefabs)
        {
            if (fishPrefab.name == deadFishName)
                return upperLevelFishPrefabs;
        }
        foreach (var fishPrefab in middleLevelFishPrefabs)
        {
            if (fishPrefab.name == deadFishName)
                return middleLevelFishPrefabs;
        }
        foreach (var fishPrefab in lowerLevelFishPrefabs)
        {
            if (fishPrefab.name == deadFishName)
                return lowerLevelFishPrefabs;
        }

        return null; // Fish type not found
    }


    private Transform SelectRandomSpawnPoint(GameObject fishPrefab)
    {
        Transform[] spawnPoints;

        if (upperLevelFishPrefabs.Contains(fishPrefab))
            spawnPoints = upperLevelSpawnPoints;
        else if (middleLevelFishPrefabs.Contains(fishPrefab))
            spawnPoints = middleLevelSpawnPoints;
        else if (lowerLevelFishPrefabs.Contains(fishPrefab))
            spawnPoints = lowerLevelSpawnPoints;
        else
            return null; // If fish type doesn't match any category

        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

}