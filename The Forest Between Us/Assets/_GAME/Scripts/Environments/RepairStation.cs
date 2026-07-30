using UnityEngine;

public class RepairStation : MonoBehaviour, Interactable
{
    [Header("Station Info")]
    public string stationName = "Bàn Sửa Chữa Trang Bị";

    [Header("🎨 Custom Asset Slots (Kéo Thả Model / Prefab / VFX / SFX Của Bạn Vào Đây)")]
    public GameObject station3DModel;
    public ParticleSystem repairSparksVFX;
    public AudioClip repairAudioSFX;

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
        return $"Sửa chữa trang bị hỏng tại {stationName} (Phím F)";
    }

    public void OnInteract()
    {
        if (InventoryManager.instance == null) return;

        InventoryManager.instance.RefreshSlots();
        bool repairedAny = false;

        foreach (InventorySlot slot in InventoryManager.instance.allSlots)
        {
            if (slot.IsEmpty()) continue;

            ItemData item = slot.GetItem();
            if (item == null) continue;

            // 1. Sửa vật phẩm hỏng hoàn toàn (Biến thể hỏng -> Vật phẩm lành)
            if (item.brokenItemVariant != null)
            {
                ItemData repairMaterial = item.repairIngredient;
                int reqAmount = item.repairIngredientAmount > 0 ? item.repairIngredientAmount : 1;

                if (repairMaterial != null && InventoryManager.instance.HasItem(repairMaterial, reqAmount))
                {
                    InventoryManager.instance.RemoveItem(repairMaterial, reqAmount);
                    ItemData restoredItem = item.brokenItemVariant;
                    InventoryManager.instance.RemoveItemFromSlot(slot, 1);
                    InventoryManager.instance.PickUpItem(restoredItem, 1);

                    repairedAny = true;
                    Debug.Log($"🔧 ⚡ ĐÃ SỬA CHỮA THÀNH CÔNG: {restoredItem.itemName}!");
                    PlayRepairEffects();
                    break;
                }
                else if (repairMaterial != null)
                {
                    Debug.LogWarning($"⚠️ Cần {reqAmount}x {repairMaterial.itemName} trong Balo để sửa {item.itemName}!");
                }
            }
        }

        if (!repairedAny)
        {
            Debug.Log($"🔧 Không tìm thấy trang bị hỏng cần sửa trong Balo.");
        }
    }

    void PlayRepairEffects()
    {
        if (repairSparksVFX != null) repairSparksVFX.Play();
        if (repairAudioSFX != null && audioSource != null) audioSource.PlayOneShot(repairAudioSFX);
    }
}
