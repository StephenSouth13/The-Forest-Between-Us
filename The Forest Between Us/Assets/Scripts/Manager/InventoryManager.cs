using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Dùng để sắp xếp danh sách dễ dàng hơn

public class InventoryManager : MonoBehaviour
{
    // Singleton: Giúp gọi Inventory từ bất cứ đâu (ví dụ: ItemObject gọi khi nhặt đồ)
    public static InventoryManager instance;

    [Header("UI Settings")]
    public List<InventorySlot> allSlots = new List<InventorySlot>();
    public Transform slotContainer; // Kéo Slot_Container vào đây

    private void Awake()
    {
        // Khởi tạo Singleton
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Tự động tìm tất cả các ô InventorySlot là con của Slot_Container
        RefreshSlots();
    }

    // Hàm cập nhật danh sách Slot (hữu ích khi bạn mở rộng balo từ 16 lên 32 ô)
    public void RefreshSlots()
    {
        allSlots.Clear();
        // Lấy tất cả các script InventorySlot trong các object con
        allSlots.AddRange(slotContainer.GetComponentsInChildren<InventorySlot>(true)); 
    }

    // --- LOGIC NHẶT ĐỒ ---
    public void PickUpItem(ItemData newItem, int amount)
    {
        // 1. Kiểm tra nếu vật phẩm có thể cộng dồn (Stackable)
        if (newItem.isStackable)
        {
            foreach (InventorySlot slot in allSlots)
            {
                // Tìm ô đang chứa đúng món đó và chưa đầy stack
                if (!slot.IsEmpty() && slot.GetItem() == newItem && slot.GetCount() < newItem.maxStackSize)
                {
                    slot.UpdateSlot(newItem, slot.GetCount() + amount);
                    return;
                }
            }
        }

        // 2. Nếu không cộng dồn được hoặc là món mới, tìm ô trống đầu tiên
        foreach (InventorySlot slot in allSlots)
        {
            if (slot.IsEmpty())
            {
                slot.UpdateSlot(newItem, amount);
                return;
            }
        }

        Debug.Log("Túi đồ đã đầy, không thể nhặt thêm: " + newItem.itemName);
    }

    // --- LOGIC BỘ LỌC (TAB SYSTEM) ---
    // Gắn hàm này vào các Button trên UI (All=0, Weapon=1, Food=2, Resource=3)
    public void FilterInventory(int categoryIndex)
    {
        ItemType selectedType = (ItemType)categoryIndex;

        foreach (InventorySlot slot in allSlots)
        {
            // Nếu chọn Tab "All" (0)
            if (selectedType == ItemType.All)
            {
                slot.gameObject.SetActive(true);
            }
            // Nếu ô có đồ và đúng loại đang lọc
            else if (!slot.IsEmpty() && slot.GetItem().category == selectedType)
            {
                slot.gameObject.SetActive(true);
            }
            // Các trường hợp còn lại (ô trống hoặc sai loại) thì ẩn đi
            else
            {
                slot.gameObject.SetActive(false);
            }
        }
    }
}