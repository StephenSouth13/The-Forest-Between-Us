using System.Collections.Generic;
using UnityEngine;

public class FoodSpoilageManager : MonoBehaviour
{
    public static FoodSpoilageManager instance;

    [Header("Spoilage Settings")]
    public float checkIntervalSeconds = 5f; // Kiểm tra mỗi 5 giây

    // Lưu trữ thời điểm nhặt item để đếm ngược thối hỏng
    private Dictionary<InventorySlot, float> itemAcquireTimes = new Dictionary<InventorySlot, float>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        InvokeRepeating(nameof(CheckSpoilage), checkIntervalSeconds, checkIntervalSeconds);
    }

    void CheckSpoilage()
    {
        if (InventoryManager.instance == null) return;

        InventoryManager.instance.RefreshSlots();
        List<InventorySlot> slots = InventoryManager.instance.allSlots;

        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty())
            {
                if (itemAcquireTimes.ContainsKey(slot)) itemAcquireTimes.Remove(slot);
                continue;
            }

            ItemData item = slot.GetItem();
            if (item == null || !item.canSpoil) continue;

            if (!itemAcquireTimes.ContainsKey(slot))
            {
                itemAcquireTimes[slot] = Time.time;
                continue;
            }

            float elapsed = Time.time - itemAcquireTimes[slot];
            if (elapsed >= item.spoilTimeSeconds)
            {
                // Tiêu hủy thực phẩm cũ và đổi thành Đồ Ăn Thối
                ItemData spoiledResult = item.spoiledItemResult;
                int count = slot.GetCount();
                slot.ClearSlot();
                itemAcquireTimes.Remove(slot);

                if (spoiledResult != null)
                {
                    InventoryManager.instance.PickUpItem(spoiledResult, count);
                    Debug.LogWarning($"🤢 {item.itemName} trong Balo đã bị thối rữa thành {spoiledResult.itemName}!");
                }
                else
                {
                    Debug.LogWarning($"🤢 {item.itemName} trong Balo đã bị hỏng và tan biến!");
                }
            }
        }
    }
}
