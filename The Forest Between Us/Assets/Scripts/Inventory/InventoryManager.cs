using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public List<InventorySlot> allSlots = new List<InventorySlot>();

    void Start()
    {
        // Nó sẽ tự động chui vào cái Grid và lấy hết 12 ô bỏ vào danh sách
        // Bạn không cần kéo thả bằng tay từng ô nữa! Team cần đưa ra chỉnh sửa thì cũng dễ hơn
        allSlots.AddRange(GetComponentsInChildren<InventorySlot>());
    }

    public void PickUpItem(ItemData newItem, int amount)
    {
        foreach (InventorySlot slot in allSlots)
        {
            if (slot.iconDisplay.enabled == false)
            {
                slot.UpdateSlot(newItem, amount);
                return;
            }
        }
    }
}