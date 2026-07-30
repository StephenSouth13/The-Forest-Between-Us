using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeBookUIController : MonoBehaviour
{
    public static RecipeBookUIController instance;

    [Header("Controls")]
    public GameObject bookPanel;
    public KeyCode toggleKey = KeyCode.L;
    public bool startHidden = true;
    public bool controlCursor = true;
    public bool playAudioOnToggle = true;

    [Header("Custom UI Asset Slots (Kéo Thả Sprite Của Bạn Vào Đây)")]
    public Sprite bookBackgroundSprite;
    public Sprite craftButtonSprite;

    [Header("UI Element References")]
    public TextMeshProUGUI selectedRecipeTitle;
    public TextMeshProUGUI selectedRecipeDesc;
    public TextMeshProUGUI selectedRecipeIngredients;
    public Button craftButton;

    private AudioSource audioSource;
    private Vector3 originalScale = Vector3.one;
    private RecipeData selectedRecipe;

    [System.Serializable]
    public class RecipeData
    {
        public string recipeName;
        public string category; // Weapon, Tool, Armor, Survival
        public string description;
        public string ingredients;
        public ItemData resultItem;
    }

    private List<RecipeData> recipesList = new List<RecipeData>();

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

        BuildRecipeDatabase();
    }

    void Start()
    {
        EnsureCanvasParent();

        if (bookPanel == null)
        {
            bookPanel = CreateFallbackBookPanel();
        }

        if (bookPanel != null)
        {
            originalScale = bookPanel.transform.localScale;
            bookPanel.SetActive(!startHidden);
        }

        if (craftButton != null) craftButton.onClick.AddListener(OnCraftButtonClicked);
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
        if (bookPanel == null) return;

        bool isOpening = !bookPanel.activeSelf;
        bookPanel.SetActive(isOpening);

        if (isOpening)
        {
            AnimateOpen();
            SelectRecipe(recipesList.Count > 0 ? recipesList[0] : null);

            if (controlCursor)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (playAudioOnToggle) PlayPageTurnSFX();
        }
        else
        {
            if (controlCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (playAudioOnToggle) PlayPageTurnSFX();
        }
    }

    void SelectRecipe(RecipeData recipe)
    {
        selectedRecipe = recipe;
        if (selectedRecipe == null) return;

        if (selectedRecipeTitle != null) selectedRecipeTitle.text = selectedRecipe.recipeName;
        if (selectedRecipeDesc != null) selectedRecipeDesc.text = selectedRecipe.description;
        if (selectedRecipeIngredients != null) selectedRecipeIngredients.text = $"<b>NGUYÊN LIỆU CẦN:</b>\n{selectedRecipe.ingredients}";
    }

    void OnCraftButtonClicked()
    {
        if (selectedRecipe == null) return;

        Debug.Log($"Crafting Item: {selectedRecipe.recipeName}");
        if (PlayerStatsManager.instance != null)
        {
            // Simple craft feedback
            PlayPageTurnSFX();
        }
    }

    void AnimateOpen()
    {
        if (bookPanel == null) return;
        bookPanel.transform.localScale = originalScale * 0.90f;
        StartCoroutine(PopScaleRoutine());
    }

    System.Collections.IEnumerator PopScaleRoutine()
    {
        float duration = 0.15f;
        float t = 0f;
        Vector3 start = originalScale * 0.90f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(t / duration);
            float eased = Mathf.Sin(progress * Mathf.PI * 0.5f);
            bookPanel.transform.localScale = Vector3.Lerp(start, originalScale, eased);
            yield return null;
        }

        bookPanel.transform.localScale = originalScale;
    }

    void PlayPageTurnSFX()
    {
        if (audioSource == null) return;
        AudioClip clip = CreatePageSFX(700f, 0.12f, 0.2f);
        audioSource.PlayOneShot(clip);
    }

    AudioClip CreatePageSFX(float freq, float duration, float volume)
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
        AudioClip clip = AudioClip.Create("PageSFX", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    void BuildRecipeDatabase()
    {
        recipesList = new List<RecipeData>
        {
            new RecipeData
            {
                recipeName = "🪵 Đuốc Tần Số",
                category = "Tool",
                description = "Xua tan sương mù độc và phát quang khu vực xung quanh đêm tối.",
                ingredients = "• 2x Gỗ\n• 1x Nhựa Phát Quang"
            },
            new RecipeData
            {
                recipeName = "🏹 Nỏ Tần Số",
                category = "Weapon",
                description = "Vũ khí tầm xa bắn ra các mũi tên tần số xua đuổi Sinh Thể Bóng Đêm.",
                ingredients = "• 3x Gỗ\n• 2x Linh Kiện Kim Loại"
            },
            new RecipeData
            {
                recipeName = "😷 Mặt Nạ Lọc Khí",
                category = "Survival",
                description = "Bảo vệ đường hô hấp khi đi vào vùng sương mù độc hại cao độ.",
                ingredients = "• 2x Vải\n• 1x Lõi Lọc Khí"
            },
            new RecipeData
            {
                recipeName = "⚡ Bẫy Tần Số",
                category = "Tool",
                description = "Đặt bẫy phát sóng làm tê liệt quái vật bóng đêm trong 5 giây.",
                ingredients = "• 2x Linh Kiện Điện Tử\n• 1x Pin Vô Tuyến"
            },
            new RecipeData
            {
                recipeName = "🛡️ Giáp Hạt Đen",
                category = "Armor",
                description = "Giáp bảo vệ làm từ vỏ hạt đen chống chịu sát thương quái càn quét.",
                ingredients = "• 4x Da Thu Thập\n• 2x Hạt Đen Mai An Tiêm"
            },
            new RecipeData
            {
                recipeName = "🧪 Thuốc Giải Độc",
                category = "Survival",
                description = "Hồi phục lập tức 50 Máu và giải trừ trạng thái nhiễm độc.",
                ingredients = "• 2x Hạt Dưa Hấu\n• 1x Thảo Dược Rừng"
            }
        };
    }

    GameObject CreateFallbackBookPanel()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return null;

        GameObject panel = new GameObject("RecipeBook_Panel (Runtime)", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.12f, 0.10f);
        panelRect.anchorMax = new Vector2(0.88f, 0.90f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image bg = panel.GetComponent<Image>();
        if (bookBackgroundSprite != null) bg.sprite = bookBackgroundSprite;
        bg.color = new Color(0.06f, 0.09f, 0.14f, 0.95f);

        // Header Title
        GameObject headerGO = new GameObject("HeaderTitle", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        headerGO.transform.SetParent(panel.transform, false);
        RectTransform headerRect = headerGO.GetComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0.05f, 0.88f);
        headerRect.anchorMax = new Vector2(0.95f, 0.97f);
        headerRect.offsetMin = Vector2.zero;
        headerRect.offsetMax = Vector2.zero;
        TextMeshProUGUI title = headerGO.GetComponent<TextMeshProUGUI>();
        title.fontSize = 24f;
        title.fontStyle = FontStyles.Bold;
        title.color = new Color(0.2f, 1f, 0.6f);
        title.text = "📖 THƯ VIỆN SÁCH HƯỚNG DẪN CHẾ TẠO (RECIPE BOOK - PHÍM L)";

        // Left Panel - Recipe Title & Desc
        GameObject leftGO = new GameObject("RecipeInfo", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        leftGO.transform.SetParent(panel.transform, false);
        RectTransform leftRect = leftGO.GetComponent<RectTransform>();
        leftRect.anchorMin = new Vector2(0.05f, 0.25f);
        leftRect.anchorMax = new Vector2(0.50f, 0.85f);
        leftRect.offsetMin = Vector2.zero;
        leftRect.offsetMax = Vector2.zero;
        selectedRecipeTitle = leftGO.GetComponent<TextMeshProUGUI>();
        selectedRecipeTitle.fontSize = 20f;
        selectedRecipeTitle.color = new Color(1f, 0.85f, 0.2f);
        selectedRecipeTitle.text = "🪵 Đuốc Tần Số";

        // Right Panel - Ingredients
        GameObject rightGO = new GameObject("RecipeIngredients", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        rightGO.transform.SetParent(panel.transform, false);
        RectTransform rightRect = rightGO.GetComponent<RectTransform>();
        rightRect.anchorMin = new Vector2(0.55f, 0.35f);
        rightRect.anchorMax = new Vector2(0.95f, 0.85f);
        rightRect.offsetMin = Vector2.zero;
        rightRect.offsetMax = Vector2.zero;
        selectedRecipeIngredients = rightGO.GetComponent<TextMeshProUGUI>();
        selectedRecipeIngredients.fontSize = 16f;
        selectedRecipeIngredients.color = Color.white;
        selectedRecipeIngredients.text = "• 2x Gỗ\n• 1x Nhựa Phát Quang";

        // Craft Button
        GameObject craftBtnGO = new GameObject("CraftButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        craftBtnGO.transform.SetParent(panel.transform, false);
        RectTransform craftRect = craftBtnGO.GetComponent<RectTransform>();
        craftRect.anchorMin = new Vector2(0.60f, 0.10f);
        craftRect.anchorMax = new Vector2(0.90f, 0.25f);
        craftRect.offsetMin = Vector2.zero;
        craftRect.offsetMax = Vector2.zero;

        Image btnImg = craftBtnGO.GetComponent<Image>();
        if (craftButtonSprite != null) btnImg.sprite = craftButtonSprite;
        btnImg.color = new Color(0.1f, 0.5f, 0.3f, 1f);

        craftButton = craftBtnGO.GetComponent<Button>();

        GameObject txtGO = new GameObject("BtnText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        txtGO.transform.SetParent(craftBtnGO.transform, false);
        RectTransform txtRect = txtGO.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.offsetMin = Vector2.zero;
        txtRect.offsetMax = Vector2.zero;

        TextMeshProUGUI btnTxt = txtGO.GetComponent<TextMeshProUGUI>();
        btnTxt.fontSize = 18f;
        btnTxt.alignment = TextAlignmentOptions.Center;
        btnTxt.color = Color.white;
        btnTxt.text = "⚡ CHẾ TẠO (CRAFT)";

        return panel;
    }
}
