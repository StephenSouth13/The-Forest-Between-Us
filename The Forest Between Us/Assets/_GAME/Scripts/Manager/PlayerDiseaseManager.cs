using UnityEngine;

public class PlayerDiseaseManager : MonoBehaviour
{
    public static PlayerDiseaseManager instance;

    [Header("Disease States (Trạng Thái Bệnh Bật/Tắt)")]
    public bool hasFlu = false;                   // Bệnh Cảm Cúm / Cảm Lạnh
    public bool hasMosquitoDisease = false;       // Bệnh Sốt Rét / Sốt Xuất Huyết do Muỗi
    public bool isProtectedByRepellent = false;   // Đang được bảo vệ bởi Thuốc Bôi Chống Muỗi

    [Header("Disease Timers & Durations")]
    public float repellentRemainingSeconds = 0f;
    public float fluDamageTimer = 0f;
    public float mosquitoDamageTimer = 0f;

    [Header("Disease Penalties (Sát Thương & Tác Dụng Phụ)")]
    public float fluHealthDrainRate = 0.5f;       // Trừ Máu do Cảm Cúm (mỗi giây)
    public float mosquitoHealthDrainRate = 1.2f;  // Trừ Máu do Sốt Rét (mỗi giây)
    public float fluStaminaMultiplier = 1.8f;      // Tốn Thể Lực hơn gấp 1.8 lần khi bị Cảm Cúm

    [Header("🎨 Custom Asset Slots (Kéo Thả VFX / SFX / Audio Của Bạn Vào Đây)")]
    public ParticleSystem mosquitoSwarmVFX;        // VFX Đàn muỗi vo ve xung quanh
    public AudioClip coughSFX;                     // Audio tiếng ho cảm cúm
    public AudioClip mosquitoBuzzSFX;              // Audio tiếng muỗi vo ve
    public AudioClip cureEffectSFX;                // Audio tiếng khôi phục khỏi bệnh

    private AudioSource audioSource;
    private float soundTimer = 0f;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        // 1. Đếm ngược Thuốc Bôi Chống Muỗi
        if (isProtectedByRepellent)
        {
            repellentRemainingSeconds -= Time.deltaTime;
            if (repellentRemainingSeconds <= 0f)
            {
                isProtectedByRepellent = false;
                repellentRemainingSeconds = 0f;
                Debug.Log("🛡️ 💨 Thuốc Bôi Chống Muỗi đã hết tác dụng!");
            }
        }

        // 2. Xử lý tác hại Bệnh Cảm Cúm
        if (hasFlu)
        {
            fluDamageTimer += Time.deltaTime;
            if (PlayerStatsManager.instance != null)
            {
                PlayerStatsManager.instance.currentHealth = Mathf.Clamp(PlayerStatsManager.instance.currentHealth - fluHealthDrainRate * Time.deltaTime, 0f, PlayerStatsManager.instance.maxHealth);
            }

            soundTimer += Time.deltaTime;
            if (soundTimer >= 12f)
            {
                soundTimer = 0f;
                PlayCoughSound();
            }
        }

        // 3. Xử lý tác hại Bệnh Sốt Rét / Sốt Xuất Huyết do Muỗi
        if (hasMosquitoDisease)
        {
            mosquitoDamageTimer += Time.deltaTime;
            if (PlayerStatsManager.instance != null)
            {
                PlayerStatsManager.instance.currentHealth = Mathf.Clamp(PlayerStatsManager.instance.currentHealth - mosquitoHealthDrainRate * Time.deltaTime, 0f, PlayerStatsManager.instance.maxHealth);
                PlayerStatsManager.instance.currentStamina = Mathf.Clamp(PlayerStatsManager.instance.currentStamina - 5f * Time.deltaTime, 0f, PlayerStatsManager.instance.maxStamina);
            }
        }
    }

    public void CatchFlu()
    {
        if (hasFlu) return;
        hasFlu = true;
        Debug.LogWarning("🤒 ⚠️ BẠN ĐÃ BỊ BỆNH CẢM CÚM! (Nhiễm lạnh do mưa hoặc sương đêm). Máu bị giảm liên tục (-0.5 HP/s). Hãy uống Trà Cảm Cúm Gừng!");
        PlayCoughSound();
    }

    public void GetMosquitoBitten()
    {
        if (isProtectedByRepellent)
        {
            Debug.Log("🛡️ Thuốc Bôi Chống Muỗi đã bảo vệ bạn khỏi bị muỗi đốt!");
            return;
        }

        if (!hasMosquitoDisease)
        {
            hasMosquitoDisease = true;
            Debug.LogWarning("🦟 ⚠️ BẠN ĐÃ BỊ MUỖI ĐỐT VÀ NGHĨÊM BỆNH SỐT RÉT! Máu giảm mạnh (-1.2 HP/s) & Kiệt sức. Hãy uống THUỐC KHÁNG SINH TỰ CHẾ!");
            if (mosquitoBuzzSFX != null && audioSource != null) audioSource.PlayOneShot(mosquitoBuzzSFX);
        }
    }

    public void CureMosquitoDisease()
    {
        if (hasMosquitoDisease)
        {
            hasMosquitoDisease = false;
            Debug.Log("💊 💉 ĐÃ UỐNG THUỐC KHÁNG SINH: Đã chữa khỏi hoàn toàn Bệnh Sốt Rét do muỗi đốt!");
            PlayCureSound();
        }
    }

    public void CureFlu()
    {
        if (hasFlu)
        {
            hasFlu = false;
            Debug.Log("🍵 🌿 ĐÃ UỐNG TRÀ DƯỢC: Đã chữa khỏi hoàn toàn Bệnh Cảm Cúm & Cảm Lạnh!");
            PlayCureSound();
        }
    }

    public void ApplyRepellent(float durationSeconds)
    {
        isProtectedByRepellent = true;
        repellentRemainingSeconds = durationSeconds;
        Debug.Log($"🧴 🛡️ Đã bôi Thuốc Chống Muỗi! Được bảo vệ chống muỗi trong {durationSeconds} giây.");
        PlayCureSound();
    }

    void PlayCoughSound()
    {
        if (coughSFX != null && audioSource != null) audioSource.PlayOneShot(coughSFX);
    }

    void PlayCureSound()
    {
        if (cureEffectSFX != null && audioSource != null) audioSource.PlayOneShot(cureEffectSFX);
    }

    public string GetDiseaseStatusReport()
    {
        string report = "";
        if (hasFlu) report += " [🤒 Cảm Cúm]";
        if (hasMosquitoDisease) report += " [🦟 Sốt Rét]";
        if (isProtectedByRepellent) report += $" [🧴 Kháng Muỗi: {Mathf.CeilToInt(repellentRemainingSeconds)}s]";
        return report;
    }
}
