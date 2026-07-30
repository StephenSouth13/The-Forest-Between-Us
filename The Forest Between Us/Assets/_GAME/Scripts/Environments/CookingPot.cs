using UnityEngine;

public class CookingPot : MonoBehaviour, Interactable
{
    [Header("Cooking Pot Settings")]
    public string potName = "Nồi Nấu Ăn Kim Loại";
    public Campfire parentCampfire; // Đống lửa bên dưới

    [Header("Cooking Recipes Assets")]
    public ItemData cleanWaterItem;
    public ItemData rawMeatItem;
    public ItemData herbalItem;
    public ItemData mushroomItem;

    [Header("Cooked & Medical Results")]
    public ItemData heartyStewItem;  // Thịt hầm thảo mộc (+75 Đói, +45 Máu, +30 Thể lực)
    public ItemData detoxTeaItem;    // Trà thảo dược giải độc (+50 Khát, +30 Máu, Giải Độc)
    public ItemData antibioticItem;  // Thuốc Kháng Sinh Tự Chế (Chữa Sốt Rét / Muỗi Đốt)
    public ItemData fluTeaItem;      // Trà Cảm Cúm Gừng (Chữa Cảm Cúm / Cảm Lạnh)

    public string GetInteractPrompt()
    {
        bool fireActive = parentCampfire != null && parentCampfire.isBurning;
        if (!fireActive)
        {
            return $"{potName} (Cần nhóm lửa trại bên dưới để nấu súp & làm thuốc!)";
        }

        return $"Nấu Súp / Dược Phẩm Kháng Sinh với {potName} (Phím F - Phím L xem công thức)";
    }

    public void OnInteract()
    {
        bool fireActive = parentCampfire != null && parentCampfire.isBurning;
        if (!fireActive)
        {
            Debug.LogWarning($"🔥 {potName}: Cần nhóm lửa đống lửa trại trước khi nấu súp!");
            return;
        }

        if (InventoryManager.instance == null) return;

        // 1. Công thức: Thuốc Kháng Sinh Tự Chế (2x Thảo Dược + 1x Nấm Rừng + 1x Nước Suối)
        if (herbalItem != null && mushroomItem != null && cleanWaterItem != null && antibioticItem != null)
        {
            if (InventoryManager.instance.HasItem(herbalItem, 2) &&
                InventoryManager.instance.HasItem(mushroomItem, 1) &&
                InventoryManager.instance.HasItem(cleanWaterItem, 1))
            {
                InventoryManager.instance.RemoveItem(herbalItem, 2);
                InventoryManager.instance.RemoveItem(mushroomItem, 1);
                InventoryManager.instance.RemoveItem(cleanWaterItem, 1);

                InventoryManager.instance.PickUpItem(antibioticItem, 1);
                Debug.Log($"💊 🔥 ĐÃ CHẾ TẠO THÀNH CÔNG: {antibioticItem.itemName}! (Đặc trị Bệnh Sốt Rét do muỗi đốt).");
                return;
            }
        }

        // 2. Công thức: Thịt Hầm Thảo Mộc (1x Thịt Sống + 1x Nước Suối + 1x Thảo Dược)
        if (rawMeatItem != null && cleanWaterItem != null && herbalItem != null && heartyStewItem != null)
        {
            if (InventoryManager.instance.HasItem(rawMeatItem, 1) &&
                InventoryManager.instance.HasItem(cleanWaterItem, 1) &&
                InventoryManager.instance.HasItem(herbalItem, 1))
            {
                InventoryManager.instance.RemoveItem(rawMeatItem, 1);
                InventoryManager.instance.RemoveItem(cleanWaterItem, 1);
                InventoryManager.instance.RemoveItem(herbalItem, 1);

                InventoryManager.instance.PickUpItem(heartyStewItem, 1);
                Debug.Log($"🍲 🔥 ĐÃ HẦM THÀNH CÔNG: {heartyStewItem.itemName}! (+75 Đói, +45 Máu, +30 Thể Lực)");
                return;
            }
        }

        // 3. Công thức: Trà Thảo Dược Giải Độc (1x Nước Suối + 2x Thảo Dược)
        if (cleanWaterItem != null && herbalItem != null && detoxTeaItem != null)
        {
            if (InventoryManager.instance.HasItem(cleanWaterItem, 1) &&
                InventoryManager.instance.HasItem(herbalItem, 2))
            {
                InventoryManager.instance.RemoveItem(cleanWaterItem, 1);
                InventoryManager.instance.RemoveItem(herbalItem, 2);

                InventoryManager.instance.PickUpItem(detoxTeaItem, 1);
                Debug.Log($"🍵 🔥 ĐÃ NẤU THÀNH CÔNG: {detoxTeaItem.itemName}! (+50 Khát, +30 Máu, Giải Cảm/Giải Độc)");
                return;
            }
        }

        Debug.Log($"📖 Chưa đủ nguyên liệu làm thuốc/nấu súp! Hãy bấm phím L mở Thư Viện để xem nguyên liệu công thức.");
    }
}
