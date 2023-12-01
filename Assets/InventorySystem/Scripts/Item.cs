using UnityEngine;

public enum ItemLocation
{
    PlayerInventory,
    EquipmentChest,
    ConsumableChest,
    // Other locations as needed
}

public enum ItemType
{
    HealthStim,
    OxygenStim,
    WaterGrenade,
    Equipment,
    Fish,
    Flippers,
    HarpoonGun,
    Mask,
    PlayerArmor,
    Projectile,
    Tank
}


[CreateAssetMenu(fileName = "New Item", menuName = "Item/Create New Item")]
public class Item : ScriptableObject
{
    public int id;
    public string itemName;
    public string description; // Added from ShopItemSO
    public Sprite icon;
    public int value;
    public int baseCost; // Base cost for purchasing
    public int maxLv; // Maximum level, relevant for shop items
    public bool isEquippable; // To check if the item can be equipped
    public bool isStackable;
    public int stackCount = 1;
    public ItemLocation currentLocation = ItemLocation.PlayerInventory; // Default location
    public ItemType itemType;
    public int healthValue;
    public int oxygenValue;
    public int damageValue;
    public float effectValue;

    // Additional fields or methods as needed
}