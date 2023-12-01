using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour
{
    Item item;

    public Button RemoveButton;
    public Button MoveButton;

    void Awake()
    {
        RemoveButton = transform.Find("RemoveButton").GetComponent<Button>();
        MoveButton = transform.Find("MoveButton").GetComponent<Button>();

        if (RemoveButton != null)
        {
            RemoveButton.onClick.AddListener(() => RemoveItem(item));
        }

        if (MoveButton != null)
        {
            MoveButton.onClick.AddListener(() => MoveItem());
        }
    }

    public void RemoveItem(Item itemToRemove)
    {
        if (itemToRemove != null)
        {
            InventoryManager.Instance.Remove(itemToRemove);
            Destroy(gameObject);
        }
    }

    private void MoveItem()
    {
        if (item != null)
        {
            bool isItemInPlayerInventory = item.currentLocation == ItemLocation.PlayerInventory;
            InventoryManager.Instance.MoveItem(item, !isItemInPlayerInventory);
        }
    }

    public void AddItem(Item newItem)
    {
        item = newItem;

        // Add a listener to the item button for item selection or other interactions
        Button itemButton = GetComponent<Button>();
        if (itemButton != null)
        {
            itemButton.onClick.RemoveAllListeners(); // Remove existing listeners to avoid duplicates
            itemButton.onClick.AddListener(() => OnItemClick());
        }
    }

    void OnItemClick()
    {
        if (InventoryManager.Instance.moveItemMode)
        {
            bool isItemInPlayerInventory = item.currentLocation == ItemLocation.PlayerInventory;
            InventoryManager.Instance.MoveItem(item, !isItemInPlayerInventory);
        }
        else
        {
            // Other behavior when not in move item mode
        }
    }

    public void UpdateMoveButtonVisibility(bool isVisible)
    {
        MoveButton.gameObject.SetActive(isVisible);
    }

    public void UpdateUI()
    {
        TextMeshProUGUI stackCountText = transform.Find("StackCountText").GetComponent<TextMeshProUGUI>();

        if (item.stackCount > 1)
        {
            stackCountText.text = item.stackCount.ToString();
            stackCountText.gameObject.SetActive(true);
        }
        else
        {
            stackCountText.gameObject.SetActive(false);
        }
    }

    public Item GetItem()
    {
        return item;
    }


}