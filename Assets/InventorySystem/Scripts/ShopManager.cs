using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public TMP_Text goldUI;
    public Item[] shopItems; // Array of items for sale in the shop
    public GameObject[] shopPanelsGO;
    public ShopPanel[] shopPanels;
    public Button[] myPurchaseBtns;
    public Button bedButton;
    public Button doorButton;
    public GameObject ShopGameObject;
    public Chest equipmentChest;

    void Start()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            shopPanelsGO[i].SetActive(true);
        }
        goldUI.text = "Gold: " + PlayerDataManager.Instance.playerData.gold.ToString();
        LoadPanels();
        CheckPurchaseable();
    }

    public void AddGold()
    {
        PlayerDataManager.Instance.playerData.gold += 1000;
        goldUI.text = "Gold: " + PlayerDataManager.Instance.playerData.gold.ToString();
        CheckPurchaseable();
    }

    public void CheckPurchaseable()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            if (PlayerDataManager.Instance.playerData.gold >= shopItems[i].baseCost)
                myPurchaseBtns[i].interactable = true;
            else
                myPurchaseBtns[i].interactable = false;
        }
    }

    public void PurchaseItem(int btnNo)
    {
        int itemCost = shopItems[btnNo].baseCost;
        if (PlayerDataManager.Instance.playerData.gold >= itemCost)
        {
            PlayerDataManager.Instance.UseGold(itemCost);
            goldUI.text = "Gold: " + PlayerDataManager.Instance.playerData.gold.ToString();

            Item newItem = CreateItemFromShopItem(shopItems[btnNo]);

            if (newItem.isStackable)
            {
                // Look for the existing item in the inventory
                Item existingItem = InventoryManager.Instance.FindItemById(newItem.id);
                if (existingItem != null)
                {
                    // Increment the stack count
                    existingItem.stackCount++;
                    // Update the inventory UI for the existing item
                    InventoryManager.Instance.UpdateInventoryItemUI(existingItem);
                }
                else
                {
                    // If the item doesn't exist, add it as a new item
                    InventoryManager.Instance.Add(newItem);
                }
            }
            else
            {
                // For non-stackable items, just add a new instance
                InventoryManager.Instance.Add(newItem);
            }
        }
        else
        {
            Debug.Log("Not enough gold to purchase this item.");
        }
    }



    public void LoadPanels()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            shopPanels[i].titleTxt.text = shopItems[i].itemName;
            shopPanels[i].descriptionTxt.text = shopItems[i].description;
            shopPanels[i].costTxt.text = "Gold: " + shopItems[i].baseCost.ToString();
            shopPanels[i].itemImage.sprite = shopItems[i].icon;
        }
    }

    public void SetShopActive(bool isActive)
    {
        bedButton.interactable = !isActive;
        doorButton.interactable = !isActive;
        if (ShopGameObject != null)
        {
            ShopGameObject.SetActive(isActive);
        }
        else
        {
            Debug.LogError("Shop GameObject is not assigned in the inspector");
        }
    }

    public void CloseShop()
    {
        ShopGameObject.SetActive(false); // This deactivates the shop GameObject
    }

    private Item CreateItemFromShopItem(Item shopItem)
    {
        Item newItem = ScriptableObject.CreateInstance<Item>();
        newItem.id = shopItem.id;
        newItem.itemName = shopItem.itemName;
        newItem.icon = shopItem.icon;
        newItem.value = shopItem.value;
        newItem.isEquippable = shopItem.isEquippable;
        newItem.description = shopItem.description;
        newItem.baseCost = shopItem.baseCost;
        newItem.maxLv = shopItem.maxLv;
        newItem.isStackable = shopItem.isStackable; // Ensure isStackable is copied
        newItem.stackCount = shopItem.stackCount; // Initialize stack count

        // Copy additional properties
        newItem.healthValue = shopItem.healthValue;
        newItem.oxygenValue = shopItem.oxygenValue;
        newItem.damageValue = shopItem.damageValue;
        newItem.effectValue = shopItem.effectValue;
        newItem.itemType = shopItem.itemType;

        return newItem;
    }

}