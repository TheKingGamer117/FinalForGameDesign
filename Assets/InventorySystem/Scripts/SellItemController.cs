using UnityEngine;
using UnityEngine.UI; // Needed for UI elements like Button

public class SellItemsController : MonoBehaviour
{
    public Button sellButton; // Reference to the sell button in the UI

    void Start()
    {
        if (sellButton != null)
        {
            sellButton.onClick.AddListener(SellAllItems);
        }
        else
        {
            Debug.LogError("Sell button not assigned in the inspector");
        }
    }

    public void SellAllItems()
    {
        InventoryManager.Instance.SellItems();
        // Any additional code needed after selling items
    }
}
