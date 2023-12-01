using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestUI : MonoBehaviour
{
    public Chest chest;
    public GameObject itemSlotPrefab;
    public Transform itemSlotContainer;
    public Toggle moveItemToggle;

    void Start()
    {
        if (chest != null)
        {
            UpdateUI(); // Update the UI to reflect the current items in the chest
        }
        else
        {
            Debug.LogError("Chest reference not set in ChestUI");
        }
    }

    public void UpdateUI()
    {
        // Clear existing UI slots
        foreach (Transform child in itemSlotContainer)
        {
            Destroy(child.gameObject);
        }

        // Create a new UI slot for each item in the chest
        foreach (Item item in chest.itemsInChest)
        {
            GameObject slot = Instantiate(itemSlotPrefab, itemSlotContainer);

            // Assuming your prefab has children named "ItemName" and "ItemIcon" that are 
            // TextMeshProUGUI and Image components respectively
            TextMeshProUGUI itemNameText = slot.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            Image itemIconImage = slot.transform.Find("ItemIcon").GetComponent<Image>();
            TextMeshProUGUI stackCountText = slot.transform.Find("StackCountText").GetComponent<TextMeshProUGUI>();

            // Set the item's name and icon
            itemNameText.text = item.itemName;
            itemIconImage.sprite = item.icon;

            // Set the stack count if greater than 1, otherwise hide the stack count text
            if (item.stackCount > 1)
            {
                stackCountText.text = item.stackCount.ToString();
                stackCountText.gameObject.SetActive(true);
            }
            else
            {
                stackCountText.gameObject.SetActive(false);
            }

            // Add listener to itemButton
            Button itemButton = slot.GetComponent<Button>();
            if (itemButton != null)
            {
                itemButton.onClick.AddListener(() => OnItemSlotClicked(item));
            }
        }
    }

    private void OnItemSlotClicked(Item item)
    {
        if (moveItemToggle.isOn)
        {
            // Logic to move the item to the player's inventory
            InventoryManager.Instance.MoveItem(item, true);
        }
        else
        {
            // Other interactions when not in move item mode
        }
    }

    // Add methods to handle interaction (e.g., opening the chest, moving items)
}