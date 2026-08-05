using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// MainMenuController – Toàn bộ main menu được tạo bằng code (không cần prefab).
/// Kéo script này vào bất kỳ GameObject nào trong scene Home.
/// Bao gồm: nhạc nền procedural, SFX, animation, New Game, Continue,
/// Settings, About, HowToPlay – tất cả hoàn chỉnh, sẵn sàng sử dụng.
/// </summary>
public class Menu : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────────
    //  Inspector Fields
    // ─────────────────────────────────────────────────────────────────
    [Header("Scene Names")]
    public string gameplaySceneName = "GamePlay";

    [Header("Background Music (để trống to generate procedural)")]
    public AudioClip menuMusicClip;

    [Header("UI Sound FX (để trống to generate tone)")]
    public AudioClip buttonClickSFX;
    public AudioClip buttonHoverSFX;
    public AudioClip panelOpenSFX;
    public AudioClip panelCloseSFX;

    // ─────────────────────────────────────────────────────────────────
    //  Private State
    // ─────────────────────────────────────────────────────────────────
    private Canvas       _canvas;
    private AudioSource  _musicSource;
    private AudioSource  _sfxSource;

    private GameObject   _mainPanel;
    private GameObject   _settingsPanel;
    private GameObject   _aboutPanel;
    private GameObject   _howToPlayPanel;
    private GameObject   _newGameConfirmPanel;
    private GameObject   _loadingOverlay;

    private Slider          _masterSlider;
    private Slider          _musicSlider;
    private Slider          _sfxSlider;
    private TextMeshProUGUI _masterLabel;
    private TextMeshProUGUI _musicLabel;
    private TextMeshProUGUI _sfxLabel;

    private bool _hasSave;
    private int  _savedDay;
    private int  _savedKarma;

    private readonly List<ParticleData> _particles = new List<ParticleData>();
    private struct ParticleData { public RectTransform rt; public float speed; public float drift; }

    // ─────────────────────────────────────────────────────────────────
    //  Colour Palette
    // ─────────────────────────────────────────────────────────────────
    static readonly Color C_BG_DARK    = new Color(0.04f, 0.07f, 0.12f, 1f);
    static readonly Color C_ACCENT     = new Color(0.22f, 0.85f, 0.55f, 1f);
    static readonly Color C_ACCENT2    = new Color(1.00f, 0.78f, 0.20f, 1f);
    static readonly Color C_TEXT_MAIN  = new Color(0.92f, 0.95f, 0.98f, 1f);
    static readonly Color C_TEXT_DIM   = new Color(0.55f, 0.65f, 0.75f, 1f);
    static readonly Color C_PANEL_BG   = new Color(0.06f, 0.10f, 0.17f, 0.97f);
    static readonly Color C_BTN_NORMAL = new Color(0.10f, 0.20f, 0.32f, 1f);
    static readonly Color C_BTN_DANGER = new Color(0.55f, 0.10f, 0.10f, 1f);
    static readonly Color C_SEPARATOR  = new Color(0.22f, 0.85f, 0.55f, 0.25f);

    // ─────────────────────────────────────────────────────────────────
    //  Lifecycle
    // ─────────────────────────────────────────────────────────────────
    void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
        Time.timeScale   = 1f;

        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop         = true;
        _musicSource.playOnAwake  = false;
        _musicSource.spatialBlend = 0f;
        _musicSource.volume       = PlayerPrefs.GetFloat("Menu_MusicVol", 0.5f);

        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.playOnAwake  = false;
        _sfxSource.spatialBlend = 0f;
        _sfxSource.volume       = PlayerPrefs.GetFloat("Menu_SfxVol", 0.8f);

        _hasSave = SaveSystem.LoadProgress(out _savedDay, out _savedKarma, out _);
    }

    void Start()
    {
        BuildCanvas();
        BuildBackground();
        BuildParticleLayer();
        BuildMainPanel();
        BuildSettingsPanel();
        BuildAboutPanel();
        BuildHowToPlayPanel();
        BuildNewGameConfirmPanel();
        BuildLoadingOverlay();

        ShowPanel(null);

        StartCoroutine(PlayMenuMusic());
        StartCoroutine(AnimateParticles());
        StartCoroutine(PulseTitle());
    }

    // ─────────────────────────────────────────────────────────────────
    //  Canvas
    // ─────────────────────────────────────────────────────────────────
    void BuildCanvas()
    {
        var cGO = new GameObject("MainMenu_Canvas");
        _canvas = cGO.AddComponent<Canvas>();
        _canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 100;
        var scaler = cGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        cGO.AddComponent<GraphicRaycaster>();
    }

    // ─────────────────────────────────────────────────────────────────
    //  Background + Vignette
    // ─────────────────────────────────────────────────────────────────
    void BuildBackground()
    {
        var bgGO = MakeImage("BG_Gradient", _canvas.transform, Vector2.zero, Vector2.one, C_BG_DARK);
        bgGO.GetComponent<Image>().sprite = GenerateGradientSprite();
        var vigGO = MakeImage("BG_Vignette", _canvas.transform, Vector2.zero, Vector2.one, Color.clear);
        vigGO.GetComponent<Image>().sprite = GenerateVignetteSprite();
    }

    // ─────────────────────────────────────────────────────────────────
    //  Firefly Particles
    // ─────────────────────────────────────────────────────────────────
    void BuildParticleLayer()
    {
        var layer = new GameObject("ParticleLayer", typeof(RectTransform));
        layer.transform.SetParent(_canvas.transform, false);
        StretchFill(layer.GetComponent<RectTransform>());
        var rng = new System.Random(42);
        for (int i = 0; i < 40; i++)
        {
            float size = (float)(rng.NextDouble() * 6 + 2);
            Color col  = i % 3 == 0 ? C_ACCENT : i % 3 == 1 ? C_ACCENT2 : C_TEXT_DIM;
            col.a = (float)(rng.NextDouble() * 0.4f + 0.1f);
            var dot = MakeImage($"P{i}", layer.transform, Vector2.zero, Vector2.one, col);
            var rt  = dot.GetComponent<RectTransform>();
            float ax = (float)rng.NextDouble();
            float ay = (float)(rng.NextDouble() * 0.5f);
            rt.anchorMin = new Vector2(ax, ay);
            rt.anchorMax = new Vector2(ax + 0.005f, ay + 0.009f);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            rt.sizeDelta = new Vector2(size, size);
            dot.GetComponent<Image>().sprite = GenerateCircleSprite();
            _particles.Add(new ParticleData
            {
                rt    = rt,
                speed = (float)(rng.NextDouble() * 0.00015f + 0.00005f),
                drift = (float)((rng.NextDouble() - 0.5) * 0.00008f)
            });
        }
    }

    // ─────────────────────────────────────────────────────────────────
    //  MAIN PANEL
    // ─────────────────────────────────────────────────────────────────
    void BuildMainPanel()
    {
        _mainPanel = new GameObject("Panel_Main", typeof(RectTransform));
        _mainPanel.transform.SetParent(_canvas.transform, false);
        StretchFill(_mainPanel.GetComponent<RectTransform>());

        // ── Logo / Title ──
        var logoArea = MakeRect("LogoArea", _mainPanel.transform,
            new Vector2(0.03f, 0.55f), new Vector2(0.52f, 0.98f));
        var frameBg = MakeImage("LogoFrame", logoArea.transform, Vector2.zero, Vector2.one,
            new Color(0.04f, 0.08f, 0.14f, 0.55f));
        frameBg.GetComponent<Image>().sprite = GenerateRoundedRectSprite(12);

        var titleGO = MakeRect("TitleTextAnimated", logoArea.transform,
            new Vector2(0.02f, 0.42f), new Vector2(0.98f, 0.90f));
        var titleTmp = titleGO.AddComponent<TextMeshProUGUI>();
        titleTmp.text      = "THE FOREST\nBETWEEN US";
        titleTmp.fontSize  = 68f;
        titleTmp.fontStyle = FontStyles.Bold;
        titleTmp.alignment = TextAlignmentOptions.Center;
        titleTmp.enableVertexGradient = true;
        titleTmp.colorGradient = new VertexGradient(
            C_ACCENT2, C_ACCENT2, C_ACCENT, new Color(0.1f, 0.55f, 0.35f));

        var subGO = MakeRect("Subtitle", logoArea.transform,
            new Vector2(0.05f, 0.28f), new Vector2(0.95f, 0.44f));
        var subTmp = subGO.AddComponent<TextMeshProUGUI>();
        subTmp.text      = "— Khu Rừng Giữa Chúng Ta —";
        subTmp.fontSize  = 22f;
        subTmp.alignment = TextAlignmentOptions.Center;
        subTmp.color     = C_TEXT_DIM;
        subTmp.fontStyle = FontStyles.Italic;

        MakeSeparator("Sep1", logoArea.transform,
            new Vector2(0.04f, 0.255f), new Vector2(0.96f, 0.270f));

        var verGO = MakeRect("Version", logoArea.transform,
            new Vector2(0.05f, 0.04f), new Vector2(0.50f, 0.16f));
        var verTmp = verGO.AddComponent<TextMeshProUGUI>();
        verTmp.text = "v1.0.0  •  Unity 6"; verTmp.fontSize = 16f;
        verTmp.color = new Color(0.35f, 0.50f, 0.60f, 0.8f);

        if (_hasSave)
        {
            var badge = MakeImage("SaveBadge", logoArea.transform,
                new Vector2(0.52f, 0.04f), new Vector2(0.98f, 0.18f),
                new Color(0.08f, 0.30f, 0.16f, 0.90f));
            badge.GetComponent<Image>().sprite = GenerateRoundedRectSprite(8);
            var bTmp = MakeRect("BadgeTxt", badge.transform, Vector2.zero, Vector2.one)
                .AddComponent<TextMeshProUGUI>();
            bTmp.text = $"💾  Ngày {_savedDay}  •  Karma {_savedKarma}";
            bTmp.fontSize = 14f; bTmp.alignment = TextAlignmentOptions.Center;
            bTmp.color = C_ACCENT;
        }

        // ── Buttons ──
        var btnArea = MakeRect("ButtonArea", _mainPanel.transform,
            new Vector2(0.54f, 0.25f), new Vector2(0.97f, 0.98f));
        var panelBg = MakeImage("BtnPanelBg", btnArea.transform,
            Vector2.zero, Vector2.one, new Color(0.05f, 0.09f, 0.15f, 0.82f));
        panelBg.GetComponent<Image>().sprite = GenerateRoundedRectSprite(18);

        const float n = 5f; const float total = 0.88f; const float gap = 0.025f;
        float bH   = (total - gap * (n - 1)) / n;
        float topY = 0.93f - bH;

        MakeMenuButton("Btn_NewGame", btnArea.transform,
            new Vector2(0.07f, topY), new Vector2(0.93f, topY + bH),
            "🌲  TRÒ CHƠI MỚI", C_BTN_NORMAL, C_ACCENT,
            () => { PlaySFX(buttonClickSFX, 800f); ShowPanel(_newGameConfirmPanel); });

        Color cc = _hasSave ? C_BTN_NORMAL : new Color(0.08f, 0.12f, 0.18f, 0.5f);
        string ct = _hasSave ? $"▶  TIẾP TỤC  (Ngày {_savedDay})" : "▶  TIẾP TỤC";
        var cBtn = MakeMenuButton("Btn_Continue", btnArea.transform,
            new Vector2(0.07f, topY - (bH + gap)), new Vector2(0.93f, topY - (bH + gap) + bH),
            ct, cc, _hasSave ? C_ACCENT2 : C_TEXT_DIM,
            () => { if (_hasSave) { PlaySFX(buttonClickSFX, 700f); ContinueGame(); } });
        if (!_hasSave) cBtn.GetComponent<Button>().interactable = false;

        MakeMenuButton("Btn_Settings", btnArea.transform,
            new Vector2(0.07f, topY - 2*(bH+gap)), new Vector2(0.93f, topY - 2*(bH+gap) + bH),
            "⚙️  CÀI ĐẶT", C_BTN_NORMAL, C_TEXT_MAIN,
            () => { PlaySFX(buttonClickSFX, 650f); ShowPanel(_settingsPanel); });

        MakeMenuButton("Btn_HowToPlay", btnArea.transform,
            new Vector2(0.07f, topY - 3*(bH+gap)), new Vector2(0.93f, topY - 3*(bH+gap) + bH),
            "📖  CÁCH CHƠI", C_BTN_NORMAL, C_TEXT_MAIN,
            () => { PlaySFX(buttonClickSFX, 650f); ShowPanel(_howToPlayPanel); });

        MakeMenuButton("Btn_About", btnArea.transform,
            new Vector2(0.07f, topY - 4*(bH+gap)), new Vector2(0.50f, topY - 4*(bH+gap) + bH),
            "ℹ  VỀ GAME", C_BTN_NORMAL, C_TEXT_DIM,
            () => { PlaySFX(buttonClickSFX, 600f); ShowPanel(_aboutPanel); });

        MakeMenuButton("Btn_Quit", btnArea.transform,
            new Vector2(0.52f, topY - 4*(bH+gap)), new Vector2(0.93f, topY - 4*(bH+gap) + bH),
            "✖  THOÁT", C_BTN_DANGER, C_TEXT_MAIN,
            () => { PlaySFX(buttonClickSFX, 400f); StartCoroutine(QuitRoutine()); });

        // ── Bottom credit strip ──
        var strip = MakeRect("CreditStrip", _mainPanel.transform,
            new Vector2(0f, 0f), new Vector2(1f, 0.055f));
        strip.AddComponent<Image>().color = new Color(0.03f, 0.05f, 0.09f, 0.92f);
        var cTmp = MakeRect("CreditTxt", strip.transform, Vector2.zero, Vector2.one)
            .AddComponent<TextMeshProUGUI>();
        cTmp.text = "© 2025  VTC Academy  •  Team Forest Between Us  •  Made with Unity 6";
        cTmp.fontSize = 15f; cTmp.alignment = TextAlignmentOptions.Center; cTmp.color = C_TEXT_DIM;
    }

    // ─────────────────────────────────────────────────────────────────
    //  NEW GAME CONFIRM
    // ─────────────────────────────────────────────────────────────────
    void BuildNewGameConfirmPanel()
    {
        _newGameConfirmPanel = BuildSidePanel("Panel_NewGameConfirm",
            new Vector2(0.22f, 0.28f), new Vector2(0.78f, 0.72f));
        BuildPanelHeader(_newGameConfirmPanel.transform, "🌲  TRÒ CHƠI MỚI");

        var bTmp = MakeRect("Body", _newGameConfirmPanel.transform,
            new Vector2(0.05f, 0.32f), new Vector2(0.95f, 0.85f))
            .AddComponent<TextMeshProUGUI>();
        bTmp.text = _hasSave
            ? "⚠️  Tiến trình đã lưu <b>sẽ bị XÓA!</b>\n\nBạn chắc chắn muốn bắt đầu\nmột hành trình mới?"
            : "Bắt đầu cuộc hành trình sinh tồn\ntrong khu rừng hoang dã bí ẩn.\n\nBạn sẵn sàng chưa?";
        bTmp.fontSize = 20f; bTmp.color = C_TEXT_MAIN; bTmp.alignment = TextAlignmentOptions.Center;

        MakeMenuButton("Btn_ConfirmNew", _newGameConfirmPanel.transform,
            new Vector2(0.06f, 0.06f), new Vector2(0.46f, 0.25f),
            "✅  BẮT ĐẦU!", C_BTN_NORMAL, C_ACCENT,
            () => { PlaySFX(buttonClickSFX, 900f); StartCoroutine(StartNewGame()); });

        MakeMenuButton("Btn_CancelNew", _newGameConfirmPanel.transform,
            new Vector2(0.54f, 0.06f), new Vector2(0.94f, 0.25f),
            "✖  HỦY", C_BTN_DANGER, C_TEXT_MAIN,
            () => { PlaySFX(panelCloseSFX, 400f); ShowPanel(null); });

        _newGameConfirmPanel.SetActive(false);
    }

    // ─────────────────────────────────────────────────────────────────
    //  SETTINGS PANEL
    // ─────────────────────────────────────────────────────────────────
    void BuildSettingsPanel()
    {
        _settingsPanel = BuildSidePanel("Panel_Settings",
            new Vector2(0.15f, 0.08f), new Vector2(0.85f, 0.92f));
        BuildPanelHeader(_settingsPanel.transform, "⚙️  CÀI ĐẶT");

        float y = 0.75f; float rowH = 0.09f; float gap = 0.015f;

        BuildSliderRow(_settingsPanel.transform, "🔊  Âm lượng tổng", ref y, rowH, gap, 0f, 1f,
            PlayerPrefs.GetFloat("Menu_MasterVol", 1f),
            v => { AudioListener.volume = v; PlayerPrefs.SetFloat("Menu_MasterVol", v); },
            ref _masterSlider, ref _masterLabel);

        BuildSliderRow(_settingsPanel.transform, "🎵  Nhạc nền", ref y, rowH, gap, 0f, 1f,
            PlayerPrefs.GetFloat("Menu_MusicVol", 0.5f),
            v => { _musicSource.volume = v; PlayerPrefs.SetFloat("Menu_MusicVol", v); },
            ref _musicSlider, ref _musicLabel);

        BuildSliderRow(_settingsPanel.transform, "🔔  Hiệu ứng âm thanh", ref y, rowH, gap, 0f, 1f,
            PlayerPrefs.GetFloat("Menu_SfxVol", 0.8f),
            v => { _sfxSource.volume = v; PlayerPrefs.SetFloat("Menu_SfxVol", v); },
            ref _sfxSlider, ref _sfxLabel);

        MakeSeparator("Sep", _settingsPanel.transform,
            new Vector2(0.05f, y), new Vector2(0.95f, y + 0.008f));
        y -= gap * 2f;

        // Fullscreen toggle button
        var fsLbl = MakeRect("FSLbl", _settingsPanel.transform,
            new Vector2(0.05f, y), new Vector2(0.70f, y + rowH)).AddComponent<TextMeshProUGUI>();
        fsLbl.text = "🖥️  Toàn màn hình"; fsLbl.fontSize = 18f;
        fsLbl.color = C_TEXT_MAIN; fsLbl.alignment = TextAlignmentOptions.MidlineLeft;

        var fsGO = MakeImage("FSBtn", _settingsPanel.transform,
            new Vector2(0.74f, y + 0.01f), new Vector2(0.93f, y + rowH - 0.01f), C_BTN_NORMAL);
        var fsBtn = fsGO.AddComponent<Button>();
        var fsTxt = MakeRect("FSTxt", fsGO.transform, Vector2.zero, Vector2.one)
            .AddComponent<TextMeshProUGUI>();
        fsTxt.alignment = TextAlignmentOptions.Center; fsTxt.fontSize = 18f;
        fsTxt.text = Screen.fullScreen ? "BẬT" : "TẮT";
        fsTxt.color = Screen.fullScreen ? C_ACCENT : C_TEXT_DIM;
        fsBtn.onClick.AddListener(() => {
            bool next = !Screen.fullScreen; Screen.fullScreen = next;
            fsTxt.text = next ? "BẬT" : "TẮT"; fsTxt.color = next ? C_ACCENT : C_TEXT_DIM;
            PlayerPrefs.SetInt("Menu_Fullscreen", next ? 1 : 0);
        });
        y -= rowH + gap;

        // Quality cycle button
        string[] qNames = { "Very Low","Low","Medium","High","Very High","Ultra" };
        int curQ = QualitySettings.GetQualityLevel();
        var qLbl = MakeRect("QLbl", _settingsPanel.transform,
            new Vector2(0.05f, y), new Vector2(0.65f, y + rowH)).AddComponent<TextMeshProUGUI>();
        qLbl.text = "🎮  Chất lượng đồ hoạ"; qLbl.fontSize = 18f;
        qLbl.color = C_TEXT_MAIN; qLbl.alignment = TextAlignmentOptions.MidlineLeft;
        var qGO = MakeImage("QBtn", _settingsPanel.transform,
            new Vector2(0.67f, y + 0.01f), new Vector2(0.93f, y + rowH - 0.01f), C_BTN_NORMAL);
        qGO.AddComponent<Button>();
        var qTxt = MakeRect("QTxt", qGO.transform, Vector2.zero, Vector2.one)
            .AddComponent<TextMeshProUGUI>();
        qTxt.text = qNames[Mathf.Clamp(curQ, 0, qNames.Length - 1)];
        qTxt.fontSize = 16f; qTxt.color = C_ACCENT; qTxt.alignment = TextAlignmentOptions.Center;
        qGO.GetComponent<Button>().onClick.AddListener(() => {
            curQ = (curQ + 1) % qNames.Length;
            QualitySettings.SetQualityLevel(curQ, true);
            qTxt.text = qNames[curQ]; PlayerPrefs.SetInt("Menu_Quality", curQ);
        });

        MakeMenuButton("Btn_ClearSave", _settingsPanel.transform,
            new Vector2(0.05f, 0.14f), new Vector2(0.47f, 0.23f),
            "🗑  XÓA TIẾN TRÌNH", C_BTN_DANGER, C_TEXT_MAIN,
            () => { SaveSystem.ClearSaveData(); _hasSave = false; Debug.Log("[Menu] Save cleared."); });

        MakeMenuButton("Btn_CloseSettings", _settingsPanel.transform,
            new Vector2(0.28f, 0.03f), new Vector2(0.72f, 0.12f),
            "← QUAY LẠI", C_BTN_NORMAL, C_ACCENT,
            () => { PlaySFX(panelCloseSFX, 500f); ShowPanel(null); });

        _settingsPanel.SetActive(false);
    }

    // ─────────────────────────────────────────────────────────────────
    //  HOW TO PLAY
    // ─────────────────────────────────────────────────────────────────
    void BuildHowToPlayPanel()
    {
        _howToPlayPanel = BuildSidePanel("Panel_HowToPlay",
            new Vector2(0.05f, 0.04f), new Vector2(0.95f, 0.96f));
        BuildPanelHeader(_howToPlayPanel.transform, "📖  HƯỚNG DẪN CHƠI");

        string txt =
            "🎮  <b>DI CHUYỂN</b>   W A S D  •  Space – Nhảy  •  Shift – Chạy  •  Ctrl – Rón rén\n\n" +
            "🖱️  <b>NHÌN QUANH</b>   Chuột – Nhìn  •  Tab – Mở túi đồ  •  Esc – Menu\n\n" +
            "⚒️  <b>TƯƠNG TÁC</b>   E – Nhặt / Tương tác  •  F – Thắp lửa  •  C – Sổ công thức\n" +
            "   Giữ chuột trái – Chặt cây / Khai thác khoáng sản\n\n" +
            "📊  <b>SINH TỒN</b>   Quản lý HP · Đói · Khát · Stamina\n" +
            "   Hái trái cây, săn thú, nấu ăn, làm lửa ban đêm!\n\n" +
            "🌿  <b>BẢO VỆ MÔI TRƯỜNG</b>   Chặt ≤ 5 cây/ngày  •  Trồng lại cây\n" +
            "   Karma cao → Kết thúc HÒA BÌNH  •  Karma thấp → HỦY DIỆT\n\n" +
            "🗿  <b>THỔ DÂN K'NU</b>   Thân thiện ban ngày – Hắc hóa ban đêm!\n\n" +
            "🐇  <b>THUẦN HÓA THÚ</b>   Cho thỏ ăn trái cây 3 lần → Trở thành pet đồng hành\n\n" +
            "🌙  <b>CHU KỲ 30 NGÀY</b>   Ngày 30 = Phán Quyết Cuối Cùng dựa trên Karma\n\n" +
            "💾  <b>LƯU GAME</b>   Tự động mỗi ngày  •  Tiếp Tục để tải lại tiến trình";

        var cTmp = MakeRect("Content", _howToPlayPanel.transform,
            new Vector2(0.03f, 0.12f), new Vector2(0.97f, 0.86f))
            .AddComponent<TextMeshProUGUI>();
        cTmp.text = txt; cTmp.fontSize = 18f; cTmp.richText = true;
        cTmp.color = C_TEXT_MAIN; cTmp.alignment = TextAlignmentOptions.TopLeft;

        MakeMenuButton("Btn_CloseHTP", _howToPlayPanel.transform,
            new Vector2(0.30f, 0.02f), new Vector2(0.70f, 0.10f),
            "← QUAY LẠI", C_BTN_NORMAL, C_ACCENT,
            () => { PlaySFX(panelCloseSFX, 500f); ShowPanel(null); });

        _howToPlayPanel.SetActive(false);
    }

    // ─────────────────────────────────────────────────────────────────
    //  ABOUT PANEL
    // ─────────────────────────────────────────────────────────────────
    void BuildAboutPanel()
    {
        _aboutPanel = BuildSidePanel("Panel_About",
            new Vector2(0.10f, 0.08f), new Vector2(0.90f, 0.92f));
        BuildPanelHeader(_aboutPanel.transform, "ℹ️  VỀ TRÒ CHƠI");

        string txt =
            "<b>THE FOREST BETWEEN US</b>  –  Khu Rừng Giữa Chúng Ta\n\n" +
            "Trò chơi sinh tồn nhập vai thế giới mở nơi mỗi lựa chọn định hình\n" +
            "cả một hệ sinh thái. Bạn là người duy nhất có thể cứu hoặc phá huỷ\n" +
            "khu rừng huyền bí này.\n\n" +
            "<b>TINH NĂNG</b>\n" +
            "•  Hệ thống sinh tồn đầy đủ: HP, Đói, Khát, Stamina, Bệnh dịch\n" +
            "•  30 ngày chiến dịch + Phán Quyết Cuối Cùng\n" +
            "•  AI thú rừng và thổ dân K'Nu độc đáo\n" +
            "•  Chế tạo, nâng cấp, sửa chữa và tháo đập trang bị\n" +
            "•  Bảo vệ môi trường ảnh hưởng trực tiếp đến kết thúc game\n" +
            "•  Bản đồ thế giới mở, sương mù & chu kỳ ngày đêm\n\n" +
            "<b>NHÓM PHÁT TRIỂN</b>\n" +
            "    VTC Academy  •  Unity 6  •  2025\n\n" +
            "<b>ENGINE & CÔNG NGHỆ</b>\n" +
            "    Unity 6 URP  •  TextMeshPro  •  Devion Games\n" +
            "    Toby Foliage Engine  •  Terrain Evo 3";

        var cTmp = MakeRect("Content", _aboutPanel.transform,
            new Vector2(0.04f, 0.12f), new Vector2(0.96f, 0.86f))
            .AddComponent<TextMeshProUGUI>();
        cTmp.text = txt; cTmp.fontSize = 17f; cTmp.richText = true;
        cTmp.color = C_TEXT_MAIN; cTmp.alignment = TextAlignmentOptions.TopLeft;

        MakeMenuButton("Btn_CloseAbout", _aboutPanel.transform,
            new Vector2(0.30f, 0.02f), new Vector2(0.70f, 0.10f),
            "← QUAY LẠI", C_BTN_NORMAL, C_ACCENT,
            () => { PlaySFX(panelCloseSFX, 500f); ShowPanel(null); });

        _aboutPanel.SetActive(false);
    }

    // ─────────────────────────────────────────────────────────────────
    //  LOADING OVERLAY
    // ─────────────────────────────────────────────────────────────────
    void BuildLoadingOverlay()
    {
        _loadingOverlay = MakeImage("LoadingOverlay", _canvas.transform,
            Vector2.zero, Vector2.one, new Color(0f, 0f, 0f, 0.96f));

        var logoTmp = MakeRect("LoadLogo", _loadingOverlay.transform,
            new Vector2(0.30f, 0.55f), new Vector2(0.70f, 0.92f))
            .AddComponent<TextMeshProUGUI>();
        logoTmp.text = "THE FOREST\nBETWEEN US"; logoTmp.fontSize = 46f;
        logoTmp.fontStyle = FontStyles.Bold; logoTmp.alignment = TextAlignmentOptions.Center;
        logoTmp.enableVertexGradient = true;
        logoTmp.colorGradient = new VertexGradient(C_ACCENT2, C_ACCENT2, C_ACCENT, C_ACCENT);

        var tipTmp = MakeRect("LoadingTipText", _loadingOverlay.transform,
            new Vector2(0.10f, 0.38f), new Vector2(0.90f, 0.52f))
            .AddComponent<TextMeshProUGUI>();
        tipTmp.text = "Đang tải thế giới..."; tipTmp.fontSize = 22f;
        tipTmp.alignment = TextAlignmentOptions.Center; tipTmp.color = C_ACCENT;

        var barBg = MakeImage("BarBg", _loadingOverlay.transform,
            new Vector2(0.10f, 0.26f), new Vector2(0.90f, 0.34f),
            new Color(0.08f, 0.12f, 0.20f, 1f));
        var fillGO = MakeImage("LoadingBarFill", barBg.transform,
            Vector2.zero, Vector2.one, C_ACCENT);
        var fillRT = fillGO.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = new Vector2(0f, 1f);

        var pctTmp = MakeRect("LoadPctText", _loadingOverlay.transform,
            new Vector2(0.43f, 0.16f), new Vector2(0.57f, 0.25f))
            .AddComponent<TextMeshProUGUI>();
        pctTmp.text = "0%"; pctTmp.fontSize = 20f;
        pctTmp.alignment = TextAlignmentOptions.Center; pctTmp.color = C_TEXT_DIM;

        _loadingOverlay.SetActive(false);
    }

    // ─────────────────────────────────────────────────────────────────
    //  Panel management
    // ─────────────────────────────────────────────────────────────────
    void ShowPanel(GameObject panel)
    {
        _mainPanel.SetActive(panel == null);
        if (_settingsPanel       != null) _settingsPanel.SetActive(panel == _settingsPanel);
        if (_aboutPanel          != null) _aboutPanel.SetActive(panel == _aboutPanel);
        if (_howToPlayPanel      != null) _howToPlayPanel.SetActive(panel == _howToPlayPanel);
        if (_newGameConfirmPanel != null) _newGameConfirmPanel.SetActive(panel == _newGameConfirmPanel);
        if (panel != null && panel != _newGameConfirmPanel) PlaySFX(panelOpenSFX, 700f);
    }

    // ─────────────────────────────────────────────────────────────────
    //  Game Actions
    // ─────────────────────────────────────────────────────────────────
    IEnumerator StartNewGame()
    {
        SaveSystem.ClearSaveData();
        yield return StartCoroutine(FadeLoadScene(gameplaySceneName));
    }

    void ContinueGame() => StartCoroutine(FadeLoadScene(gameplaySceneName));

    IEnumerator FadeLoadScene(string sceneName)
    {
        _loadingOverlay.SetActive(true);
        var fillRT = _loadingOverlay.transform.Find("BarBg/LoadingBarFill")?.GetComponent<RectTransform>();
        var tipTmp = _loadingOverlay.transform.Find("LoadingTipText")?.GetComponent<TextMeshProUGUI>();
        var pctTmp = _loadingOverlay.transform.Find("LoadPctText")?.GetComponent<TextMeshProUGUI>();

        string[] tips = {
            "Bao ve rung: chat it hon 5 cay moi ngay!",
            "Ban dem lanh – hay mang theo bat lua.",
            "Cho tho an 3 lan de thuan hoa lam pet.",
            "Tho dan K'Nu than thien ban ngay – nguy hiem ban dem!",
            "Giu du nuoc uong de khong ngat xiu.",
            "Karma cao = Ket thuc Hoa Binh. Karma thap = Huy Diet.",
        };

        float vol0 = _musicSource.volume; float fadeDur = 1.5f; float elapsed = 0f; int tipIdx = 0;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            elapsed += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(op.progress / 0.9f);
            if (fillRT != null) fillRT.anchorMax = new Vector2(p, 1f);
            if (pctTmp != null) pctTmp.text = Mathf.RoundToInt(p * 100f) + "%";
            int ni = Mathf.FloorToInt(elapsed / 2.5f) % tips.Length;
            if (ni != tipIdx && tipTmp != null) { tipIdx = ni; tipTmp.text = tips[tipIdx]; }
            _musicSource.volume = Mathf.Lerp(vol0, 0f, Mathf.Clamp01(elapsed / fadeDur));
            if (op.progress >= 0.9f && elapsed >= 1.5f) op.allowSceneActivation = true;
            yield return null;
        }
    }

    IEnumerator QuitRoutine()
    {
        PlayerPrefs.Save(); yield return new WaitForSeconds(0.25f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ─────────────────────────────────────────────────────────────────
    //  Procedural Forest Ambient Music
    // ─────────────────────────────────────────────────────────────────
    IEnumerator PlayMenuMusic()
    {
        yield return new WaitForEndOfFrame();
        _musicSource.clip = menuMusicClip != null ? menuMusicClip : GenerateForestAmbient();
        _musicSource.volume = 0f;
        _musicSource.Play();
        float target = PlayerPrefs.GetFloat("Menu_MusicVol", 0.5f);
        float t = 0f;
        while (t < 3f) { t += Time.deltaTime; _musicSource.volume = Mathf.Lerp(0f, target, t / 3f); yield return null; }
        _musicSource.volume = target;
    }

    AudioClip GenerateForestAmbient()
    {
        int sr = 44100; int dur = 30; int total = sr * dur;
        float[] buf = new float[total];
        var rng = new System.Random(1337);
        float[] penta = { 261.63f, 293.66f, 329.63f, 392f, 440f };

        for (int i = 0; i < total; i++)
        {
            float t = (float)i / sr;
            // Low drone
            float drone = Mathf.Sin(Mathf.PI*2f*55f*t)*0.06f + Mathf.Sin(Mathf.PI*2f*82.5f*t)*0.04f + Mathf.Sin(Mathf.PI*2f*110f*t)*0.03f;
            float breath = (Mathf.Sin(Mathf.PI*2f*0.25f*t)+1f)*0.5f;
            drone *= 0.5f + breath*0.5f;
            // Wind
            float wind = (float)(rng.NextDouble()*2.0-1.0)*0.025f;
            float windE = (Mathf.Sin(Mathf.PI*2f*0.07f*t+1.2f)+1f)*0.5f;
            wind *= windE;
            // Pentatonic ping every 4s
            float ping = 0f; float pingT = t % 4f;
            if (pingT < 0.6f)
            {
                int ni = ((int)(t / 4f)) % penta.Length;
                float env = Mathf.Exp(-pingT*7f);
                ping = Mathf.Sin(Mathf.PI*2f*penta[ni]*t)*env*0.055f;
                ping += Mathf.Sin(Mathf.PI*2f*penta[ni]*1.5f*t)*env*0.02f;
            }
            buf[i] = Mathf.Clamp(drone+wind+ping, -1f, 1f);
        }
        var clip = AudioClip.Create("ForestAmbient", total, 1, sr, false);
        clip.SetData(buf, 0); return clip;
    }

    // ─────────────────────────────────────────────────────────────────
    //  SFX
    // ─────────────────────────────────────────────────────────────────
    void PlaySFX(AudioClip clip, float fallbackFreq = 600f)
    {
        if (_sfxSource == null) return;
        _sfxSource.PlayOneShot(clip != null ? clip : GenerateTone(fallbackFreq, 0.07f, 0.3f));
    }

    AudioClip GenerateTone(float freq, float dur, float vol)
    {
        int sr = 44100; int n = (int)(sr * dur); float[] d = new float[n];
        for (int i = 0; i < n; i++)
        {
            float t = (float)i / sr;
            d[i] = Mathf.Sin(Mathf.PI*2f*freq*t) * Mathf.Clamp01(1f - t/dur) * vol;
        }
        var c = AudioClip.Create("SFX", n, 1, sr, false); c.SetData(d, 0); return c;
    }

    // ─────────────────────────────────────────────────────────────────
    //  Animations
    // ─────────────────────────────────────────────────────────────────
    IEnumerator AnimateParticles()
    {
        var rng = new System.Random(99);
        while (true)
        {
            foreach (var p in _particles)
            {
                if (p.rt == null) continue;
                var mn = p.rt.anchorMin + new Vector2(p.drift, p.speed);
                var mx = p.rt.anchorMax + new Vector2(p.drift, p.speed);
                if (mn.y > 1.05f) { mn.y = -0.05f; mx.y = mn.y + 0.009f; }
                if (mn.x < -0.05f || mn.x > 1.05f) { float nx = (float)rng.NextDouble(); mn.x = nx; mx.x = nx + 0.005f; }
                p.rt.anchorMin = mn; p.rt.anchorMax = mx;
            }
            yield return null;
        }
    }

    IEnumerator PulseTitle()
    {
        yield return new WaitForSeconds(0.5f);
        var go = _canvas.transform.Find("Panel_Main/LogoArea/TitleTextAnimated");
        if (go == null) yield break;
        var tmp = go.GetComponent<TextMeshProUGUI>();
        if (tmp == null) yield break;
        while (true)
        {
            float t = 0f;
            while (t < 3f) { t += Time.deltaTime; tmp.fontSize = 68f + Mathf.Sin(t * Mathf.PI * 0.5f) * 2f; yield return null; }
            t = 0f;
            while (t < 3f) { t += Time.deltaTime; tmp.fontSize = 70f - Mathf.Sin(t * Mathf.PI * 0.5f) * 2f; yield return null; }
        }
    }

    // ─────────────────────────────────────────────────────────────────
    //  Helper: Slider row
    // ─────────────────────────────────────────────────────────────────
    void BuildSliderRow(Transform parent, string label, ref float y,
        float rowH, float gap, float min, float max, float init,
        System.Action<float> onChange, ref Slider slider, ref TextMeshProUGUI valLabel)
    {
        var lTmp = MakeRect(label+"_L", parent, new Vector2(0.04f, y), new Vector2(0.52f, y+rowH))
            .AddComponent<TextMeshProUGUI>();
        lTmp.text = label; lTmp.fontSize = 18f; lTmp.color = C_TEXT_MAIN; lTmp.alignment = TextAlignmentOptions.MidlineLeft;

        valLabel = MakeRect(label+"_V", parent, new Vector2(0.86f, y), new Vector2(0.96f, y+rowH))
            .AddComponent<TextMeshProUGUI>();
        valLabel.fontSize = 15f; valLabel.color = C_ACCENT; valLabel.alignment = TextAlignmentOptions.Center;
        valLabel.text = Mathf.RoundToInt(init * 100f) + "%";

        var sGO = MakeRect(label+"_S", parent, new Vector2(0.52f, y+rowH*0.2f), new Vector2(0.84f, y+rowH*0.8f));
        slider = sGO.AddComponent<Slider>();
        MakeImage("BG", sGO.transform, Vector2.zero, Vector2.one, new Color(0.08f, 0.14f, 0.22f));
        var fillArea = MakeRect("FillArea", sGO.transform, new Vector2(0f, 0.25f), new Vector2(1f, 0.75f));
        var fill = MakeImage("Fill", fillArea.transform, Vector2.zero, Vector2.one, C_ACCENT);
        var hSA  = MakeRect("HandleArea", sGO.transform, Vector2.zero, Vector2.one);
        var handle = MakeImage("Handle", hSA.transform, new Vector2(0f, 0f), new Vector2(0f, 1f), C_ACCENT2);
        handle.GetComponent<RectTransform>().sizeDelta = new Vector2(14f, 0f);
        slider.fillRect = fill.GetComponent<RectTransform>();
        slider.handleRect = handle.GetComponent<RectTransform>();
        slider.targetGraphic = handle.GetComponent<Image>();
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = min; slider.maxValue = max; slider.value = init;
        var capturedLabel = valLabel;
        slider.onValueChanged.AddListener(v => { onChange(v); capturedLabel.text = Mathf.RoundToInt(v*100f)+"%"; });
        y -= rowH + gap;
    }

    // ─────────────────────────────────────────────────────────────────
    //  Panel builders
    // ─────────────────────────────────────────────────────────────────
    GameObject BuildSidePanel(string name, Vector2 amin, Vector2 amax)
    {
        var panel = MakeImage(name, _canvas.transform, amin, amax, C_PANEL_BG);
        panel.GetComponent<Image>().sprite = GenerateRoundedRectSprite(20);
        var border = MakeImage(name+"_B", panel.transform,
            new Vector2(-0.004f,-0.004f), new Vector2(1.004f,1.004f),
            new Color(C_ACCENT.r, C_ACCENT.g, C_ACCENT.b, 0.18f));
        border.GetComponent<Image>().sprite = GenerateRoundedRectSprite(20);
        border.transform.SetAsFirstSibling();
        return panel;
    }

    void BuildPanelHeader(Transform parent, string title)
    {
        var hGO = MakeRect("Header", parent, new Vector2(0f, 0.88f), new Vector2(1f, 1f));
        hGO.AddComponent<Image>().color = new Color(C_ACCENT.r, C_ACCENT.g, C_ACCENT.b, 0.12f);
        var tTmp = MakeRect("HdrTitle", hGO.transform, Vector2.zero, Vector2.one)
            .AddComponent<TextMeshProUGUI>();
        tTmp.text = title; tTmp.fontSize = 26f; tTmp.fontStyle = FontStyles.Bold;
        tTmp.alignment = TextAlignmentOptions.Center; tTmp.color = C_ACCENT;
        MakeSeparator("HdrSep", parent, new Vector2(0.02f, 0.866f), new Vector2(0.98f, 0.876f));
    }

    // ─────────────────────────────────────────────────────────────────
    //  Button builder
    // ─────────────────────────────────────────────────────────────────
    GameObject MakeMenuButton(string name, Transform parent,
        Vector2 amin, Vector2 amax, string label, Color bgColor, Color txtColor,
        System.Action onClick)
    {
        var go = MakeImage(name, parent, amin, amax, bgColor);
        go.GetComponent<Image>().sprite = GenerateRoundedRectSprite(10);
        var btn = go.AddComponent<Button>();
        var tmp = MakeRect(name+"_T", go.transform, Vector2.zero, Vector2.one)
            .AddComponent<TextMeshProUGUI>();
        tmp.text = label; tmp.fontSize = 20f; tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center; tmp.color = txtColor;

        var et = go.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        var img = go.GetComponent<Image>();
        Color hov = new Color(Mathf.Clamp01(bgColor.r*1.5f), Mathf.Clamp01(bgColor.g*1.5f), Mathf.Clamp01(bgColor.b*1.5f), bgColor.a);
        var eIn = new UnityEngine.EventSystems.EventTrigger.Entry
            { eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter };
        eIn.callback.AddListener(_ => { img.color = hov; PlaySFX(buttonHoverSFX, 950f); });
        et.triggers.Add(eIn);
        var eOut = new UnityEngine.EventSystems.EventTrigger.Entry
            { eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit };
        eOut.callback.AddListener(_ => img.color = bgColor);
        et.triggers.Add(eOut);

        if (onClick != null) btn.onClick.AddListener(() => onClick());
        return go;
    }

    // ─────────────────────────────────────────────────────────────────
    //  Low-level helpers
    // ─────────────────────────────────────────────────────────────────
    static GameObject MakeRect(string name, Transform parent, Vector2 amin, Vector2 amax)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = amin; rt.anchorMax = amax;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        return go;
    }

    static GameObject MakeImage(string name, Transform parent, Vector2 amin, Vector2 amax, Color col)
    {
        var go = MakeRect(name, parent, amin, amax);
        go.AddComponent<Image>().color = col;
        return go;
    }

    static void StretchFill(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    static GameObject MakeSeparator(string name, Transform parent, Vector2 amin, Vector2 amax)
        => MakeImage(name, parent, amin, amax, C_SEPARATOR);

    // ─────────────────────────────────────────────────────────────────
    //  Sprite generators
    // ─────────────────────────────────────────────────────────────────
    static Sprite GenerateGradientSprite(int w = 4, int h = 256)
    {
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < h; y++)
        {
            Color c = Color.Lerp(new Color(0.01f,0.03f,0.07f), new Color(0.05f,0.10f,0.18f), (float)y/h);
            for (int x = 0; x < w; x++) tex.SetPixel(x, y, c);
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0,0,w,h), new Vector2(0.5f,0.5f));
    }

    static Sprite GenerateVignetteSprite(int s = 512)
    {
        var tex = new Texture2D(s, s, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        Vector2 c = new Vector2(s/2f, s/2f);
        float mx = Vector2.Distance(Vector2.zero, c);
        for (int y = 0; y < s; y++)
            for (int x = 0; x < s; x++)
            {
                float a = Mathf.SmoothStep(0f, 0.72f, Vector2.Distance(new Vector2(x,y), c)/mx);
                tex.SetPixel(x, y, new Color(0f,0f,0f,a));
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0,0,s,s), new Vector2(0.5f,0.5f));
    }

    static Sprite GenerateRoundedRectSprite(int radius = 10, int w = 128, int h = 64)
    {
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                int cx = Mathf.Clamp(x, radius, w-radius-1);
                int cy = Mathf.Clamp(y, radius, h-radius-1);
                float d = Mathf.Sqrt((x-cx)*(x-cx)+(y-cy)*(y-cy));
                tex.SetPixel(x, y, d <= radius ? Color.white : Color.clear);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0,0,w,h), new Vector2(0.5f,0.5f), 100f,
            0, SpriteMeshType.FullRect, new Vector4(radius,radius,radius,radius));
    }

    static Sprite GenerateCircleSprite(int s = 24)
    {
        var tex = new Texture2D(s, s, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        Vector2 c = new Vector2(s/2f, s/2f); float r = s/2f-1f;
        for (int y = 0; y < s; y++)
            for (int x = 0; x < s; x++)
            {
                float a = Mathf.Clamp01(r - Mathf.Max(0f, Vector2.Distance(new Vector2(x,y), c)-r+1f));
                tex.SetPixel(x, y, new Color(1f,1f,1f,a));
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0,0,s,s), new Vector2(0.5f,0.5f));
    }
}
