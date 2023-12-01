using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class Fish : MonoBehaviour
{
    [Header("Attributes")]
    public int minHealth;
    public int maxHealth;
    public float minSpeed;
    public float maxSpeed;
    public int minPackSize;
    public int maxPackSize;

    [Header("Components")]
    public Transform bodyTransform;
    public AnimationClip animationClip;

    [Header("Dead Prefab")]
    public GameObject deadFishPrefab;

    public int health;
    public float speed;
    public int packSize;
    public Animator animator;
    public NavMeshAgent agent;
    public int navMeshLayerMask;
    public float wanderRadius = 10.0f;
    public float wanderTimer = 0.1f;
    public float timer;

    [Header("AI Activation")]
    public float activationDistance = 100.0f; // Distance within which the fish AI activates
    protected GameObject player; // Reference to the player object
    public GameObject itemDropPrefab;
    protected bool isAIActive = false;
    public FishSpawner spawner;


    // Inside Fish class
    protected List<Transform> destinationPoints = new List<Transform>();


    protected virtual void Start()
    {
        InitializeFishAttributes();
        InitializeComponents();
        InitializeDestinationPoints();
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        player = GameObject.FindWithTag("Player"); // Assuming the player has the tag "Player"
        StartCoroutine(AIActivationCheck());
    }


    private void InitializeFishAttributes()
    {
        health = Random.Range(minHealth, maxHealth + 1);
        speed = Random.Range(minSpeed, maxSpeed);
        packSize = Random.Range(minPackSize, maxPackSize + 1);
    }

    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent not found on " + gameObject.name);
            return;
        }
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.radius = 0.5f;
        agent.height = 1.0f;
        agent.speed = speed;
        navMeshLayerMask = LayerMask.GetMask("Navmesh");
        timer = wanderTimer;
        PlayInitialAnimation();
    }

    private void PlayInitialAnimation()
    {
        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogError("AnimatorController not assigned to " + gameObject.name);
            return;
        }

        if (animationClip != null)
        {
            animator.Play(animationClip.name);
        }
        else
        {
            Debug.LogWarning("Animation clip is not set for this fish.");
        }
    }

    private void InitializeDestinationPoints()
    {
        FishSwimPointMaster swimPointMaster = FindObjectOfType<FishSwimPointMaster>();
        if (swimPointMaster != null)
        {
            foreach (Transform child in swimPointMaster.transform)
            {
                destinationPoints.Add(child);
            }
        }
        else
        {
            Debug.LogError("No FishSwimPointMaster found in the scene.");
        }
    }

    protected virtual void Move()
    {
        if (!agent.isOnNavMesh)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= wanderTimer && destinationPoints.Count > 0)
        {
            if (IsCloseToDestination())
            {
                StartCoroutine(ChooseRandomDestination());
                timer = 0;
            }
        }
    }

    private IEnumerator ChooseRandomDestination()
    {
        NavMeshPath path = new NavMeshPath();
        Vector3 newPos = Vector3.zero;
        bool pathFound = false;
        int attempts = 0;

        while (!pathFound && attempts < destinationPoints.Count)
        {
            int randomIndex = Random.Range(0, destinationPoints.Count);
            newPos = destinationPoints[randomIndex].position;

            NavMesh.CalculatePath(transform.position, newPos, NavMesh.AllAreas, path);
            yield return null; // Wait for one frame

            if (path.status == NavMeshPathStatus.PathComplete)
            {
               // Debug.Log("Path found for fish: " + gameObject.name + " to position: " + newPos);
                pathFound = true;
                agent.SetDestination(newPos);
            }
            else
            {
               // Debug.Log("Path failed for fish: " + gameObject.name + " on attempt: " + attempts);
            }

            attempts++;
        }
    }


    private bool IsCloseToDestination()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    protected virtual void Update()
    {
        Move();
        ResetZPosition();
        RotateTowardsMovementDirection();
    }

    private void ResetZPosition()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    private void RotateTowardsMovementDirection()
    {
        if (agent.velocity != Vector3.zero)
        {
            Vector2 direction = agent.velocity;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Flip the sprite based on the direction
            if (direction.x >= 0)
            {
                transform.localScale = new Vector3(transform.localScale.x, Mathf.Abs(transform.localScale.y), transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(transform.localScale.x, -Mathf.Abs(transform.localScale.y), transform.localScale.z);
            }

            // Smoothly rotate towards the target angle
            float rotationSpeed = 5.0f; // Adjust this value as needed
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public virtual void OnAttacked(float damage)
    {
        health -= (int)damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            if (destinationPoints.Count > 0)
            {
                if (IsCloseToDestination() || !agent.hasPath)
                {
                    yield return ChooseRandomDestination();
                }
            }

            yield return new WaitForSeconds(wanderTimer); // Wait for the specified wander time before recalculating
        }
    }

    private IEnumerator AIActivationCheck()
    {
        while (true)
        {
            if (player == null)
            {
                Debug.LogError("Player reference not found in Fish script.");
                yield break; // Stop the coroutine if player is not found
            }

            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
           // Debug.Log(gameObject.name + " distance to player: " + distanceToPlayer); // Debug line

            if (distanceToPlayer <= activationDistance && !isAIActive)
            {
                ActivateAI();
               // Debug.Log(gameObject.name + " activated."); // Additional debug line
            }
            yield return new WaitForSeconds(1f); // Check every second, adjust as needed
        }
    }

    public void ActivateAI()
    {
        isAIActive = true;
        agent.enabled = true;
        StartCoroutine(MoveRoutine());
        // Enable other AI components or behaviors here
    }

    public virtual GameObject SpawnItemDrop(Item item)
    {
        GameObject itemDrop = Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
        ItemPickup itemPickup = itemDrop.GetComponent<ItemPickup>();
        if (itemPickup != null)
        {
            itemPickup.Item = item; // Assign the Item ScriptableObject to the spawned item

            // Debug.Log($"Item dropped: {item.itemName} at position {transform.position}"); // Debug
        }
        else
        {
            Debug.Log("SpawnItemDrop: ItemPickup component not found on itemDropPrefab"); // Debug
        }

        return itemDrop; // Return the instantiated item drop GameObject
    }


    protected virtual void Die()
    {
        if (deadFishPrefab != null)
        {
            Instantiate(deadFishPrefab, transform.position, transform.rotation);
        }

        // Notify the spawner to respawn
        spawner?.RespawnFish(gameObject);

        Destroy(gameObject);
    }

}