using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    public static PlayerStatusUI instance;

    [Header("Panel & Controls")]
    public GameObject statusPanel;
    public KeyCode toggleKey = KeyCode.Tab;
    public bool startHidden = true;
    public bool createFallbackPanel = true;
    public bool controlCursor = true;
    public bool playAudioOnToggle = true;

    [Header("Player Vitals (0 - 100)")]
    [Range(0f, 100f)] public float health = 100f;
    [Range(0f, 100f)] public float stamina = 100f;
    [Range(0f, 100f)] public float hunger = 85f;
    [Range(0f, 100f)] public float thirst = 80f;
    [Range(0f, 100f)] public float karma = 50f;

    [Header("UI Text Displays")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI staminaText;
    public TextMeshProUGUI hungerText;
    public TextMeshProUGUI thirstText;
    public TextMeshProUGUI karmaText;
    public TextMeshProUGUI titleText;

    [Header("UI Stat Sliders")]
    public Slider healthSlider;
    public Slider staminaSlider;
    public Slider hungerSlider;
    public Slider thirstSlider;

    private AudioSource audioSource;
    private Vector3 originalScale = Vector3.one;

    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }
    }

    void Start()
    {
        if (statusPanel == null && createFallbackPanel)
        {
            statusPanel = CreateFallbackPanel();
        }

        if (statusPanel != null)
        {
            originalScale = statusPanel.transform.localScale;
            statusPanel.SetActive(!startHidden);
        }

        Refresh();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        if (statusPanel == null) return;

        bool isOpening = !statusPanel.activeSelf;
        statusPanel.SetActive(isOpening);

        if (isOpening)
        {
            Refresh();
            AnimateOpen();

            if (controlCursor)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (playAudioOnToggle) PlayStatusSFX(true);
        }
        else
        {
            if (controlCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (playAudioOnToggle) PlayStatusSFX(false);
        }
    }

    public void Refresh()
    {
        SetText(healthText, "MÁU (HEALTH)", health);
        SetText(staminaText, "THỂ LỰC (STAMINA)", stamina);
        SetText(hungerText, "CƠN ĐÓI (HUNGER)", hunger);
        SetText(thirstText, "CƠN KHÁT (THIRST)", thirst);
        if (karmaText != null) karmaText.text = $"NĂNG LƯỢNG KARMA: {Mathf.RoundToInt(karma)}/100";

        SetSlider(healthSlider, health);
        SetSlider(staminaSlider, stamina);
        SetSlider(hungerSlider, hunger);
        SetSlider(thirstSlider, thirst);
    }

    public void UpdateVitals(float h, float st, float hu, float th)
    {
        health = Mathf.Clamp(h, 0f, 100f);
        stamina = Mathf.Clamp(st, 0f, 100f);
        hunger = Mathf.Clamp(hu, 0f, 100f);
        thirst = Mathf.Clamp(th, 0f, 100f);
        Refresh();
    }

    void AnimateOpen()
    {
        if (statusPanel == null) return;
        statusPanel.transform.localScale = originalScale * 0.92f;
        StartCoroutine(PopScaleRoutine());
    }

    System.Collections.IEnumerator PopScaleRoutine()
    {
        float duration = 0.15f;
        float t = 0f;
        Vector3 start = originalScale * 0.92f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(t / duration);
            float eased = Mathf.Sin(progress * Mathf.PI * 0.5f);
            statusPanel.transform.localScale = Vector3.Lerp(start, originalScale, eased);
            yield return null;
        }

        statusPanel.transform.localScale = originalScale;
    }

    void SetText(TextMeshProUGUI label, string name, float value)
    {
        if (label != null) label.text = $"{name}: {Mathf.RoundToInt(value)}%";
    }

    void SetSlider(Slider slider, float value)
    {
        if (slider == null) return;
        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.value = value;
    }

    void PlayStatusSFX(bool open)
    {
        if (audioSource == null) return;
        AudioClip clip = CreateStatusSFXClip(open ? 587.33f : 440f, 0.12f, 0.25f);
        audioSource.PlayOneShot(clip);
    }

    AudioClip CreateStatusSFXClip(float freq, float duration, float volume)
    {
        int sampleRate = 44100;
        int samples = (int)(sampleRate * duration);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = Mathf.Clamp01(1f - t / duration);
            data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * envelope * volume;
        }
        AudioClip clip = AudioClip.Create("StatusSFX", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    GameObject CreateFallbackPanel()
    {
        GameObject panel = new GameObject("Player Status Panel (Runtime)", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(transform, false);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.04f, 0.50f);
        panelRect.anchorMax = new Vector2(0.36f, 0.94f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = panel.GetComponent<Image>();
        panelImage.color = new Color(0.06f, 0.09f, 0.14f, 0.85f); // Sleek Cyber Charcoal

        // Header Title
        GameObject titleGO = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        titleGO.transform.SetParent(panel.transform, false);
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.05f, 0.82f);
        titleRect.anchorMax = new Vector2(0.95f, 0.96f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        titleText = titleGO.GetComponent<TextMeshProUGUI>();
        titleText.fontSize = 20f;
        titleText.fontStyle = FontStyles.Bold;
        titleText.color = new Color(0.2f, 1f, 0.6f, 1f); // Neon Cyan
        titleText.text = "--- TRẠNG THÁI NHÂN VẬT ---";

        healthText = CreateFallbackLabel(panel.transform, "MÁU (HEALTH)", 0, new Color(1f, 0.3f, 0.3f));
        staminaText = CreateFallbackLabel(panel.transform, "THỂ LỰC (STAMINA)", 1, new Color(0.3f, 0.8f, 1f));
        hungerText = CreateFallbackLabel(panel.transform, "CƠN ĐÓI (HUNGER)", 2, new Color(1f, 0.7f, 0.2f));
        thirstText = CreateFallbackLabel(panel.transform, "CƠN KHÁT (THIRST)", 3, new Color(0.2f, 0.9f, 0.9f));

        return panel;
    }

    TextMeshProUGUI CreateFallbackLabel(Transform parent, string label, int index, Color textColor)
    {
        GameObject textObject = new GameObject(label, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.08f, 0.62f - index * 0.18f);
        rectTransform.anchorMax = new Vector2(0.92f, 0.78f - index * 0.18f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.fontSize = 18f;
        text.color = textColor;
        text.text = label;

        return text;
    }
}

