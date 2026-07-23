using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("UI Settings")]
    public List<InventorySlot> allSlots = new List<InventorySlot>();
    public Transform slotContainer;

    [Header("Drop Settings")]
    public GameObject itemPickupPrefab; // Prefab dùng chung khi thả item ra thế giới (cần có sẵn ItemObject)
    public Transform dropPoint;

    public event System.Action<ItemData, int> OnItemUsed;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        RefreshSlots();
    }

    public void RefreshSlots()
    {
        allSlots.Clear();
        if (slotContainer == null) return;

        allSlots.AddRange(slotContainer.GetComponentsInChildren<InventorySlot>(true));
    }

    public void PickUpItem(ItemData newItem, int amount)
    {
        if (newItem == null || amount <= 0) return;

        int remainingAmount = amount;

        if (newItem.isStackable)
        {
            foreach (InventorySlot slot in allSlots)
            {
                if (!slot.IsEmpty() && slot.GetItem() == newItem && slot.GetCount() < newItem.maxStackSize)
                {
                    int spaceLeft = newItem.maxStackSize - slot.GetCount();
                    int amountToAdd = Mathf.Min(remainingAmount, spaceLeft);
                    int nextCount = slot.GetCount() + amountToAdd;
                    slot.UpdateSlot(newItem, nextCount);
                    remainingAmount -= amountToAdd;

                    if (remainingAmount <= 0) return;
                }
            }
        }

        foreach (InventorySlot slot in allSlots)
        {
            if (slot.IsEmpty())
            {
                int slotAmount = newItem.isStackable ? Mathf.Min(remainingAmount, newItem.maxStackSize) : remainingAmount;
                slot.UpdateSlot(newItem, slotAmount);
                remainingAmount -= slotAmount;

                if (remainingAmount <= 0) return;
            }
        }

        Debug.Log($"Inventory is full. Could not pick up {remainingAmount}x {newItem.itemName}.");
    }

    public bool HasItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0) return false;

        int total = 0;
        foreach (InventorySlot slot in allSlots)
        {
            if (!slot.IsEmpty() && slot.GetItem() == item)
            {
                total += slot.GetCount();
                if (total >= amount) return true;
            }
        }

        return false;
    }

    // Trừ dần từ nhiều slot chứa cùng 1 item, dùng cho craft/quest/tiêu hao không quan tâm slot cụ thể
    public bool RemoveItem(ItemData item, int amount)
    {
        if (!HasItem(item, amount)) return false;

        int remaining = amount;
        foreach (InventorySlot slot in allSlots)
        {
            if (remaining <= 0) break;
            if (slot.IsEmpty() || slot.GetItem() != item) continue;

            remaining -= RemoveFromSlot(slot, remaining);
        }

        return true;
    }

    // Trừ trực tiếp trên 1 slot cụ thể, dùng khi thao tác từ UI (click dùng/thả)
    public bool RemoveItemFromSlot(InventorySlot slot, int amount)
    {
        if (slot == null || slot.IsEmpty() || amount <= 0) return false;

        return RemoveFromSlot(slot, amount) > 0;
    }

    int RemoveFromSlot(InventorySlot slot, int amount)
    {
        int amountToRemove = Mathf.Min(amount, slot.GetCount());
        int nextCount = slot.GetCount() - amountToRemove;

        if (nextCount <= 0) slot.ClearSlot();
        else slot.UpdateSlot(slot.GetItem(), nextCount);

        return amountToRemove;
    }

    public void UseItem(InventorySlot slot)
    {
        if (slot == null || slot.IsEmpty()) return;

        ItemData item = slot.GetItem();
        if (RemoveItemFromSlot(slot, 1))
        {
            OnItemUsed?.Invoke(item, 1);
        }
    }

    public void DropItem(InventorySlot slot, int amount)
    {
        if (slot == null || slot.IsEmpty() || amount <= 0) return;

        ItemData item = slot.GetItem();
        int amountToDrop = Mathf.Min(amount, slot.GetCount());

        if (!RemoveItemFromSlot(slot, amountToDrop)) return;

        if (itemPickupPrefab == null) return;

        Vector3 spawnPos = dropPoint != null ? dropPoint.position : transform.position;
        GameObject dropped = Instantiate(itemPickupPrefab, spawnPos, Quaternion.identity);

        if (dropped.TryGetComponent(out ItemObject itemObject))
        {
            itemObject.itemData = item;
            itemObject.amount = amountToDrop;
        }
    }

    public void FilterInventory(int categoryIndex)
    {
        ItemType selectedType = (ItemType)categoryIndex;

        foreach (InventorySlot slot in allSlots)
        {
            if (selectedType == ItemType.All)
            {
                slot.gameObject.SetActive(true);
            }
            else if (!slot.IsEmpty() && slot.GetItem().category == selectedType)
            {
                slot.gameObject.SetActive(true);
            }
            else
            {
                slot.gameObject.SetActive(false);
            }
        }
    }
}
