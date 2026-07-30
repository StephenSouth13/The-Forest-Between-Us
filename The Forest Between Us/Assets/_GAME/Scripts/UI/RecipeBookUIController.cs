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
    public Sprite recipeButtonSprite;
    public Sprite craftButtonSprite;

    [Header("UI Element References")]
    public TextMeshProUGUI selectedRecipeTitle;
    public TextMeshProUGUI selectedRecipeCategory;
    public TextMeshProUGUI selectedRecipeDesc;
    public TextMeshProUGUI selectedRecipeIngredients;
    public Button craftButton;
    public Transform recipeListContainer;

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

        PopulateRecipeList();
        if (recipesList.Count > 0) SelectRecipe(recipesList[0]);

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
            SelectRecipe(selectedRecipe ?? (recipesList.Count > 0 ? recipesList[0] : null));

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

    public void SelectRecipe(RecipeData recipe)
    {
        selectedRecipe = recipe;
        if (selectedRecipe == null) return;

        if (selectedRecipeTitle != null) selectedRecipeTitle.text = selectedRecipe.recipeName;
        if (selectedRecipeCategory != null) selectedRecipeCategory.text = $"DANH MỤC: {selectedRecipe.category.ToUpper()}";
        if (selectedRecipeDesc != null) selectedRecipeDesc.text = selectedRecipe.description;
        if (selectedRecipeIngredients != null) selectedRecipeIngredients.text = $"<b>NGUYÊN LIỆU CẦN CHẾ TẠO:</b>\n{selectedRecipe.ingredients}";
    }

    void OnCraftButtonClicked()
    {
        if (selectedRecipe == null) return;

        Debug.Log($"[Crafting] Successfully Crafted: {selectedRecipe.recipeName}");
        if (playAudioOnToggle) PlayPageTurnSFX();
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
                recipeName = "🍲 Nồi Nấu Ăn (Cooking Pot)",
                category = "Dụng Cụ Nấu Nướng",
                description = "Nồi kim loại đặt lên đống lửa trại để mở khóa các công thức hầm súp và nấu ăn cao cấp.",
                ingredients = "• 3x Đá Cuội\n• 2x Thanh Kim Loại / Sắt"
            },
            new RecipeData
            {
                recipeName = "🥩 Thịt Hầm Thảo Mộc",
                category = "Ẩm Thực (+75 Đói)",
                description = "Món thịt hầm dinh dưỡng cao chế biến trong Nồi Nấu. Hồi +75 Đói, +45 Máu, +30 Thể Lực.",
                ingredients = "• 1x Thịt Sống\n• 1x Nước Suối\n• 1x Thảo Dược Rừng\n• 🛠️ Yêu cầu: Nồi Nấu Ăn"
            },
            new RecipeData
            {
                recipeName = "🥣 Súp Nấm & Rau Rừng",
                category = "Ẩm Thực (+40 Đói)",
                description = "Súp rau nóng hổi thanh lọc cơ thể. Hồi +40 Đói, +35 Khát, +25 Máu.",
                ingredients = "• 1x Nước Suối\n• 2x Nấm Rừng\n• 1x Thảo Dược Rừng\n• 🛠️ Yêu cầu: Nồi Nấu Ăn"
            },
            new RecipeData
            {
                recipeName = "🍵 Trà Thảo Dược Giải Độc",
                category = "Ẩm Thực (+50 Khát)",
                description = "Trà thảo dược đun sôi giúp giải sạch độc ngộ độc thực phẩm và hồi +50 Khát, +30 Máu.",
                ingredients = "• 1x Nước Suối\n• 2x Thảo Dược Rừng\n• 🛠️ Yêu cầu: Nồi Nấu Ăn"
            },
            new RecipeData
            {
                recipeName = "💊 Thuốc Kháng Sinh Tự Chế (Antibiotic)",
                category = "Y Tế & Thuốc",
                description = "Thuốc kháng sinh cô đọng từ dược liệu rừng. ĐẶC TRỊ CHỮA KHỎI BỆNH SỐT RÉT DO MUỖI ĐỐT & HỒI +35 MÁU.",
                ingredients = "• 2x Thảo Dược Rừng\n• 1x Nấm Rừng Tươi\n• 1x Nước Suối\n• 🛠️ Yêu cầu: Nồi Nấu Ăn"
            },
            new RecipeData
            {
                recipeName = "🍵 Trà Cảm Cúm Gừng & Thảo Dược",
                category = "Y Tế & Thuốc",
                description = "Trà nóng giải cảm lạnh. CHỮA KHỎI BỆNH CẢM CÚM, HỒI +25 THỂ LỰC & +20 MÁU.",
                ingredients = "• 2x Thảo Dược Rừng\n• 1x Trái Cây Rừng\n• 1x Nước Suối\n• 🛠️ Yêu cầu: Nồi Nấu Ăn"
            },
            new RecipeData
            {
                recipeName = "🧴 Thuốc Bôi Chống Muỗi (Repellent)",
                category = "Y Tế & Thuốc",
                description = "Thuốc cao bôi thảo dược. BẢO VỆ NHÂN VẬT KHỎI MUỖI ĐỐT TRONG 5 PHÚT (300 giây).",
                ingredients = "• 2x Thảo Dược Rừng\n• 1x Nhựa Cây"
            },
            new RecipeData
            {
                recipeName = "🍖 Thịt Nướng Chín Sốt Nấm",
                category = "Ẩm Thực (+60 Đói)",
                description = "Thịt nướng chín thơm nức trên lửa trại. Hồi +60 Đói, +30 Máu, không gây ngộ độc.",
                ingredients = "• 1x Thịt Sống\n• 1x Nấm Rừng\n• 🛠️ Yêu cầu: Đống Lửa Trại"
            },
            new RecipeData
            {
                recipeName = "🪵 Đuốc Tần Số",
                category = "Dụng Cụ",
                description = "Xua tan sương mù độc và phát quang khu vực xung quanh đêm tối.",
                ingredients = "• 2x Gỗ\n• 1x Nhựa Phát Quang"
            },
            new RecipeData
            {
                recipeName = "🏹 Nỏ Tần Số",
                category = "Vũ Khí",
                description = "Vũ khí tầm xa bắn ra các mũi tên tần số xua đuổi Sinh Thể Bóng Đêm.",
                ingredients = "• 3x Gỗ\n• 2x Linh Kiện Kim Loại"
            },
            new RecipeData
            {
                recipeName = "😷 Mặt Nạ Lọc Khí",
                category = "Sinh Tồn",
                description = "Bảo vệ đường hô hấp khi đi vào vùng sương mù độc hại cao độ.",
                ingredients = "• 2x Vải\n• 1x Lõi Lọc Khí"
            },
            new RecipeData
            {
                recipeName = "⚡ Bẫy Tần Số",
                category = "Dụng Cụ",
                description = "Đặt bẫy phát sóng làm tê liệt quái vật bóng đêm trong 5 giây.",
                ingredients = "• 2x Linh Kiện Điện Tử\n• 1x Pin Vô Tuyến"
            },
            new RecipeData
            {
                recipeName = "🛡️ Giáp Hạt Đen",
                category = "Giáp",
                description = "Giáp bảo vệ làm từ vỏ hạt đen chống chịu sát thương quái càn quét.",
                ingredients = "• 4x Da Thu Thập\n• 2x Hạt Đen Mai An Tiêm"
            },
            new RecipeData
            {
                recipeName = "🧪 Thuốc Giải Độc",
                category = "Thuốc",
                description = "Hồi phục lập tức 50 Máu và giải trừ trạng thái nhiễm độc.",
                ingredients = "• 2x Hạt Dưa Hấu\n• 1x Thảo Dược Rừng"
            }
        };
    }

    void PopulateRecipeList()
    {
        if (recipeListContainer == null) return;

        // Clear existing children
        foreach (Transform child in recipeListContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (RecipeData recipe in recipesList)
        {
            RecipeData r = recipe;
            GameObject itemGO = new GameObject(r.recipeName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            itemGO.transform.SetParent(recipeListContainer, false);

            RectTransform rTransform = itemGO.GetComponent<RectTransform>();
            rTransform.sizeDelta = new Vector2(0, 42);

            Image img = itemGO.GetComponent<Image>();
            if (recipeButtonSprite != null) img.sprite = recipeButtonSprite;
            img.color = new Color(0.12f, 0.18f, 0.25f, 0.9f);

            GameObject textGO = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            textGO.transform.SetParent(itemGO.transform, false);
            RectTransform txtRect = textGO.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = new Vector2(10, 0);
            txtRect.offsetMax = new Vector2(-10, 0);

            TextMeshProUGUI txt = textGO.GetComponent<TextMeshProUGUI>();
            txt.fontSize = 16f;
            txt.alignment = TextAlignmentOptions.Left;
            txt.color = Color.white;
            txt.text = r.recipeName;

            Button btn = itemGO.GetComponent<Button>();
            btn.onClick.AddListener(() => SelectRecipe(r));
        }
    }

    GameObject CreateFallbackBookPanel()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return null;

        // 2-Page Book Container
        GameObject panel = new GameObject("RecipeBook_Panel (Runtime)", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.10f, 0.08f);
        panelRect.anchorMax = new Vector2(0.90f, 0.92f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image bg = panel.GetComponent<Image>();
        if (bookBackgroundSprite != null) bg.sprite = bookBackgroundSprite;
        bg.color = new Color(0.05f, 0.08f, 0.13f, 0.96f); // Sleek Cyber Charcoal Book

        // Header Title
        GameObject headerGO = new GameObject("HeaderTitle", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        headerGO.transform.SetParent(panel.transform, false);
        RectTransform headerRect = headerGO.GetComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0.04f, 0.90f);
        headerRect.anchorMax = new Vector2(0.96f, 0.98f);
        headerRect.offsetMin = Vector2.zero;
        headerRect.offsetMax = Vector2.zero;

        TextMeshProUGUI title = headerGO.GetComponent<TextMeshProUGUI>();
        title.fontSize = 22f;
        title.fontStyle = FontStyles.Bold;
        title.color = new Color(0.2f, 1f, 0.6f);
        title.text = "📖 THƯ VIỆN SÁCH HƯỚNG DẪN CHẾ TẠO (RECIPE BOOK - PHÍM L)";

        // LEFT PAGE - Scrollable List Container
        GameObject leftPageGO = new GameObject("LeftPage_List", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(VerticalLayoutGroup));
        leftPageGO.transform.SetParent(panel.transform, false);
        RectTransform leftRect = leftPageGO.GetComponent<RectTransform>();
        leftRect.anchorMin = new Vector2(0.04f, 0.06f);
        leftRect.anchorMax = new Vector2(0.48f, 0.88f);
        leftRect.offsetMin = Vector2.zero;
        leftRect.offsetMax = Vector2.zero;

        leftPageGO.GetComponent<Image>().color = new Color(0.03f, 0.05f, 0.08f, 0.5f);
        recipeListContainer = leftPageGO.transform;

        VerticalLayoutGroup layout = leftPageGO.GetComponent<VerticalLayoutGroup>();
        layout.spacing = 8;
        layout.padding = new RectOffset(10, 10, 10, 10);
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;

        // RIGHT PAGE - Detailed Recipe View
        GameObject rightPageGO = new GameObject("RightPage_Details", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        rightPageGO.transform.SetParent(panel.transform, false);
        RectTransform rightRect = rightPageGO.GetComponent<RectTransform>();
        rightRect.anchorMin = new Vector2(0.52f, 0.06f);
        rightRect.anchorMax = new Vector2(0.96f, 0.88f);
        rightRect.offsetMin = Vector2.zero;
        rightRect.offsetMax = Vector2.zero;
        rightPageGO.GetComponent<Image>().color = new Color(0.03f, 0.05f, 0.08f, 0.5f);

        // Right Title
        GameObject rTitleGO = new GameObject("RecipeTitle", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        rTitleGO.transform.SetParent(rightPageGO.transform, false);
        RectTransform rTitleRect = rTitleGO.GetComponent<RectTransform>();
        rTitleRect.anchorMin = new Vector2(0.05f, 0.82f);
        rTitleRect.anchorMax = new Vector2(0.95f, 0.95f);
        rTitleRect.offsetMin = Vector2.zero;
        rTitleRect.offsetMax = Vector2.zero;

        selectedRecipeTitle = rTitleGO.GetComponent<TextMeshProUGUI>();
        selectedRecipeTitle.fontSize = 24f;
        selectedRecipeTitle.fontStyle = FontStyles.Bold;
        selectedRecipeTitle.color = new Color(1f, 0.85f, 0.2f);
        selectedRecipeTitle.text = "🪵 Đuốc Tần Số";

        // Right Category
        GameObject catGO = new GameObject("RecipeCategory", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        catGO.transform.SetParent(rightPageGO.transform, false);
        RectTransform catRect = catGO.GetComponent<RectTransform>();
        catRect.anchorMin = new Vector2(0.05f, 0.72f);
        catRect.anchorMax = new Vector2(0.95f, 0.82f);
        catRect.offsetMin = Vector2.zero;
        catRect.offsetMax = Vector2.zero;

        selectedRecipeCategory = catGO.GetComponent<TextMeshProUGUI>();
        selectedRecipeCategory.fontSize = 14f;
        selectedRecipeCategory.color = new Color(0.2f, 1f, 0.6f);
        selectedRecipeCategory.text = "DANH MỤC: DỤNG CỤ";

        // Right Desc
        GameObject descGO = new GameObject("RecipeDesc", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        descGO.transform.SetParent(rightPageGO.transform, false);
        RectTransform descRect = descGO.GetComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0.05f, 0.48f);
        descRect.anchorMax = new Vector2(0.95f, 0.70f);
        descRect.offsetMin = Vector2.zero;
        descRect.offsetMax = Vector2.zero;

        selectedRecipeDesc = descGO.GetComponent<TextMeshProUGUI>();
        selectedRecipeDesc.fontSize = 15f;
        selectedRecipeDesc.color = Color.white;
        selectedRecipeDesc.text = "Xua tan sương mù độc và phát quang khu vực xung quanh.";

        // Right Ingredients
        GameObject ingGO = new GameObject("RecipeIngredients", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        ingGO.transform.SetParent(rightPageGO.transform, false);
        RectTransform ingRect = ingGO.GetComponent<RectTransform>();
        ingRect.anchorMin = new Vector2(0.05f, 0.22f);
        ingRect.anchorMax = new Vector2(0.95f, 0.46f);
        ingRect.offsetMin = Vector2.zero;
        ingRect.offsetMax = Vector2.zero;

        selectedRecipeIngredients = ingGO.GetComponent<TextMeshProUGUI>();
        selectedRecipeIngredients.fontSize = 16f;
        selectedRecipeIngredients.color = new Color(0.9f, 0.9f, 0.9f);
        selectedRecipeIngredients.text = "• 2x Gỗ\n• 1x Nhựa Phát Quang";

        // Craft Button
        GameObject craftBtnGO = new GameObject("CraftButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        craftBtnGO.transform.SetParent(rightPageGO.transform, false);
        RectTransform craftRect = craftBtnGO.GetComponent<RectTransform>();
        craftRect.anchorMin = new Vector2(0.20f, 0.05f);
        craftRect.anchorMax = new Vector2(0.80f, 0.18f);
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
        btnTxt.fontStyle = FontStyles.Bold;
        btnTxt.alignment = TextAlignmentOptions.Center;
        btnTxt.color = Color.white;
        btnTxt.text = "⚡ CHẾ TẠO (CRAFT)";

        return panel;
    }
}
