using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager instance;

    [Header("Real Player Vitals (0 - 100)")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    public float maxStamina = 100f;
    public float currentStamina = 100f;

    public float maxThirst = 100f;
    public float currentThirst = 100f;

    public float maxHunger = 100f;
    public float currentHunger = 100f;

    public float maxSleep = 100f;
    public float currentSleep = 100f; // 100 = Tỉnh táo hoàn toàn, 0 = Đuối sức kiệt huệ

    public int karma = 50;

    [Header("Drain Rates (per second)")]
    public float thirstDrainRate = 0.08f;
    public float hungerDrainRate = 0.05f;
    public float sleepDrainRate = 0.03f;
    public float staminaDrainRate = 20f;
    public float staminaRegenRate = 15f;

    [Header("State Flags")]
    public bool isRunning;
    public bool isStarving;
    public bool isDehydrated;

    private float nextDamageTime;

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
        currentThirst = Mathf.Clamp(currentThirst - thirstDrainRate * Time.deltaTime, 0f, maxThirst);
        currentHunger = Mathf.Clamp(currentHunger - hungerDrainRate * Time.deltaTime, 0f, maxHunger);
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

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
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
        currentHealth = PlayerPrefs.GetFloat("Player_Health", maxHealth);
        currentStamina = PlayerPrefs.GetFloat("Player_Stamina", maxStamina);
        currentThirst = PlayerPrefs.GetFloat("Player_Thirst", maxThirst);
        currentHunger = PlayerPrefs.GetFloat("Player_Hunger", maxHunger);
        currentSleep = PlayerPrefs.GetFloat("Player_Sleep", maxSleep);
        karma = PlayerPrefs.GetInt("Player_Karma", 50);
    }
}
