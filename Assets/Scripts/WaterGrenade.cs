using UnityEngine;

public class WaterGrenade : MonoBehaviour
{
    public float damage;
    public float explosionDelay = 2f; // Delay before explosion
    public float explosionRadius = 5f; // Radius of effect
    public GameObject explosionPrefab;

    private void Start()
    {
        Invoke("Explode", explosionDelay); // Trigger explosion after a delay
    }

    public void Explode()
    {
        Debug.Log("Explode method called");

        // Detecting all colliders within the explosion radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
       // Debug.Log("Number of colliders detected: " + hitColliders.Length);

        foreach (var hitCollider in hitColliders)
        {
           // Debug.Log("Detected collider: " + hitCollider.gameObject.name);

            if (hitCollider.CompareTag("Fish"))
            {
               // Debug.Log("Fish detected: " + hitCollider.gameObject.name);
                Fish fish = hitCollider.GetComponentInParent<Fish>(); // Get Fish component from parent
                if (fish != null)
                {
                  //  Debug.Log("Applying damage to fish");
                    fish.OnAttacked(damage); // This method should handle the fish taking damage
                }
                else
                {
                   // Debug.Log("Fish component not found on parent object");
                }
            }
        }

        // Add other explosion effects here (like visual or sound effects)

        // Instantiate the WaterExplosion prefab at the grenade's position
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Destroy the grenade object itself
        Destroy(gameObject);

        Debug.Log("Grenade destroyed after explosion");
    }





    private void OnDrawGizmos()
    {
        // This is just for visualization in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    public class AutoDestroyAfterAnimation : MonoBehaviour
    {
        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
            {
                Destroy(gameObject);
            }
        }
    }

}
