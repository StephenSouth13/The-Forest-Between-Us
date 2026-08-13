using System;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    public static DayManager instance;
    public static DayManager Instance => instance;

    [Header("Day Settings")]
    public int currentDay = 1;
    public int maxDays = 30;
    public float dayDurationSeconds = 300f; // 5 minutes per day

    [Header("Day / Night Cycle")]
    public Light directionalLight;
    public Gradient lightColorGradient;
    public AnimationCurve lightIntensityCurve;
    public float currentTimeOfDay = 0.25f; // 0.0 = Midnight, 0.5 = Noon, 1.0 = Midnight

    [Header("Fog Settings")]
    public Color dayFogColor = new Color(0.5f, 0.6f, 0.7f, 1f);
    public Color nightFogColor = new Color(0.05f, 0.05f, 0.1f, 1f);
    public float dayFogDensity = 0.01f;
    public float nightFogDensity = 0.04f;

    [Header("Environmental Protection (Quy Tắc Bảo Vệ Môi Trường)")]
    public int maxTreesAllowedPerDay = 5; // Tối đa 5 cây / ngày
    public int treesChoppedToday = 0;

    public bool CanChopTree()
    {
        return treesChoppedToday < maxTreesAllowedPerDay;
    }

    public void RegisterTreeChopped()
    {
        treesChoppedToday++;
        Debug.Log($"🌱 [Bảo Vệ Môi Trường] Đã chặt {treesChoppedToday}/{maxTreesAllowedPerDay} cây trong Ngày {currentDay}.");
    }

    public event Action<int> OnDayChanged;
    public event Action OnNightStarted;
    public event Action OnDayStarted;

    private bool isNight;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        RenderSettings.fog = true;
        OnDayChanged?.Invoke(currentDay);
    }

    void Update()
    {
        UpdateDayCycle();
    }

    void UpdateDayCycle()
    {
        float speed = 1f;
        if (GameDirector.instance != null) speed = GameDirector.instance.dayNightCycleSpeed;

        currentTimeOfDay += ((Time.deltaTime * speed) / dayDurationSeconds);
        if (currentTimeOfDay >= 1f)
        {
            currentTimeOfDay = 0f;
            NextDay();
        }

        // Lighting & Fog updates
        float intensityFactor = GetLightIntensity(currentTimeOfDay);
        if (directionalLight != null)
        {
            directionalLight.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90f, 170f, 0f);
            if (lightColorGradient != null && lightColorGradient.colorKeys.Length > 0)
            {
                directionalLight.color = lightColorGradient.Evaluate(currentTimeOfDay);
            }
            directionalLight.intensity = intensityFactor;
        }

        bool currentlyNight = (currentTimeOfDay < 0.2f || currentTimeOfDay > 0.75f);
        if (currentlyNight != isNight)
        {
            isNight = currentlyNight;
            if (isNight) OnNightStarted?.Invoke();
            else OnDayStarted?.Invoke();
        }

        // Update Fog
        RenderSettings.fogColor = Color.Lerp(nightFogColor, dayFogColor, Mathf.Clamp01(intensityFactor / 1.5f));
        RenderSettings.fogDensity = Mathf.Lerp(nightFogDensity, dayFogDensity, Mathf.Clamp01(intensityFactor / 1.5f));
    }

    public float GetLightIntensity(float timeOfDay)
    {
        if (lightIntensityCurve != null && lightIntensityCurve.length > 0)
        {
            float val = lightIntensityCurve.Evaluate(timeOfDay);
            if (val > 0.05f) return val;
        }

        // Default curve calculation if curve is missing or 0: daytime (0.2-0.8) -> bright, night -> dim
        if (timeOfDay >= 0.2f && timeOfDay <= 0.8f)
        {
            float sin = Mathf.Sin((timeOfDay - 0.2f) / 0.6f * Mathf.PI);
            return Mathf.Lerp(0.3f, 1.4f, sin);
        }
        return 0.25f; // Night time ambient
    }

    public void NextDay()
    {
        if (currentDay >= maxDays)
        {
            Debug.Log("Day 30 Reached! Triggering Judgment Day.");
            if (EndingManager.instance != null)
            {
                EndingManager.instance.ShowEndingChoiceUI();
            }
            return;
        }

        currentDay++;
        treesChoppedToday = 0; // Reset số lượng cây đã chặt cho ngày mới
        Debug.Log($"Advanced to Day {currentDay}. Reset tree chopping limit for environmental protection.");

        OnDayChanged?.Invoke(currentDay);

        if (MissionManager.instance != null)
        {
            MissionManager.instance.currentDay = currentDay;
            MissionManager.instance.UpdateDailyQuest();
        }
    }

    public bool IsNight() => isNight;
}
