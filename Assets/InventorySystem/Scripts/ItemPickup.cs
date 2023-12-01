using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item Item;
    private bool playerInRange = false; // Flag to check if the player is in range

    void Start()
    {
        UpdateItemVisuals();
    }

    // Function to pick up the item
    void Pickup()
    {
        Item itemCopy = Instantiate(Item); // Create a new instance
        itemCopy.stackCount = 1; // Set stack count to 1 for the picked up item
        InventoryManager.Instance.Add(itemCopy);
        Destroy(gameObject);
    }

    // This function is called when another collider enters the trigger zone of this gameObject
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") // replace "Player" with the actual tag of your player gameObject
        {
            playerInRange = true; // Set the flag to true
        }
    }

    // This function is called when another collider leaves the trigger zone of this gameObject
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player") // replace "Player" with the actual tag of your player gameObject
        {
            playerInRange = false; // Set the flag to false
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Pickup();
        }
    }

    void UpdateItemVisuals()
    {
        if (Item != null)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = Item.icon; // Set the sprite to the item's icon
            }
            // Add any other visual updates (like name, etc.) here if needed
        }
    }
}