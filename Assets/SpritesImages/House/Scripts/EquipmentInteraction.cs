using UnityEngine;

public class EquipmentInteraction : MonoBehaviour
{
    public ChestUI chestUI; // Reference to your ChestUI script

    public void OnEquipmentClicked()
    {
        if (chestUI != null)
        {
            InventoryManager.Instance.activeChest = chestUI.chest; // Set the active chest
            chestUI.UpdateUI(); // Update the UI to reflect the current items in the chest
            chestUI.gameObject.SetActive(true); // Show the chest UI
        }
        else
        {
            Debug.LogError("ChestUI reference not set in EquipmentInteraction");
        }
    }
}

