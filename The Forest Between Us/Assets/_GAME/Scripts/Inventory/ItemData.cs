//D:\VTC_Academy\game3d\The-Forest-Between-Us\The Forest Between Us\Assets\Scripts\Inventory\ItemData.cs
using UnityEngine;

// Định nghĩa các loại vật phẩm để làm bộ lọc
public enum ItemType { All, Weapon, Food, Resource, Consumable, Tool, Equipment }
public enum EquipmentSlot { None, Head, Chest, Legs, Boots, Weapon }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType category; // Nhãn để bộ lọc nhận diện
    public Sprite icon;
    public bool isStackable = true;
    public int maxStackSize = 99;

    [Header("Equipment Stats (Trang Bị)")]
    public EquipmentSlot equipSlot;
    public float armorValue = 0f;      // Giảm sát thương nhận vào
    public float speedBoost = 0f;      // Tăng tốc độ chạy

    [Header("Consumable Effects (Ăn / Uống / Hồi Máu)")]
    public bool isConsumable;
    public bool isRawFood; // Thịt sống / Thực phẩm sống -> Ăn vào bị tác dụng phụ ngộ độc
    public float hungerRestore = 25f;
    public float thirstRestore = 25f;
    public float healthRestore = 15f;
    public float staminaRestore = 10f;

    [Header("Backpack Weight (Trọng Lượng kg)")]
    public float itemWeight = 1.0f; // Cân nặng của 1 vật phẩm

    [Header("Spoilage / Food Decay (Thối Hỏng)")]
    public bool canSpoil;
    public float spoilTimeSeconds = 300f; // Thời gian hư hỏng (giây)
    public ItemData spoiledItemResult;   // Vật phẩm nhận được khi thối (ví dụ: Trái cây thối)

    [Header("Water Container / Bottle (Bình Nước)")]
    public bool isWaterContainer;
    public bool isFullWaterBottle;
    public ItemData emptyBottleVariant;
    public ItemData fullBottleVariant;

    [Header("Farming / Crop Seed (Trồng Trọt)")]
    public bool isSeed;
    public ItemData cropHarvestResult;
    public float cropGrowthTimeSeconds = 45f;

    [Header("Durability & Repair (Độ Bền & Sửa Chữa)")]
    public bool hasDurability;
    public float maxDurability = 100f;
    public ItemData repairIngredient;      // Nguyên liệu để sửa (ví dụ: Đá cuội / Gỗ)
    public int repairIngredientAmount = 1;
    public ItemData brokenItemVariant;     // Biến thể vật phẩm hỏng hoàn toàn (ví dụ: Rìu gãy)

    [Header("Upgrades (Nâng Cấp Trang Bị)")]
    public int itemLevel = 1;
    public ItemData upgradedItemVariant;   // Vật phẩm cấp cao hơn (ví dụ: Rìu Thép Lvl 2)
    public ItemData upgradeMaterial;       // Nguyên liệu cần nâng cấp (ví dụ: Thanh Kim Loại)
    public int upgradeMaterialAmount = 2;

    [Header("Salvage / Dismantle (Tinh Giản / Đập Bỏ Lấy 50% Nguyên Liệu)")]
    public bool canDismantle = true;
    public ItemData salvageReturnItem;     // Nguyên liệu nhận lại khi đập bỏ (ví dụ: Gỗ/Đá)
    public int salvageReturnAmount = 1;    // Số lượng nhận lại (~50% nguyên liệu ban đầu)

    [Header("🎨 Custom Asset Slots (Kéo Thả Model / Prefab / SFX Của Bạn Vào Đây)")]
    public GameObject worldModelPrefab;    // Model 3D thả ra ngoài thế giới
    public GameObject equippedHandPrefab;  // Model 3D khi cầm trên tay nhân vật
    public AudioClip useAudioSFX;          // Âm thanh khi sử dụng item
    public ParticleSystem useEffectVFX;    // Hiệu ứng hạt VFX khi dùng
}