using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CircularSurvivalHUD : MonoBehaviour
{
    public static CircularSurvivalHUD instance;

    [Header("Position & Persistence")]
    public bool isPersistentAcrossScenes = true;

    [Header("Custom UI Asset Slots (Kéo Thả Sprite Của Bạn Vào Đây)")]
    public Sprite hudFrameSprite;
    public Sprite healthArcSprite;
    public Sprite staminaArcSprite;
    public Sprite badgeHealthSprite;
    public Sprite badgeThirstSprite;
    public Sprite badgeHungerSprite;
    public Sprite badgeSleepSprite;
    public Sprite compassPointerSprite;
    public Sprite questPanelBackgroundSprite;

    [Header("UI Element References")]
    public RectTransform hudMainContainer;
    public Image healthArcFill;
    public Image staminaArcFill;
    public Image thirstArcFill;
    public Image hungerArcFill;
    public Image sleepArcFill;

    public Image badgeHealthIcon;
    public Image badgeThirstIcon;
    public Image badgeHungerIcon;
    public Image badgeSleepIcon;

    public RectTransform compassNeedle;
    public TextMeshProUGUI questTitleText;
    public TextMeshProUGUI questDescText;
    public CanvasGroup questPanelGroup;

    private Transform playerTransform;
    private StringBuilder stringBuilder = new StringBuilder(128);

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                transform.SetParent(canvas.transform, false);
            }
        }

        if (isPersistentAcrossScenes)
        {
            if (canvas != null && canvas.transform.parent == null)
            {
                DontDestroyOnLoad(canvas.gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }

    void Start()
    {
        FindPlayer();
        BuildAAAUIStructure();
    }

    void FindPlayer()
    {
        if (playerTransform == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) playerTransform = p.transform;
            else if (Camera.main != null) playerTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) FindPlayer();

        UpdateVitalsFromManager();
        UpdateCompass();
    }

    void UpdateVitalsFromManager()
    {
        PlayerStatsManager stats = PlayerStatsManager.instance;
        if (stats == null) return;

        // Health Arc (Red)
        if (healthArcFill != null)
            healthArcFill.fillAmount = Mathf.Lerp(healthArcFill.fillAmount, stats.currentHealth / stats.maxHealth, Time.deltaTime * 10f);

        // Stamina Arc (Cyan)
        if (staminaArcFill != null)
            staminaArcFill.fillAmount = Mathf.Lerp(staminaArcFill.fillAmount, stats.currentStamina / stats.maxStamina, Time.deltaTime * 12f);

        // Thirst Fill (Blue)
        if (thirstArcFill != null)
            thirstArcFill.fillAmount = Mathf.Lerp(thirstArcFill.fillAmount, stats.currentThirst / stats.maxThirst, Time.deltaTime * 5f);

        // Hunger Fill (Amber)
        if (hungerArcFill != null)
            hungerArcFill.fillAmount = Mathf.Lerp(hungerArcFill.fillAmount, stats.currentHunger / stats.maxHunger, Time.deltaTime * 5f);

        // Sleep Fill (Purple)
        if (sleepArcFill != null)
            sleepArcFill.fillAmount = Mathf.Lerp(sleepArcFill.fillAmount, stats.currentSleep / stats.maxSleep, Time.deltaTime * 5f);

        // Warning Pulse for Low Vitals (< 25%)
        float pulse = Mathf.Sin(Time.time * 7f) * 0.5f + 0.5f;

        if (badgeHealthIcon != null)
            badgeHealthIcon.color = (stats.currentHealth < 25f) ? Color.Lerp(Color.red, Color.white, pulse) : new Color(1f, 0.23f, 0.19f);

        if (badgeThirstIcon != null)
            badgeThirstIcon.color = (stats.currentThirst < 25f) ? Color.Lerp(Color.red, Color.cyan, pulse) : new Color(0f, 0.9f, 1f);

        if (badgeHungerIcon != null)
            badgeHungerIcon.color = (stats.currentHunger < 25f) ? Color.Lerp(Color.red, Color.yellow, pulse) : new Color(1f, 0.58f, 0f);

        if (badgeSleepIcon != null)
            badgeSleepIcon.color = (stats.currentSleep < 25f) ? Color.Lerp(Color.red, new Color(0.68f, 0.32f, 0.87f), pulse) : new Color(0.68f, 0.32f, 0.87f);
    }

    void UpdateCompass()
    {
        if (playerTransform == null || compassNeedle == null) return;
        float yAngle = playerTransform.eulerAngles.y;
        compassNeedle.localRotation = Quaternion.Euler(0f, 0f, -yAngle);
    }

    public void SetQuestObjective(string title, string description)
    {
        if (questTitleText != null) questTitleText.text = title;
        if (questDescText != null) questDescText.text = description;
    }

    void BuildAAAUIStructure()
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

        Transform existing = canvas.transform.Find("SonsOfForest_HUD_Container");
        if (existing != null) return;

        // Parent Container (Bottom-Right Anchor)
        GameObject containerGO = new GameObject("SonsOfForest_HUD_Container", typeof(RectTransform));
        containerGO.transform.SetParent(canvas.transform, false);

        hudMainContainer = containerGO.GetComponent<RectTransform>();
        hudMainContainer.anchorMin = new Vector2(0.74f, 0.03f); // BOTTOM-RIGHT SIDE!
        hudMainContainer.anchorMax = new Vector2(0.97f, 0.35f);
        hudMainContainer.offsetMin = Vector2.zero;
        hudMainContainer.offsetMax = Vector2.zero;

        // Dark Glassmorphic Ring Outer Frame
        GameObject frameGO = new GameObject("HUD_GlassFrame", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        frameGO.transform.SetParent(containerGO.transform, false);
        RectTransform fRect = frameGO.GetComponent<RectTransform>();
        fRect.anchorMin = Vector2.zero;
        fRect.anchorMax = Vector2.one;
        fRect.offsetMin = Vector2.zero;
        fRect.offsetMax = Vector2.zero;

        Image fImg = frameGO.GetComponent<Image>();
        fImg.sprite = GenerateCircleSprite(256, true);
        fImg.color = new Color(0.04f, 0.07f, 0.11f, 0.88f);

        // Circular Arc Fills
        healthArcFill = CreateArcImage(containerGO.transform, "HealthArc", new Color(1f, 0.23f, 0.19f, 0.9f), 0.75f, 0.95f);
        staminaArcFill = CreateArcImage(containerGO.transform, "StaminaArc", new Color(0f, 0.9f, 1f, 0.9f), 0.55f, 0.73f);

        // Center Compass & Player Arrow Pointer
        GameObject pointerGO = new GameObject("CompassPointer", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        pointerGO.transform.SetParent(containerGO.transform, false);
        compassNeedle = pointerGO.GetComponent<RectTransform>();
        compassNeedle.anchorMin = new Vector2(0.35f, 0.35f);
        compassNeedle.anchorMax = new Vector2(0.65f, 0.65f);
        compassNeedle.offsetMin = Vector2.zero;
        compassNeedle.offsetMax = Vector2.zero;

        TextMeshProUGUI pText = pointerGO.GetComponent<TextMeshProUGUI>();
        pText.fontSize = 26f;
        pText.alignment = TextAlignmentOptions.Center;
        pText.color = new Color(1f, 0.85f, 0.2f);
        pText.text = "▲";

        // Top-Right Curved Badges
        CreateBadgeIcon(containerGO.transform, "Badge_Health", "💪", new Vector2(0.12f, 0.88f), new Color(1f, 0.23f, 0.19f), out badgeHealthIcon);
        CreateBadgeIcon(containerGO.transform, "Badge_Thirst", "💧", new Vector2(0.37f, 0.96f), new Color(0f, 0.9f, 1f), out badgeThirstIcon);
        CreateBadgeIcon(containerGO.transform, "Badge_Hunger", "🍗", new Vector2(0.63f, 0.96f), new Color(1f, 0.58f, 0f), out badgeHungerIcon);
        CreateBadgeIcon(containerGO.transform, "Badge_Sleep", "🌙", new Vector2(0.88f, 0.88f), new Color(0.68f, 0.32f, 0.87f), out badgeSleepIcon);

        // AAA Quest Objective Banner (Top-Left Position)
        BuildQuestBanner(canvas.transform);
    }

    void BuildQuestBanner(Transform canvasTransform)
    {
        GameObject qPanel = new GameObject("AAA_QuestBannerPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
        qPanel.transform.SetParent(canvasTransform, false);

        RectTransform qRect = qPanel.GetComponent<RectTransform>();
        qRect.anchorMin = new Vector2(0.02f, 0.84f);
        qRect.anchorMax = new Vector2(0.36f, 0.97f);
        qRect.offsetMin = Vector2.zero;
        qRect.offsetMax = Vector2.zero;

        Image bg = qPanel.GetComponent<Image>();
        bg.color = new Color(0.05f, 0.08f, 0.14f, 0.90f);

        questPanelGroup = qPanel.GetComponent<CanvasGroup>();

        // Quest Title Header
        GameObject titleGO = new GameObject("QuestTitle", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        titleGO.transform.SetParent(qPanel.transform, false);
        RectTransform tRect = titleGO.GetComponent<RectTransform>();
        tRect.anchorMin = new Vector2(0.04f, 0.55f);
        tRect.anchorMax = new Vector2(0.96f, 0.95f);
        tRect.offsetMin = Vector2.zero;
        tRect.offsetMax = Vector2.zero;

        questTitleText = titleGO.GetComponent<TextMeshProUGUI>();
        questTitleText.fontSize = 18f;
        questTitleText.fontStyle = FontStyles.Bold;
        questTitleText.color = new Color(0.2f, 1f, 0.6f);
        questTitleText.text = "🎯 HỒI 1: KHỞI ĐẦU BÍ ẨN";

        // Quest Description
        GameObject descGO = new GameObject("QuestDesc", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        descGO.transform.SetParent(qPanel.transform, false);
        RectTransform dRect = descGO.GetComponent<RectTransform>();
        dRect.anchorMin = new Vector2(0.04f, 0.05f);
        dRect.anchorMax = new Vector2(0.96f, 0.52f);
        dRect.offsetMin = Vector2.zero;
        dRect.offsetMax = Vector2.zero;

        questDescText = descGO.GetComponent<TextMeshProUGUI>();
        questDescText.fontSize = 14f;
        questDescText.color = Color.white;
        questDescText.text = "Tìm đài Radio SM_Radio bị nhiễu sóng trong rừng.";
    }

    Image CreateArcImage(Transform parent, string name, Color color, float innerRatio, float outerRatio)
    {
        GameObject arcGO = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        arcGO.transform.SetParent(parent, false);

        RectTransform rect = arcGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.05f, 0.05f);
        rect.anchorMax = new Vector2(0.95f, 0.95f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image img = arcGO.GetComponent<Image>();
        img.sprite = GenerateCircleSprite(256, false, innerRatio);
        img.type = Image.Type.Filled;
        img.fillMethod = Image.FillMethod.Radial360;
        img.fillOrigin = (int)Image.Origin360.Top;
        img.color = color;

        return img;
    }

    void CreateBadgeIcon(Transform parent, string name, string symbol, Vector2 anchorPos, Color color, out Image iconRef)
    {
        GameObject badgeGO = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        badgeGO.transform.SetParent(parent, false);

        RectTransform rect = badgeGO.GetComponent<RectTransform>();
        rect.anchorMin = anchorPos - new Vector2(0.09f, 0.09f);
        rect.anchorMax = anchorPos + new Vector2(0.09f, 0.09f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        iconRef = badgeGO.GetComponent<Image>();
        iconRef.color = color;
        iconRef.sprite = GenerateCircleSprite(64, true);

        GameObject textGO = new GameObject("Symbol", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textGO.transform.SetParent(badgeGO.transform, false);
        RectTransform tRect = textGO.GetComponent<RectTransform>();
        tRect.anchorMin = Vector2.zero;
        tRect.anchorMax = Vector2.one;
        tRect.offsetMin = Vector2.zero;
        tRect.offsetMax = Vector2.zero;

        TextMeshProUGUI txt = textGO.GetComponent<TextMeshProUGUI>();
        txt.fontSize = 15f;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.white;
        txt.text = symbol;
    }

    // Procedural High-Res Circle Sprite Generator for Crisp Vector Curves without Asset Dependencies
    static Sprite GenerateCircleSprite(int size, bool solid, float innerRatio = 0f)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float outerRadius = size / 2f - 2f;
        float innerRadius = outerRadius * innerRatio;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                float alpha = 0f;

                if (solid)
                {
                    if (dist <= outerRadius)
                        alpha = Mathf.Clamp01(outerRadius - dist);
                }
                else
                {
                    if (dist <= outerRadius && dist >= innerRadius)
                    {
                        alpha = Mathf.Min(Mathf.Clamp01(outerRadius - dist), Mathf.Clamp01(dist - innerRadius));
                    }
                }

                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
