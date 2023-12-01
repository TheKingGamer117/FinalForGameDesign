using UnityEngine;

public class GameControl : MonoBehaviour
{
    public GameObject inventoryUI;  // The GameObject that contains your entire inventory UI
    private bool isInventoryOpen = false;  // To track if the inventory is open

    // Update is called once per frame
    void Update()
    {
        // Listen for the 'I' key to toggle inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    // Function to toggle inventory open/closed
    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;  // Flip the state

        if (isInventoryOpen)
        {
            inventoryUI.SetActive(true);  // Show inventory UI
            // You may want to pause game or disable controls here
        }
        else
        {
            inventoryUI.SetActive(false);  // Hide inventory UI
            // You may want to unpause game or enable controls here
        }
    }
}
