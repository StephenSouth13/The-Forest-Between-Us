using UnityEngine;

public class WaterCollector : MonoBehaviour, Interactable
{
    [Header("Water Storage Settings")]
    public string collectorName = "Bể Tích Trữ Nước Suối";
    public int currentWaterUnits = 10;
    public int maxWaterUnits = 20;

    [Header("Direct Drink / Refill Bottle")]
    public float directThirstRestore = 40f;
    public ItemData emptyBottleItem;
    public ItemData fullBottleItem;

    public string GetInteractPrompt()
    {
        if (currentWaterUnits <= 0) return $"{collectorName} (Đã cạn nước - Chờ mưa...)";

        if (InventoryManager.instance != null && emptyBottleItem != null && InventoryManager.instance.HasItem(emptyBottleItem, 1))
        {
            return $"Múc nước đầy {fullBottleItem?.itemName ?? "Bình Nước"} (Phím F)";
        }

        return $"Uống nước từ {collectorName} (+{directThirstRestore} Khát)";
    }

    public void OnInteract()
    {
        if (currentWaterUnits <= 0) return;

        // Ưu tiên múc nước vào Bình Rỗng nếu người chơi có mang theo bình rỗng trong Balo
        if (InventoryManager.instance != null && emptyBottleItem != null && fullBottleItem != null && InventoryManager.instance.HasItem(emptyBottleItem, 1))
        {
            if (InventoryManager.instance.RemoveItem(emptyBottleItem, 1))
            {
                InventoryManager.instance.PickUpItem(fullBottleItem, 1);
                currentWaterUnits--;
                Debug.Log($"🍼 Đã múc nước thành công! Nhận {fullBottleItem.itemName} vào Balo.");
                return;
            }
        }

        // Uống trực tiếp nếu không có bình rỗng
        if (PlayerStatsManager.instance != null)
        {
            PlayerStatsManager.instance.DrinkWater(directThirstRestore);
            currentWaterUnits--;
            Debug.Log($"💧 Đã uống nước trực tiếp từ {collectorName} (+{directThirstRestore} Thirst)");
        }
    }
}
