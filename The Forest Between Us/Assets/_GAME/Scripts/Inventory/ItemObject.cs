//D:\VTC_Academy\game3d\The-Forest-Between-Us\The Forest Between Us\Assets\Scripts\Inventory\Interactable.cs
using UnityEngine;

public class ItemObject : MonoBehaviour, Interactable
{
    [Header("Item Configuration")]
    public ItemData itemData; // Kéo file ScriptableObject (ItemData) tương ứng vào đây
    public int amount = 1;        // Số lượng sẽ nhận được khi nhặt

    // Trả về tên
    public string GetInteractPrompt()
    {
        if (itemData == null) return "Error Item";
        return itemData.itemName;
    }
    public void OnInteract()
    {
        if (itemData == null) return;

        // Gọi Singleton của InventoryManager để thêm đồ vào túi
        // Đảm bảo bạn đã kéo Slot_Container vào InventoryManager trong Inspector
        if (InventoryManager.instance == null)
        {
            Debug.LogWarning("InventoryManager is missing. Cannot pick up item.");
            return;
        }

        InventoryManager.instance.PickUpItem(itemData, amount);

        // Sau khi nhặt xong, xóa vật thể khỏi thế giới game
        Destroy(gameObject);
    }
}
