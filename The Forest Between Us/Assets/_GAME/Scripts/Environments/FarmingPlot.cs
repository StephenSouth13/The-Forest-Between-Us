using System.Collections;
using UnityEngine;

public enum FarmState
{
    Empty,       // Ô đất trống
    Planted,     // Đã gieo hạt (chờ tưới nước)
    Growing,     // Đang lớn
    Harvestable  // Đã chín - Sẵn sàng thu hoạch
}

public class FarmingPlot : MonoBehaviour, Interactable
{
    [Header("Farm Settings")]
    public string plotName = "Ô Đất Trồng Trọt";
    public FarmState currentState = FarmState.Empty;

    [Header("Item Config")]
    public ItemData seedItem;         // Hạt giống để gieo
    public ItemData harvestResultItem;// Nông sản thu hoạch được
    public int harvestAmountMin = 2;
    public int harvestAmountMax = 4;
    public float growthDurationSeconds = 45f; // Thời gian lớn

    [Header("Visual Stages (Visual Prefabs / Models)")]
    public GameObject sproutModel;   // Mô hình mầm cây
    public GameObject ripeCropModel;  // Mô hình cây chín quả

    private float growthTimer = 0f;
    private bool isWatered = false;

    void Start()
    {
        UpdateVisuals();
    }

    void Update()
    {
        if (currentState == FarmState.Growing && isWatered)
        {
            growthTimer += Time.deltaTime;
            if (growthTimer >= growthDurationSeconds)
            {
                currentState = FarmState.Harvestable;
                UpdateVisuals();
                Debug.Log($"🌾 {plotName}: Nông sản đã chín! Sẵn sàng thu hoạch.");
            }
        }
    }

    public string GetInteractPrompt()
    {
        switch (currentState)
        {
            case FarmState.Empty:
                string seedName = seedItem != null ? seedItem.itemName : "Hạt Giống";
                return $"Gieo {seedName} (Phím F)";
            case FarmState.Planted:
                return "Tưới nước cho ô đất (Phím F)";
            case FarmState.Growing:
                int percent = Mathf.FloorToInt((growthTimer / growthDurationSeconds) * 100f);
                return $"Cây đang lớn... ({percent}%)";
            case FarmState.Harvestable:
                string cropName = harvestResultItem != null ? harvestResultItem.itemName : "Nông Sản";
                return $"Thu hoạch {cropName} (Phím F)";
            default:
                return plotName;
        }
    }

    public void OnInteract()
    {
        switch (currentState)
        {
            case FarmState.Empty:
                TryPlantSeed();
                break;
            case FarmState.Planted:
                WaterPlot();
                break;
            case FarmState.Growing:
                Debug.Log($"🌱 Cây đang phát triển, hãy chờ thêm chút nữa! ({Mathf.FloorToInt((growthTimer / growthDurationSeconds) * 100f)}%)");
                break;
            case FarmState.Harvestable:
                HarvestCrop();
                break;
        }
    }

    void TryPlantSeed()
    {
        if (seedItem == null)
        {
            Debug.LogWarning("Chưa gán SeedItem cho Ô đất!");
            return;
        }

        if (InventoryManager.instance != null && InventoryManager.instance.HasItem(seedItem, 1))
        {
            if (InventoryManager.instance.RemoveItem(seedItem, 1))
            {
                currentState = FarmState.Planted;
                isWatered = false;
                growthTimer = 0f;
                UpdateVisuals();
                Debug.Log($"🌱 Đã gieo hạt {seedItem.itemName} vào {plotName}. Hãy tưới nước!");
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ Bạn không có {seedItem.itemName} trong Balo để gieo!");
        }
    }

    void WaterPlot()
    {
        isWatered = true;
        currentState = FarmState.Growing;
        UpdateVisuals();
        Debug.Log($"💧 Đã tưới nước cho {plotName}! Cây bắt đầu lớn...");
    }

    void HarvestCrop()
    {
        if (harvestResultItem == null) return;

        int amount = Random.Range(harvestAmountMin, harvestAmountMax + 1);
        if (InventoryManager.instance != null)
        {
            bool success = InventoryManager.instance.PickUpItem(harvestResultItem, amount);
            if (success)
            {
                Debug.Log($"🧺 Đã thu hoạch thành công {amount}x {harvestResultItem.itemName}!");
                currentState = FarmState.Empty;
                isWatered = false;
                growthTimer = 0f;
                UpdateVisuals();
            }
            else
            {
                Debug.LogWarning("⚠️ Balo đã đầy, không thể thu hoạch thêm!");
            }
        }
    }

    void UpdateVisuals()
    {
        if (sproutModel != null) sproutModel.SetActive(currentState == FarmState.Growing || currentState == FarmState.Planted);
        if (ripeCropModel != null) ripeCropModel.SetActive(currentState == FarmState.Harvestable);
    }
}
