using UnityEngine;

public class ItemObject : MonoBehaviour, Interactable
{
    [Header("Item Configuration")]
    public ItemData itemData; // Kéo file ScriptableObject (ItemData) tương ứng vào đây
    public int amount = 1;        // Số lượng sẽ nhận được khi nhặt

    public string GetInteractPrompt()
    {
        if (itemData == null) return "Vật Phẩm Khống";
        return $"{itemData.itemName} (x{amount})";
    }

    public void OnInteract()
    {
        if (itemData == null) return;

        if (InventoryManager.instance == null)
        {
            Debug.LogWarning("InventoryManager is missing. Cannot pick up item.");
            return;
        }

        bool success = InventoryManager.instance.PickUpItem(itemData, amount);

        if (success)
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning($"⚠️ Balo đã đầy! Không thể nhặt {itemData.itemName}.");
        }
    }
}
