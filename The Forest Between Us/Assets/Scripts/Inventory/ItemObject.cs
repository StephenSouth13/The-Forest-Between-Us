using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    [Header("Item Configuration")]
    public ItemData itemData; // Kéo file ScriptableObject (ItemData) tương ứng vào đây
    public int amount = 1;    // Số lượng sẽ nhận được khi nhặt

    // Trả về tên món đồ để hiện lên màn hình thông báo (Prompt Text)
    public string GetInteractPrompt()
    {
        if (itemData == null) return "Vật phẩm lỗi";
        return itemData.itemName;
    }

    // Hàm này được gọi từ InteractionManager khi người chơi nhấn phím [F]
    public void OnInteract()
    {
        if (itemData == null) return;

        // Gọi Singleton của InventoryManager để thêm đồ vào túi
        // Đảm bảo bạn đã kéo Slot_Container vào InventoryManager trong Inspector
        InventoryManager.instance.PickUpItem(itemData, amount);

        // Sau khi nhặt xong, xóa vật thể khỏi thế giới game
        Destroy(gameObject);
    }
}