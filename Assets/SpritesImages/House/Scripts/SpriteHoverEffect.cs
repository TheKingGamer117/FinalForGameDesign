using UnityEngine;

public class SpriteHoverEffect : MonoBehaviour
{
    public Sprite normalSprite;   // Assign in inspector
    public Sprite hoverSprite;    // Assign in inspector
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
           // Debug.LogError("SpriteRenderer not found on the GameObject.");
        }
    }

    void OnMouseEnter()
    {
        //Debug.Log("Mouse entered: " + gameObject.name);
        if (spriteRenderer != null && hoverSprite != null)
        {
            spriteRenderer.sprite = hoverSprite;
        }
    }

    void OnMouseExit()
    {
        //Debug.Log("Mouse exited: " + gameObject.name);
        if (spriteRenderer != null && normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }
    }
}
