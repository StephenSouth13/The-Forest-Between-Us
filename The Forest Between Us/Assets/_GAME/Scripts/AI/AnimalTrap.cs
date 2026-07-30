using UnityEngine;

public class AnimalTrap : MonoBehaviour, Interactable
{
    [Header("Trap Settings")]
    public string trapName = "Bẫy Lồng Săn Bắt";
    public bool isArmed = true;
    public bool isTriggered = false;

    [Header("Captured Loot / Target")]
    public ItemData capturedLootItem;
    public int capturedLootAmount = 1;

    [Header("Visuals")]
    public GameObject trapOpenModel;
    public GameObject trapClosedModel;

    void Start()
    {
        UpdateVisuals();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isArmed || isTriggered) return;

        // Bẫy sập khi Động vật hoặc Quái dẫm vào
        AnimalAI animal = other.GetComponentInParent<AnimalAI>();
        if (animal != null && !animal.isTamed)
        {
            TriggerTrap(animal.animalName, animal.dropMeatItem);
            Destroy(animal.gameObject);
            return;
        }

        EnemyAI enemy = other.GetComponentInParent<EnemyAI>();
        if (enemy != null)
        {
            TriggerTrap("Quái Vật Bóng Đêm", null);
            return;
        }
    }

    void TriggerTrap(string targetName, ItemData loot)
    {
        isArmed = false;
        isTriggered = true;
        if (loot != null) capturedLootItem = loot;
        UpdateVisuals();

        Debug.Log($"🪤 💥 BẪY ĐÃ SẬP! Đã bắt giữ được: {targetName}!");
    }

    public string GetInteractPrompt()
    {
        if (isTriggered) return $"Thu hoạch chiến lợi phẩm từ {trapName} (Phím F)";
        if (isArmed) return $"{trapName} (Đang giăng bẫy săn bắt...)";
        return $"Giăng lại {trapName} (Phím F)";
    }

    public void OnInteract()
    {
        if (isTriggered)
        {
            if (capturedLootItem != null && InventoryManager.instance != null)
            {
                bool success = InventoryManager.instance.PickUpItem(capturedLootItem, capturedLootAmount);
                if (success)
                {
                    Debug.Log($"🧺 Đã thu hoạch chiến lợi phẩm {capturedLootItem.itemName} từ Bẫy!");
                    isTriggered = false;
                    isArmed = true;
                    UpdateVisuals();
                }
                else
                {
                    Debug.LogWarning("⚠️ Balo đầy! Không thể thu hoạch bẫy.");
                }
            }
            else
            {
                isTriggered = false;
                isArmed = true;
                UpdateVisuals();
            }
        }
    }

    void UpdateVisuals()
    {
        if (trapOpenModel != null) trapOpenModel.SetActive(isArmed && !isTriggered);
        if (trapClosedModel != null) trapClosedModel.SetActive(isTriggered);
    }
}
