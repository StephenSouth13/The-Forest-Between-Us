using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [Header("UI References")]
    public Image iconDisplay;       // Kéo Image con vào đây
    public TextMeshProUGUI countText; // Kéo TextMeshPro con vào đây

    [Header("Data")]
    private ItemData currentItem;   // Lưu dữ liệu món đồ đang giữ
    private int currentCount;       // Lưu số lượng hiện tại

    // Hàm cập nhật dữ liệu và hiển thị cho ô rương
    public void UpdateSlot(ItemData item, int amount)
    {
        currentItem = item;
        currentCount = amount;

        if (item != null)
        {
            iconDisplay.sprite = item.icon; 
            iconDisplay.enabled = true;
            
            // Chỉ hiện số lượng nếu món đồ có thể cộng dồn và số lượng > 1
            countText.text = (item.isStackable && amount > 1) ? amount.ToString() : "";
        }
        else
        {
            ClearSlot();
        }
    }

    // Hàm xóa trống ô rương
    public void ClearSlot()
    {
        currentItem = null;
        currentCount = 0;
        iconDisplay.enabled = false;
        iconDisplay.sprite = null;
        countText.text = "";
    }

    // --- Các hàm hỗ trợ cho InventoryManager ---
    public bool IsEmpty() => currentItem == null;
    public ItemData GetItem() => currentItem;
    public int GetCount() => currentCount;
}