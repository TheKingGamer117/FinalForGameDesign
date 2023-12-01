using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum ChestType
{
    Consumable,
    Equipment,
    // Add other types as needed
}

public class Chest : MonoBehaviour
{
    public ChestType chestType;
    public List<Item> itemsInChest;
    public ChestUI chestUI;
    public Toggle moveItemToggle;


    void Awake()
    {
        itemsInChest = new List<Item>();
    }

    public void AddItem(Item item)
    {
        Debug.Log($"[Chest] AddItem - Adding item: {item.itemName}, Stackable: {item.isStackable}, ID: {item.id}");

        if (item.isStackable)
        {
            // Check if the same item already exists in the chest
            Item existingItem = itemsInChest.Find(i => i.id == item.id);
            if (existingItem != null)
            {
                // Update stack count if item exists
                existingItem.stackCount += item.stackCount;
                Debug.Log($"[Chest] AddItem - Updated stack count for item: {item.itemName}, New stack count: {existingItem.stackCount}");
            }
            else
            {
                // Add new item if it doesn't exist

                itemsInChest.Add(item);
                Debug.Log($"[Chest] AddItem - Added new stackable item: {item.itemName}");
            }
        }
        else
        {
            // Add non-stackable item
            itemsInChest.Add(item);
            Debug.Log($"[Chest] AddItem - Added non-stackable item: {item.itemName}");

        }
        item.currentLocation = chestType == ChestType.Equipment ? ItemLocation.EquipmentChest : ItemLocation.ConsumableChest;
        UpdateChestUI();
    }


    public bool RemoveItem(Item item)
    {
        Debug.Log($"[Chest] RemoveItem - Attempting to remove item: {item.itemName}");
        bool removed = itemsInChest.Remove(item);
        if (removed)
        {
            Debug.Log($"[Chest] RemoveItem - Removed item: {item.itemName} from chest");
            item.currentLocation = ItemLocation.PlayerInventory;
            UpdateChestUI();
        }
        return removed;
    }


    public Item FindItemById(int id)
    {
        return itemsInChest.Find(item => item.id == id);
    }

    public void UpdateChestUI()
    {
        if (chestUI != null)
        {
            chestUI.UpdateUI();
        }
    }

    public void ToggleMoveItemMode(bool isOn)
    {
        if (chestUI != null)
        {
            chestUI.moveItemToggle.isOn = isOn;
            // Additional logic if needed when toggling the move item mode
        }
        else
        {
            Debug.LogError("ChestUI is not assigned in Chest");
        }
    }


}