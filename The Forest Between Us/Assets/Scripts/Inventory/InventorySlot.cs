using UnityEngine;
using UnityEngine.UI;
using TMPro; // Nếu bạn dùng TextMeshPro cho đẹp

public class InventorySlot : MonoBehaviour
{
    public Image iconDisplay;       // Kéo cái Image con (nơi hiện ảnh vật phẩm) vào đây
    public TextMeshProUGUI countText; // Kéo cái chữ hiện số lượng vào đây

    // Hàm này để "vẽ" vật phẩm lên ô rương
    public void UpdateSlot(ItemData item, int amount)
    {
        if (item != null)
        {
            iconDisplay.sprite = item.icon; // Đổi ảnh ô rương thành ảnh vật phẩm
            iconDisplay.enabled = true;
            countText.text = (amount > 1) ? amount.ToString() : ""; // Hiện số nếu > 1
        }
        else
        {
            iconDisplay.enabled = false; // Ô trống thì ẩn ảnh đi
            countText.text = "";
        }
    }
}