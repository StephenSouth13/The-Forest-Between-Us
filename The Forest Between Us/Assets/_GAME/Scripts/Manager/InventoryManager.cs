using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("UI Settings")]
    public List<InventorySlot> allSlots = new List<InventorySlot>();
    public Transform slotContainer;

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

        if (newItem.isStackable)
        {
            foreach (InventorySlot slot in allSlots)
            {
                if (!slot.IsEmpty() && slot.GetItem() == newItem && slot.GetCount() < newItem.maxStackSize)
                {
                    int nextCount = Mathf.Min(slot.GetCount() + amount, newItem.maxStackSize);
                    slot.UpdateSlot(newItem, nextCount);
                    return;
                }
            }
        }

        foreach (InventorySlot slot in allSlots)
        {
            if (slot.IsEmpty())
            {
                int slotAmount = newItem.isStackable ? Mathf.Min(amount, newItem.maxStackSize) : amount;
                slot.UpdateSlot(newItem, slotAmount);
                return;
            }
        }

        Debug.Log("Inventory is full. Cannot pick up: " + newItem.itemName);
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
