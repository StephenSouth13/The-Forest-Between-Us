using UnityEngine;

public class SalvageStation : MonoBehaviour, Interactable
{
    [Header("Station Info")]
    public string stationName = "Bàn Tinh Giản & Tháo Đập Trang Bị";

    [Header("🎨 Custom Asset Slots (Kéo Thả Model / Prefab / VFX / SFX Của Bạn Vào Đây)")]
    public GameObject station3DModel;
    public ParticleSystem dismantleBreakVFX;
    public AudioClip dismantleAudioSFX;

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
        return $"Đập bỏ trang bị lấy lại 50% nguyên liệu tại {stationName} (Phím F)";
    }

    public void OnInteract()
    {
        if (InventoryManager.instance == null) return;

        InventoryManager.instance.RefreshSlots();
        bool salvagedAny = false;

        foreach (InventorySlot slot in InventoryManager.instance.allSlots)
        {
            if (slot.IsEmpty()) continue;

            ItemData item = slot.GetItem();
            if (item == null || !item.canDismantle || item.salvageReturnItem == null) continue;

            ItemData returnMaterial = item.salvageReturnItem;
            int returnAmount = item.salvageReturnAmount > 0 ? item.salvageReturnAmount : 1;

            if (InventoryManager.instance.RemoveItemFromSlot(slot, 1))
            {
                InventoryManager.instance.PickUpItem(returnMaterial, returnAmount);
                salvagedAny = true;

                Debug.Log($"🔨 ⚡ ĐÃ ĐẬP BỎ THÀNH CÔNG: {item.itemName}! Thu lại {returnAmount}x {returnMaterial.itemName} (~50% nguyên liệu).");
                PlayDismantleEffects();
                break;
            }
        }

        if (!salvagedAny)
        {
            Debug.Log($"🔨 Không tìm thấy trang bị có thể đập bỏ trong Balo.");
        }
    }

    void PlayDismantleEffects()
    {
        if (dismantleBreakVFX != null) dismantleBreakVFX.Play();
        if (dismantleAudioSFX != null && audioSource != null) audioSource.PlayOneShot(dismantleAudioSFX);
    }
}
