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
