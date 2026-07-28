using System;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    public static DayManager instance;

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
    public Color dayFogColor = new Color(0.8f, 0.8f, 0.8f);
    public Color nightFogColor = new Color(0.05f, 0.05f, 0.1f);
    public float dayFogDensity = 0.01f;
    public float nightFogDensity = 0.05f;

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
        currentTimeOfDay += (Time.deltaTime / dayDurationSeconds);
        if (currentTimeOfDay >= 1f)
        {
            currentTimeOfDay = 0f;
            NextDay();
        }

        // Lighting & Fog updates
        if (directionalLight != null)
        {
            directionalLight.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90f, 170f, 0f);
            if (lightColorGradient != null)
            {
                directionalLight.color = lightColorGradient.Evaluate(currentTimeOfDay);
            }
            directionalLight.intensity = lightIntensityCurve.Evaluate(currentTimeOfDay);
        }

        bool currentlyNight = (currentTimeOfDay < 0.2f || currentTimeOfDay > 0.75f);
        if (currentlyNight != isNight)
        {
            isNight = currentlyNight;
            if (isNight) OnNightStarted?.Invoke();
            else OnDayStarted?.Invoke();
        }

        // Update Fog
        RenderSettings.fogColor = Color.Lerp(nightFogColor, dayFogColor, lightIntensityCurve.Evaluate(currentTimeOfDay));
        RenderSettings.fogDensity = Mathf.Lerp(nightFogDensity, dayFogDensity, lightIntensityCurve.Evaluate(currentTimeOfDay));
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
        Debug.Log($"Advanced to Day {currentDay}");

        OnDayChanged?.Invoke(currentDay);

        if (MissionManager.instance != null)
        {
            MissionManager.instance.currentDay = currentDay;
            MissionManager.instance.UpdateDailyQuest();
        }
    }

    public bool IsNight() => isNight;
}
