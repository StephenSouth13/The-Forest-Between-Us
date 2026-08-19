using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager instance;

    [Header("🌟 CẤU HÌNH CẤP ĐỘ & XP (LEVEL SYSTEM)")]
    [Tooltip("Cấp độ hiện tại của nhân vật sinh tồn.")]
    public int level = 1;
    [Tooltip("Điểm kinh nghiệm XP hiện tại.")]
    public float currentXP = 0f;
    [Tooltip("Số điểm XP cần đạt để thăng cấp tiếp theo.")]
    public float xpToNextLevel = 100f;

    [Header("❤️ CHỈ SỐ SINH TỒN THỰC TẾ (0 - 100)")]
    [Tooltip("Máu tối đa của nhân vật.")]
    public float maxHealth = 100f;
    [Tooltip("Máu hiện tại (Nếu về 0 sẽ kích hoạt màn hình Chết/Hồi sinh).")]
    public float currentHealth = 100f;

    [Tooltip("Thể lực tối đa.")]
    public float maxStamina = 100f;
    [Tooltip("Thể lực hiện tại (Dùng khi chạy nhanh / chặt cây).")]
    public float currentStamina = 100f;

    [Tooltip("Mức độ Khát nước tối đa.")]
    public float maxThirst = 100f;
    [Tooltip("Cột Nước hiện tại (Tụt dần theo thời gian).")]
    public float currentThirst = 100f;

    [Tooltip("Mức độ Đói bụng tối đa.")]
    public float maxHunger = 100f;
    [Tooltip("Cột Đói hiện tại (Tụt dần theo thời gian).")]
    public float currentHunger = 100f;

    [Tooltip("Cột Tỉnh táo / Giấc ngủ tối đa.")]
    public float maxSleep = 100f;
    [Tooltip("Cột Giấc ngủ (100 = Tỉnh táo hoàn toàn, 0 = Đuối sức kiệt huệ).")]
    public float currentSleep = 100f;

    [Tooltip("Điểm Karma Nhân Quả (0 = Ác/Bị tha hóa bóng đêm, 50 = Trung lập, 100 = Thiện/Anh hùng).")]
    public int karma = 50;

    [Header("⏱️ TỐC ĐỘ TỤT CHỈ SỐ (DRAIN RATES / GIÂY)")]
    [Tooltip("Tốc độ tụt cột Khát nước mỗi giây.")]
    public float thirstDrainRate = 0.08f;
    [Tooltip("Tốc độ tụt cột Đói bụng mỗi giây.")]
    public float hungerDrainRate = 0.05f;
    [Tooltip("Tốc độ tụt cột Buồn ngủ mỗi giây.")]
    public float sleepDrainRate = 0.03f;
    [Tooltip("Tốc độ tiêu hao Thể lực khi chạy Sprint mỗi giây.")]
    public float staminaDrainRate = 20f;
    [Tooltip("Tốc độ hồi lại Thể lực khi đi bộ/đứng yên mỗi giây.")]
    public float staminaRegenRate = 15f;

    [Header("🎒 TRẠNG THÁI & HỆ SỐ NẶNG BALO (ENCUMBRANCE)")]
    [Tooltip("Đang bấm phím chạy Sprint hay không.")]
    public bool isRunning;
    [Tooltip("Đang bị nhịn đói (Hunger = 0).")]
    public bool isStarving;
    [Tooltip("Đang bị khát nước (Thirst = 0).")]
    public bool isDehydrated;
    [Tooltip("Hệ số giảm tốc độ do Balo nặng (1.0 = Đi 100% bình thường, 0.5 = Nặng quá giảm 50% tốc độ).")]
    public float currentWeightPenaltyMultiplier = 1.0f;

    private float nextDamageTime;

    // 🎒 TÍNH TOÁN HỆ SỐ TỐC ĐỘ DI CHUYỂN DỰA TRÊN TRỌNG LƯỢNG BALO (ENCUMBRANCE SYSTEM)
    public float GetMovementSpeedMultiplier()
    {
        if (InventoryManager.instance == null) return 1.0f;

        float currentWeight = InventoryManager.instance.GetTotalWeight();
        float maxWeight = InventoryManager.instance.maxWeightCapacity; // Mặc định 30kg

        if (maxWeight <= 0) return 1.0f;

        float weightRatio = currentWeight / maxWeight; // Tỷ lệ từ 0.0 -> 1.0+

        // 🟢 Dưới 50% tải trọng (<= 15kg): Tốc độ 100% bình thường
        if (weightRatio <= 0.5f)
        {
            currentWeightPenaltyMultiplier = 1.0f;
        }
        // 🟡 Từ 50% -> 90% tải trọng (15kg - 27kg): Giảm dần từ 100% xuống 75% tốc độ
        else if (weightRatio <= 0.9f)
        {
            float t = (weightRatio - 0.5f) / 0.4f;
            currentWeightPenaltyMultiplier = Mathf.Lerp(1.0f, 0.75f, t);
        }
        // 🔴 Trên 90% tải trọng (>= 27kg): Balo quá nặng! Giảm còn 50% tốc độ & không thể chạy nhanh (Sprint)
        else
        {
            float t = Mathf.Clamp01((weightRatio - 0.9f) / 0.1f);
            currentWeightPenaltyMultiplier = Mathf.Lerp(0.75f, 0.5f, t);
        }

        return currentWeightPenaltyMultiplier;
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        LoadStats();
    }

    void Update()
    {
        HandleVitalsDepletion();
        HandleStaminaLogic();
        HandleStarvationDamage();
    }

    void HandleVitalsDepletion()
    {
        float hDrain = hungerDrainRate;
        float tDrain = thirstDrainRate;
        float sDrain = staminaDrainRate; // Actually we use HandleStaminaLogic for stamina drain, but we can scale it there.

        if (GameDirector.instance != null)
        {
            hDrain *= GameDirector.instance.globalHungerDepletionRate;
            tDrain *= GameDirector.instance.globalThirstDepletionRate;
        }

        currentThirst = Mathf.Clamp(currentThirst - tDrain * Time.deltaTime, 0f, maxThirst);
        currentHunger = Mathf.Clamp(currentHunger - hDrain * Time.deltaTime, 0f, maxHunger);
        currentSleep = Mathf.Clamp(currentSleep - sleepDrainRate * Time.deltaTime, 0f, maxSleep);

        isDehydrated = currentThirst <= 0f;
        isStarving = currentHunger <= 0f;
    }

    void HandleStaminaLogic()
    {
        isRunning = Input.GetKey(KeyCode.LeftShift) && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0);

        if (isRunning && currentStamina > 0f)
        {
            currentStamina = Mathf.Clamp(currentStamina - staminaDrainRate * Time.deltaTime, 0f, maxStamina);
        }
        else if (!isRunning && currentStamina < maxStamina)
        {
            currentStamina = Mathf.Clamp(currentStamina + staminaRegenRate * Time.deltaTime, 0f, maxStamina);
        }
    }

    void HandleStarvationDamage()
    {
        if ((isStarving || isDehydrated) && Time.time >= nextDamageTime)
        {
            nextDamageTime = Time.time + 3f;
            TakeDamage(5f);
        }
    }

    public void AddXP(float amount)
    {
        currentXP += amount;
        Debug.Log($"Nhận {amount} XP! ({currentXP}/{xpToNextLevel})");
        
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
        SaveStats();
    }

    void LevelUp()
    {
        level++;
        currentXP -= xpToNextLevel;
        xpToNextLevel = Mathf.Round(xpToNextLevel * 1.5f); // Tăng lượng XP cần thiết cho cấp tiếp theo

        // Tăng giới hạn chỉ số tối đa
        maxHealth += 10f;
        maxStamina += 10f;
        maxHunger += 10f;
        maxThirst += 10f;
        maxSleep += 5f;

        // Hồi phục toàn bộ khi lên cấp
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentHunger = maxHunger;
        currentThirst = maxThirst;
        currentSleep = maxSleep;

        Debug.Log($"🎉 CHÚC MỪNG LÊN CẤP {level}! Chỉ số tối đa đã được tăng cường.");
        
        if (PlayerStatusUI.instance != null)
        {
            PlayerStatusUI.instance.Refresh();
        }
    }

    public void TakeDamage(float amount)
    {
        float multiplier = 1f;
        if (GameDirector.instance != null) multiplier = GameDirector.instance.enemyDamageMultiplier;
        
        float finalDamage = amount * multiplier;
        
        // Trừ đi Giáp từ trang bị
        if (PlayerEquipmentManager.instance != null)
        {
            float armor = PlayerEquipmentManager.instance.GetTotalArmor();
            finalDamage = Mathf.Max(0f, finalDamage - armor);
        }

        currentHealth = Mathf.Clamp(currentHealth - finalDamage, 0f, maxHealth);
        SaveStats();
        if (currentHealth <= 0f)
        {
            Debug.LogWarning("Player is dead!");
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        SaveStats();
    }

    public void EatFood(float amount)
    {
        currentHunger = Mathf.Clamp(currentHunger + amount, 0f, maxHunger);
        SaveStats();
    }

    public void DrinkWater(float amount)
    {
        currentThirst = Mathf.Clamp(currentThirst + amount, 0f, maxThirst);
        SaveStats();
    }

    public void RestSleep(float amount)
    {
        currentSleep = Mathf.Clamp(currentSleep + amount, 0f, maxSleep);
        SaveStats();
    }

    public void ModifyKarma(int amount)
    {
        karma = Mathf.Clamp(karma + amount, 0, 100);
        SaveStats();
    }

    public void SaveStats()
    {
        PlayerPrefs.SetInt("Player_Level", level);
        PlayerPrefs.SetFloat("Player_CurrentXP", currentXP);
        PlayerPrefs.SetFloat("Player_XPToNextLevel", xpToNextLevel);

        PlayerPrefs.SetFloat("Player_MaxHealth", maxHealth);
        PlayerPrefs.SetFloat("Player_MaxStamina", maxStamina);
        PlayerPrefs.SetFloat("Player_MaxHunger", maxHunger);
        PlayerPrefs.SetFloat("Player_MaxThirst", maxThirst);
        PlayerPrefs.SetFloat("Player_MaxSleep", maxSleep);

        PlayerPrefs.SetFloat("Player_Health", currentHealth);
        PlayerPrefs.SetFloat("Player_Stamina", currentStamina);
        PlayerPrefs.SetFloat("Player_Thirst", currentThirst);
        PlayerPrefs.SetFloat("Player_Hunger", currentHunger);
        PlayerPrefs.SetFloat("Player_Sleep", currentSleep);
        PlayerPrefs.SetInt("Player_Karma", karma);
        PlayerPrefs.Save();
    }

    public void LoadStats()
    {
        level = PlayerPrefs.GetInt("Player_Level", 1);
        currentXP = PlayerPrefs.GetFloat("Player_CurrentXP", 0f);
        xpToNextLevel = PlayerPrefs.GetFloat("Player_XPToNextLevel", 100f);

        maxHealth = PlayerPrefs.GetFloat("Player_MaxHealth", 100f);
        maxStamina = PlayerPrefs.GetFloat("Player_MaxStamina", 100f);
        maxHunger = PlayerPrefs.GetFloat("Player_MaxHunger", 100f);
        maxThirst = PlayerPrefs.GetFloat("Player_MaxThirst", 100f);
        maxSleep = PlayerPrefs.GetFloat("Player_MaxSleep", 100f);

        currentHealth = PlayerPrefs.GetFloat("Player_Health", maxHealth);
        currentStamina = PlayerPrefs.GetFloat("Player_Stamina", maxStamina);
        currentThirst = PlayerPrefs.GetFloat("Player_Thirst", maxThirst);
        currentHunger = PlayerPrefs.GetFloat("Player_Hunger", maxHunger);
        currentSleep = PlayerPrefs.GetFloat("Player_Sleep", maxSleep);
        karma = PlayerPrefs.GetInt("Player_Karma", 50);

        if (currentHealth <= 0f)
        {
            currentHealth = maxHealth;
            currentStamina = maxStamina;
            currentThirst = maxThirst;
            currentHunger = maxHunger;
            currentSleep = maxSleep;
            SaveStats();
        }
    }

    public void ResetVitalsToFull()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentThirst = maxThirst;
        currentHunger = maxHunger;
        currentSleep = maxSleep;
        isStarving = false;
        isDehydrated = false;
        SaveStats();
    }
}
