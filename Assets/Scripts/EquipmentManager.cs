using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    private PlayerStats playerStats;
    public List<ItemSpriteMapping> itemSpriteMappings = new List<ItemSpriteMapping>();
    public event System.Action OnEquipmentChanged;


    [System.Serializable]
    public class ItemSpriteMapping
    {
        public int itemId; // Unique ID for each item
        public SpriteRenderer targetRenderer; // The SpriteRenderer to change
        public Sprite newSprite; // The new sprite to apply
    }

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component not found on the same GameObject");
        }
    }

    public void UpdateEquipmentStats()
    {
        if (playerStats == null) return;
        // ResetPlayerStatsToBase();
        var highestEquipments = GetHighestLevelEquipments();
        foreach (var equipment in highestEquipments)
        {
            ApplyEquipmentEffect(equipment);
        }
        playerStats.UpdateUI();

        // Trigger the OnEquipmentChanged event
        OnEquipmentChanged?.Invoke();
    }

    //private void ResetPlayerStatsToBase()
    // {
    //Reset stats to base values here
    //playerStats.ResetToBaseStats();
    // }


    private List<Item> GetHighestLevelEquipments()
    {
        Dictionary<ItemType, Item> highestLevelItems = new Dictionary<ItemType, Item>();

        foreach (var item in InventoryManager.Instance.Items)
        {
            if (item.isEquippable)
            {
                if (!highestLevelItems.ContainsKey(item.itemType) || item.maxLv > highestLevelItems[item.itemType].maxLv)
                {
                    highestLevelItems[item.itemType] = item;
                }
            }
        }

        return new List<Item>(highestLevelItems.Values);
    }

    private void ApplyEquipmentEffect(Item equipment)
    {
        switch (equipment.itemType)
        {
            case ItemType.Flippers:
                playerStats.moveSpeed = equipment.effectValue;
                //Debug.Log($"moveSpeed Changed to: {equipment.effectValue}");
                break;
            case ItemType.HarpoonGun:
                playerStats.fireRate = equipment.effectValue;
               // Debug.Log($"FireRate Changed to: {equipment.effectValue}");
                break;
            case ItemType.Mask:
                playerStats.SetViewDistance(equipment.effectValue);
               // Debug.Log($"View Distance Set to: {equipment.effectValue}");
                break;
            case ItemType.PlayerArmor:
                playerStats.maxHealth = equipment.effectValue;
               // Debug.Log($"Max Health Set to: {equipment.effectValue}");
                break;
            case ItemType.Projectile:
                playerStats.damage = equipment.effectValue;
                //Debug.Log($"Damage Set to: {equipment.effectValue}");
                break;
            case ItemType.Tank:
                playerStats.maxOxygen = equipment.effectValue;
               // Debug.Log($"Max Oxygen Set to: {equipment.effectValue}");
                break;
            default:
                switch (equipment.itemType)
                {
                    case ItemType.Flippers:
                        playerStats.moveSpeed = equipment.effectValue;
                       // Debug.Log($"moveSpeed Changed to: {equipment.effectValue}");
                        break;
                        // Add additional nested cases if necessary
                }
                break;
                // Add any additional cases for other equipment types as necessary
        }
    }



    public void UpdatePlayerSpriteBasedOnInventory()
    {
        foreach (Item item in InventoryManager.Instance.Items)
        {
            // Check if the item is in the player's inventory and is equippable
            if (item.currentLocation == ItemLocation.PlayerInventory && item.isEquippable)
            {
                ItemSpriteMapping mapping = itemSpriteMappings.Find(m => m.itemId == item.id);
                if (mapping != null && mapping.targetRenderer != null)
                {
                    mapping.targetRenderer.sprite = mapping.newSprite;
                    Debug.Log($"Sprite updated for item ID: {item.id}");
                }
            }
        }
    }
}