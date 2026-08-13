using System.Collections;
using UnityEngine;

public enum ResourceType
{
    Tree,        // Chặt gỗ
    BerryBush,   // Hái trái cây
    Rock,        // Khai thác đá
    WaterSource  // Uống nước trực tiếp
}

public class ResourceNode : MonoBehaviour, Interactable
{
    [Header("Resource Type & Name")]
    public ResourceType resourceType = ResourceType.Tree;
    public string resourceName = "Cây Gỗ Rừng";

    [Header("Harvest Settings")]
    public int maxHits = 3;
    public int currentHits = 3;
    public string requiredToolType = "Axe"; // "Axe", "Pickaxe", hoặc "None"
    public bool allowHandHarvest = true;

    [Header("Drop Item Settings")]
    public ItemData dropItemData;
    public int dropAmountMin = 1;
    public int dropAmountMax = 3;
    public bool directToInventory = true;
    public GameObject dropItemPrefab; // Nếu rớt ra đất

    [Header("Direct Consumption (Water Source / Direct Food)")]
    public float directThirstRestore = 30f;
    public float directHungerRestore = 15f;

    [Header("Respawn Settings (Cây Tự Mọc Lại)")]
    public bool autoRespawn = true;
    public float respawnTimeSeconds = 60f; // Số giây mọc lại
    public ParticleSystem harvestEffect;
    public ParticleSystem respawnEffect;

    private bool isDepleted = false;
    private MeshRenderer[] meshRenderers;
    private Collider[] nodeColliders;
    private Vector3 originalScale;

    void Awake()
    {
        currentHits = maxHits;
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        nodeColliders = GetComponentsInChildren<Collider>();
        originalScale = transform.localScale;
    }

    public string GetInteractPrompt()
    {
        if (isDepleted) return $"{resourceName} (Đang chờ mọc lại...)";

        switch (resourceType)
        {
            case ResourceType.Tree:
                return $"Chặt {resourceName} (Cần {currentHits} lần chặt)";
            case ResourceType.BerryBush:
                return $"Hái {resourceName}";
            case ResourceType.Rock:
                return $"Đập {resourceName}";
            case ResourceType.WaterSource:
                return $"Uống {resourceName} (Phím F)";
            default:
                return $"Thu hoạch {resourceName}";
        }
    }

    public void OnInteract()
    {
        if (isDepleted) return;

        // Xử lý riêng cho Nguồn Nước
        if (resourceType == ResourceType.WaterSource)
        {
            DrinkWaterDirectly();
            return;
        }

        // Quy tắc Bảo Vệ Môi Trường: Giới hạn chặt cây mỗi ngày
        if (resourceType == ResourceType.Tree && DayManager.instance != null)
        {
            if (!DayManager.instance.CanChopTree())
            {
                string msg = $"🌱 HÔM NAY BẠN ĐÃ CHẶT ĐỦ {DayManager.instance.maxTreesAllowedPerDay} CÂY!\nVì quy tắc bảo vệ môi trường, bạn dừng chặt cây và chờ sang ngày mới.";
                if (RadioDialogueUIController.instance != null)
                {
                    RadioDialogueUIController.instance.ShowSubtitle("🌱 QUY TẮC BẢO VỆ MÔI TRƯỜNG", msg, 5f);
                }
                Debug.LogWarning(msg);
                return;
            }
        }

        // Chặt / Hái / Đập
        currentHits--;

        if (harvestEffect != null) harvestEffect.Play();

        if (currentHits <= 0)
        {
            HarvestComplete();
        }
        else
        {
            Debug.Log($"🪓 Đã chặt/thu hoạch {resourceName} ({currentHits}/{maxHits} hits)");
        }
    }

    void DrinkWaterDirectly()
    {
        if (PlayerStatsManager.instance != null)
        {
            PlayerStatsManager.instance.DrinkWater(directThirstRestore);
            Debug.Log($"💧 Đã uống nước trực tiếp từ {resourceName} (+{directThirstRestore} Thirst)");
        }

        if (autoRespawn)
        {
            StartCoroutine(CoRespawnRoutine());
        }
    }

    void HarvestComplete()
    {
        // Ghi nhận chặt cây nếu thuộc loại Cây
        if (resourceType == ResourceType.Tree && DayManager.instance != null)
        {
            DayManager.instance.RegisterTreeChopped();
        }

        // Ghi nhận chỉ số sinh tồn cho bảng thống kê Kết game
        if (EndingManager.instance != null)
        {
            if (resourceType == ResourceType.Tree) EndingManager.instance.RecordTreeChopped();
            else if (resourceType == ResourceType.Rock) EndingManager.instance.RecordRockMined();
        }

        int amountToDrop = Random.Range(dropAmountMin, dropAmountMax + 1);

        if (dropItemData != null && InventoryManager.instance != null)
        {
            if (directToInventory)
            {
                bool success = InventoryManager.instance.PickUpItem(dropItemData, amountToDrop);
                if (success)
                {
                    Debug.Log($"🎒 Đã thu nhặt {amountToDrop}x {dropItemData.itemName} vào Balo!");
                }
                else
                {
                    // Nếu Balo đầy -> Tự rớt ra đất
                    SpawnItemOnGround(amountToDrop);
                }
            }
            else
            {
                SpawnItemOnGround(amountToDrop);
            }
        }

        // Chuyển sang trạng thái kiệt tài nguyên & bắt đầu mọc lại
        SetNodeState(false);

        if (autoRespawn)
        {
            StartCoroutine(CoRespawnRoutine());
        }
    }

    void SpawnItemOnGround(int amount)
    {
        if (dropItemData == null) return;

        Vector3 spawnPos = transform.position + Vector3.up * 0.5f + Random.insideUnitSphere * 0.5f;
        spawnPos.y = Mathf.Max(spawnPos.y, transform.position.y);

        GameObject dropObj;
        if (dropItemPrefab != null)
        {
            dropObj = Instantiate(dropItemPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            // Tạo 3D Sphere đơn giản đại diện cho item rớt
            dropObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            dropObj.transform.position = spawnPos;
            dropObj.transform.localScale = Vector3.one * 0.3f;
        }

        ItemObject itemComp = dropObj.GetComponent<ItemObject>();
        if (itemComp == null) itemComp = dropObj.AddComponent<ItemObject>();

        itemComp.itemData = dropItemData;
        itemComp.amount = amount;
    }

    IEnumerator CoRespawnRoutine()
    {
        isDepleted = true;
        yield return new WaitForSeconds(respawnTimeSeconds);

        // Mọc lại
        currentHits = maxHits;
        SetNodeState(true);
        isDepleted = false;

        if (respawnEffect != null) respawnEffect.Play();

        Debug.Log($"🌱 {resourceName} đã tự động mọc lại!");
    }

    void SetNodeState(bool active)
    {
        foreach (var mr in meshRenderers)
        {
            if (mr != null) mr.enabled = active;
        }
        foreach (var col in nodeColliders)
        {
            if (col != null) col.enabled = active;
        }
    }
}
