using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("🎒 BẬT GỢI Ý INSPETOR: GIAO DIỆN & TẢI TRỌNG BALO")]
    [Tooltip("Danh sách toàn bộ các ô Slot chứa đồ trong Balo (Tự động nạp khi bấm nút Setup).")]
    public List<InventorySlot> allSlots = new List<InventorySlot>();

    [Tooltip("Kéo Transform của Panel chứa các ô Slot (Grid) vào đây.")]
    public Transform slotContainer;

    [Header("📦 VẬT PHẨM VỨT RA ĐẤT (DROP ITEM)")]
    [Tooltip("Prefab hiển thị vật phẩm rớt ra đất khi vứt đồ khỏi Balo (Có chứa script ItemObject).")]
    public GameObject itemPickupPrefab;

    [Tooltip("Vị trí ngực/tay nhân vật để rớt đồ ra trước mặt khi vứt.")]
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

    [Header("⚖️ CẤU HÌNH SỨC CHỨA BALO (MAX WEIGHT)")]
    [Tooltip("Sức chứa trọng lượng tối đa của Balo (kg). Vượt quá 50% bắt đầu giảm tốc độ chạy, vượt quá 30kg sẽ bị cấm nhặt đồ.")]
    public float maxWeightCapacity = 30.0f;

    public float GetTotalWeight()
    {
        RefreshSlots();
        float total = 0f;
        foreach (InventorySlot slot in allSlots)
        {
            if (!slot.IsEmpty() && slot.GetItem() != null)
            {
                total += slot.GetItem().itemWeight * slot.GetCount();
            }
        }
        return total;
    }

    public bool IsInventoryFull()
    {
        RefreshSlots();
        foreach (InventorySlot slot in allSlots)
        {
            if (slot.IsEmpty()) return false;
        }
        return true;
    }

    public bool PickUpItem(ItemData newItem, int amount)
    {
        if (newItem == null || amount <= 0) return false;

        RefreshSlots();

        // 1. Kiểm tra Giới Hạn Trọng Lượng Balo (Weight Capacity Limit)
        float currentWeight = GetTotalWeight();
        float addedWeight = newItem.itemWeight * amount;
        if (currentWeight + addedWeight > maxWeightCapacity)
        {
            string msg = $"⚠️ QUÁ TẢI TRỌNG LƯỢNG BALO! ({currentWeight + addedWeight:F1} / {maxWeightCapacity:F1} kg). Không thể nhặt thêm {newItem.itemName}.";
            Debug.LogWarning(msg);
            if (RadioDialogueUIController.instance != null)
            {
                RadioDialogueUIController.instance.ShowSubtitle("🎒 BALO BỊ QUÁ TẢI TRỌNG LƯỢNG", msg, 3.5f);
            }
            return false;
        }

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

                    if (remainingAmount <= 0) return true;
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

                if (remainingAmount <= 0) return true;
            }
        }

        string fullMsg = $"⚠️ BẢNG Ô BALO ĐÃ ĐẦY! Không thể chứa thêm {remainingAmount}x {newItem.itemName}.";
        Debug.LogWarning(fullMsg);
        if (RadioDialogueUIController.instance != null)
        {
            RadioDialogueUIController.instance.ShowSubtitle("🎒 BALO ĐÃ ĐẦY Ô CHỨA", fullMsg, 3.5f);
        }
        return false;
    }

    public bool HasItem(ItemData item, int amount)
    {
        return GetItemCount(item) >= amount;
    }

    public int GetItemCount(ItemData item)
    {
        if (item == null) return 0;
        int total = 0;
        foreach (InventorySlot slot in allSlots)
        {
            if (!slot.IsEmpty() && slot.GetItem() == item)
            {
                total += slot.GetCount();
            }
        }
        return total;
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

        // Xử lý Trang Bị (Equipment)
        if (item.category == ItemType.Equipment)
        {
            if (PlayerEquipmentManager.instance != null)
            {
                PlayerEquipmentManager.instance.EquipItem(item);
                RemoveItemFromSlot(slot, 1);
                OnItemUsed?.Invoke(item, 1);
            }
            return;
        }

        // Xử lý tiêu thụ (Ăn trái cây, uống nước, dùng thuốc)
        if (item.isConsumable && PlayerStatsManager.instance != null)
        {
            if (item.isRawFood)
            {
                PlayerStatsManager.instance.EatFood(item.hungerRestore > 0 ? item.hungerRestore : 15f);
                PlayerStatsManager.instance.TakeDamage(25f);
                PlayerStatsManager.instance.RestSleep(-20f);
                Debug.LogWarning($"🤢 ⚠️ ĐÃ ĂN {item.itemName.ToUpper()} SỐNG! Bị ngộ độc thực phẩm (-25 Máu / -20 Thể Lực)!");
            }
            else
            {
                if (item.hungerRestore > 0) PlayerStatsManager.instance.EatFood(item.hungerRestore);
                if (item.thirstRestore > 0) PlayerStatsManager.instance.DrinkWater(item.thirstRestore);
                if (item.healthRestore > 0) PlayerStatsManager.instance.Heal(item.healthRestore);
                if (item.staminaRestore > 0) PlayerStatsManager.instance.RestSleep(item.staminaRestore);
                Debug.Log($"🍎 Đã dùng {item.itemName}: +{item.hungerRestore} Đói / +{item.thirstRestore} Khát");
            }
        }

        // Xử lý Thuốc Kháng Sinh, Trà Cảm Cúm & Thuốc Chống Muỗi
        if (item.itemName.Contains("Kháng Sinh") || item.itemName.Contains("Antibiotic"))
        {
            PlayerDiseaseManager.instance?.CureMosquitoDisease();
        }
        if (item.itemName.Contains("Cảm Cúm") || item.itemName.Contains("Flu") || item.itemName.Contains("Trà Dược"))
        {
            PlayerDiseaseManager.instance?.CureFlu();
        }
        if (item.itemName.Contains("Chống Muỗi") || item.itemName.Contains("Repellent"))
        {
            PlayerDiseaseManager.instance?.ApplyRepellent(300f);
        }

        ItemData emptyVariant = item.emptyBottleVariant;

        if (RemoveItemFromSlot(slot, 1))
        {
            OnItemUsed?.Invoke(item, 1);

            // Uống bình nước đầy -> Trả lại vỏ bình nước rỗng vào Balo
            if (item.isFullWaterBottle && emptyVariant != null)
            {
                PickUpItem(emptyVariant, 1);
                Debug.Log($"🍼 Đã trả vỏ {emptyVariant.itemName} vào Balo!");
            }
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

    // 🧹 TỰ ĐỘNG XẮP XẾP & DỒN SLOT BALO (AUTO-ARRANGE & STACK)
    public void AutoSortAndArrangeInventory()
    {
        RefreshSlots();

        // 1. Gom tất cả vật phẩm hiện có ra danh sách tạm
        List<KeyValuePair<ItemData, int>> itemsList = new List<KeyValuePair<ItemData, int>>();

        foreach (InventorySlot slot in allSlots)
        {
            if (!slot.IsEmpty() && slot.GetItem() != null && slot.GetCount() > 0)
            {
                itemsList.Add(new KeyValuePair<ItemData, int>(slot.GetItem(), slot.GetCount()));
                slot.ClearSlot();
            }
        }

        // 2. Sắp xếp danh sách theo Loại (Category) -> Tên (Name)
        itemsList.Sort((pair1, pair2) =>
        {
            int catCompare = pair1.Key.category.CompareTo(pair2.Key.category);
            if (catCompare != 0) return catCompare;
            return pair1.Key.itemName.CompareTo(pair2.Key.itemName);
        });

        // 3. Nạp lại vào Balo và tự động gộp dồn Stack tối đa
        foreach (var pair in itemsList)
        {
            PickUpItem(pair.Key, pair.Value);
        }

        Debug.Log("<b>[InventoryManager]</b> 🧹 Đã tự động dồn ô & sắp xếp Balo theo Loại & Tên!");
    }

    public void ClearInventory()
    {
        foreach (InventorySlot slot in allSlots)
        {
            if (!slot.IsEmpty())
            {
                slot.ClearSlot();
            }
        }
    }
}
