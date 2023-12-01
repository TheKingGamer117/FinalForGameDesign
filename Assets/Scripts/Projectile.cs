using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifespan = 5f; // Time in seconds until the projectile is automatically destroyed
    private Vector3 direction;
    private Collider2D col;
    private float damage;

    private void Start()
    {
        col = gameObject.GetComponent<BoxCollider2D>();
        if (col == null)
        {
            Debug.LogError("BoxCollider2D component is missing from the GameObject.");
        }

        // Automatically destroy the projectile after its lifespan (5 seconds)
        Destroy(gameObject, lifespan);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void SetDirection(Vector3 dir, float dmg)
    {
        direction = dir;
        damage = dmg;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object is not null
        if (collision != null)
        {
            // Check if the collided object's gameObject is not null and is active
            if (collision.gameObject != null && collision.gameObject.activeInHierarchy)
            {
                // Try to get the Fish component directly from the collided object
                Fish fish = collision.GetComponent<Fish>();

                // If the Fish component is not found directly, check the parent
                if (fish == null)
                {
                    Transform parent = collision.transform.parent;
                    if (parent != null)
                    {
                        fish = parent.GetComponent<Fish>();
                    }
                }

                // Check if the fish component is not null
                if (fish != null)
                {
                    // Invoke the OnAttacked method on the fish
                    fish.OnAttacked(damage);

                    // Destroy the projectile
                    Destroy(gameObject);
                }
            }
        }
    }

}