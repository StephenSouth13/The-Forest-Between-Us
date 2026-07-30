using UnityEngine;

public class Campfire : MonoBehaviour, Interactable
{
    [Header("Fire Settings")]
    public string fireName = "Đống Lửa Trại";
    public bool isBurning = false;
    public float currentFuelSeconds = 0f;
    public float maxFuelSeconds = 600f; // Tối đa 10 phút lửa

    [Header("Fuel & Items")]
    public ItemData woodFuelItem;     // Gỗ củi để thêm lửa (mỗi khúc +120s)
    public ItemData rawFoodItem;      // Thịt sống / Trái cây sống để nấu
    public ItemData cookedFoodItem;   // Thịt chín / Đồ ăn nướng

    [Header("Visual Effects")]
    public Light fireLight;
    public ParticleSystem fireParticles;

    void Update()
    {
        if (isBurning)
        {
            currentFuelSeconds -= Time.deltaTime;
            if (currentFuelSeconds <= 0f)
            {
                ExtinguishFire();
            }

            // Sưởi ấm player khi đứng gần đống lửa đêm
            HeatPlayerNearby();
        }
    }

    public string GetInteractPrompt()
    {
        if (!isBurning)
        {
            return $"Đốt lửa / Thêm gỗ củi vào {fireName} (Phím F)";
        }
        else
        {
            int remainingMinutes = Mathf.CeilToInt(currentFuelSeconds / 60f);
            if (InventoryManager.instance != null && rawFoodItem != null && InventoryManager.instance.HasItem(rawFoodItem, 1))
            {
                return $"Nấu {rawFoodItem.itemName} trên đống lửa (Phím F)";
            }
            return $"Thêm gỗ củi vào {fireName} (Còn {remainingMinutes} phút lửa)";
        }
    }

    public void OnInteract()
    {
        // 1. Nếu đống lửa đang cháy và người chơi có thịt sống / đồ sống -> Nấu ăn!
        if (isBurning && rawFoodItem != null && cookedFoodItem != null && InventoryManager.instance != null && InventoryManager.instance.HasItem(rawFoodItem, 1))
        {
            if (InventoryManager.instance.RemoveItem(rawFoodItem, 1))
            {
                bool success = InventoryManager.instance.PickUpItem(cookedFoodItem, 1);
                if (success)
                {
                    Debug.Log($"🔥 Đã nướng chín {rawFoodItem.itemName} thành {cookedFoodItem.itemName}!");
                }
                else
                {
                    Debug.LogWarning($"⚠️ Balo đầy! Không thể nhận {cookedFoodItem.itemName}.");
                }
                return;
            }
        }

        // 2. Thêm củi gỗ để đốt/duy trì lửa
        if (InventoryManager.instance != null && woodFuelItem != null && InventoryManager.instance.HasItem(woodFuelItem, 1))
        {
            if (InventoryManager.instance.RemoveItem(woodFuelItem, 1))
            {
                currentFuelSeconds = Mathf.Min(currentFuelSeconds + 120f, maxFuelSeconds);
                if (!isBurning) LightFire();
                Debug.Log($"🔥 Đã thêm {woodFuelItem.itemName} vào đống lửa! Lửa còn: {Mathf.CeilToInt(currentFuelSeconds)}s");
                return;
            }
        }

        // 3. Đốt lửa nếu có củi mồi sẵn
        if (!isBurning)
        {
            currentFuelSeconds = 120f;
            LightFire();
            Debug.Log($"🔥 Đã nhóm lửa {fireName}!");
        }
    }

    void LightFire()
    {
        isBurning = true;
        if (fireLight != null) fireLight.enabled = true;
        if (fireParticles != null) fireParticles.Play();
    }

    void ExtinguishFire()
    {
        isBurning = false;
        currentFuelSeconds = 0f;
        if (fireLight != null) fireLight.enabled = false;
        if (fireParticles != null) fireParticles.Stop();
        Debug.Log($"💨 {fireName} đã tàn lửa!");
    }

    void HeatPlayerNearby()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= 4f)
        {
            if (PlayerStatsManager.instance != null)
            {
                PlayerStatsManager.instance.RestSleep(2f * Time.deltaTime);
            }
        }
    }
}
