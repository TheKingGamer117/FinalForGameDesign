using UnityEngine;

public class MovingBackground : MonoBehaviour
{
    // Reference to the Renderer component
    private Renderer rend;

    // Speed at which the texture should move
    public float speed = 0.5f;

    // Tiling based on the scale of the GameObject
    public Vector2 tiling = new Vector2(1, 1);

    // Start is called before the first frame update
    void Start()
    {
        // Get the Renderer component on this GameObject
        rend = GetComponent<Renderer>();

        // Set the initial tiling based on the scale of the GameObject
        rend.material.mainTextureScale = tiling;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the new offset based on time and speed
        float offset = Time.time * speed;

        // Apply the offset to the material's main texture
        rend.material.mainTextureOffset = new Vector2(offset, 0);
    }
}
