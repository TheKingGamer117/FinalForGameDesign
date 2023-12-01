using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<Item> Items = new List<Item>();

    public Transform ItemContent;
    public GameObject InventoryItem;

    public Toggle EnableRemove;
    public InventoryItemController[] InventoryItems;
    public Chest activeChest; // Active chest reference
    public bool moveItemMode = false;
    public Toggle moveItemToggle;

    public delegate void OnInventoryChangedDelegate();
    public event OnInventoryChangedDelegate OnInventoryChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Add(Item item)
    {
        if (item.isStackable)
        {
            Item existingItem = Items.Find(i => i.id == item.id);
            if (existingItem != null)
            {
                existingItem.stackCount += 1;
                UpdateInventoryItemUI(existingItem);
            }
            else
            {
                item.stackCount = 1;
                item.name = item.itemName;
                Items.Add(item);
            }
        }
        else
        {
            item.name = item.itemName;
            Items.Add(item);
        }

        item.currentLocation = ItemLocation.PlayerInventory;
        ListItems();
        // Debug.Log($"Adding item: {item.itemName}, Stackable: {item.isStackable}, New Count: {Items.Count}");
        OnInventoryChanged?.Invoke();
    }

    public void Remove(Item item)
    {
        if (item.isStackable && item.stackCount > 1)
        {
            item.stackCount -= 1;
            UpdateInventoryItemUI(item); // Update UI to reflect new count
        }
        else
        {
            Items.Remove(item);
            // Other cleanup as necessary
        }

        ListItems(); // Refresh the inventory list
    }


    public void ListItems()
    {
        // Reuse existing UI elements or create new ones if needed
        while (ItemContent.childCount < Items.Count)
        {
            Instantiate(InventoryItem, ItemContent);
        }

        // Adjust the number of UI elements to match the number of items
        for (int i = 0; i < ItemContent.childCount; i++)
        {
            Transform itemTransform = ItemContent.GetChild(i);
            GameObject itemGameObject = itemTransform.gameObject;

            if (i < Items.Count)
            {
                Item item = Items[i];

                var itemName = itemTransform.Find("ItemName").GetComponent<TextMeshProUGUI>();
                var itemIcon = itemTransform.Find("ItemIcon").GetComponent<Image>();
                var removeButton = itemTransform.Find("RemoveButton").GetComponent<Button>();
                var moveButton = itemTransform.Find("MoveButton").GetComponent<Button>();
                moveButton.gameObject.SetActive(moveItemMode);

                itemName.text = item.itemName;
                itemIcon.sprite = item.icon;

                // Set the remove button active state based on the toggle
                removeButton.gameObject.SetActive(EnableRemove.isOn);

                InventoryItemController controller = itemGameObject.GetComponent<InventoryItemController>();
                controller.AddItem(item);

                // Clear existing listeners and add a new one
                removeButton.onClick.RemoveAllListeners();
                removeButton.onClick.AddListener(() => { controller.RemoveItem(item); });

                itemGameObject.SetActive(true); // Make the item visible
            }
            else
            {
                itemGameObject.SetActive(false); // Hide extra UI elements not needed
            }
        }

        // Update the array of InventoryItemControllers
        SetInventoryItems();
    }


    public void EnableItemRemove()
    {
        if (EnableRemove.isOn)
        {
            foreach (Transform item in ItemContent)
            {
                item.Find("RemoveButton").gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Transform item in ItemContent)
            {
                item.Find("RemoveButton").gameObject.SetActive(false);
            }
        }
    }

    public void SetInventoryItems()
    {
        InventoryItems = ItemContent.GetComponentsInChildren<InventoryItemController>();

        for (int i = 0; i < InventoryItems.Length; i++)
        {
            if (i < Items.Count)
            {
                InventoryItems[i].AddItem(Items[i]);
                InventoryItems[i].gameObject.SetActive(true); // Make sure the item is visible
            }
            else
            {
                // Disable or hide the UI element
                InventoryItems[i].gameObject.SetActive(false);
            }
        }
    }

    public void SellItems()
    {
        int totalValue = 0;

        List<int> deadFishIDs = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
        // Loop through items and sum the value of dead fish and treasure
        for (int i = Items.Count - 1; i >= 0; i--)
        {
            if (deadFishIDs.Contains(Items[i].id) || Items[i].id == 20)
            {
                totalValue += Items[i].value;
                Items.RemoveAt(i);
            }
        }

        // Add gold to PlayerDataManager and trigger the OnGoldChanged event
        PlayerDataManager.Instance.AddGold(totalValue);

        ListItems();
        // Optionally, update inventory UI or other actions...
    }

    public void ToggleMoveItemMode(bool isOn)
    {
        moveItemMode = isOn;
        // Additional feedback logic can be added here
    }

    public void MoveItem(Item item, bool toPlayerInventory)
    {
        Debug.Log($"Attempting to move item: {item.itemName}, Stackable: {item.isStackable}, ToPlayerInventory: {toPlayerInventory}");
        if (item.isStackable)
        {
            // Handling stackable items
            if (toPlayerInventory)
            {
                // Move to player inventory
                MoveStackableItemToInventory(item, ItemLocation.PlayerInventory);
            }
            else
            {
                // Move to chest
                if (activeChest != null)
                {
                    MoveStackableItemToInventory(item, ItemLocation.EquipmentChest);
                }
            }
        }
        else
        {
            // Handling non-stackable items
            if (toPlayerInventory)
            {
                Add(item);
                if (activeChest != null)
                {
                    activeChest.RemoveItem(item);
                }
            }
            else
            {
                if (activeChest != null)
                {
                    activeChest.AddItem(item);
                    Remove(item);
                }
            }
            item.currentLocation = toPlayerInventory ? ItemLocation.PlayerInventory : ItemLocation.EquipmentChest;
        }
        ListItems();
        // Update Chest UI as necessary
        Debug.Log($"Move completed. Item location: {item.currentLocation}");
        if (activeChest != null)
        {
            activeChest.UpdateChestUI();
        }

    }

    public void ToggleItemRemovalMode()
    {
        EnableRemove.isOn = !EnableRemove.isOn;
    }

    private void MoveStackableItemToInventory(Item item, ItemLocation targetLocation)
    {
        List<Item> targetInventory = (targetLocation == ItemLocation.PlayerInventory) ? Items : activeChest.itemsInChest; // Adjust this line according to your Chest implementation
        Item existingItem = targetInventory.Find(i => i.id == item.id);

        if (existingItem != null)
        {
            existingItem.stackCount += item.stackCount;
        }
        else
        {
            targetInventory.Add(item);
        }

        item.currentLocation = targetLocation;

        // Remove item from the source inventory
        if (targetLocation == ItemLocation.PlayerInventory)
        {
            if (activeChest != null)
            {
                activeChest.RemoveItem(item);
            }
        }
        else
        {
            Remove(item);
        }
    }

    public void UpdateInventoryItemUI(Item item)
    {
        foreach (InventoryItemController controller in InventoryItems)
        {
            if (controller.GetItem() != null && controller.GetItem().id == item.id)
            {
                controller.UpdateUI();
                break;
            }
        }
    }

    public Item FindItemById(int id)
    {
        return Items.Find(item => item.id == id);
    }


}