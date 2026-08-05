using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// MainMenuController – AAA-quality main menu for "The Forest Between Us".
/// Sons-of-the-Forest inspired: cinematic, dark, atmospheric.
/// Self-contained: builds all UI in code, no prefabs needed.
/// Attach to any GameObject in scene "Home".
/// </summary>
public class MainMenuController : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────────
    //  Inspector
    // ─────────────────────────────────────────────────────────────────
    [Header("Scene")]
    public string gameplaySceneName = "GamePlay";

    [Header("Optional: assign real AudioClips")]
    public AudioClip menuMusicClip;     // forest ambient
    public AudioClip sfxHoverClip;
    public AudioClip sfxClickClip;
    public AudioClip sfxOpenClip;
    public AudioClip sfxCloseClip;

    // ─────────────────────────────────────────────────────────────────
    //  Private runtime
    // ─────────────────────────────────────────────────────────────────
    Canvas      _cvs;
    AudioSource _music, _sfx;

    // Panels
    GameObject _panelMain, _panelSettings, _panelControls, _panelStory, _panelConfirmNew;
    GameObject _overlay;   // loading screen

    // Save data
    bool _hasSave;
    int  _saveDay, _saveKarma;

    // Fireflies
    readonly List<FireflyData> _flies = new();
    struct FireflyData { public RectTransform rt; public float speed, drift, phase; }

    // Fog strips for scanline
    readonly List<FogStrip> _fog = new();
    struct FogStrip { public RectTransform rt; public float speed, alpha; }

    // ─────────────────────────────────────────────────────────────────
    //  AAA Colour System  (Sons of Forest dark-green palette)
    // ─────────────────────────────────────────────────────────────────
    static Color C(float r, float g, float b, float a = 1f) => new(r, g, b, a);

    // Backgrounds
    static readonly Color BG_ABYSS   = C(0.020f, 0.030f, 0.045f);   // near-black teal
    static readonly Color BG_PANEL   = C(0.025f, 0.038f, 0.055f, 0.96f);

    // Accents
    static readonly Color ACC_GREEN  = C(0.180f, 0.820f, 0.450f);    // bioluminescent forest green
    static readonly Color ACC_AMBER  = C(0.920f, 0.680f, 0.100f);    // torch/fire amber
    static readonly Color ACC_RED    = C(0.780f, 0.140f, 0.140f);    // danger
    static readonly Color ACC_DIM    = C(0.380f, 0.520f, 0.480f);    // muted teal

    // Text
    static readonly Color TXT_HERO   = C(0.940f, 0.960f, 0.960f);    // near-white
    static readonly Color TXT_BODY   = C(0.640f, 0.740f, 0.700f);    // dim seafoam
    static readonly Color TXT_LABEL  = C(0.300f, 0.420f, 0.400f);    // very dim

    // UI chrome
    static readonly Color CHROME_LINE = C(0.180f, 0.820f, 0.450f, 0.20f); // separator
    static readonly Color BTN_DARK   = C(0.060f, 0.100f, 0.130f, 0.90f); // button bg

    // ─────────────────────────────────────────────────────────────────
    //  Lifecycle
    // ─────────────────────────────────────────────────────────────────
    void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
        Time.timeScale   = 1f;

        _music = Mk<AudioSource>(); _music.loop = true; _music.spatialBlend = 0f;
        _music.volume = PlayerPrefs.GetFloat("MV", 0.55f);
        _sfx   = Mk<AudioSource>(); _sfx.playOnAwake = false; _sfx.spatialBlend = 0f;
        _sfx.volume = PlayerPrefs.GetFloat("SV", 0.85f);

        _hasSave = SaveSystem.LoadProgress(out _saveDay, out _saveKarma, out _);
    }

    T Mk<T>() where T : Component => gameObject.AddComponent<T>();

    void Start()
    {
        BuildCanvas();
        BuildCinematicBackground();
        BuildFireflyLayer();
        BuildFogLayer();
        BuildPanelMain();
        BuildPanelSettings();
        BuildPanelControls();
        BuildPanelStory();
        BuildPanelConfirmNew();
        BuildLoadingOverlay();
        SwitchTo(null);

        StartCoroutine(CoMusic());
        StartCoroutine(CoFireflies());
        StartCoroutine(CoFog());
        StartCoroutine(CoTitleBreath());
        StartCoroutine(CoChromaShift());
    }

    // ─────────────────────────────────────────────────────────────────
    //  CANVAS
    // ─────────────────────────────────────────────────────────────────
    void BuildCanvas()
    {
        var go = new GameObject("MainMenu_Canvas");
        _cvs = go.AddComponent<Canvas>();
        _cvs.renderMode   = RenderMode.ScreenSpaceOverlay;
        _cvs.sortingOrder = 200;
        var cs = go.AddComponent<CanvasScaler>();
        cs.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cs.screenMatchMode     = CanvasScaler.ScreenMatchMode.Expand;
        go.AddComponent<GraphicRaycaster>();
    }

    // ─────────────────────────────────────────────────────────────────
    //  CINEMATIC BACKGROUND  (3-layer depth)
    // ─────────────────────────────────────────────────────────────────
    void BuildCinematicBackground()
    {
        // Layer 0: deep abyss gradient
        var g0 = Img("BG_Deep", _cvs.transform, V2(0,0), V2(1,1), BG_ABYSS);
        g0.GetComponent<Image>().sprite = SprGradient(
            C(0.010f,0.015f,0.025f), C(0.035f,0.060f,0.080f));

        // Layer 1: mid-tone radial fog (light from center)
        var g1 = Img("BG_Radial", _cvs.transform, V2(0,0), V2(1,1), Color.clear);
        g1.GetComponent<Image>().sprite = SprRadialGlow(C(0.04f,0.10f,0.08f,0.35f));

        // Layer 2: vignette (heavy, cinematic black corners)
        var g2 = Img("BG_Vignette", _cvs.transform, V2(0,0), V2(1,1), Color.clear);
        g2.GetComponent<Image>().sprite = SprVignette(0.85f);

        // Layer 3: top dark bar (letterbox top)
        var topBar = Img("Bar_Top", _cvs.transform, V2(0,0.92f), V2(1,1), C(0f,0f,0f,0.7f));
        // Layer 4: bottom bar
        var botBar = Img("Bar_Bot", _cvs.transform, V2(0,0), V2(1,0.04f), C(0f,0f,0f,0.7f));

        // Glowing horizontal scanline
        var scan = Img("Scanline", _cvs.transform, V2(0,0.435f), V2(1,0.440f),
            C(ACC_GREEN.r, ACC_GREEN.g, ACC_GREEN.b, 0.06f));
        scan.name = "Scanline_Animated";
    }

    // ─────────────────────────────────────────────────────────────────
    //  FIREFLY LAYER (40 bioluminescent specks)
    // ─────────────────────────────────────────────────────────────────
    void BuildFireflyLayer()
    {
        var layer = Rect("FireflyLayer", _cvs.transform, V2(0,0), V2(1,1));
        var rng = new System.Random(77);
        for (int i = 0; i < 45; i++)
        {
            float sz  = (float)(rng.NextDouble() * 5 + 1.5f);
            Color col = i % 4 == 0 ? ACC_AMBER
                      : i % 4 == 1 ? ACC_GREEN
                      : i % 4 == 2 ? C(0.4f,0.9f,0.7f)
                      : C(0.6f,0.8f,1.0f);
            col.a = (float)(rng.NextDouble() * 0.5f + 0.12f);

            var dot = Img($"Fly{i}", layer.transform, V2(0,0), V2(0,0), col);
            var rt  = dot.GetComponent<RectTransform>();
            float ax = (float)rng.NextDouble();
            float ay = (float)rng.NextDouble();
            rt.anchorMin = V2(ax, ay);
            rt.anchorMax = V2(ax + 0.004f, ay + 0.007f);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            dot.GetComponent<Image>().sprite = SprCircle(12);

            _flies.Add(new FireflyData
            {
                rt    = rt,
                speed = (float)(rng.NextDouble() * 0.00012f + 0.00004f),
                drift = (float)((rng.NextDouble() - 0.5) * 0.00007f),
                phase = (float)(rng.NextDouble() * Mathf.PI * 2f)
            });
        }
    }

    // ─────────────────────────────────────────────────────────────────
    //  FOG STRIPS (horizontal moving mist bands)
    // ─────────────────────────────────────────────────────────────────
    void BuildFogLayer()
    {
        var layer = Rect("FogLayer", _cvs.transform, V2(0,0), V2(1,1));
        var rng = new System.Random(33);
        for (int i = 0; i < 6; i++)
        {
            float y = (float)(rng.NextDouble() * 0.6f + 0.05f);
            float h = (float)(rng.NextDouble() * 0.04f + 0.015f);
            Color fogCol = C(0.10f, 0.22f, 0.18f, (float)(rng.NextDouble() * 0.06f + 0.02f));
            var strip = Img($"Fog{i}", layer.transform, V2(-0.1f, y), V2(1.1f, y + h), fogCol);
            strip.GetComponent<Image>().sprite = SprFogStrip();
            float spd = (float)((rng.NextDouble() - 0.5) * 0.00006f);
            _fog.Add(new FogStrip { rt = strip.GetComponent<RectTransform>(), speed = spd });
        }
    }

    // ─────────────────────────────────────────────────────────────────
    //  MAIN PANEL
    // ─────────────────────────────────────────────────────────────────
    void BuildPanelMain()
    {
        _panelMain = Rect("Panel_Main", _cvs.transform, V2(0,0), V2(1,1)).gameObject;
        var rt = _panelMain.GetComponent<RectTransform>();

        // ══ LEFT COLUMN — title + lore ══════════════════════════════════
        var left = Rect("Left", _panelMain.transform, V2(0.03f,0.08f), V2(0.50f,0.92f));

        // Glowing title badge
        var titleBadge = Img("TitleBadge", left.transform, V2(0,0.68f), V2(1,1.0f),
            C(0.03f,0.07f,0.06f,0.55f));
        titleBadge.GetComponent<Image>().sprite = SprRR(16);
        // Left accent strip
        var accentStrip = Img("Accent", left.transform, V2(0,0.68f), V2(0.012f,1f),
            C(ACC_GREEN.r, ACC_GREEN.g, ACC_GREEN.b, 0.9f));

        // Game title
        var titleGO = Rect("Title_THE", left.transform, V2(0.04f,0.80f), V2(0.98f,0.98f));
        var t1 = titleGO.AddComponent<TextMeshProUGUI>();
        t1.text      = "THE FOREST";
        t1.fontSize  = 72f;
        t1.fontStyle = FontStyles.Bold;
        t1.alignment = TextAlignmentOptions.Left;
        t1.enableVertexGradient = true;
        t1.colorGradient = new VertexGradient(TXT_HERO, TXT_HERO,
            C(0.60f, 0.90f, 0.72f), C(0.30f, 0.60f, 0.45f));
        titleGO.name = "TitleAnimated";

        var t2GO = Rect("Title_BETWEEN", left.transform, V2(0.04f,0.69f), V2(0.98f,0.82f));
        var t2 = t2GO.AddComponent<TextMeshProUGUI>();
        t2.text      = "BETWEEN US";
        t2.fontSize  = 52f;
        t2.fontStyle = FontStyles.Bold;
        t2.alignment = TextAlignmentOptions.Left;
        t2.color     = ACC_GREEN;
        t2.characterSpacing = 8f;

        // Subtitle / lore tagline
        var sub = Rect("Sub", left.transform, V2(0.04f,0.61f), V2(0.95f,0.69f));
        var subT = sub.AddComponent<TextMeshProUGUI>();
        subT.text      = "Khu Rừng Giữa Chúng Ta  ·  30 Ngày Phán Quyết";
        subT.fontSize  = 18f;
        subT.color     = ACC_DIM;
        subT.fontStyle = FontStyles.Italic;

        // Separator line
        Sep("Sep1", left.transform, V2(0.03f, 0.595f), V2(0.90f, 0.600f));

        // Lore blurb — 3 lines of atmosphere
        var lore = Rect("Lore", left.transform, V2(0.02f,0.36f), V2(0.96f,0.59f));
        var loreT = lore.AddComponent<TextMeshProUGUI>();
        loreT.text =
            "Bạn tỉnh dậy trong sương mù dày đặc.\n" +
            "Chiếc đài Radio cũ phát ra tiếng rè của\n" +
            "<color=#2ED07A>Mai An Tiêm</color> — người đã biến mất 300 năm trước.\n\n" +
            "30 ngày. 5 kết thúc. Một Vùng Đứt Gãy.<b> Void Rift.</b>\n" +
            "Lựa chọn của bạn sẽ định đoạt vận mệnh nhân loại.";
        loreT.fontSize  = 17.5f;
        loreT.color     = TXT_BODY;
        loreT.richText  = true;
        loreT.lineSpacing = 6f;

        // Chapter badge row
        Sep("Sep2", left.transform, V2(0.03f, 0.345f), V2(0.90f, 0.350f));
        BuildChapterBadges(left.transform);

        // Save badge
        if (_hasSave)
        {
            var sb = Img("SaveBadge", left.transform, V2(0.02f,0.09f), V2(0.70f,0.18f),
                C(0.05f,0.20f,0.12f,0.88f));
            sb.GetComponent<Image>().sprite = SprRR(8);
            var sBorderL = Img("SBorderL", sb.transform, V2(0,0), V2(0.008f,1f),
                C(ACC_GREEN.r,ACC_GREEN.g,ACC_GREEN.b,0.9f));
            var sT = Rect("SaveTxt", sb.transform, V2(0.04f,0), V2(1,1)).AddComponent<TextMeshProUGUI>();
            sT.text = $"💾  Ngày <b><color=#2ED07A>{_saveDay}</color></b>  /30     Karma  <b><color=#F5C332>{_saveKarma}</color></b> / 100";
            sT.fontSize = 16f; sT.color = TXT_BODY; sT.alignment = TextAlignmentOptions.MidlineLeft;
        }

        // Version
        var ver = Rect("Ver", left.transform, V2(0.02f,0.02f), V2(0.50f,0.08f)).AddComponent<TextMeshProUGUI>();
        ver.text = "v1.0  ·  Unity 6  ·  VTC Academy 2025";
        ver.fontSize = 13f; ver.color = TXT_LABEL;

        // ══ RIGHT COLUMN — button panel ═════════════════════════════════
        var right = Rect("Right", _panelMain.transform, V2(0.52f,0.08f), V2(0.97f,0.92f));
        var rightBg = Img("RightBg", right.transform, V2(0,0), V2(1,1), BG_PANEL);
        rightBg.GetComponent<Image>().sprite = SprRR(20);
        // Top accent line
        var topAccent = Img("TopAccent", right.transform, V2(0.05f,0.965f), V2(0.95f,0.972f), ACC_GREEN);

        // Section header inside button panel
        var hdrT = Rect("PanelHdr", right.transform, V2(0.06f,0.90f), V2(0.94f,0.97f)).AddComponent<TextMeshProUGUI>();
        hdrT.text = "▶  ĐIỀU HƯỚNG";
        hdrT.fontSize = 14f; hdrT.color = ACC_DIM; hdrT.characterSpacing = 5f;
        hdrT.alignment = TextAlignmentOptions.MidlineLeft;

        // Buttons — AAA styled
        float btnH = 0.10f, gap = 0.018f, startY = 0.88f;
        Btn("Btn_New",      right.transform, V2(0.05f, startY-0*(btnH+gap)), V2(0.95f, startY-0*(btnH+gap)+btnH),
            "TRÒ CHƠI MỚI", "Bắt đầu hành trình 30 ngày", ACC_GREEN,
            () => { Click(); SwitchTo(_panelConfirmNew); });

        Btn("Btn_Continue", right.transform, V2(0.05f, startY-1*(btnH+gap)), V2(0.95f, startY-1*(btnH+gap)+btnH),
            _hasSave ? $"TIẾP TỤC" : "TIẾP TỤC",
            _hasSave ? $"Ngày {_saveDay}  ·  Karma {_saveKarma}" : "Chưa có tiến trình",
            _hasSave ? ACC_AMBER : C(0.3f,0.4f,0.38f),
            () => { if (_hasSave) { Click(); StartCoroutine(CoLoad(gameplaySceneName)); } },
            !_hasSave);

        Btn("Btn_Settings",  right.transform, V2(0.05f, startY-2*(btnH+gap)), V2(0.95f, startY-2*(btnH+gap)+btnH),
            "CÀI ĐẶT", "Âm thanh  ·  Đồ hoạ  ·  Toàn màn hình", TXT_HERO,
            () => { Click(); SwitchTo(_panelSettings); });

        Btn("Btn_Controls", right.transform, V2(0.05f, startY-3*(btnH+gap)), V2(0.95f, startY-3*(btnH+gap)+btnH),
            "ĐIỀU KHIỂN", "Xem hướng dẫn chơi & phím tắt", TXT_HERO,
            () => { Click(); SwitchTo(_panelControls); });

        Btn("Btn_Story",    right.transform, V2(0.05f, startY-4*(btnH+gap)), V2(0.95f, startY-4*(btnH+gap)+btnH),
            "CỐT TRUYỆN", "5 Hồi  ·  30 Ngày  ·  5 Kết Thúc", ACC_AMBER,
            () => { Click(); SwitchTo(_panelStory); });

        // Quit — bottom right, smaller
        var quitGO = Img("Btn_Quit", right.transform, V2(0.05f,0.03f), V2(0.48f,0.10f),
            C(ACC_RED.r*0.5f,0.04f,0.04f,0.85f));
        quitGO.GetComponent<Image>().sprite = SprRR(8);
        var qBtn = quitGO.AddComponent<Button>();
        AddHover(quitGO, C(ACC_RED.r*0.5f,0.04f,0.04f,0.85f), C(0.55f,0.10f,0.10f));
        var qT = Rect("QT",quitGO.transform,V2(0,0),V2(1,1)).AddComponent<TextMeshProUGUI>();
        qT.text="THOÁT"; qT.fontSize=18f; qT.fontStyle=FontStyles.Bold;
        qT.alignment=TextAlignmentOptions.Center; qT.color=TXT_HERO;
        qBtn.onClick.AddListener(()=>{Click();StartCoroutine(CoQuit());});

        // About — bottom right
        var aboutGO = Img("Btn_About", right.transform, V2(0.52f,0.03f), V2(0.95f,0.10f),
            BTN_DARK);
        aboutGO.GetComponent<Image>().sprite = SprRR(8);
        var aBtn = aboutGO.AddComponent<Button>();
        AddHover(aboutGO, BTN_DARK, C(0.08f,0.18f,0.14f));
        var aT = Rect("AT",aboutGO.transform,V2(0,0),V2(1,1)).AddComponent<TextMeshProUGUI>();
        aT.text="VỀ GAME"; aT.fontSize=15f;
        aT.alignment=TextAlignmentOptions.Center; aT.color=TXT_BODY;
        aBtn.onClick.AddListener(()=>{Click();SwitchTo(_panelStory);});
    }

    // Chapter badges — 5 acts as small colored tags
    void BuildChapterBadges(Transform parent)
    {
        (string label, Color col)[] acts = {
            ("HỒI 1: KHỞI ĐẦU", C(0.2f,0.7f,0.5f)),
            ("HỒI 2: CHIẾN ĐẤU", C(0.8f,0.5f,0.1f)),
            ("HỒI 3: BÍ ẨN", C(0.5f,0.2f,0.8f)),
            ("HỒI 4: ĐẠI CHIẾN", C(0.8f,0.2f,0.2f)),
            ("HỒI 5: PHÁN QUYẾT", C(0.2f,0.5f,0.9f)),
        };
        float bw = 0.185f;
        for (int i = 0; i < acts.Length; i++)
        {
            float x = 0.02f + i * (bw + 0.01f);
            var b = Img($"Act{i}", parent, V2(x,0.27f), V2(x+bw,0.34f),
                C(acts[i].col.r*0.25f, acts[i].col.g*0.25f, acts[i].col.b*0.25f, 0.9f));
            b.GetComponent<Image>().sprite = SprRR(6);
            var line = Img($"ActLine{i}", b.transform, V2(0,0.88f), V2(1,1), acts[i].col);
            var t = Rect("T",b.transform,V2(0.05f,0),V2(0.95f,0.88f)).AddComponent<TextMeshProUGUI>();
            t.text = acts[i].label; t.fontSize = 10f;
            t.color = TXT_BODY; t.alignment = TextAlignmentOptions.Center;
        }
    }

    // ─────────────────────────────────────────────────────────────────
    //  CONFIRM NEW GAME PANEL
    // ─────────────────────────────────────────────────────────────────
    void BuildPanelConfirmNew()
    {
        _panelConfirmNew = SidePanel("Panel_ConfirmNew", V2(0.20f,0.28f), V2(0.80f,0.72f));
        PanelHeader(_panelConfirmNew.transform, "TRÒ CHƠI MỚI", ACC_GREEN);

        var bodyT = Rect("Body",_panelConfirmNew.transform,V2(0.06f,0.32f),V2(0.94f,0.84f))
            .AddComponent<TextMeshProUGUI>();
        bodyT.text = _hasSave
            ? $"Tiến trình <b>Ngày {_saveDay}</b> (Karma {_saveKarma}) sẽ bị <color=#CF2222><b>XÓA VĨNH VIỄN</b></color>.\n\nBạn sẵn sàng bắt đầu lại từ <b>Ngày 1: Tín Hiệu Lạc Lối</b>?"
            : "Bắt đầu cuộc hành trình sinh tồn trong <color=#2ED07A>Khu Rừng Huyền Bí</color>.\n\n30 ngày. 5 kết thúc. Số phận nhân loại trong tay bạn.";
        bodyT.fontSize=19f; bodyT.color=TXT_BODY; bodyT.richText=true;
        bodyT.alignment=TextAlignmentOptions.Center;

        SmallBtn("Btn_Confirm", _panelConfirmNew.transform, V2(0.06f,0.06f), V2(0.46f,0.22f),
            "BẮT ĐẦU", ACC_GREEN, ()=>{ Click(); StartCoroutine(CoNewGame()); });
        SmallBtn("Btn_CancelNew", _panelConfirmNew.transform, V2(0.54f,0.06f), V2(0.94f,0.22f),
            "HỦY", ACC_RED, ()=>{ Click(); SwitchTo(null); });
    }

    // ─────────────────────────────────────────────────────────────────
    //  SETTINGS PANEL
    // ─────────────────────────────────────────────────────────────────
    void BuildPanelSettings()
    {
        _panelSettings = SidePanel("Panel_Settings", V2(0.12f,0.07f), V2(0.88f,0.93f));
        PanelHeader(_panelSettings.transform, "CÀI ĐẶT", TXT_HERO);

        float y = 0.74f; float rh = 0.09f; float g = 0.015f;
        Slider masterS = null, musicS = null, sfxS = null;
        TextMeshProUGUI masterL = null, musicL = null, sfxL = null;

        SliderRow(_panelSettings.transform, "🔊  Âm lượng tổng",   ref y, rh, g, 0f, 1f,
            PlayerPrefs.GetFloat("MasterVol",1f), v=>{AudioListener.volume=v;PlayerPrefs.SetFloat("MasterVol",v);}, ref masterS, ref masterL);
        SliderRow(_panelSettings.transform, "🎵  Nhạc nền",          ref y, rh, g, 0f, 1f,
            PlayerPrefs.GetFloat("MV",0.55f), v=>{_music.volume=v;PlayerPrefs.SetFloat("MV",v);}, ref musicS, ref musicL);
        SliderRow(_panelSettings.transform, "🔔  Hiệu ứng âm thanh", ref y, rh, g, 0f, 1f,
            PlayerPrefs.GetFloat("SV",0.85f), v=>{_sfx.volume=v;PlayerPrefs.SetFloat("SV",v);}, ref sfxS, ref sfxL);

        Sep("SepS", _panelSettings.transform, V2(0.05f,y), V2(0.95f,y+0.006f)); y -= g*2f;

        // Fullscreen toggle
        var fsLbl = Rect("FSL",_panelSettings.transform,V2(0.05f,y),V2(0.70f,y+rh)).AddComponent<TextMeshProUGUI>();
        fsLbl.text="🖥️  Toàn màn hình"; fsLbl.fontSize=18f; fsLbl.color=TXT_HERO; fsLbl.alignment=TextAlignmentOptions.MidlineLeft;
        var fsGO = Img("FSBtn",_panelSettings.transform,V2(0.74f,y+0.01f),V2(0.93f,y+rh-0.01f),BTN_DARK);
        fsGO.GetComponent<Image>().sprite = SprRR(8);
        var fsBtn = fsGO.AddComponent<Button>();
        var fsTxt = Rect("FSTxt",fsGO.transform,V2(0,0),V2(1,1)).AddComponent<TextMeshProUGUI>();
        fsTxt.alignment=TextAlignmentOptions.Center; fsTxt.fontSize=17f;
        fsTxt.text=Screen.fullScreen?"BẬT":"TẮT"; fsTxt.color=Screen.fullScreen?ACC_GREEN:TXT_LABEL;
        fsBtn.onClick.AddListener(()=>{bool n=!Screen.fullScreen; Screen.fullScreen=n;
            fsTxt.text=n?"BẬT":"TẮT"; fsTxt.color=n?ACC_GREEN:TXT_LABEL; PlayerPrefs.SetInt("Fullscreen",n?1:0);});
        y -= rh+g;

        // Quality cycle
        string[] qNames={"Very Low","Low","Medium","High","Very High","Ultra"};
        int cq=QualitySettings.GetQualityLevel();
        var qLbl=Rect("QL",_panelSettings.transform,V2(0.05f,y),V2(0.65f,y+rh)).AddComponent<TextMeshProUGUI>();
        qLbl.text="🎮  Chất lượng đồ hoạ"; qLbl.fontSize=18f; qLbl.color=TXT_HERO; qLbl.alignment=TextAlignmentOptions.MidlineLeft;
        var qGO=Img("QBtn",_panelSettings.transform,V2(0.67f,y+0.01f),V2(0.93f,y+rh-0.01f),BTN_DARK);
        qGO.GetComponent<Image>().sprite=SprRR(8); qGO.AddComponent<Button>();
        AddHover(qGO,BTN_DARK,C(0.10f,0.22f,0.18f));
        var qTxt=Rect("QT",qGO.transform,V2(0,0),V2(1,1)).AddComponent<TextMeshProUGUI>();
        qTxt.text=qNames[Mathf.Clamp(cq,0,5)]; qTxt.fontSize=16f; qTxt.color=ACC_GREEN; qTxt.alignment=TextAlignmentOptions.Center;
        qGO.GetComponent<Button>().onClick.AddListener(()=>{cq=(cq+1)%6;QualitySettings.SetQualityLevel(cq,true);
            qTxt.text=qNames[cq];PlayerPrefs.SetInt("Quality",cq);});

        SmallBtn("Btn_ClearSave",_panelSettings.transform,V2(0.05f,0.13f),V2(0.45f,0.21f),
            "XÓA TIẾN TRÌNH",ACC_RED,()=>{SaveSystem.ClearSaveData();_hasSave=false;});
        SmallBtn("Btn_BackS",_panelSettings.transform,V2(0.30f,0.03f),V2(0.70f,0.11f),
            "← QUAY LẠI",ACC_GREEN,()=>{Click();SwitchTo(null);});
    }

    // ─────────────────────────────────────────────────────────────────
    //  CONTROLS PANEL
    // ─────────────────────────────────────────────────────────────────
    void BuildPanelControls()
    {
        _panelControls = SidePanel("Panel_Controls", V2(0.04f,0.04f), V2(0.96f,0.96f));
        PanelHeader(_panelControls.transform, "ĐIỀU KHIỂN & HƯỚNG DẪN", ACC_GREEN);

        // Two-column layout
        var colL = Rect("ColL",_panelControls.transform,V2(0.03f,0.12f),V2(0.50f,0.88f));
        var colR = Rect("ColR",_panelControls.transform,V2(0.53f,0.12f),V2(0.97f,0.88f));

        ControlGroup(colL.transform, "DI CHUYỂN", new[]{
            ("W A S D",     "Di chuyển cơ bản"),
            ("Shift",       "Chạy (tiêu Stamina)"),
            ("Space",       "Nhảy"),
            ("Ctrl / C",    "Cúi người / Rón rén"),
            ("X",           "Nằm xuống"),
        });
        ControlGroup(colL.transform, "TƯƠNG TÁC", new[]{
            ("E",           "Nhặt vật phẩm / Tương tác"),
            ("F",           "Thắp lửa / Kích hoạt thiết bị"),
            ("Tab",         "Mở / Đóng Túi Đồ"),
            ("K",           "Mở Sổ Công Thức (Crafting)"),
            ("Chuột Trái",  "Tấn công / Khai thác / Chặt"),
        });

        ControlGroup(colR.transform, "SINH TỒN", new[]{
            ("HP",          "Máu – bị tấn công / đói / khát"),
            ("Stamina",     "Năng lượng – chạy & chiến đấu"),
            ("Đói",         "Ăn thức ăn để phục hồi"),
            ("Khát",        "Uống nước sạch"),
            ("Ngủ",         "Nghỉ ngơi để phục hồi Stamina"),
        });
        ControlGroup(colR.transform, "CHIẾN ĐẤU & BẢO TỒN", new[]{
            ("Nỏ Tần Số",   "Vũ khí tầm xa, tốn đạn"),
            ("Cây ≤5/ngày", "Bảo vệ môi trường – tăng Karma"),
            ("Bẫy Thú",     "Đặt bẫy bắt thú tự động"),
            ("Karma",       "Ảnh hưởng kết thúc game"),
            ("Esc",         "Mở menu Pause / Thoát"),
        });

        SmallBtn("Btn_BackC",_panelControls.transform,V2(0.35f,0.02f),V2(0.65f,0.10f),
            "← QUAY LẠI",ACC_GREEN,()=>{Click();SwitchTo(null);});
    }

    void ControlGroup(Transform parent, string title, (string key, string desc)[] rows)
    {
        // Find current bottom (first call starts at top)
        var allChildren = parent.childCount;

        float startY = 1f - allChildren * 0.50f;  // rough stacking
        var container = Rect($"CG_{title}", parent, V2(0,0), V2(1,1));

        // Vertical layout via absolute positions
        var titleT = Rect("T",container.transform,V2(0,0.90f),V2(1,1f)).AddComponent<TextMeshProUGUI>();
        titleT.text = title; titleT.fontSize = 14f; titleT.fontStyle = FontStyles.Bold;
        titleT.color = ACC_GREEN; titleT.alignment = TextAlignmentOptions.Left;

        // Use a VerticalLayoutGroup for automatic stacking
        var vl = container.gameObject.AddComponent<VerticalLayoutGroup>();
        vl.childControlHeight = false; vl.childControlWidth = true;
        vl.spacing = 2f; vl.padding = new RectOffset(4,4,4,4);

        // Title entry
        var titleEntry = new GameObject("TGroup_"+title, typeof(RectTransform));
        titleEntry.transform.SetParent(container.transform, false);
        titleEntry.GetComponent<RectTransform>().sizeDelta = new Vector2(0,22);
        var tt = titleEntry.AddComponent<TextMeshProUGUI>();
        tt.text = $"── {title} ──"; tt.fontSize = 13f; tt.fontStyle = FontStyles.Bold;
        tt.color = ACC_GREEN; tt.richText = true;

        foreach (var (key, desc) in rows)
        {
            var row = new GameObject($"Row_{key}", typeof(RectTransform));
            row.transform.SetParent(container.transform, false);
            row.GetComponent<RectTransform>().sizeDelta = new Vector2(0,20);
            var rt2 = row.AddComponent<TextMeshProUGUI>();
            rt2.text = $"<color=#2ED07A><b>{key}</b></color>  {desc}";
            rt2.fontSize = 14f; rt2.color = TXT_BODY; rt2.richText = true;
        }

        // Spacer
        var spacer = new GameObject("Spacer", typeof(RectTransform));
        spacer.transform.SetParent(container.transform, false);
        spacer.GetComponent<RectTransform>().sizeDelta = new Vector2(0,12);

        // Reset container to fill parent
        Stretch(container.GetComponent<RectTransform>());
        Destroy(titleGO: container.GetComponent<TextMeshProUGUI>());
    }

    // ─────────────────────────────────────────────────────────────────
    //  STORY / ABOUT PANEL
    // ─────────────────────────────────────────────────────────────────
    void BuildPanelStory()
    {
        _panelStory = SidePanel("Panel_Story", V2(0.04f,0.04f), V2(0.96f,0.96f));
        PanelHeader(_panelStory.transform, "CỐT TRUYỆN & THẾ GIỚI", ACC_AMBER);

        string storyText =
            "<b><color=#F5C332>THẾ GIỚI</color></b>\n" +
            "Năm 20XX. Một Vùng Đứt Gãy Không-Thời Gian bí ẩn (<b>Void Rift</b>) xuất hiện " +
            "giữa khu rừng nguyên sinh hàng trăm năm tuổi. Sinh vật bóng đêm tràn ra, " +
            "sương độc phủ kín mọi con đường. Bạn tỉnh dậy không một ký ức.\n\n" +

            "<b><color=#2ED07A>MAI AN TIÊM</color></b>\n" +
            "Qua sóng vô tuyến, giọng của một người đàn ông cổ xưa hướng dẫn bạn tìm " +
            "những hạt giống huyền bí. Ông chính là <b>Mai An Tiêm</b> — người đã sống " +
            "300 năm bảo vệ ranh giới giữa hai thế giới.\n\n" +

            "<b><color=#F5C332>5 HỒI CHIẾN DịCH (30 NGÀY)</color></b>\n" +
            "<color=#2ED07A>HỒI 1</color>  Ngày 1-3: Khởi Đầu Bí Ẩn  ·  Tìm đài Radio · Sinh tồn đêm sương độc\n" +
            "<color=#F5C332>HỒI 2</color>  Ngày 4-10: Chiến Đấu & Trường Kỳ Kháng Cự  ·  Boss Shadow Berserker\n" +
            "<color=#AA55FF>HỒI 3</color>  Ngày 11-20: Bí Ẩn Cổ Đại  ·  Giải đố mạch điện · Phế Tích Mai An Tiêm\n" +
            "<color=#CF3333>HỒI 4</color>  Ngày 21-29: Đại Chiến  ·  Void Leviathan · Pháo Đài Vô Tuyến\n" +
            "<color=#5588FF>HỒI 5</color>  Ngày 30: NGÀY PHÁN QUYẾT  ·  Chọn 1 trong 5 kết thúc\n\n" +

            "<b><color=#F5C332>5 KẾT THÚC</color></b>\n" +
            "① <color=#2ED07A>True Ending</color>  Sứ Giả Trở Về — Cứu thế giới & trở về làm anh hùng\n" +
            "② <color=#AA55FF>Tiếng Vọng</color>  Gặp Mai An Tiêm — Ở lại bảo vệ ranh giới 2 thế giới\n" +
            "③ <color=#CF3333>Tân Vương Bóng Đêm</color>  Dark Karma — Trị vì Void Rift vĩnh viễn\n" +
            "④ <color=#AAAAAA>Hy Sinh Thầm Lặng</color>  Tự hủy Trạm Vô Tuyến — Không ai biết bạn đã hy sinh\n" +
            "⑤ <color=#5588FF>Vòng Lặp Bí Ẩn</color>  Secret Ending — Bạn chính là người tạo ra vòng lặp";

        var contentGO = Rect("Content",_panelStory.transform,V2(0.03f,0.12f),V2(0.97f,0.87f));
        var mask = contentGO.AddComponent<RectMask2D>();
        var txt = contentGO.AddComponent<TextMeshProUGUI>();
        txt.text = storyText; txt.fontSize = 16f; txt.richText = true;
        txt.color = TXT_BODY; txt.alignment = TextAlignmentOptions.TopLeft;
        txt.lineSpacing = 5f;

        SmallBtn("Btn_BackSt",_panelStory.transform,V2(0.35f,0.02f),V2(0.65f,0.10f),
            "← QUAY LẠI",ACC_GREEN,()=>{Click();SwitchTo(null);});
    }

    // ─────────────────────────────────────────────────────────────────
    //  LOADING OVERLAY
    // ─────────────────────────────────────────────────────────────────
    void BuildLoadingOverlay()
    {
        _overlay = Img("LoadOverlay",_cvs.transform,V2(0,0),V2(1,1),C(0,0,0,0.97f)).gameObject;

        // Logo
        var logo = Rect("LoadLogo",_overlay.transform,V2(0.30f,0.55f),V2(0.70f,0.90f)).AddComponent<TextMeshProUGUI>();
        logo.text = "THE FOREST\nBETWEEN US"; logo.fontSize=48f; logo.fontStyle=FontStyles.Bold;
        logo.alignment=TextAlignmentOptions.Center; logo.enableVertexGradient=true;
        logo.colorGradient=new VertexGradient(ACC_AMBER,ACC_AMBER,ACC_GREEN,ACC_GREEN);

        // Tip
        var tip = Rect("LoadTip",_overlay.transform,V2(0.10f,0.37f),V2(0.90f,0.50f)).AddComponent<TextMeshProUGUI>();
        tip.name="LoadTip"; tip.text="Đang tải..."; tip.fontSize=20f;
        tip.color=ACC_GREEN; tip.alignment=TextAlignmentOptions.Center;

        // Progress bar bg
        var barBg = Img("BarBg",_overlay.transform,V2(0.10f,0.26f),V2(0.90f,0.33f),C(0.06f,0.10f,0.12f));
        Img("BarFill",barBg.transform,V2(0,0),V2(0,1),ACC_GREEN).name="LoadFill";

        // Pct
        var pct = Rect("Pct",_overlay.transform,V2(0.44f,0.16f),V2(0.56f,0.25f)).AddComponent<TextMeshProUGUI>();
        pct.name="LoadPct"; pct.text="0%"; pct.fontSize=20f;
        pct.alignment=TextAlignmentOptions.Center; pct.color=TXT_BODY;

        _overlay.SetActive(false);
    }

    // ─────────────────────────────────────────────────────────────────
    //  Panel switch
    // ─────────────────────────────────────────────────────────────────
    void SwitchTo(GameObject panel)
    {
        SafeSet(_panelMain,        panel == null);
        SafeSet(_panelSettings,    panel == _panelSettings);
        SafeSet(_panelControls,    panel == _panelControls);
        SafeSet(_panelStory,       panel == _panelStory);
        SafeSet(_panelConfirmNew,  panel == _panelConfirmNew);
        if (panel != null && panel != _panelConfirmNew) Open();
    }
    static void SafeSet(GameObject g, bool v){ if(g!=null) g.SetActive(v); }

    // ─────────────────────────────────────────────────────────────────
    //  Game actions
    // ─────────────────────────────────────────────────────────────────
    IEnumerator CoNewGame()
    {
        SaveSystem.ClearSaveData();
        yield return StartCoroutine(CoLoad(gameplaySceneName));
    }

    IEnumerator CoLoad(string scene)
    {
        _overlay.SetActive(true);
        var fill = _overlay.transform.Find("BarBg/LoadFill")?.GetComponent<RectTransform>();
        var tip  = _overlay.transform.Find("LoadTip")?.GetComponent<TextMeshProUGUI>();
        var pct  = _overlay.transform.Find("LoadPct")?.GetComponent<TextMeshProUGUI>();

        string[] tips = {
            "Bảo vệ rừng: chặt ≤ 5 cây mỗi ngày để giữ Karma tốt.",
            "Ban đêm nguy hiểm – Thổ dân K'Nu bị Hắc Hóa lúc nửa đêm!",
            "Cho Thỏ Rừng ăn 3 lần sẽ thuần hóa thành pet bảo vệ bạn.",
            "Karma cao = Ending Hòa Bình. Karma thấp = Tân Vương Bóng Đêm.",
            "Giải đố mạch điện cổ tại Phế Tích để mở bí mật Mai An Tiêm.",
            "Void Rift ngày càng lớn – hãy hành động trước ngày 30!",
        };
        float vol0=_music.volume; float fd=1.8f; float el=0; int ti=0;
        var op = SceneManager.LoadSceneAsync(scene);
        op.allowSceneActivation = false;
        while (!op.isDone)
        {
            el += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(op.progress/0.9f);
            if (fill!=null) fill.anchorMax=V2(p,1);
            if (pct!=null)  pct.text=Mathf.RoundToInt(p*100f)+"%";
            int ni=Mathf.FloorToInt(el/2.5f)%tips.Length;
            if (ni!=ti && tip!=null){ti=ni;tip.text=tips[ti];}
            _music.volume=Mathf.Lerp(vol0,0f,Mathf.Clamp01(el/fd));
            if (op.progress>=0.9f && el>=1.5f) op.allowSceneActivation=true;
            yield return null;
        }
    }

    IEnumerator CoQuit()
    {
        PlayerPrefs.Save(); yield return new WaitForSeconds(0.2f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ─────────────────────────────────────────────────────────────────
    //  PROCEDURAL FOREST AMBIENT MUSIC  (layers: drone+wind+pentatonic)
    // ─────────────────────────────────────────────────────────────────
    IEnumerator CoMusic()
    {
        yield return new WaitForEndOfFrame();
        _music.clip   = menuMusicClip ?? GenerateForestAmbient();
        _music.volume = 0f;
        _music.Play();
        float tgt = PlayerPrefs.GetFloat("MV",0.55f);
        float t=0f;
        while(t<3.5f){t+=Time.deltaTime;_music.volume=Mathf.Lerp(0,tgt,t/3.5f);yield return null;}
        _music.volume=tgt;
    }

    AudioClip GenerateForestAmbient()
    {
        int sr=44100; int dur=40; int total=sr*dur;
        float[] buf=new float[total];
        var rng=new System.Random(9001);
        float[] penta={261.63f,293.66f,329.63f,392f,440f,523.25f};
        for(int i=0;i<total;i++)
        {
            float t=(float)i/sr;
            // Deep drone (3 harmonics)
            float drone=Mathf.Sin(Mathf.PI*2*44f*t)*0.05f
                       +Mathf.Sin(Mathf.PI*2*66f*t)*0.03f
                       +Mathf.Sin(Mathf.PI*2*88f*t)*0.02f;
            float breath=(Mathf.Sin(Mathf.PI*2*0.18f*t)+1f)*0.5f;
            drone*=0.4f+breath*0.6f;
            // Filtered wind (low-frequency noise)
            float wind=(float)(rng.NextDouble()*2-1)*0.022f;
            float we=(Mathf.Sin(Mathf.PI*2*0.06f*t+0.8f)+1f)*0.5f; wind*=we;
            // Pentatonic ping (every 5 seconds, random note)
            float ping=0; float pt=t%5f;
            if(pt<0.8f){int ni=((int)(t/5f))%penta.Length;
                float env=Mathf.Exp(-pt*6f);
                ping=Mathf.Sin(Mathf.PI*2*penta[ni]*t)*env*0.05f;
                ping+=Mathf.Sin(Mathf.PI*2*penta[ni]*2f*t)*env*0.018f;}
            // Cricket chirp (subtle high-freq texture)
            float cricket=(float)(rng.NextDouble()*2-1)*0.008f;
            float ce=(Mathf.Sin(Mathf.PI*2*2.3f*t)+1f)*0.5f; cricket*=ce;
            buf[i]=Mathf.Clamp(drone+wind+ping+cricket,-1f,1f);
        }
        var clip=AudioClip.Create("ForestAmbient",total,1,sr,false);
        clip.SetData(buf,0); return clip;
    }

    // ─────────────────────────────────────────────────────────────────
    //  SFX helpers
    // ─────────────────────────────────────────────────────────────────
    void Click() => PlaySFX(sfxClickClip, 680f, 0.06f);
    void Open()  => PlaySFX(sfxOpenClip,  820f, 0.05f);
    void PlaySFX(AudioClip c, float freq, float dur)
    {
        if (_sfx==null) return;
        _sfx.PlayOneShot(c ?? Tone(freq,dur,0.25f));
    }
    AudioClip Tone(float f, float dur, float vol)
    {
        int sr=44100; int n=(int)(sr*dur); float[] d=new float[n];
        for(int i=0;i<n;i++){float t=(float)i/sr;
            d[i]=Mathf.Sin(Mathf.PI*2*f*t)*Mathf.Clamp01(1f-t/dur)*vol;}
        var c=AudioClip.Create("SFX",n,1,sr,false);c.SetData(d,0);return c;
    }

    // ─────────────────────────────────────────────────────────────────
    //  COROUTINE ANIMATIONS
    // ─────────────────────────────────────────────────────────────────
    IEnumerator CoFireflies()
    {
        var rng=new System.Random(55);
        while(true)
        {
            float dt=Time.deltaTime;
            foreach(var f in _flies)
            {
                if(f.rt==null) continue;
                float glow=0.5f+0.5f*Mathf.Sin(Time.time*1.2f+f.phase);
                var col=f.rt.GetComponent<Image>()?.color ?? Color.white;
                col.a=Mathf.Lerp(0.05f,0.55f,glow);
                if(f.rt.TryGetComponent<Image>(out var img)) img.color=col;

                var mn=f.rt.anchorMin+new Vector2(f.drift,f.speed);
                var mx=f.rt.anchorMax+new Vector2(f.drift,f.speed);
                if(mn.y>1.06f){mn.y=-0.06f;mx.y=mn.y+0.007f;}
                if(mn.x<-0.06f||mn.x>1.06f){float nx=(float)rng.NextDouble();mn.x=nx;mx.x=nx+0.004f;}
                f.rt.anchorMin=mn; f.rt.anchorMax=mx;
            }
            yield return null;
        }
    }

    IEnumerator CoFog()
    {
        while(true)
        {
            foreach(var f in _fog)
            {
                if(f.rt==null) continue;
                var mn=f.rt.anchorMin; var mx=f.rt.anchorMax;
                mn.x+=f.speed; mx.x+=f.speed;
                if(mn.x>1.1f){mn.x=-0.2f;mx.x=mn.x+(mx.x-mn.x);}
                if(mx.x<-0.1f){mn.x=0.9f;mx.x=mn.x+(mx.x-mn.x);}
                f.rt.anchorMin=mn; f.rt.anchorMax=mx;
            }
            yield return null;
        }
    }

    IEnumerator CoTitleBreath()
    {
        yield return new WaitForSeconds(1f);
        var go=_cvs?.transform.Find("Panel_Main/Left/TitleAnimated");
        if(go==null) yield break;
        var tmp=go.GetComponent<TextMeshProUGUI>();
        if(tmp==null) yield break;
        while(true)
        {
            float t=0;
            while(t<4f){t+=Time.deltaTime;tmp.fontSize=72f+Mathf.Sin(t*Mathf.PI*0.4f)*1.8f;yield return null;}
            t=0;
            while(t<4f){t+=Time.deltaTime;tmp.fontSize=73.8f-Mathf.Sin(t*Mathf.PI*0.4f)*1.8f;yield return null;}
        }
    }

    IEnumerator CoChromaShift()
    {
        // Animate the scanline position slowly up and down
        var scan=_cvs?.transform.Find("Scanline_Animated");
        if(scan==null) yield break;
        var rt=scan.GetComponent<RectTransform>();
        while(true)
        {
            float t=Time.time;
            float y=0.40f+Mathf.Sin(t*0.15f)*0.12f;
            rt.anchorMin=V2(0,y); rt.anchorMax=V2(1,y+0.004f);
            yield return null;
        }
    }

    // ─────────────────────────────────────────────────────────────────
    //  UI COMPONENT BUILDERS
    // ─────────────────────────────────────────────────────────────────
    // Full AAA-style button with icon accent + sub-label
    void Btn(string name, Transform p, Vector2 amin, Vector2 amax,
        string label, string sublabel, Color accentCol, System.Action onClick, bool disabled=false)
    {
        var go = Img(name, p, amin, amax, disabled
            ? C(0.04f,0.06f,0.08f,0.6f) : BTN_DARK);
        go.GetComponent<Image>().sprite = SprRR(10);

        // Left accent bar
        var bar = Img("Bar",go.transform,V2(0,0),V2(0.007f,1),
            C(accentCol.r,accentCol.g,accentCol.b, disabled?0.25f:1f));

        // Main label
        var lbl = Rect("Lbl",go.transform,V2(0.03f,0.42f),V2(0.85f,0.92f)).AddComponent<TextMeshProUGUI>();
        lbl.text=label; lbl.fontSize=22f; lbl.fontStyle=FontStyles.Bold;
        lbl.color=disabled?TXT_LABEL:TXT_HERO; lbl.alignment=TextAlignmentOptions.MidlineLeft;

        // Sub label
        var sub = Rect("Sub",go.transform,V2(0.03f,0.08f),V2(0.85f,0.44f)).AddComponent<TextMeshProUGUI>();
        sub.text=sublabel; sub.fontSize=13f; sub.color=disabled?TXT_LABEL:ACC_DIM;
        sub.alignment=TextAlignmentOptions.MidlineLeft;

        // Arrow indicator
        var arr = Rect("Arr",go.transform,V2(0.88f,0.25f),V2(0.97f,0.75f)).AddComponent<TextMeshProUGUI>();
        arr.text="›"; arr.fontSize=28f; arr.color=disabled?TXT_LABEL:accentCol;
        arr.alignment=TextAlignmentOptions.Center;

        // Hover / unhover
        Color normalBg = disabled?C(0.04f,0.06f,0.08f,0.6f):BTN_DARK;
        Color hoverBg  = C(0.08f,0.16f,0.12f,0.95f);
        AddHover(go, normalBg, hoverBg);

        var btn = go.AddComponent<Button>();
        if(!disabled && onClick!=null) btn.onClick.AddListener(()=>onClick());
        else btn.interactable=!disabled;
    }

    void SmallBtn(string name, Transform p, Vector2 amin, Vector2 amax,
        string label, Color col, System.Action onClick)
    {
        var go = Img(name,p,amin,amax, C(col.r*0.25f,col.g*0.25f,col.b*0.25f,0.9f));
        go.GetComponent<Image>().sprite=SprRR(8);
        AddHover(go, C(col.r*0.25f,col.g*0.25f,col.b*0.25f,0.9f), C(col.r*0.45f,col.g*0.45f,col.b*0.45f));
        var t=Rect("T",go.transform,V2(0,0),V2(1,1)).AddComponent<TextMeshProUGUI>();
        t.text=label; t.fontSize=18f; t.fontStyle=FontStyles.Bold;
        t.alignment=TextAlignmentOptions.Center; t.color=TXT_HERO;
        var b=go.AddComponent<Button>(); if(onClick!=null)b.onClick.AddListener(()=>onClick());
    }

    void AddHover(GameObject go, Color normal, Color hover)
    {
        var img=go.GetComponent<Image>();
        var et=go.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        void Add(UnityEngine.EventSystems.EventTriggerType type, System.Action<UnityEngine.EventSystems.BaseEventData> cb)
        {
            var e=new UnityEngine.EventSystems.EventTrigger.Entry{eventID=type};
            e.callback.AddListener(d=>cb(d)); et.triggers.Add(e);
        }
        Add(UnityEngine.EventSystems.EventTriggerType.PointerEnter,_=>{if(img)img.color=hover;PlaySFX(sfxHoverClip,920f,0.04f);});
        Add(UnityEngine.EventSystems.EventTriggerType.PointerExit, _=>{if(img)img.color=normal;});
    }

    // ─────────────────────────────────────────────────────────────────
    //  Panel template
    // ─────────────────────────────────────────────────────────────────
    GameObject SidePanel(string name, Vector2 amin, Vector2 amax)
    {
        var p=Img(name,_cvs.transform,amin,amax,BG_PANEL);
        p.GetComponent<Image>().sprite=SprRR(20);
        // Glow border
        var b=Img(name+"_B",p.transform,V2(-0.003f,-0.003f),V2(1.003f,1.003f),
            C(ACC_GREEN.r,ACC_GREEN.g,ACC_GREEN.b,0.12f));
        b.GetComponent<Image>().sprite=SprRR(20);
        b.transform.SetAsFirstSibling();
        return p.gameObject;
    }

    void PanelHeader(Transform p, string title, Color col)
    {
        var h=Rect("Hdr",p,V2(0,0.90f),V2(1,1));
        h.AddComponent<Image>().color=C(col.r,col.g,col.b,0.09f);
        var t=Rect("HdrT",h.transform,V2(0.04f,0),V2(0.96f,1)).AddComponent<TextMeshProUGUI>();
        t.text=title; t.fontSize=24f; t.fontStyle=FontStyles.Bold;
        t.alignment=TextAlignmentOptions.MidlineLeft; t.color=col;
        Sep("HdrSep",p,V2(0.02f,0.893f),V2(0.98f,0.899f));
    }

    void SliderRow(Transform p, string label, ref float y, float rh, float gap,
        float min, float max, float init, System.Action<float> onChange,
        ref Slider slider, ref TextMeshProUGUI valLbl)
    {
        Rect(label+"_L",p,V2(0.04f,y),V2(0.52f,y+rh)).AddComponent<TextMeshProUGUI>()
            .text=label; var lt=p.Find(label+"_L").GetComponent<TextMeshProUGUI>();
        lt.fontSize=17f; lt.color=TXT_HERO; lt.alignment=TextAlignmentOptions.MidlineLeft;

        valLbl=Rect(label+"_V",p,V2(0.87f,y),V2(0.97f,y+rh)).AddComponent<TextMeshProUGUI>();
        valLbl.fontSize=14f; valLbl.color=ACC_GREEN; valLbl.alignment=TextAlignmentOptions.Center;
        valLbl.text=Mathf.RoundToInt(init*100)+"%";

        var sGO=Rect(label+"_S",p,V2(0.52f,y+rh*0.22f),V2(0.85f,y+rh*0.78f));
        slider=sGO.AddComponent<Slider>();
        Img("SBg",sGO.transform,V2(0,0),V2(1,1),C(0.06f,0.10f,0.12f));
        var fa=Rect("FA",sGO.transform,V2(0,0.2f),V2(1,0.8f));
        var fill=Img("Fill",fa.transform,V2(0,0),V2(1,1),ACC_GREEN);
        var hsa=Rect("HA",sGO.transform,V2(0,0),V2(1,1));
        var hand=Img("Hand",hsa.transform,V2(0,0),V2(0,1),C(0.9f,0.7f,0.2f));
        hand.GetComponent<RectTransform>().sizeDelta=new Vector2(12,0);
        slider.fillRect=fill.GetComponent<RectTransform>();
        slider.handleRect=hand.GetComponent<RectTransform>();
        slider.targetGraphic=hand.GetComponent<Image>();
        slider.direction=Slider.Direction.LeftToRight;
        slider.minValue=min; slider.maxValue=max; slider.value=init;
        var vl=valLbl;
        slider.onValueChanged.AddListener(v=>{onChange(v);vl.text=Mathf.RoundToInt(v*100)+"%";});
        y-=rh+gap;
    }

    // ─────────────────────────────────────────────────────────────────
    //  LOW-LEVEL HELPERS
    // ─────────────────────────────────────────────────────────────────
    static GameObject Rect(string name, Transform p, Vector2 amin, Vector2 amax)
    {
        var go=new GameObject(name,typeof(RectTransform));
        go.transform.SetParent(p,false);
        var rt=go.GetComponent<RectTransform>();
        rt.anchorMin=amin; rt.anchorMax=amax; rt.offsetMin=rt.offsetMax=Vector2.zero;
        return go;
    }
    static GameObject Img(string name, Transform p, Vector2 amin, Vector2 amax, Color col)
    {
        var go=Rect(name,p,amin,amax); go.AddComponent<Image>().color=col; return go;
    }
    static void Sep(string name, Transform p, Vector2 amin, Vector2 amax)
        => Img(name,p,amin,amax,CHROME_LINE);
    static void Stretch(RectTransform rt){rt.anchorMin=Vector2.zero;rt.anchorMax=Vector2.one;rt.offsetMin=rt.offsetMax=Vector2.zero;}
    static Vector2 V2(float x, float y)=>new(x,y);
    static void Destroy(TextMeshProUGUI titleGO){if(titleGO)Object.Destroy(titleGO);}

    // ─────────────────────────────────────────────────────────────────
    //  PROCEDURAL SPRITES
    // ─────────────────────────────────────────────────────────────────
    static Sprite SprGradient(Color top, Color bot, int w=4, int h=512)
    {
        var tex=new Texture2D(w,h,TextureFormat.RGBA32,false);
        tex.wrapMode=TextureWrapMode.Clamp;
        for(int y=0;y<h;y++){Color c=Color.Lerp(bot,top,(float)y/h);for(int x=0;x<w;x++)tex.SetPixel(x,y,c);}
        tex.Apply(); return Sprite.Create(tex,new Rect(0,0,w,h),V2(0.5f,0.5f));
    }
    static Sprite SprRadialGlow(Color center, int s=512)
    {
        var tex=new Texture2D(s,s,TextureFormat.RGBA32,false); tex.wrapMode=TextureWrapMode.Clamp;
        Vector2 c=V2(s/2f,s/2f); float r=s/2f;
        for(int y=0;y<s;y++) for(int x=0;x<s;x++)
        {float d=Vector2.Distance(V2(x,y),c)/r;float a=Mathf.SmoothStep(1,0,d);tex.SetPixel(x,y,new Color(center.r,center.g,center.b,center.a*a));}
        tex.Apply(); return Sprite.Create(tex,new Rect(0,0,s,s),V2(0.5f,0.5f));
    }
    static Sprite SprVignette(float strength=0.85f, int s=512)
    {
        var tex=new Texture2D(s,s,TextureFormat.RGBA32,false); tex.wrapMode=TextureWrapMode.Clamp;
        Vector2 c=V2(s/2f,s/2f); float r=Vector2.Distance(Vector2.zero,c);
        for(int y=0;y<s;y++) for(int x=0;x<s;x++)
        {float a=Mathf.SmoothStep(0,strength,Vector2.Distance(V2(x,y),c)/r);tex.SetPixel(x,y,new Color(0,0,0,a));}
        tex.Apply(); return Sprite.Create(tex,new Rect(0,0,s,s),V2(0.5f,0.5f));
    }
    static Sprite SprFogStrip(int w=256, int h=32)
    {
        var tex=new Texture2D(w,h,TextureFormat.RGBA32,false); tex.wrapMode=TextureWrapMode.Clamp;
        for(int y=0;y<h;y++) for(int x=0;x<w;x++)
        {float ex=1f-Mathf.Abs(x/(float)w*2f-1f);float ey=1f-Mathf.Abs(y/(float)h*2f-1f);tex.SetPixel(x,y,new Color(1,1,1,ex*ey));}
        tex.Apply(); return Sprite.Create(tex,new Rect(0,0,w,h),V2(0.5f,0.5f));
    }
    static Sprite SprRR(int radius=10, int w=128, int h=64)
    {
        var tex=new Texture2D(w,h,TextureFormat.RGBA32,false); tex.wrapMode=TextureWrapMode.Clamp;
        for(int y=0;y<h;y++) for(int x=0;x<w;x++)
        {int cx=Mathf.Clamp(x,radius,w-radius-1);int cy=Mathf.Clamp(y,radius,h-radius-1);
            float d=Mathf.Sqrt((x-cx)*(x-cx)+(y-cy)*(y-cy));tex.SetPixel(x,y,d<=radius?Color.white:Color.clear);}
        tex.Apply();
        return Sprite.Create(tex,new Rect(0,0,w,h),V2(0.5f,0.5f),100f,0,SpriteMeshType.FullRect,new Vector4(radius,radius,radius,radius));
    }
    static Sprite SprCircle(int s=24)
    {
        var tex=new Texture2D(s,s,TextureFormat.RGBA32,false); tex.wrapMode=TextureWrapMode.Clamp;
        Vector2 c=V2(s/2f,s/2f); float r=s/2f-1f;
        for(int y=0;y<s;y++) for(int x=0;x<s;x++)
        {float a=Mathf.Clamp01(r-Mathf.Max(0,Vector2.Distance(V2(x,y),c)-r+1));tex.SetPixel(x,y,new Color(1,1,1,a));}
        tex.Apply(); return Sprite.Create(tex,new Rect(0,0,s,s),V2(0.5f,0.5f));
    }
}
