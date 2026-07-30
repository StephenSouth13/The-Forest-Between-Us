using UnityEngine;

public class UpgradeStation : MonoBehaviour, Interactable
{
    [Header("Station Info")]
    public string stationName = "Bàn Nâng Cấp Trang Bị";

    [Header("🎨 Custom Asset Slots (Kéo Thả Model / Prefab / VFX / SFX Của Bạn Vào Đây)")]
    public GameObject station3DModel;
    public ParticleSystem upgradeAnvilVFX;
    public AudioClip upgradeAudioSFX;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    public string GetInteractPrompt()
    {
        return $"Nâng cấp trang bị lên cấp cao hơn tại {stationName} (Phím F)";
    }

    public void OnInteract()
    {
        if (InventoryManager.instance == null) return;

        InventoryManager.instance.RefreshSlots();
        bool upgradedAny = false;

        foreach (InventorySlot slot in InventoryManager.instance.allSlots)
        {
            if (slot.IsEmpty()) continue;

            ItemData item = slot.GetItem();
            if (item == null || item.upgradedItemVariant == null) continue;

            ItemData nextLevelItem = item.upgradedItemVariant;
            ItemData matRequired = item.upgradeMaterial;
            int matAmount = item.upgradeMaterialAmount > 0 ? item.upgradeMaterialAmount : 2;

            if (matRequired != null && InventoryManager.instance.HasItem(matRequired, matAmount))
            {
                InventoryManager.instance.RemoveItem(matRequired, matAmount);
                InventoryManager.instance.RemoveItemFromSlot(slot, 1);
                InventoryManager.instance.PickUpItem(nextLevelItem, 1);

                upgradedAny = true;
                Debug.Log($"⭐ ⚡ ĐÃ NÂNG CẤP THÀNH CÔNG: {item.itemName} ➔ {nextLevelItem.itemName} (Cấp {nextLevelItem.itemLevel})!");
                PlayUpgradeEffects();
                break;
            }
            else if (matRequired != null)
            {
                Debug.LogWarning($"⚠️ Cần {matAmount}x {matRequired.itemName} trong Balo để nâng cấp {item.itemName} lên {nextLevelItem.itemName}!");
            }
        }

        if (!upgradedAny)
        {
            Debug.Log($"⭐ Không tìm thấy trang bị đủ điều kiện nâng cấp trong Balo.");
        }
    }

    void PlayUpgradeEffects()
    {
        if (upgradeAnvilVFX != null) upgradeAnvilVFX.Play();
        if (upgradeAudioSFX != null && audioSource != null) audioSource.PlayOneShot(upgradeAudioSFX);
    }
}
