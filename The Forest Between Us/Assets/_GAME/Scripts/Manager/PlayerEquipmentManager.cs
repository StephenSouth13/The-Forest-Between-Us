using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : MonoBehaviour
{
    public static PlayerEquipmentManager instance;

    [Header("Equipped Items")]
    public ItemData equippedHead;
    public ItemData equippedChest;
    public ItemData equippedLegs;
    public ItemData equippedBoots;
    public ItemData equippedWeapon;

    public event System.Action OnEquipmentChanged;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void EquipItem(ItemData item)
    {
        if (item == null || item.category != ItemType.Equipment) return;

        // Bỏ món đồ hiện tại ra (nếu có)
        UnequipSlot(item.equipSlot);

        // Mặc món đồ mới vào
        switch (item.equipSlot)
        {
            case EquipmentSlot.Head: equippedHead = item; break;
            case EquipmentSlot.Chest: equippedChest = item; break;
            case EquipmentSlot.Legs: equippedLegs = item; break;
            case EquipmentSlot.Boots: equippedBoots = item; break;
            case EquipmentSlot.Weapon: equippedWeapon = item; break;
        }

        Debug.Log($"👕 Đã mặc trang bị: {item.itemName}");
        OnEquipmentChanged?.Invoke();
    }

    public void UnequipSlot(EquipmentSlot slot)
    {
        ItemData oldItem = null;
        switch (slot)
        {
            case EquipmentSlot.Head: oldItem = equippedHead; equippedHead = null; break;
            case EquipmentSlot.Chest: oldItem = equippedChest; equippedChest = null; break;
            case EquipmentSlot.Legs: oldItem = equippedLegs; equippedLegs = null; break;
            case EquipmentSlot.Boots: oldItem = equippedBoots; equippedBoots = null; break;
            case EquipmentSlot.Weapon: oldItem = equippedWeapon; equippedWeapon = null; break;
        }

        if (oldItem != null)
        {
            if (InventoryManager.instance != null)
            {
                // Thêm lại vào balo, nếu đầy thì rớt ra đất
                if (!InventoryManager.instance.PickUpItem(oldItem, 1))
                {
                    // (TODO: Fallback drop item if full, require drop logic from player pos)
                    Debug.LogWarning("Balo đầy, trang bị rơi mất!");
                }
            }
            Debug.Log($"Đã tháo trang bị: {oldItem.itemName}");
        }
    }

    public float GetTotalArmor()
    {
        float armor = 0f;
        if (equippedHead != null) armor += equippedHead.armorValue;
        if (equippedChest != null) armor += equippedChest.armorValue;
        if (equippedLegs != null) armor += equippedLegs.armorValue;
        if (equippedBoots != null) armor += equippedBoots.armorValue;
        return armor;
    }

    public float GetTotalSpeedBoost()
    {
        float speed = 0f;
        if (equippedHead != null) speed += equippedHead.speedBoost;
        if (equippedChest != null) speed += equippedChest.speedBoost;
        if (equippedLegs != null) speed += equippedLegs.speedBoost;
        if (equippedBoots != null) speed += equippedBoots.speedBoost;
        return speed;
    }
}
