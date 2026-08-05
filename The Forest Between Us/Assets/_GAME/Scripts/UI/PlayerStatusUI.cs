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

    [Header("Custom UI Asset Slots (Kéo Thả Sprite Của Bạn Vào Đây)")]
    public Sprite statusPanelBackgroundSprite;
    public Sprite statusHeaderIconSprite;
    public Sprite statIconHealth;
    public Sprite statIconStamina;
    public Sprite statIconHunger;
    public Sprite statIconThirst;
    public Sprite sliderFillSprite;

    [Header("Character Portrait & Equipment")]
    public Sprite characterPortraitSprite;
    public Sprite equipHelmetSprite;
    public Sprite equipArmorSprite;
    public Sprite equipWeaponSprite;
    public Sprite equipBootsSprite;
    public string characterName = "Người Sinh Tồn";
    public int characterLevel = 1;

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
    public TextMeshProUGUI levelText;

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
        EnsureCanvasParent();

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

    void EnsureCanvasParent()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                canvas = canvasGO.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
            }

            transform.SetParent(canvas.transform, false);
        }
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
        float maxH = 100f, maxS = 100f, maxHu = 100f, maxT = 100f;
        if (PlayerStatsManager.instance != null)
        {
            health = PlayerStatsManager.instance.currentHealth;
            stamina = PlayerStatsManager.instance.currentStamina;
            hunger = PlayerStatsManager.instance.currentHunger;
            thirst = PlayerStatsManager.instance.currentThirst;
            karma = PlayerStatsManager.instance.karma;
            characterLevel = PlayerStatsManager.instance.level;
            
            maxH = PlayerStatsManager.instance.maxHealth;
            maxS = PlayerStatsManager.instance.maxStamina;
            maxHu = PlayerStatsManager.instance.maxHunger;
            maxT = PlayerStatsManager.instance.maxThirst;
        }

        if (levelText != null)
        {
            levelText.text = $"Cấp độ: {characterLevel}";
            if (PlayerStatsManager.instance != null)
            {
                levelText.text += $" (XP: {PlayerStatsManager.instance.currentXP}/{PlayerStatsManager.instance.xpToNextLevel})";
            }
        }

        SetText(healthText, "MÁU (HEALTH)", health, maxH);
        SetText(staminaText, "THỂ LỰC (STAMINA)", stamina, maxS);
        SetText(hungerText, "CƠN ĐÓI (HUNGER)", hunger, maxHu);
        SetText(thirstText, "CƠN KHÁT (THIRST)", thirst, maxT);
        if (karmaText != null) karmaText.text = $"NĂNG LƯỢNG KARMA: {Mathf.RoundToInt(karma)}/100";

        SetSlider(healthSlider, health, maxH);
        SetSlider(staminaSlider, stamina, maxS);
        SetSlider(hungerSlider, hunger, maxHu);
        SetSlider(thirstSlider, thirst, maxT);
    }

    public void UpdateVitals(float h, float st, float hu, float th)
    {
        health = h;
        stamina = st;
        hunger = hu;
        thirst = th;
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

    void SetText(TextMeshProUGUI label, string name, float value, float maxValue)
    {
        if (label != null) label.text = $"{name}: {Mathf.RoundToInt(value)}/{Mathf.RoundToInt(maxValue)}";
    }

    void SetSlider(Slider slider, float value, float maxValue)
    {
        if (slider == null) return;
        slider.minValue = 0f;
        slider.maxValue = maxValue;
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
        panelRect.anchorMin = new Vector2(0.15f, 0.2f); // Rộng hơn một chút để chứa 2 cột
        panelRect.anchorMax = new Vector2(0.85f, 0.8f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = panel.GetComponent<Image>();
        panelImage.color = new Color(0.06f, 0.09f, 0.14f, 0.95f); // Sleek Cyber Charcoal

        // --- CỘT TRÁI (Chân dung & Thông tin cơ bản) ---
        GameObject leftCol = new GameObject("LeftColumn", typeof(RectTransform));
        leftCol.transform.SetParent(panel.transform, false);
        RectTransform leftRect = leftCol.GetComponent<RectTransform>();
        leftRect.anchorMin = new Vector2(0.02f, 0.05f);
        leftRect.anchorMax = new Vector2(0.35f, 0.95f);
        leftRect.offsetMin = Vector2.zero;
        leftRect.offsetMax = Vector2.zero;
        
        // Portrait BG
        GameObject portraitBg = new GameObject("PortraitBG", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        portraitBg.transform.SetParent(leftCol.transform, false);
        RectTransform pbRect = portraitBg.GetComponent<RectTransform>();
        pbRect.anchorMin = new Vector2(0.1f, 0.35f);
        pbRect.anchorMax = new Vector2(0.9f, 0.95f);
        pbRect.offsetMin = Vector2.zero;
        pbRect.offsetMax = Vector2.zero;
        portraitBg.GetComponent<Image>().color = new Color(0.1f, 0.15f, 0.2f, 1f);

        // Portrait Image
        GameObject portraitImg = new GameObject("PortraitImage", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        portraitImg.transform.SetParent(portraitBg.transform, false);
        RectTransform piRect = portraitImg.GetComponent<RectTransform>();
        piRect.anchorMin = new Vector2(0.05f, 0.05f);
        piRect.anchorMax = new Vector2(0.95f, 0.95f);
        piRect.offsetMin = Vector2.zero;
        piRect.offsetMax = Vector2.zero;
        Image pImg = portraitImg.GetComponent<Image>();
        if (characterPortraitSprite != null)
        {
            pImg.sprite = characterPortraitSprite;
        }
        else
        {
            pImg.color = new Color(0.3f, 0.4f, 0.5f, 1f); // Placeholder color
            // Add placeholder text
            GameObject ptTxt = new GameObject("PHText", typeof(RectTransform), typeof(TextMeshProUGUI));
            ptTxt.transform.SetParent(portraitImg.transform, false);
            RectTransform ptRect = ptTxt.GetComponent<RectTransform>();
            ptRect.anchorMin = Vector2.zero; ptRect.anchorMax = Vector2.one;
            ptRect.offsetMin = Vector2.zero; ptRect.offsetMax = Vector2.zero;
            TextMeshProUGUI txt = ptTxt.GetComponent<TextMeshProUGUI>();
            txt.text = "ẢNH\nNHÂN VẬT";
            txt.alignment = TextAlignmentOptions.Center;
            txt.color = new Color(0.7f, 0.8f, 0.9f, 1f);
        }

        // Name & Level
        GameObject nameTxtGO = new GameObject("NameText", typeof(RectTransform), typeof(TextMeshProUGUI));
        nameTxtGO.transform.SetParent(leftCol.transform, false);
        RectTransform nRect = nameTxtGO.GetComponent<RectTransform>();
        nRect.anchorMin = new Vector2(0f, 0.2f);
        nRect.anchorMax = new Vector2(1f, 0.3f);
        nRect.offsetMin = Vector2.zero; nRect.offsetMax = Vector2.zero;
        TextMeshProUGUI nTxt = nameTxtGO.GetComponent<TextMeshProUGUI>();
        nTxt.text = characterName;
        nTxt.fontSize = 24f;
        nTxt.fontStyle = FontStyles.Bold;
        nTxt.alignment = TextAlignmentOptions.Center;
        nTxt.color = new Color(0.2f, 1f, 0.6f, 1f); // Neon Cyan

        GameObject lvlTxtGO = new GameObject("LevelText", typeof(RectTransform), typeof(TextMeshProUGUI));
        lvlTxtGO.transform.SetParent(leftCol.transform, false);
        RectTransform lRect = lvlTxtGO.GetComponent<RectTransform>();
        lRect.anchorMin = new Vector2(0f, 0.1f);
        lRect.anchorMax = new Vector2(1f, 0.2f);
        lRect.offsetMin = Vector2.zero; lRect.offsetMax = Vector2.zero;
        levelText = lvlTxtGO.GetComponent<TextMeshProUGUI>();
        levelText.text = "Cấp độ: " + characterLevel;
        levelText.fontSize = 18f;
        levelText.alignment = TextAlignmentOptions.Center;
        levelText.color = new Color(0.8f, 0.9f, 0.9f, 1f);


        // --- CỘT PHẢI (Chỉ số & Slot trang bị) ---
        GameObject rightCol = new GameObject("RightColumn", typeof(RectTransform));
        rightCol.transform.SetParent(panel.transform, false);
        RectTransform rightRect = rightCol.GetComponent<RectTransform>();
        rightRect.anchorMin = new Vector2(0.4f, 0.05f);
        rightRect.anchorMax = new Vector2(0.98f, 0.95f);
        rightRect.offsetMin = Vector2.zero;
        rightRect.offsetMax = Vector2.zero;

        // Header
        GameObject titleGO = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        titleGO.transform.SetParent(rightCol.transform, false);
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 0.85f);
        titleRect.anchorMax = new Vector2(1f, 0.95f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        titleText = titleGO.GetComponent<TextMeshProUGUI>();
        titleText.fontSize = 24f;
        titleText.fontStyle = FontStyles.Bold;
        titleText.color = new Color(0.2f, 1f, 0.6f, 1f);
        titleText.text = "--- TRẠNG THÁI & TRANG BỊ ---";
        titleText.alignment = TextAlignmentOptions.Center;

        // Vitals Section (Top part of right column)
        healthText = CreateFallbackLabel(rightCol.transform, "MÁU (HEALTH)", 0, new Color(1f, 0.3f, 0.3f));
        staminaText = CreateFallbackLabel(rightCol.transform, "THỂ LỰC (STAMINA)", 1, new Color(0.3f, 0.8f, 1f));
        hungerText = CreateFallbackLabel(rightCol.transform, "CƠN ĐÓI (HUNGER)", 2, new Color(1f, 0.7f, 0.2f));
        thirstText = CreateFallbackLabel(rightCol.transform, "CƠN KHÁT (THIRST)", 3, new Color(0.2f, 0.9f, 0.9f));

        // Equipment Section (Bottom part of right column)
        GameObject equipArea = new GameObject("EquipmentArea", typeof(RectTransform));
        equipArea.transform.SetParent(rightCol.transform, false);
        RectTransform eaRect = equipArea.GetComponent<RectTransform>();
        eaRect.anchorMin = new Vector2(0f, 0f);
        eaRect.anchorMax = new Vector2(1f, 0.35f);
        eaRect.offsetMin = Vector2.zero;
        eaRect.offsetMax = Vector2.zero;
        
        CreateEquipSlot(equipArea.transform, "Mũ", 0.05f, equipHelmetSprite);
        CreateEquipSlot(equipArea.transform, "Giáp", 0.3f, equipArmorSprite);
        CreateEquipSlot(equipArea.transform, "Vũ Khí", 0.55f, equipWeaponSprite);
        CreateEquipSlot(equipArea.transform, "Giày", 0.8f, equipBootsSprite);

        return panel;
    }

    TextMeshProUGUI CreateFallbackLabel(Transform parent, string label, int index, Color textColor)
    {
        GameObject textObject = new GameObject(label, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        // Adjust anchors to fit in the top part of the right column (Y from 0.4 to 0.8)
        float startY = 0.7f - index * 0.1f;
        rectTransform.anchorMin = new Vector2(0.05f, startY);
        rectTransform.anchorMax = new Vector2(0.95f, startY + 0.1f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.fontSize = 20f;
        text.color = textColor;
        text.text = label;
        text.alignment = TextAlignmentOptions.MidlineLeft;

        return text;
    }
    
    void CreateEquipSlot(Transform parent, string slotName, float xPos, Sprite icon)
    {
        float width = 0.15f;
        
        // Slot BG
        GameObject slotBg = new GameObject(slotName + "_Slot", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        slotBg.transform.SetParent(parent, false);
        RectTransform bgRect = slotBg.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(xPos, 0.2f);
        bgRect.anchorMax = new Vector2(xPos + width, 0.8f);
        bgRect.offsetMin = Vector2.zero; bgRect.offsetMax = Vector2.zero;
        Image bgImg = slotBg.GetComponent<Image>();
        bgImg.color = new Color(0.1f, 0.12f, 0.18f, 1f);
        
        // Border
        GameObject border = new GameObject("Border", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        border.transform.SetParent(slotBg.transform, false);
        RectTransform borderRect = border.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero; borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = new Vector2(-2, -2); borderRect.offsetMax = new Vector2(2, 2);
        Image borderImg = border.GetComponent<Image>();
        borderImg.color = new Color(0.2f, 1f, 0.6f, 0.5f);
        border.transform.SetSiblingIndex(0); // Move behind BG
        
        // Icon
        GameObject iconGO = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        iconGO.transform.SetParent(slotBg.transform, false);
        RectTransform iconRect = iconGO.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.1f, 0.1f);
        iconRect.anchorMax = new Vector2(0.9f, 0.9f);
        iconRect.offsetMin = Vector2.zero; iconRect.offsetMax = Vector2.zero;
        Image iconImg = iconGO.GetComponent<Image>();
        
        if (icon != null)
        {
            iconImg.sprite = icon;
        }
        else
        {
            iconImg.color = new Color(0.2f, 0.2f, 0.25f, 1f); // Empty slot color
            
            // Add label text inside empty slot
            GameObject txtGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            txtGO.transform.SetParent(iconGO.transform, false);
            RectTransform txtRect = txtGO.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero; txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero; txtRect.offsetMax = Vector2.zero;
            TextMeshProUGUI txt = txtGO.GetComponent<TextMeshProUGUI>();
            txt.text = slotName;
            txt.alignment = TextAlignmentOptions.Center;
            txt.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            txt.fontSize = 14f;
        }
    }
}

