using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// MainMenuController — AAA main menu for "The Forest Between Us".
/// Sons-of-the-Forest cinematic style.
/// Fully code-driven, no prefabs required.
/// Scene flow: Home ──► Tutorial ──► GamePlay
/// </summary>
[ExecuteAlways]
public class MainMenuController : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────────
    //  Inspector – Scene Names
    // ─────────────────────────────────────────────────────────────────
    [Header("─── Scene Names ───────────────────")]
    public string tutorialSceneName = "Tutorial";    // Home → Tutorial (first time)
    public string gameplaySceneName = "GamePlay";    // Home → GamePlay (continue)

    [Header("─── Video Background (Kéo file video .mp4/.webm vào đây) ───")]
    [Tooltip("Kéo file video cảnh rừng quay sẵn (.mp4) vào đây để làm nền động cho Main Menu.")]
    public UnityEngine.Video.VideoClip backgroundVideoClip;

    [Header("─── Music (để trống = dùng nhạc sinh bằng code) ───")]
    [Tooltip("Kéo file .mp3/.ogg nhạc menu vào đây. Nếu trống, nhạc rừng procedural tự phát.")]
    public AudioClip menuMusicClip;

    [Header("─── SFX (để trống = dùng tone sinh bằng code) ───")]
    public AudioClip sfxHover;
    public AudioClip sfxClick;
    public AudioClip sfxOpen;
    public AudioClip sfxClose;

    [Header("─── Story Panel Images (kéo Texture2D vào) ───")]
    [Tooltip("Ảnh cho HỒI 1 – cảnh mù sương, đài radio")]
    public Texture2D storyImg_Act1;
    [Tooltip("Ảnh cho HỒI 2 – chiến đấu bóng đêm")]
    public Texture2D storyImg_Act2;
    [Tooltip("Ảnh cho HỒI 3 – phế tích cổ đại")]
    public Texture2D storyImg_Act3;
    [Tooltip("Ảnh cho HỒI 4 – đại chiến Void Leviathan")]
    public Texture2D storyImg_Act4;
    [Tooltip("Ảnh cho HỒI 5 – ngày phán quyết")]
    public Texture2D storyImg_Act5;

    [Header("─── Controls Panel Images (kéo Texture2D vào) ───")]
    public Texture2D controlsImg_Left;   // Ảnh minh họa trái (di chuyển/tương tác)
    public Texture2D controlsImg_Right;  // Ảnh minh họa phải (sinh tồn/chiến đấu)

    // ─────────────────────────────────────────────────────────────────
    //  Private
    // ─────────────────────────────────────────────────────────────────
    Canvas      _cvs;
    AudioSource _music, _sfx;

    GameObject _panelMain, _panelSettings, _panelControls;
    GameObject _panelStory, _panelCredits, _panelConfirmNew;
    GameObject _overlay;

    bool _hasSave;
    int  _saveDay, _saveKarma;

    readonly List<FireflyData> _flies = new();
    readonly List<FogData>     _fogStrips = new();
    struct FireflyData { public RectTransform rt; public float speed, drift, phase; public Image img; }
    struct FogData     { public RectTransform rt; public float speed; }

    int _storyPage; // 0-4

    // ─────────────────────────────────────────────────────────────────
    //  AAA Palette (Sons-of-the-Forest dark-green)
    // ─────────────────────────────────────────────────────────────────
    static Color HC(float r,float g,float b,float a=1f)=>new(r,g,b,a);
    static readonly Color C_ABYSS    = HC(0.016f,0.024f,0.038f);
    static readonly Color C_PANEL    = HC(0.055f,0.095f,0.120f,0.97f);  // lighter for visibility
    static readonly Color C_BTN      = HC(0.080f,0.145f,0.175f,0.95f);  // clearly visible button bg
    static readonly Color C_GREEN    = HC(0.16f, 0.82f, 0.44f);
    static readonly Color C_AMBER    = HC(0.92f, 0.70f, 0.12f);
    static readonly Color C_RED      = HC(0.78f, 0.14f, 0.14f);
    static readonly Color C_PURPLE   = HC(0.58f, 0.22f, 0.88f);
    static readonly Color C_BLUE     = HC(0.20f, 0.52f, 0.92f);
    static readonly Color C_DIM      = HC(0.36f, 0.52f, 0.46f);
    static readonly Color C_TXT      = HC(0.92f, 0.96f, 0.94f);
    static readonly Color C_TXT2     = HC(0.60f, 0.72f, 0.66f);
    static readonly Color C_SEP      = HC(0.16f, 0.82f, 0.44f, 0.18f);

    // ─────────────────────────────────────────────────────────────────
    //  Story Act Data
    // ─────────────────────────────────────────────────────────────────
    struct ActData
    {
        public string accentHex;
        public string actTag;
        public string title;
        public string dayRange;
        public string lore;
        public string[] highlights;
    }

    static readonly ActData[] ACTS = {
        new ActData {
            accentHex="#2ED07A", actTag="HỒI I", title="KHỞI ĐẦU BÍ ẨN",
            dayRange="Ngày 1 – 3",
            lore=
                "Bạn tỉnh dậy giữa khu rừng nguyên sinh phủ dày sương mù tím. " +
                "Không có ký ức, không có vũ khí, chỉ có tiếng rè của một chiếc " +
                "<b>đài Radio cũ</b> vọng ra từ xa.\n\n" +
                "Qua làn sóng nhiễu loạn, giọng một người đàn ông cổ xưa vang lên: " +
                "<i>\"Con ơi… đừng phá rừng… đừng phá rừng…\"</i>\n\n" +
                "Đó là <color=#2ED07A><b>Mai An Tiêm</b></color> — người đã tồn tại " +
                "hơn 300 năm, bảo vệ ranh giới giữa hai thế giới.",
            highlights=new[]{"Tìm Đài Radio","Nhặt Dưa Hấu Hạt Đen","Đêm Sương Độc đầu tiên","Kích hoạt Trạm Tiếp Sóng 01"}
        },
        new ActData {
            accentHex="#F5A623", actTag="HỒI II", title="CHIẾN ĐẤU & TRƯỜNG KỲ",
            dayRange="Ngày 4 – 10",
            lore=
                "Bóng tối có sự sống. Những sinh thể được gọi là " +
                "<color=#F5A623><b>Quái Vật Bóng Đêm</b></color> — tàn dư của " +
                "Vùng Đứt Gãy Void Rift bắt đầu chủ động săn lùng bạn.\n\n" +
                "Bạn chế tạo <b>Nỏ Tần Số</b>, xây dựng căn cứ, đặt bẫy. " +
                "Nhưng mọi thứ leo thang đến đêm <b>Trăng Máu Hạt Đen</b> — " +
                "đợt càn quét đầu tiên tàn bạo nhất.\n\n" +
                "Trùm đầu: <color=#F5A623><b>Shadow Berserker</b></color> chặn đường " +
                "đến tháp viễn thông.",
            highlights=new[]{"Đêm Trăng Máu Hạt Đen (Wave 5)","Boss Shadow Berserker","Chế tạo Mặt Nạ Lọc Khí","Cơn Bão Rift đầu tiên (Wave 10)"}
        },
        new ActData {
            accentHex="#AA55FF", actTag="HỒI III", title="BÍ ẨN CỔ ĐẠI",
            dayRange="Ngày 11 – 20",
            lore=
                "Dưới nền rừng cổ đại, những <b>bia đá khắc chữ Nôm</b> lẫn " +
                "mạch điện kỳ lạ hé lộ sự thật: khu rừng này từng là " +
                "<color=#AA55FF><b>Phế Tích Mai An Tiêm</b></color>.\n\n" +
                "Truyền thuyết và khoa học đan xen. Bạn giải đố mạch điện cổ đại, " +
                "chiến đấu với <b>Shadow Drakes</b> bay lượn, và thu thập " +
                "4 mảnh <b>Chìa Khóa Tần Số Trụ Vũ Trụ</b>.\n\n" +
                "Void Rift đang lớn dần. Thời gian không còn nhiều.",
            highlights=new[]{"Phế Tích Cổ (3 phần)","Boss Shadow Drakes","Thu Chìa Khóa Tần Số (4 mảnh)","Giải đố mạch điện cổ đại"}
        },
        new ActData {
            accentHex="#CF3333", actTag="HỒI IV", title="ĐẠI CHIẾN RANH GIỚI",
            dayRange="Ngày 21 – 29",
            lore=
                "Pháo đài cuối cùng của nhân loại: <b>Trạm Vô Tuyến Siêu Cấp</b>. " +
                "Bạn gia cố tường thành, lắp đặt Pháo Sóng Âm tự động trong khi " +
                "bầy quái ngày càng đông.\n\n" +
                "Và rồi nó xuất hiện — <color=#CF3333><b>Void Leviathan</b></color>: " +
                "Trùm Cổ Đại từ sâu thẳm Vùng Đứt Gãy, thực thể đã ngủ yên " +
                "hàng nghìn năm.\n\n" +
                "<b>Ngày 29:</b> Mai An Tiêm trò chuyện lần cuối qua vô tuyến. " +
                "Ông hỏi bạn: <i>\"Con sẽ chọn vận mệnh nào cho nhân loại?\"</i>",
            highlights=new[]{"Pháo Đài Vô Tuyến Siêu Cấp","Boss Void Leviathan","Cuộc chiến Wave 12","Đêm Trước Phán Quyết"}
        },
        new ActData {
            accentHex="#4488FF", actTag="HỒI V", title="NGÀY PHÁN QUYẾT",
            dayRange="Ngày 30  ·  5 Kết Thúc",
            lore=
                "Cổng <color=#4488FF><b>Void Rift</b></color> mở ra hoàn toàn. " +
                "Không gian và thời gian rạn nứt. Đây là khoảnh khắc quyết định.\n\n" +
                "Karma của bạn suốt 30 ngày — mỗi cây chặt, mỗi sinh linh giết, " +
                "mỗi lựa chọn — tất cả hội tụ về đây.\n\n" +
                "Bạn có <b>5 lựa chọn vận mệnh</b>. Mỗi lựa chọn mở ra một " +
                "kết thúc khác nhau, không thể đảo ngược.",
            highlights=new[]{
                "① Sứ Giả Trở Về  (True Ending)",
                "② Tiếng Vọng Không-Thời Gian",
                "③ Tân Vương Bóng Đêm  (Dark Karma)",
                "④ Sự Hy Sinh Thầm Lặng",
                "⑤ Vòng Lặp Vĩnh Hằng  (Secret)"
            }
        }
    };

    // ─────────────────────────────────────────────────────────────────
    //  Lifecycle
    // ─────────────────────────────────────────────────────────────────
    void Awake()
    {
        if (Application.isPlaying)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
            Time.timeScale   = 1f;
            _music = gameObject.AddComponent<AudioSource>();
            _music.loop = true; _music.spatialBlend = 0f;
            _music.volume = PlayerPrefs.GetFloat("MV", 0.55f);
            _sfx = gameObject.AddComponent<AudioSource>();
            _sfx.spatialBlend = 0f;
            _sfx.volume = PlayerPrefs.GetFloat("SV", 0.85f);
            _hasSave = SaveSystem.LoadProgress(out _saveDay, out _saveKarma, out _);
        }
    }

    void OnEnable()
    {
        if (!Application.isPlaying)
        {
            GenerateMenuUI();
        }
    }

    void OnDisable()
    {
        if (!Application.isPlaying && _cvs != null)
        {
            DestroyImmediate(_cvs.gameObject);
            _cvs = null;
        }
    }

    void Start()
    {
        GenerateMenuUI();

        if (Application.isPlaying)
        {
            StartCoroutine(CoStartMusic());
            StartCoroutine(CoAnimateFireflies());
            StartCoroutine(CoAnimateFog());
            StartCoroutine(CoTitleBreath());
            StartCoroutine(CoScanlineFloat());
        }
    }

    void GenerateMenuUI()
    {
        if (_cvs != null) return;

        DestroyOldCanvases(); // must run first
        BuildCanvas();
        BuildBackground();
        BuildFirefliesLayer();
        BuildFogLayer();
        BuildPanelMain();
        BuildPanelStory();
        BuildPanelControls();
        BuildPanelSettings();
        BuildPanelCredits();
        BuildPanelConfirmNew();
        BuildLoadingOverlay();
        SwitchTo(null);
    }

    /// <summary>Destroy any Canvas in the scene that is NOT ours, so nothing covers the menu.</summary>
    void DestroyOldCanvases()
    {
        Canvas[] all = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (Canvas c in all)
        {
            // Skip the one we're about to create (name match) and DontDestroyOnLoad ones
            if (c == null) continue;
            string n = c.gameObject.name;
            if (n == "MainMenu_Canvas" || n == "SceneTransitionManager") continue;
            // Destroy the old scene Canvas and all its UI children
            Destroy(c.gameObject);
        }
    }

    // ─────────────────────────────────────────────────────────────────
    //  CANVAS
    // ─────────────────────────────────────────────────────────────────
    void BuildCanvas()
    {
        var go = new GameObject("MainMenu_Canvas");
        _cvs = go.AddComponent<Canvas>();
        _cvs.renderMode   = RenderMode.ScreenSpaceOverlay;
        _cvs.sortingOrder = 9999; // highest possible — on top of everything
        var cs = go.AddComponent<CanvasScaler>();
        cs.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cs.screenMatchMode     = CanvasScaler.ScreenMatchMode.Expand;
        go.AddComponent<GraphicRaycaster>();
    }

    // ─────────────────────────────────────────────────────────────────
    //  CINEMATIC BACKGROUND (ẢNH & VIDEO)
    // ─────────────────────────────────────────────────────────────────
    void BuildBackground()
    {
        MkImg("BG_Grad", _cvs.transform, V2(0,0), V2(1,1), C_ABYSS)
            .GetComponent<Image>().sprite = MkGradient(HC(0.008f,0.012f,0.022f), HC(0.030f,0.055f,0.072f));

        // Nếu có Video Clip background, dựng RawImage & VideoPlayer để phát Video loop
        UnityEngine.Video.VideoPlayer videoPlayer = FindFirstObjectByType<UnityEngine.Video.VideoPlayer>();
        if (backgroundVideoClip != null || videoPlayer != null)
        {
            var rawImgGO = new GameObject("BG_VideoRawImage", typeof(RectTransform), typeof(RawImage));
            rawImgGO.transform.SetParent(_cvs.transform, false);
            var rect = rawImgGO.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            RawImage rawImage = rawImgGO.GetComponent<RawImage>();
            rawImage.color = new Color(0.85f, 0.85f, 0.85f, 0.95f);

            if (videoPlayer == null)
            {
                videoPlayer = rawImgGO.AddComponent<UnityEngine.Video.VideoPlayer>();
            }

            if (backgroundVideoClip != null)
            {
                videoPlayer.clip = backgroundVideoClip;
            }

            videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.APIOnly;
            videoPlayer.isLooping = true;
            videoPlayer.playOnAwake = true;
            videoPlayer.audioOutputMode = UnityEngine.Video.VideoAudioOutputMode.None;

            RenderTexture rt = new RenderTexture(1920, 1080, 16, RenderTextureFormat.ARGB32);
            rt.Create();
            videoPlayer.targetTexture = rt;
            rawImage.texture = rt;

            if (Application.isPlaying)
            {
                videoPlayer.Play();
            }
        }

        MkImg("BG_Radial", _cvs.transform, V2(0,0), V2(1,1), Color.clear)
            .GetComponent<Image>().sprite = MkRadialGlow(HC(0.04f,0.12f,0.09f,0.30f));

        MkImg("BG_Vignette", _cvs.transform, V2(0,0), V2(1,1), Color.clear)
            .GetComponent<Image>().sprite = MkVignette(0.88f);

        // Cinematic letterbox bars
        MkImg("Bar_Top",    _cvs.transform, V2(0,0.935f), V2(1,1.000f), HC(0,0,0,0.82f));
        MkImg("Bar_Bottom", _cvs.transform, V2(0,0.000f), V2(1,0.040f), HC(0,0,0,0.82f));

        // Animated scanline
        var scan = MkImg("Scanline", _cvs.transform, V2(0,0.44f), V2(1,0.445f),
            HC(C_GREEN.r,C_GREEN.g,C_GREEN.b,0.055f));
        scan.name = "Scanline";
    }

    // ─────────────────────────────────────────────────────────────────
    //  FIREFLIES
    // ─────────────────────────────────────────────────────────────────
    void BuildFirefliesLayer()
    {
        var layer = MkRect("Flies", _cvs.transform, V2(0,0), V2(1,1));
        var rng = new System.Random(404);
        for (int i = 0; i < 48; i++)
        {
            float sz = (float)(rng.NextDouble()*5.5f+1.5f);
            Color col = i%5==0 ? C_AMBER
                      : i%5==1 ? C_GREEN
                      : i%5==2 ? HC(0.4f,0.9f,0.7f)
                      : i%5==3 ? HC(0.6f,0.8f,1.0f)
                      : HC(0.9f,0.6f,0.2f);
            col.a = (float)(rng.NextDouble()*0.45f+0.1f);
            float ax=(float)rng.NextDouble(), ay=(float)rng.NextDouble();
            var dot = MkImg($"F{i}", layer.transform, V2(ax,ay), V2(ax+0.004f,ay+0.007f), col);
            dot.GetComponent<Image>().sprite = MkCircle(14);
            _flies.Add(new FireflyData{
                rt    = dot.GetComponent<RectTransform>(),
                img   = dot.GetComponent<Image>(),
                speed = (float)(rng.NextDouble()*0.00013f+0.00004f),
                drift = (float)((rng.NextDouble()-0.5)*0.00008f),
                phase = (float)(rng.NextDouble()*Mathf.PI*2f)
            });
        }
    }

    // ─────────────────────────────────────────────────────────────────
    //  FOG
    // ─────────────────────────────────────────────────────────────────
    void BuildFogLayer()
    {
        var layer = MkRect("Fog", _cvs.transform, V2(0,0), V2(1,1));
        var rng = new System.Random(7);
        for (int i = 0; i < 7; i++)
        {
            float y=(float)(rng.NextDouble()*0.65f+0.04f);
            float h=(float)(rng.NextDouble()*0.045f+0.012f);
            Color c=HC(0.08f,0.20f,0.16f,(float)(rng.NextDouble()*0.07f+0.02f));
            var strip = MkImg($"Fog{i}", layer.transform, V2(-0.12f,y), V2(1.12f,y+h), c);
            strip.GetComponent<Image>().sprite = MkFogStrip();
            _fogStrips.Add(new FogData{
                rt    = strip.GetComponent<RectTransform>(),
                speed = (float)((rng.NextDouble()-0.5)*0.00007f)
            });
        }
    }

    // ─────────────────────────────────────────────────────────────────
    //  MAIN PANEL
    // ─────────────────────────────────────────────────────────────────
    void BuildPanelMain()
    {
        _panelMain = MkRect("Panel_Main", _cvs.transform, V2(0,0), V2(1,1)).gameObject;

        // ══ LEFT – title + lore ═════════════════════════════════════
        var L = MkRect("Left", _panelMain.transform, V2(0.03f,0.08f), V2(0.50f,0.92f));

        // Badge background
        var bg = MkImg("TitleBg", L.transform, V2(0f,0.68f), V2(1f,1.00f),
            HC(0.02f,0.05f,0.04f,0.55f));
        bg.GetComponent<Image>().sprite = MkRR(16);
        // Accent strip (left edge)
        MkImg("AccentStrip", L.transform, V2(0f,0.68f), V2(0.010f,1f), C_GREEN);

        // THE FOREST
        var t1 = MkRect("T1", L.transform, V2(0.04f,0.82f), V2(0.98f,0.99f)).AddComponent<TextMeshProUGUI>();
        t1.text = "THE FOREST"; t1.fontSize = 74f; t1.fontStyle = FontStyles.Bold;
        t1.alignment = TextAlignmentOptions.Left;
        t1.enableVertexGradient = true;
        t1.colorGradient = new VertexGradient(C_TXT, C_TXT, HC(0.55f,0.88f,0.70f), HC(0.25f,0.55f,0.40f));
        t1.gameObject.name = "TitleAnim";

        // BETWEEN US
        var t2 = MkRect("T2", L.transform, V2(0.04f,0.70f), V2(0.98f,0.83f)).AddComponent<TextMeshProUGUI>();
        t2.text = "BETWEEN US"; t2.fontSize = 54f; t2.fontStyle = FontStyles.Bold;
        t2.alignment = TextAlignmentOptions.Left;
        t2.color = C_GREEN; t2.characterSpacing = 9f;

        // Tagline
        var tag = MkRect("Tag", L.transform, V2(0.04f,0.625f), V2(0.95f,0.705f)).AddComponent<TextMeshProUGUI>();
        tag.text = "Khu Rừng Giữa Chúng Ta  ·  30 Ngày  ·  5 Kết Thúc";
        tag.fontSize = 17f; tag.fontStyle = FontStyles.Italic; tag.color = C_DIM;

        Sep("S1", L.transform, V2(0.03f,0.612f), V2(0.90f,0.618f));

        // Lore intro text
        var lore = MkRect("Lore", L.transform, V2(0.02f,0.40f), V2(0.96f,0.610f)).AddComponent<TextMeshProUGUI>();
        lore.richText = true;
        lore.text =
            "Bạn tỉnh dậy giữa sương mù dày đặc.\n" +
            "Chiếc đài Radio cũ thì thầm tiếng của " +
            "<color=#2ED07A><b>Mai An Tiêm</b></color> — người đã biến mất 300 năm.\n\n" +
            "Một <b>Vùng Đứt Gãy Void Rift</b> bí ẩn đang mở ra, " +
            "nuốt chửng mọi ranh giới giữa thế giới ánh sáng và bóng tối.\n\n" +
            "<color=#F5A623>30 ngày.</color>  <color=#AA55FF>5 kết thúc.</color>  " +
            "<color=#CF3333>Một phán quyết.</color>  Số phận nhân loại trong tay bạn.";
        lore.fontSize = 17.5f; lore.color = C_TXT2; lore.lineSpacing = 5f;

        Sep("S2", L.transform, V2(0.03f,0.385f), V2(0.90f,0.391f));

        // Act badges row
        BuildActBadgesRow(L.transform);

        // Save badge
        if (_hasSave)
        {
            var sb = MkImg("SaveBadge", L.transform, V2(0.02f,0.105f), V2(0.72f,0.185f),
                HC(0.04f,0.18f,0.10f,0.90f));
            sb.GetComponent<Image>().sprite = MkRR(8);
            MkImg("SBL", sb.transform, V2(0,0), V2(0.007f,1), C_GREEN);
            var st = MkRect("ST", sb.transform, V2(0.03f,0), V2(0.97f,1)).AddComponent<TextMeshProUGUI>();
            st.richText = true;
            st.text = $"💾  Ngày <b><color=#2ED07A>{_saveDay}</color></b> / 30    Karma <b><color=#F5A623>{_saveKarma}</color></b> / 100";
            st.fontSize = 15.5f; st.color = C_TXT2; st.alignment = TextAlignmentOptions.MidlineLeft;
        }

        // Version & credits mini
        var ver = MkRect("Ver", L.transform, V2(0.02f,0.022f), V2(0.65f,0.085f)).AddComponent<TextMeshProUGUI>();
        ver.text = "v1.0  ·  Unity 6 URP  ·  VTC Academy  ·  2025";
        ver.fontSize = 13f; ver.color = HC(0.28f,0.38f,0.36f);

        // ══ RIGHT – button panel ════════════════════════════════════
        var R = MkRect("Right", _panelMain.transform, V2(0.52f,0.08f), V2(0.97f,0.92f));
        MkImg("RightBg", R.transform, V2(0,0), V2(1,1), C_PANEL)
            .GetComponent<Image>().sprite = MkRR(22);
        // Top color accent bar
        MkImg("TopBar", R.transform, V2(0.04f,0.968f), V2(0.96f,0.975f), C_GREEN);

        var hdr = MkRect("Hdr", R.transform, V2(0.06f,0.90f), V2(0.94f,0.97f)).AddComponent<TextMeshProUGUI>();
        hdr.text = "ĐIỀU HƯỚNG"; hdr.fontSize = 13f; hdr.characterSpacing = 6f;
        hdr.color = C_DIM; hdr.alignment = TextAlignmentOptions.MidlineLeft;

        // ─── Buttons ─────────────────────────────────────────────
        const float BH = 0.100f, GAP = 0.018f;
        float sy = 0.875f;

        AaaBtn(R.transform, "Btn_New",
            V2(0.05f, sy - 0*(BH+GAP)), V2(0.95f, sy - 0*(BH+GAP) + BH),
            "TRÒ CHƠI MỚI", "Bắt đầu hành trình – 30 ngày sinh tồn", "🌲", C_GREEN,
            () => { SFX_Click(); SwitchTo(_panelConfirmNew); });

        bool canContinue = _hasSave;
        AaaBtn(R.transform, "Btn_Continue",
            V2(0.05f, sy - 1*(BH+GAP)), V2(0.95f, sy - 1*(BH+GAP) + BH),
            canContinue ? "TIẾP TỤC" : "TIẾP TỤC",
            canContinue ? $"Ngày {_saveDay} / 30   ·   Karma {_saveKarma}" : "Chưa có tiến trình được lưu",
            "▶", canContinue ? C_AMBER : C_DIM,
            () => { if (canContinue) { SFX_Click(); GoToScene(gameplaySceneName); } },
            !canContinue);

        AaaBtn(R.transform, "Btn_Story",
            V2(0.05f, sy - 2*(BH+GAP)), V2(0.95f, sy - 2*(BH+GAP) + BH),
            "CỐT TRUYỆN", "5 Hồi  ·  5 Kết Thúc  ·  Void Rift", "📖", C_AMBER,
            () => { SFX_Open(); SwitchTo(_panelStory); });

        AaaBtn(R.transform, "Btn_Controls",
            V2(0.05f, sy - 3*(BH+GAP)), V2(0.95f, sy - 3*(BH+GAP) + BH),
            "ĐIỀU KHIỂN", "Xem hướng dẫn chơi & phím tắt đầy đủ", "🎮", C_TXT,
            () => { SFX_Open(); SwitchTo(_panelControls); });

        AaaBtn(R.transform, "Btn_Settings",
            V2(0.05f, sy - 4*(BH+GAP)), V2(0.95f, sy - 4*(BH+GAP) + BH),
            "CÀI ĐẶT", "Âm thanh  ·  Đồ hoạ  ·  Toàn màn hình", "⚙", C_TXT,
            () => { SFX_Open(); SwitchTo(_panelSettings); });

        // Credits & Quit side-by-side
        SmallBtn(R.transform, "Btn_Credits", V2(0.05f,0.030f), V2(0.46f,0.108f),
            "TÁC GIẢ", C_PURPLE, () => { SFX_Open(); SwitchTo(_panelCredits); });
        SmallBtn(R.transform, "Btn_Quit", V2(0.54f,0.030f), V2(0.95f,0.108f),
            "✖  THOÁT", C_RED, () => { SFX_Click(); StartCoroutine(CoQuit()); });
    }

    void BuildActBadgesRow(Transform p)
    {
        Color[] cols = { C_GREEN, C_AMBER, C_PURPLE, C_RED, C_BLUE };
        string[] lbls = { "HỒI I", "HỒI II", "HỒI III", "HỒI IV", "HỒI V" };
        string[] days = { "1-3", "4-10", "11-20", "21-29", "30" };
        float bw = 0.185f;
        for (int i = 0; i < 5; i++)
        {
            float x = 0.02f + i * (bw + 0.008f);
            var b = MkImg($"Act{i}", p, V2(x,0.28f), V2(x+bw,0.36f),
                HC(cols[i].r*0.18f, cols[i].g*0.18f, cols[i].b*0.18f, 0.92f));
            b.GetComponent<Image>().sprite = MkRR(5);
            MkImg($"ActTop{i}", b.transform, V2(0f,0.82f), V2(1f,1f), cols[i]);
            var t = MkRect($"AT{i}", b.transform, V2(0.04f,0f), V2(0.96f,0.82f)).AddComponent<TextMeshProUGUI>();
            t.text = $"{lbls[i]}\n<size=9>Ngày {days[i]}</size>";
            t.fontSize = 10.5f; t.color = C_TXT2; t.richText = true;
            t.alignment = TextAlignmentOptions.Center;
        }
    }

    // ─────────────────────────────────────────────────────────────────
    //  STORY PANEL  (paginated — 1 act per page)
    // ─────────────────────────────────────────────────────────────────
    void BuildPanelStory()
    {
        _panelStory = SidePanel("Panel_Story", V2(0.02f,0.03f), V2(0.98f,0.97f));
        // ── header ──
        PanelHdr(_panelStory.transform, "CỐT TRUYỆN  ·  THE FOREST BETWEEN US", C_AMBER);

        // ── placeholder image slot ──
        var imgSlot = MkImg("StoryImg", _panelStory.transform, V2(0.55f,0.18f), V2(0.96f,0.87f),
            HC(0.03f,0.06f,0.05f,0.92f));
        imgSlot.GetComponent<Image>().sprite = MkRR(14);
        imgSlot.name = "StoryImg";
        // "Chèn ảnh vào đây" placeholder text
        var ph = MkRect("ImgPH", imgSlot.transform, V2(0.05f,0.3f), V2(0.95f,0.7f)).AddComponent<TextMeshProUGUI>();
        ph.name = "ImgPH";
        ph.text = "[ Ảnh minh hoạ\nkéo Texture2D vào\nInspector → storyImg_Act1..5 ]";
        ph.fontSize = 14f; ph.color = HC(0.3f,0.45f,0.4f); ph.alignment = TextAlignmentOptions.Center;
        ph.richText = true;

        // ── text area ──
        // Act tag badge
        var actBadge = MkImg("ActBadge", _panelStory.transform, V2(0.03f,0.84f), V2(0.22f,0.89f),
            HC(C_GREEN.r*0.2f,C_GREEN.g*0.2f,C_GREEN.b*0.2f,0.9f));
        actBadge.GetComponent<Image>().sprite = MkRR(6);
        actBadge.name = "ActBadge";
        var actTagT = MkRect("ActTag", actBadge.transform, V2(0,0), V2(1,1)).AddComponent<TextMeshProUGUI>();
        actTagT.name = "ActTag";
        actTagT.fontSize = 14f; actTagT.fontStyle = FontStyles.Bold;
        actTagT.alignment = TextAlignmentOptions.Center; actTagT.color = C_TXT;

        // Day range
        var dayT = MkRect("DayRange", _panelStory.transform, V2(0.24f,0.84f), V2(0.50f,0.89f)).AddComponent<TextMeshProUGUI>();
        dayT.name = "DayRange"; dayT.fontSize = 14f; dayT.color = C_DIM;
        dayT.alignment = TextAlignmentOptions.MidlineLeft;

        // Act title
        var titleT = MkRect("ActTitle", _panelStory.transform, V2(0.03f,0.75f), V2(0.52f,0.845f)).AddComponent<TextMeshProUGUI>();
        titleT.name = "ActTitle"; titleT.fontSize = 32f; titleT.fontStyle = FontStyles.Bold;
        titleT.color = C_TXT; titleT.alignment = TextAlignmentOptions.Left;

        Sep("StorySep", _panelStory.transform, V2(0.03f,0.742f), V2(0.53f,0.747f));

        // Lore body
        var loreT = MkRect("LoreTxt", _panelStory.transform, V2(0.03f,0.40f), V2(0.52f,0.740f)).AddComponent<TextMeshProUGUI>();
        loreT.name = "LoreTxt"; loreT.fontSize = 17f; loreT.color = C_TXT2;
        loreT.richText = true; loreT.lineSpacing = 6f; loreT.alignment = TextAlignmentOptions.TopLeft;

        // Highlights header
        var hlHdr = MkRect("HlHdr", _panelStory.transform, V2(0.03f,0.355f), V2(0.52f,0.400f)).AddComponent<TextMeshProUGUI>();
        hlHdr.text = "ĐIỂM NỔI BẬT"; hlHdr.fontSize = 13f; hlHdr.fontStyle = FontStyles.Bold;
        hlHdr.color = C_DIM; hlHdr.characterSpacing = 5f;

        // Highlights bullets
        var hlTxt = MkRect("HlTxt", _panelStory.transform, V2(0.03f,0.18f), V2(0.52f,0.355f)).AddComponent<TextMeshProUGUI>();
        hlTxt.name = "HlTxt"; hlTxt.fontSize = 15.5f; hlTxt.richText = true;
        hlTxt.color = C_TXT2; hlTxt.lineSpacing = 4f;

        // Progress dots
        var dotsGO = MkRect("Dots", _panelStory.transform, V2(0.03f,0.12f), V2(0.52f,0.175f));
        dotsGO.name = "Dots";
        for (int i = 0; i < 5; i++)
        {
            float dx = 0.06f + i * 0.07f;
            var dot = MkImg($"Dot{i}", dotsGO.transform, V2(dx,0.2f), V2(dx+0.04f,0.8f),
                HC(0.2f,0.3f,0.28f,0.8f));
            dot.GetComponent<Image>().sprite = MkRR(4); dot.name = $"Dot{i}";
        }

        // ── navigation buttons ──
        SmallBtn(_panelStory.transform, "Btn_Prev", V2(0.03f,0.035f), V2(0.22f,0.110f),
            "◀  TRƯỚC", C_DIM, () => { SFX_Click(); StoryPage(_storyPage - 1); });
        SmallBtn(_panelStory.transform, "Btn_Next", V2(0.33f,0.035f), V2(0.52f,0.110f),
            "TIẾP  ▶", C_GREEN, () => { SFX_Click(); StoryPage(_storyPage + 1); });
        SmallBtn(_panelStory.transform, "Btn_Back", V2(0.58f,0.035f), V2(0.97f,0.110f),
            "← QUAY LẠI", HC(0.15f,0.25f,0.22f), () => { SFX_Close(); SwitchTo(null); });

        _panelStory.SetActive(false);
        StoryPage(0);
    }

    void StoryPage(int page)
    {
        _storyPage = Mathf.Clamp(page, 0, ACTS.Length - 1);
        if (_panelStory == null) return;
        var a = ACTS[_storyPage];
        Color accentCol = HexColor(a.accentHex);

        // Act badge
        var badge = _panelStory.transform.Find("ActBadge");
        if (badge)
        {
            badge.GetComponent<Image>().color = HC(accentCol.r*0.22f,accentCol.g*0.22f,accentCol.b*0.22f,0.92f);
            badge.Find("ActTag")?.GetComponent<TextMeshProUGUI>()?.SetText(a.actTag);
        }

        // Day range
        SetTMP(_panelStory.transform, "DayRange", $"<color=#{a.accentHex.TrimStart('#')}>{a.dayRange}</color>");
        // Title
        var titleT = _panelStory.transform.Find("ActTitle")?.GetComponent<TextMeshProUGUI>();
        if (titleT) { titleT.text = a.title; titleT.color = accentCol; }
        // Lore
        SetTMP(_panelStory.transform, "LoreTxt", a.lore);
        // Highlights
        string hl = "";
        foreach (var h in a.highlights) hl += $"<color=#{a.accentHex.TrimStart('#')}>▸</color>  {h}\n";
        SetTMP(_panelStory.transform, "HlTxt", hl.TrimEnd('\n'));

        // Progress dots
        var dots = _panelStory.transform.Find("Dots");
        if (dots)
            for (int i = 0; i < 5; i++)
            {
                var d = dots.Find($"Dot{i}")?.GetComponent<Image>();
                if (d) d.color = i == _storyPage ? accentCol : HC(0.2f,0.3f,0.28f,0.8f);
            }

        // Story image (placeholder or actual)
        Texture2D[] imgs = { storyImg_Act1, storyImg_Act2, storyImg_Act3, storyImg_Act4, storyImg_Act5 };
        var imgGO = _panelStory.transform.Find("StoryImg");
        if (imgGO)
        {
            var imgComp = imgGO.GetComponent<Image>();
            var phT = imgGO.Find("ImgPH")?.GetComponent<TextMeshProUGUI>();
            if (imgs[_storyPage] != null)
            {
                var spr = Sprite.Create(imgs[_storyPage],
                    new Rect(0,0,imgs[_storyPage].width,imgs[_storyPage].height),
                    new Vector2(0.5f,0.5f));
                imgComp.sprite = spr;
                imgComp.color = Color.white;
                if (phT) phT.enabled = false;
            }
            else
            {
                imgComp.sprite = MkRR(14);
                imgComp.color = HC(0.03f,0.06f,0.05f,0.92f);
                if (phT) { phT.enabled = true; phT.text = $"[ Ảnh HỒI {_storyPage+1}\n→ Inspector: storyImg_Act{_storyPage+1} ]"; }
            }
        }

        // Separator color
        var sep = _panelStory.transform.Find("StorySep")?.GetComponent<Image>();
        if (sep) sep.color = HC(accentCol.r,accentCol.g,accentCol.b,0.22f);
    }

    // ─────────────────────────────────────────────────────────────────
    //  CONTROLS PANEL
    // ─────────────────────────────────────────────────────────────────
    void BuildPanelControls()
    {
        _panelControls = SidePanel("Panel_Controls", V2(0.03f,0.03f), V2(0.97f,0.97f));
        PanelHdr(_panelControls.transform, "ĐIỀU KHIỂN & HƯỚNG DẪN CHƠI", C_GREEN);

        // Layout mới: Left (20%), Mid (20%), Right (Image Slot)
        var colL = MkRect("CL", _panelControls.transform, V2(0.03f,0.12f), V2(0.33f,0.88f));
        var colM = MkRect("CM", _panelControls.transform, V2(0.35f,0.12f), V2(0.65f,0.88f));

        string loreL =
            "<color=#2ED07A><b>── DI CHUYỂN ────────</b></color>\n" +
            "<color=#2ED07A>W A S D</color>   Di chuyển\n" +
            "<color=#2ED07A>Shift</color>     Chạy nhanh\n" +
            "<color=#2ED07A>Space</color>     Nhảy\n" +
            "<color=#2ED07A>C</color>         Cúi / Rón rén\n" +
            "<color=#2ED07A>X</color>         Né tránh\n\n" +
            "<color=#F5A623><b>── TƯƠNG TÁC ────────</b></color>\n" +
            "<color=#F5A623>E</color>         Nhặt / Tương tác\n" +
            "<color=#F5A623>F</color>         Thắp lửa / Bật\n" +
            "<color=#F5A623>Tab</color>       Nhân vật & Túi Đồ\n" +
            "<color=#F5A623>K / L</color>     Sổ Công Thức\n" +
            "<color=#F5A623>Chuột Trái</color> Tấn công\n" +
            "<color=#F5A623>Esc</color>       Menu Pause";

        string loreM =
            "<color=#AA55FF><b>── SINH TỒN ────────</b></color>\n" +
            "<color=#AA55FF>HP</color>        Bị đánh/đói/khát\n" +
            "<color=#AA55FF>Stamina</color>   Chạy & đánh\n" +
            "<color=#AA55FF>Đói/Khát</color>  Ăn/Uống phục hồi\n" +
            "<color=#AA55FF>Ngủ</color>       Hồi thể lực\n\n" +
            "<color=#CF3333><b>── CHIẾN ĐẤU ────────</b></color>\n" +
            "<color=#CF3333>Nỏ Tần Số</color> Vũ khí chính\n" +
            "<color=#CF3333>Bẫy Thú</color>   Bắt thú/Phòng ngự\n" +
            "<color=#CF3333>Đuốc</color>      Đuổi bóng đêm\n\n" +
            "<color=#4488FF><b>── KARMA ────────────</b></color>\n" +
            "<color=#4488FF>≤ 5 cây/ngày</color> Giữ Karma tốt\n" +
            "<color=#4488FF>Trồng cây</color> +Karma\n" +
            "<color=#4488FF>Cao/Thấp</color>  Quyết định kết thúc";

        var lt = colL.AddComponent<TextMeshProUGUI>();
        lt.text = loreL; lt.fontSize = 15f; lt.richText = true;
        lt.color = C_TXT2; lt.lineSpacing = 5f;

        var mt = colM.AddComponent<TextMeshProUGUI>();
        mt.text = loreM; mt.fontSize = 15f; mt.richText = true;
        mt.color = C_TXT2; mt.lineSpacing = 5f;

        // Vùng bên phải hiển thị ảnh minh họa (có thể chia làm 2 mảnh trên dưới nếu cần, hoặc trái/phải).
        // Ta tạo 2 slot ảnh (trên/dưới)
        var imgL = MkImg("ImgSlotLeft", _panelControls.transform, V2(0.68f, 0.50f), V2(0.97f, 0.88f), HC(0.03f, 0.06f, 0.05f, 0.92f));
        imgL.GetComponent<Image>().sprite = MkRR(10);
        var phL = MkRect("PH_L", imgL.transform, V2(0.05f, 0.2f), V2(0.95f, 0.8f)).AddComponent<TextMeshProUGUI>();
        phL.text = "[ Kéo ảnh hướng dẫn vào\nInspector → controlsImg_Left ]";
        phL.fontSize = 13f; phL.color = HC(0.3f, 0.45f, 0.4f); phL.alignment = TextAlignmentOptions.Center;
        if (controlsImg_Left != null) {
            imgL.GetComponent<Image>().sprite = Sprite.Create(controlsImg_Left, new Rect(0,0,controlsImg_Left.width,controlsImg_Left.height), V2(0.5f,0.5f));
            imgL.GetComponent<Image>().color = Color.white;
            phL.enabled = false;
        }

        var imgR = MkImg("ImgSlotRight", _panelControls.transform, V2(0.68f, 0.12f), V2(0.97f, 0.48f), HC(0.03f, 0.06f, 0.05f, 0.92f));
        imgR.GetComponent<Image>().sprite = MkRR(10);
        var phR = MkRect("PH_R", imgR.transform, V2(0.05f, 0.2f), V2(0.95f, 0.8f)).AddComponent<TextMeshProUGUI>();
        phR.text = "[ Kéo ảnh hướng dẫn vào\nInspector → controlsImg_Right ]";
        phR.fontSize = 13f; phR.color = HC(0.3f, 0.45f, 0.4f); phR.alignment = TextAlignmentOptions.Center;
        if (controlsImg_Right != null) {
            imgR.GetComponent<Image>().sprite = Sprite.Create(controlsImg_Right, new Rect(0,0,controlsImg_Right.width,controlsImg_Right.height), V2(0.5f,0.5f));
            imgR.GetComponent<Image>().color = Color.white;
            phR.enabled = false;
        }

        SmallBtn(_panelControls.transform, "Btn_Back", V2(0.35f,0.025f), V2(0.65f,0.100f),
            "← QUAY LẠI", C_GREEN, () => { SFX_Close(); SwitchTo(null); });
        _panelControls.SetActive(false);
    }

    // ─────────────────────────────────────────────────────────────────
    //  SETTINGS PANEL
    // ─────────────────────────────────────────────────────────────────
    void BuildPanelSettings()
    {
        _panelSettings = SidePanel("Panel_Settings", V2(0.12f,0.06f), V2(0.88f,0.94f));
        PanelHdr(_panelSettings.transform, "CÀI ĐẶT", C_TXT);
        float y = 0.75f; float rh = 0.09f; float g = 0.015f;
        Slider ms=null,mus=null,ss=null; TextMeshProUGUI ml=null,muL=null,sl=null;
        SliderRow(_panelSettings.transform,"🔊  Âm lượng tổng", ref y,rh,g,0,1,
            PlayerPrefs.GetFloat("MasterVol",1f),v=>{AudioListener.volume=v;PlayerPrefs.SetFloat("MasterVol",v);},ref ms,ref ml);
        SliderRow(_panelSettings.transform,"🎵  Nhạc nền",       ref y,rh,g,0,1,
            PlayerPrefs.GetFloat("MV",0.55f),v=>{_music.volume=v;PlayerPrefs.SetFloat("MV",v);},ref mus,ref muL);
        SliderRow(_panelSettings.transform,"🔔  Hiệu ứng âm",    ref y,rh,g,0,1,
            PlayerPrefs.GetFloat("SV",0.85f),v=>{_sfx.volume=v;PlayerPrefs.SetFloat("SV",v);},ref ss,ref sl);

        Sep("SetSep",_panelSettings.transform,V2(0.05f,y),V2(0.95f,y+0.006f)); y-=g*2f;

        // Fullscreen
        ToggleRow(_panelSettings.transform,"🖥️  Toàn màn hình",ref y,rh,g,Screen.fullScreen,
            v=>{Screen.fullScreen=v;PlayerPrefs.SetInt("Fullscreen",v?1:0);});

        // Quality
        string[] qN={"Very Low","Low","Medium","High","Very High","Ultra"};
        int cq=QualitySettings.GetQualityLevel();
        var qlbl=MkRect("QL",_panelSettings.transform,V2(0.05f,y),V2(0.65f,y+rh)).AddComponent<TextMeshProUGUI>();
        qlbl.text="🎮  Chất lượng đồ hoạ";qlbl.fontSize=18f;qlbl.color=C_TXT;qlbl.alignment=TextAlignmentOptions.MidlineLeft;
        var qb=MkImg("QB",_panelSettings.transform,V2(0.67f,y+0.01f),V2(0.93f,y+rh-0.01f),C_BTN);
        qb.GetComponent<Image>().sprite=MkRR(8);qb.AddComponent<Button>();
        AddHover(qb,C_BTN,HC(0.10f,0.22f,0.18f));
        var qt=MkRect("QT",qb.transform,V2(0,0),V2(1,1)).AddComponent<TextMeshProUGUI>();
        qt.text=qN[Mathf.Clamp(cq,0,5)];qt.fontSize=16f;qt.color=C_GREEN;qt.alignment=TextAlignmentOptions.Center;
        qb.GetComponent<Button>().onClick.AddListener(()=>{cq=(cq+1)%6;QualitySettings.SetQualityLevel(cq,true);qt.text=qN[cq];PlayerPrefs.SetInt("Quality",cq);});

        SmallBtn(_panelSettings.transform,"Btn_Clear",V2(0.05f,0.13f),V2(0.44f,0.21f),
            "🗑  XÓA TIẾN TRÌNH",C_RED,()=>{SaveSystem.ClearSaveData();_hasSave=false;});
        SmallBtn(_panelSettings.transform,"Btn_Back",V2(0.30f,0.03f),V2(0.70f,0.11f),
            "← QUAY LẠI",C_GREEN,()=>{SFX_Close();SwitchTo(null);});
        _panelSettings.SetActive(false);
    }

    void ToggleRow(Transform p, string label, ref float y, float rh, float g, bool init, System.Action<bool> onChange)
    {
        var lbl=MkRect(label+"L",p,V2(0.05f,y),V2(0.70f,y+rh)).AddComponent<TextMeshProUGUI>();
        lbl.text=label;lbl.fontSize=18f;lbl.color=C_TXT;lbl.alignment=TextAlignmentOptions.MidlineLeft;
        var bg=MkImg(label+"B",p,V2(0.74f,y+0.01f),V2(0.93f,y+rh-0.01f),C_BTN);
        bg.GetComponent<Image>().sprite=MkRR(8); bg.AddComponent<Button>();
        AddHover(bg,C_BTN,HC(0.08f,0.20f,0.15f));
        bool cur=init;
        var t=MkRect(label+"T",bg.transform,V2(0,0),V2(1,1)).AddComponent<TextMeshProUGUI>();
        t.text=cur?"BẬT":"TẮT";t.color=cur?C_GREEN:C_DIM;t.fontSize=17f;t.alignment=TextAlignmentOptions.Center;
        bg.GetComponent<Button>().onClick.AddListener(()=>{cur=!cur;onChange(cur);t.text=cur?"BẬT":"TẮT";t.color=cur?C_GREEN:C_DIM;});
        y-=rh+g;
    }

    // ─────────────────────────────────────────────────────────────────
    //  CREDITS / TÁC GIẢ PANEL
    // ─────────────────────────────────────────────────────────────────
    void BuildPanelCredits()
    {
        _panelCredits = SidePanel("Panel_Credits", V2(0.08f,0.05f), V2(0.92f,0.95f));
        PanelHdr(_panelCredits.transform, "TÁC GIẢ  &  LỜI CẢM ƠN", C_PURPLE);

        // ── Game info box ──
        var infoBox = MkImg("InfoBox", _panelCredits.transform, V2(0.04f,0.72f), V2(0.96f,0.88f),
            HC(C_PURPLE.r*0.15f, C_PURPLE.g*0.15f, C_PURPLE.b*0.15f, 0.85f));
        infoBox.GetComponent<Image>().sprite = MkRR(10);
        MkImg("InfoAccent", infoBox.transform, V2(0,0), V2(0.007f,1), C_PURPLE);
        var infoT = MkRect("InfoT", infoBox.transform, V2(0.03f,0.05f), V2(0.97f,0.95f)).AddComponent<TextMeshProUGUI>();
        infoT.richText = true;
        infoT.text =
            "<b><color=#AA55FF>THE FOREST BETWEEN US</color></b>    " +
            "Game sinh tồn 3D thế giới mở  ·  Unity 6 URP  ·  2025\n" +
            "<size=13><color=#60727E>Thể loại: Sinh tồn · RPG · Khám phá · Kinh dị nhẹ  " +
            "·  Nền tảng: PC / WebGL</color></size>";
        infoT.fontSize = 16.5f; infoT.color = C_TXT; infoT.lineSpacing = 5f;

        // ── Credits text ──
        string creditsText =
            "<color=#AA55FF><b>──  NHÓM PHÁT TRIỂN  ──────────────────────────────</b></color>\n\n" +
            "  Trưởng Nhóm  /  Game Designer\n" +
            "  <color=#F5A623><b>[ Tên trưởng nhóm ]</b></color>  ·  VTC Academy\n\n" +
            "  Lập Trình (Unity C#)\n" +
            "  <color=#2ED07A><b>[ Tên lập trình viên 1 ]</b></color>\n" +
            "  <color=#2ED07A><b>[ Tên lập trình viên 2 ]</b></color>\n\n" +
            "  Đồ Hoạ  /  3D Art  /  UI Design\n" +
            "  <color=#4488FF><b>[ Tên artist ]</b></color>\n\n" +
            "  Âm Nhạc  /  Sound Design\n" +
            "  <color=#4488FF><b>[ Tên composer ]</b></color>\n\n" +
            "  Game Tester  /  QA\n" +
            "  <color=#60727E>[ Danh sách tester ]</color>\n\n" +
            "<color=#AA55FF><b>──  GIẢNG VIÊN HƯỚNG DẪN  ────────────────────────</b></color>\n\n" +
            "  <color=#F5A623><b>[ Tên Giảng Viên ]</b></color>  ·  VTC Academy  ·  Bộ Môn Game Development\n\n" +
            "<color=#AA55FF><b>──  TÀI NGUYÊN SỬ DỤNG  ──────────────────────────</b></color>\n\n" +
            "  <color=#60727E>Unity 6 LTS  ·  TextMeshPro  ·  Universal Render Pipeline (URP)\n" +
            "  Devion Games Inventory & Stat System  ·  Toby Foliage Engine\n" +
            "  Terrain Evo 3  ·  Joystick Pack  ·  StarterAssets (ThirdPerson)\n\n" +
            "  Nhạc: [ Tên bài / nguồn ]  ·  Hiệu ứng âm thanh: [ Nguồn ]</color>\n\n" +
            "<color=#AA55FF><b>──  LỜI CẢM ƠN  ──────────────────────────────────</b></color>\n\n" +
            "  <color=#60727E>Cảm ơn gia đình, bạn bè và tất cả người chơi đã ủng hộ.\n" +
            "  Game được tạo ra với tình yêu dành cho văn hoá Việt Nam\n" +
            "  và truyền thuyết Mai An Tiêm cổ đại.\n\n" +
            "  \"Khu rừng không chỉ là khung cảnh — đó là nhân vật.\"</color>\n\n" +
            "<color=#AA55FF><b>──  BẢN QUYỀN  ────────────────────────────────────</b></color>\n\n" +
            "  <color=#60727E>© 2025 VTC Academy. Bảo lưu mọi quyền.\n" +
            "  Dự án học thuật — không vì mục đích thương mại.</color>";

        var creditsTxt = MkRect("CredTxt", _panelCredits.transform, V2(0.04f,0.12f), V2(0.96f,0.715f)).AddComponent<TextMeshProUGUI>();
        creditsTxt.text = creditsText; creditsTxt.fontSize = 15f; creditsTxt.richText = true;
        creditsTxt.color = C_TXT2; creditsTxt.lineSpacing = 3f;

        SmallBtn(_panelCredits.transform, "Btn_Back", V2(0.32f,0.025f), V2(0.68f,0.100f),
            "← QUAY LẠI", C_PURPLE, () => { SFX_Close(); SwitchTo(null); });
        _panelCredits.SetActive(false);
    }

    // ─────────────────────────────────────────────────────────────────
    //  CONFIRM NEW GAME PANEL
    // ─────────────────────────────────────────────────────────────────
    void BuildPanelConfirmNew()
    {
        _panelConfirmNew = SidePanel("Panel_ConfirmNew", V2(0.20f,0.26f), V2(0.80f,0.74f));
        PanelHdr(_panelConfirmNew.transform, "TRÒ CHƠI MỚI", C_GREEN);

        var body = MkRect("Body", _panelConfirmNew.transform, V2(0.06f,0.30f), V2(0.94f,0.85f)).AddComponent<TextMeshProUGUI>();
        body.richText = true;
        body.text = _hasSave
            ? $"Tiến trình <b>Ngày {_saveDay}</b> (Karma {_saveKarma}) sẽ bị\n" +
              "<color=#CF3333><b>XÓA VĨNH VIỄN</b></color>.\n\n" +
              "Bạn sẵn sàng bắt đầu lại từ\n<b>Ngày 1: Tín Hiệu Lạc Lối</b>?"
            : "Bắt đầu hành trình sinh tồn trong\n<color=#2ED07A>Khu Rừng Bí Ẩn</color>.\n\n" +
              "Bạn sẽ được hướng dẫn qua Tutorial\ntrước khi vào thế giới chính.";
        body.fontSize = 19f; body.color = C_TXT2; body.alignment = TextAlignmentOptions.Center;

        SmallBtn(_panelConfirmNew.transform, "Btn_Yes", V2(0.06f,0.07f), V2(0.46f,0.22f),
            "▶  BẮT ĐẦU", C_GREEN, () => { SFX_Click(); StartCoroutine(CoNewGame()); });
        SmallBtn(_panelConfirmNew.transform, "Btn_No",  V2(0.54f,0.07f), V2(0.94f,0.22f),
            "✖  HỦY",     C_RED,   () => { SFX_Click(); SwitchTo(null); });

        _panelConfirmNew.SetActive(false);
    }

    // ─────────────────────────────────────────────────────────────────
    //  LOADING OVERLAY  (cinematic)
    // ─────────────────────────────────────────────────────────────────
    void BuildLoadingOverlay()
    {
        _overlay = MkImg("LoadOverlay", _cvs.transform, V2(0,0), V2(1,1), HC(0,0,0,0.97f)).gameObject;

        // Top/Bottom cinematic bars
        MkImg("LBar_Top",    _overlay.transform, V2(0,0.90f), V2(1,1.00f), HC(0,0,0,1));
        MkImg("LBar_Bottom", _overlay.transform, V2(0,0.00f), V2(1,0.06f), HC(0,0,0,1));

        // Logo
        var logo = MkRect("Logo", _overlay.transform, V2(0.28f,0.55f), V2(0.72f,0.88f)).AddComponent<TextMeshProUGUI>();
        logo.text = "THE FOREST\nBETWEEN US";
        logo.fontSize = 52f; logo.fontStyle = FontStyles.Bold;
        logo.alignment = TextAlignmentOptions.Center;
        logo.enableVertexGradient = true;
        logo.colorGradient = new VertexGradient(C_AMBER, C_AMBER, C_GREEN, HC(0.1f,0.5f,0.3f));

        // Destination label
        var dest = MkRect("Dest", _overlay.transform, V2(0.30f,0.505f), V2(0.70f,0.555f)).AddComponent<TextMeshProUGUI>();
        dest.name = "LoadDest";
        dest.fontSize = 16f; dest.color = C_DIM; dest.alignment = TextAlignmentOptions.Center;

        // Tip
        var tip = MkRect("Tip", _overlay.transform, V2(0.10f,0.38f), V2(0.90f,0.50f)).AddComponent<TextMeshProUGUI>();
        tip.name = "LoadTip"; tip.fontSize = 19f; tip.richText = true;
        tip.color = C_GREEN; tip.alignment = TextAlignmentOptions.Center;

        // Bar track
        var barBg = MkImg("BarBg", _overlay.transform, V2(0.10f,0.27f), V2(0.90f,0.33f),
            HC(0.05f,0.09f,0.10f));
        // Glow fill
        var fill = MkImg("Fill", barBg.transform, V2(0,0), V2(0,1), C_GREEN);
        fill.name = "LoadFill";
        fill.GetComponent<Image>().sprite = null;

        // Pct
        var pct = MkRect("Pct", _overlay.transform, V2(0.44f,0.17f), V2(0.56f,0.26f)).AddComponent<TextMeshProUGUI>();
        pct.name = "LoadPct"; pct.text = "0%"; pct.fontSize = 22f;
        pct.alignment = TextAlignmentOptions.Center; pct.color = C_TXT2;

        _overlay.SetActive(false);
    }

    // ─────────────────────────────────────────────────────────────────
    //  PANEL SWITCH
    // ─────────────────────────────────────────────────────────────────
    void SwitchTo(GameObject panel)
    {
        void S(GameObject g, bool v) { if (g) g.SetActive(v); }
        S(_panelMain,        panel == null);
        S(_panelStory,       panel == _panelStory);
        S(_panelControls,    panel == _panelControls);
        S(_panelSettings,    panel == _panelSettings);
        S(_panelCredits,     panel == _panelCredits);
        S(_panelConfirmNew,  panel == _panelConfirmNew);
        if (Application.isPlaying && panel != null && panel != _panelConfirmNew) SFX_Open();
        if (panel == _panelStory) StoryPage(_storyPage);
    }

    // ─────────────────────────────────────────────────────────────────
    //  GAME ACTIONS
    // ─────────────────────────────────────────────────────────────────
    IEnumerator CoNewGame()
    {
        SaveSystem.ClearSaveData();
        // New Game always goes to Tutorial first
        yield return StartCoroutine(CoCinematicLoad(tutorialSceneName, "TUTORIAL  ·  Ngày 1: Tín Hiệu Lạc Lối"));
    }

    void GoToScene(string scene)
    {
        string label = scene == gameplaySceneName
            ? $"GAMEPLAY  ·  Ngày {_saveDay} / 30"
            : scene;
        StartCoroutine(CoCinematicLoad(scene, label));
    }

    // ─────────────────────────────────────────────────────────────────
    //  CINEMATIC SCENE LOAD  (fade + cinematic bars + tip rotation)
    // ─────────────────────────────────────────────────────────────────
    IEnumerator CoCinematicLoad(string sceneName, string destLabel)
    {
        // 1. Show overlay
        _overlay.SetActive(true);
        var fill = _overlay.transform.Find("BarBg/Fill")?.GetComponent<RectTransform>();
        var tip  = _overlay.transform.Find("Tip")?.GetComponent<TextMeshProUGUI>();
        var pct  = _overlay.transform.Find("Pct")?.GetComponent<TextMeshProUGUI>();
        var dest = _overlay.transform.Find("Dest")?.GetComponent<TextMeshProUGUI>();
        if (dest) dest.text = destLabel;

        string[] tips = {
            "Bảo vệ rừng: <b>chặt ≤ 5 cây/ngày</b> để giữ Karma tốt.",
            "Ban đêm: <b>Thổ dân K'Nu bị Hắc Hóa</b> — rất nguy hiểm!",
            "Cho <b>Thỏ Rừng</b> ăn 3 lần → thuần hóa thành pet đồng hành.",
            "<b>Karma cao</b> = kết thúc Hòa Bình. <b>Karma thấp</b> = Tân Vương Bóng Đêm.",
            "Giải đố <b>mạch điện cổ đại</b> tại Phế Tích để mở bí mật Mai An Tiêm.",
            "<b>Void Rift</b> đang lớn dần — hành động trước ngày 30 hoặc mọi thứ kết thúc.",
            "Đặt <b>bẫy Tần Số</b> quanh căn cứ để tự động bảo vệ ban đêm.",
            "<b>Nỏ Tần Số</b> hiệu quả nhất lúc bóng đêm — tích lũy đạn mỗi ngày.",
        };

        if (tip != null) tip.text = tips[0];
        if (fill != null) fill.anchorMax = V2(0f, 1f);
        if (pct != null) pct.text = "0%";

        // 2. Fade overlay IN (alpha 0 → 1)
        var cg = _overlay.GetComponent<CanvasGroup>();
        if (cg == null) cg = _overlay.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        float t = 0f;
        while (t < 1.2f)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Clamp01(t / 1.2f);
            // Fade music out simultaneously
            _music.volume = Mathf.Lerp(PlayerPrefs.GetFloat("MV",0.55f), 0f, t/1.2f);
            yield return null;
        }

        // 3. Async load
        float elapsed = 0f; int tipIdx = 0;
        var op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            elapsed += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(op.progress / 0.9f);
            if (fill != null) fill.anchorMax = V2(p, 1f);
            if (pct  != null) pct.text = Mathf.RoundToInt(p * 100f) + "%";

            // Tip rotation every 2.5s
            int ni = Mathf.FloorToInt(elapsed / 2.5f) % tips.Length;
            if (ni != tipIdx && tip != null) { tipIdx = ni; tip.text = tips[tipIdx]; }

            if (op.progress >= 0.9f && elapsed >= 1.8f)
                op.allowSceneActivation = true;

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
    //  PROCEDURAL FOREST AMBIENT  (drone + wind + pentatonic + cricket)
    // ─────────────────────────────────────────────────────────────────
    IEnumerator CoStartMusic()
    {
        yield return new WaitForEndOfFrame();
        _music.clip   = menuMusicClip != null ? menuMusicClip : GenerateForestAmbient();
        _music.volume = 0f;
        _music.Play();
        float tgt = PlayerPrefs.GetFloat("MV", 0.55f);
        float t = 0f;
        while (t < 3.5f)
        {
            t += Time.deltaTime;
            _music.volume = Mathf.Lerp(0f, tgt, t / 3.5f);
            yield return null;
        }
        _music.volume = tgt;
    }

    AudioClip GenerateForestAmbient()
    {
        int sr = 44100; int dur = 40;
        float[] buf = new float[sr * dur];
        var rng = new System.Random(9001);
        float[] penta = { 261.63f, 293.66f, 329.63f, 392f, 440f, 523.25f };
        for (int i = 0; i < buf.Length; i++)
        {
            float t = (float)i / sr;
            // Deep drone – 3 harmonics with breath
            float drone = Mathf.Sin(Mathf.PI*2*44*t)*0.05f
                         +Mathf.Sin(Mathf.PI*2*66*t)*0.03f
                         +Mathf.Sin(Mathf.PI*2*88*t)*0.02f;
            float breath = (Mathf.Sin(Mathf.PI*2*0.18f*t)+1)*0.5f;
            drone *= 0.4f + breath*0.6f;
            // Wind
            float wind=(float)(rng.NextDouble()*2-1)*0.022f;
            wind*=(Mathf.Sin(Mathf.PI*2*0.07f*t+0.9f)+1)*0.5f;
            // Pentatonic ping every 5s
            float ping=0; float pt=t%5f;
            if(pt<0.9f){int ni=((int)(t/5f))%penta.Length;
                float env=Mathf.Exp(-pt*5.5f);
                ping=Mathf.Sin(Mathf.PI*2*penta[ni]*t)*env*0.05f
                    +Mathf.Sin(Mathf.PI*2*penta[ni]*2*t)*env*0.018f;}
            // Cricket texture
            float cricket=(float)(rng.NextDouble()*2-1)*0.009f;
            cricket*=(Mathf.Sin(Mathf.PI*2*2.2f*t)+1)*0.5f;
            buf[i]=Mathf.Clamp(drone+wind+ping+cricket,-1f,1f);
        }
        var clip=AudioClip.Create("ForestAmbient",buf.Length,1,sr,false);
        clip.SetData(buf,0); return clip;
    }

    // ─────────────────────────────────────────────────────────────────
    //  SFX
    // ─────────────────────────────────────────────────────────────────
    void SFX_Click() => PlaySFX(sfxClick,  680f, 0.065f);
    void SFX_Open()  => PlaySFX(sfxOpen,   820f, 0.055f);
    void SFX_Close() => PlaySFX(sfxClose,  440f, 0.055f);
    void PlaySFX(AudioClip c, float freq, float dur)
    {
        if (_sfx == null) return;
        _sfx.PlayOneShot(c ?? MkTone(freq, dur, 0.22f));
    }
    static AudioClip MkTone(float f, float d, float v)
    {
        int sr=44100;int n=(int)(sr*d);float[] buf=new float[n];
        for(int i=0;i<n;i++){float t=(float)i/sr;buf[i]=Mathf.Sin(Mathf.PI*2*f*t)*Mathf.Clamp01(1f-t/d)*v;}
        var c=AudioClip.Create("T",n,1,sr,false);c.SetData(buf,0);return c;
    }

    // ─────────────────────────────────────────────────────────────────
    //  ANIMATIONS
    // ─────────────────────────────────────────────────────────────────
    IEnumerator CoAnimateFireflies()
    {
        var rng=new System.Random(55);
        while(true)
        {
            foreach(var f in _flies)
            {
                if(f.rt==null)continue;
                if(f.img)
                {
                    var c=f.img.color;
                    c.a=Mathf.Lerp(0.06f,0.55f,(Mathf.Sin(Time.time*1.3f+f.phase)+1)*0.5f);
                    f.img.color=c;
                }
                var mn=f.rt.anchorMin+V2(f.drift,f.speed);
                var mx=f.rt.anchorMax+V2(f.drift,f.speed);
                if(mn.y>1.08f){mn.y=-0.08f;mx.y=mn.y+0.007f;}
                if(mn.x<-0.08f||mn.x>1.08f){float nx=(float)rng.NextDouble();mn.x=nx;mx.x=nx+0.004f;}
                f.rt.anchorMin=mn;f.rt.anchorMax=mx;
            }
            yield return null;
        }
    }

    IEnumerator CoAnimateFog()
    {
        while(true)
        {
            foreach(var f in _fogStrips)
            {
                if(f.rt==null)continue;
                var mn=f.rt.anchorMin;var mx=f.rt.anchorMax;
                mn.x+=f.speed;mx.x+=f.speed;
                if(mn.x>1.12f)mn.x=mx.x=-0.15f;
                if(mx.x<-0.12f){mn.x=0.88f;mx.x=mn.x+(mx.x-mn.x);}
                f.rt.anchorMin=mn;f.rt.anchorMax=mx;
            }
            yield return null;
        }
    }

    IEnumerator CoTitleBreath()
    {
        yield return new WaitForSeconds(0.8f);
        var go=_cvs?.transform.Find("Panel_Main/Left/TitleAnim");
        if(go==null)yield break;
        var t=go.GetComponent<TextMeshProUGUI>();
        if(t==null)yield break;
        while(true){
            float s=0;while(s<5f){s+=Time.deltaTime;t.fontSize=74f+Mathf.Sin(s*Mathf.PI*0.35f)*2f;yield return null;}
            s=0;while(s<5f){s+=Time.deltaTime;t.fontSize=76f-Mathf.Sin(s*Mathf.PI*0.35f)*2f;yield return null;}
        }
    }

    IEnumerator CoScanlineFloat()
    {
        var go=_cvs?.transform.Find("Scanline");
        if(go==null)yield break;
        var rt=go.GetComponent<RectTransform>();
        while(true){
            float y=0.38f+Mathf.Sin(Time.time*0.12f)*0.14f;
            rt.anchorMin=V2(0,y);rt.anchorMax=V2(1,y+0.004f);
            yield return null;
        }
    }

    // ─────────────────────────────────────────────────────────────────
    //  UI BUILDERS
    // ─────────────────────────────────────────────────────────────────
    // Full AAA button with left accent bar + subtitle
    void AaaBtn(Transform p, string name, Vector2 amin, Vector2 amax,
        string label, string sub, string icon, Color accent, System.Action onClick, bool disabled=false)
    {
        Color bg = disabled ? HC(0.03f,0.05f,0.07f,0.6f) : C_BTN;
        var go = MkImg(name, p, amin, amax, bg);
        go.GetComponent<Image>().sprite = MkRR(10);
        // Left accent bar
        MkImg("Bar",go.transform,V2(0,0),V2(0.007f,1), HC(accent.r,accent.g,accent.b,disabled?0.25f:1f));
        // Icon
        var iconT=MkRect("Icon",go.transform,V2(0.012f,0.15f),V2(0.075f,0.85f)).AddComponent<TextMeshProUGUI>();
        iconT.text=icon; iconT.fontSize=22f; iconT.color=disabled?C_DIM:accent; iconT.alignment=TextAlignmentOptions.Center;
        // Label
        var lblT=MkRect("Lbl",go.transform,V2(0.08f,0.46f),V2(0.85f,0.92f)).AddComponent<TextMeshProUGUI>();
        lblT.text=label; lblT.fontSize=22f; lblT.fontStyle=FontStyles.Bold;
        lblT.color=disabled?C_DIM:C_TXT; lblT.alignment=TextAlignmentOptions.MidlineLeft;
        // Sub
        var subT=MkRect("Sub",go.transform,V2(0.08f,0.06f),V2(0.85f,0.48f)).AddComponent<TextMeshProUGUI>();
        subT.text=sub; subT.fontSize=13f;
        subT.color=disabled?HC(0.25f,0.3f,0.28f):C_DIM; subT.alignment=TextAlignmentOptions.MidlineLeft;
        // Arrow
        var arr=MkRect("Arr",go.transform,V2(0.87f,0.2f),V2(0.97f,0.8f)).AddComponent<TextMeshProUGUI>();
        arr.text="›"; arr.fontSize=30f; arr.color=disabled?HC(0.2f,0.25f,0.23f):accent;
        arr.alignment=TextAlignmentOptions.Center;
        // Hover
        AddHover(go, bg, HC(0.07f,0.14f,0.11f,0.95f));
        var btn=go.AddComponent<Button>();
        if(!disabled&&onClick!=null)btn.onClick.AddListener(()=>onClick());
        else btn.interactable=!disabled;
    }

    void SmallBtn(Transform p, string name, Vector2 amin, Vector2 amax,
        string label, Color col, System.Action onClick)
    {
        var bg = HC(col.r*0.22f,col.g*0.22f,col.b*0.22f,0.90f);
        var hv = HC(col.r*0.40f,col.g*0.40f,col.b*0.40f,0.95f);
        var go = MkImg(name, p, amin, amax, bg);
        go.GetComponent<Image>().sprite = MkRR(8);
        AddHover(go, bg, hv);
        var t = MkRect("T",go.transform,V2(0,0),V2(1,1)).AddComponent<TextMeshProUGUI>();
        t.text=label; t.fontSize=17f; t.fontStyle=FontStyles.Bold;
        t.alignment=TextAlignmentOptions.Center; t.color=C_TXT;
        var b=go.AddComponent<Button>(); if(onClick!=null)b.onClick.AddListener(()=>onClick());
    }

    void AddHover(GameObject go, Color normal, Color hover)
    {
        var img=go.GetComponent<Image>();
        var et=go.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        void Add(UnityEngine.EventSystems.EventTriggerType type,
                 System.Action<UnityEngine.EventSystems.BaseEventData> cb)
        {
            var e=new UnityEngine.EventSystems.EventTrigger.Entry{eventID=type};
            e.callback.AddListener(d=>cb(d)); et.triggers.Add(e);
        }
        Add(UnityEngine.EventSystems.EventTriggerType.PointerEnter,
            _=>{if(img)img.color=hover;PlaySFX(sfxHover,920f,0.035f);});
        Add(UnityEngine.EventSystems.EventTriggerType.PointerExit,
            _=>{if(img)img.color=normal;});
    }

    // ─────────────────────────────────────────────────────────────────
    //  Panel helpers
    // ─────────────────────────────────────────────────────────────────
    GameObject SidePanel(string name, Vector2 amin, Vector2 amax)
    {
        var p = MkImg(name, _cvs.transform, amin, amax, C_PANEL);
        p.GetComponent<Image>().sprite = MkRR(20);
        var border = MkImg(name+"_B", p.transform, V2(-0.003f,-0.003f), V2(1.003f,1.003f),
            HC(C_GREEN.r,C_GREEN.g,C_GREEN.b,0.10f));
        border.GetComponent<Image>().sprite = MkRR(20);
        border.transform.SetAsFirstSibling();
        return p.gameObject;
    }

    void PanelHdr(Transform p, string title, Color col)
    {
        var h = MkRect("Hdr",p,V2(0,0.910f),V2(1,1.00f));
        h.AddComponent<Image>().color=HC(col.r,col.g,col.b,0.10f);
        var t = MkRect("HdrT",h.transform,V2(0.04f,0),V2(0.96f,1)).AddComponent<TextMeshProUGUI>();
        t.text=title; t.fontSize=22f; t.fontStyle=FontStyles.Bold;
        t.alignment=TextAlignmentOptions.MidlineLeft; t.color=col;
        Sep("HdrSep",p,V2(0.02f,0.902f),V2(0.98f,0.907f));
    }

    void SliderRow(Transform p, string label, ref float y, float rh, float g,
        float min, float max, float init, System.Action<float> onChange,
        ref Slider slider, ref TextMeshProUGUI valLbl)
    {
        var lt=MkRect(label+"_L",p,V2(0.04f,y),V2(0.52f,y+rh)).AddComponent<TextMeshProUGUI>();
        lt.text=label; lt.fontSize=17f; lt.color=C_TXT; lt.alignment=TextAlignmentOptions.MidlineLeft;
        valLbl=MkRect(label+"_V",p,V2(0.87f,y),V2(0.97f,y+rh)).AddComponent<TextMeshProUGUI>();
        valLbl.fontSize=14f; valLbl.color=C_GREEN; valLbl.alignment=TextAlignmentOptions.Center;
        valLbl.text=Mathf.RoundToInt(init*100)+"%";
        var sGO=MkRect(label+"_S",p,V2(0.52f,y+rh*0.22f),V2(0.85f,y+rh*0.78f));
        slider=sGO.AddComponent<Slider>();
        MkImg("SBg",sGO.transform,V2(0,0),V2(1,1),HC(0.05f,0.09f,0.10f));
        var fa=MkRect("FA",sGO.transform,V2(0,0.2f),V2(1,0.8f));
        var fill=MkImg("Fill",fa.transform,V2(0,0),V2(1,1),C_GREEN);
        var hsa=MkRect("HA",sGO.transform,V2(0,0),V2(1,1));
        var hand=MkImg("Hand",hsa.transform,V2(0,0),V2(0,1),C_AMBER);
        hand.GetComponent<RectTransform>().sizeDelta=new Vector2(13,0);
        slider.fillRect=fill.GetComponent<RectTransform>();
        slider.handleRect=hand.GetComponent<RectTransform>();
        slider.targetGraphic=hand.GetComponent<Image>();
        slider.direction=Slider.Direction.LeftToRight;
        slider.minValue=min; slider.maxValue=max; slider.value=init;
        var vl=valLbl;
        slider.onValueChanged.AddListener(v=>{onChange(v);vl.text=Mathf.RoundToInt(v*100)+"%";});
        y-=rh+g;
    }

    // ─────────────────────────────────────────────────────────────────
    //  LOW-LEVEL HELPERS
    // ─────────────────────────────────────────────────────────────────
    static GameObject MkRect(string n, Transform p, Vector2 amin, Vector2 amax)
    {
        var go=new GameObject(n,typeof(RectTransform));
        go.transform.SetParent(p,false);
        var rt=go.GetComponent<RectTransform>();
        rt.anchorMin=amin; rt.anchorMax=amax; rt.offsetMin=rt.offsetMax=Vector2.zero;
        return go;
    }
    static GameObject MkImg(string n, Transform p, Vector2 amin, Vector2 amax, Color c)
    {
        var go=MkRect(n,p,amin,amax); go.AddComponent<Image>().color=c; return go;
    }
    static void Sep(string n, Transform p, Vector2 amin, Vector2 amax)
        => MkImg(n,p,amin,amax,C_SEP);
    static Vector2 V2(float x, float y) => new(x,y);
    static void SetTMP(Transform root, string path, string text)
    {
        var t=root.Find(path)?.GetComponent<TextMeshProUGUI>();
        if(t) t.text=text;
    }
    static Color HexColor(string hex)
    {
        if(ColorUtility.TryParseHtmlString(hex,out var c)) return c;
        return Color.white;
    }

    // ─────────────────────────────────────────────────────────────────
    //  SPRITE GENERATORS
    // ─────────────────────────────────────────────────────────────────
    static Sprite MkGradient(Color bot, Color top, int w=4, int h=512)
    {
        var tex=new Texture2D(w,h,TextureFormat.RGBA32,false); tex.wrapMode=TextureWrapMode.Clamp;
        for(int y=0;y<h;y++){Color c=Color.Lerp(bot,top,(float)y/h);for(int x=0;x<w;x++)tex.SetPixel(x,y,c);}
        tex.Apply(); return Sprite.Create(tex,new Rect(0,0,w,h),V2(0.5f,0.5f));
    }
    static Sprite MkRadialGlow(Color center, int s=512)
    {
        var tex=new Texture2D(s,s,TextureFormat.RGBA32,false); tex.wrapMode=TextureWrapMode.Clamp;
        Vector2 c=V2(s/2f,s/2f); float r=s/2f;
        for(int y=0;y<s;y++) for(int x=0;x<s;x++){
            float a=Mathf.SmoothStep(1,0,Vector2.Distance(V2(x,y),c)/r);
            tex.SetPixel(x,y,new Color(center.r,center.g,center.b,center.a*a));}
        tex.Apply(); return Sprite.Create(tex,new Rect(0,0,s,s),V2(0.5f,0.5f));
    }
    static Sprite MkVignette(float strength=0.88f, int s=512)
    {
        var tex=new Texture2D(s,s,TextureFormat.RGBA32,false); tex.wrapMode=TextureWrapMode.Clamp;
        Vector2 c=V2(s/2f,s/2f); float r=Vector2.Distance(Vector2.zero,c);
        for(int y=0;y<s;y++) for(int x=0;x<s;x++){
            float a=Mathf.SmoothStep(0,strength,Vector2.Distance(V2(x,y),c)/r);
            tex.SetPixel(x,y,new Color(0,0,0,a));}
        tex.Apply(); return Sprite.Create(tex,new Rect(0,0,s,s),V2(0.5f,0.5f));
    }
    static Sprite MkFogStrip(int w=256, int h=32)
    {
        var tex=new Texture2D(w,h,TextureFormat.RGBA32,false); tex.wrapMode=TextureWrapMode.Clamp;
        for(int y=0;y<h;y++) for(int x=0;x<w;x++){
            float ex=1f-Mathf.Abs(x/(float)w*2f-1f);
            float ey=1f-Mathf.Abs(y/(float)h*2f-1f);
            tex.SetPixel(x,y,new Color(1,1,1,ex*ey));}
        tex.Apply(); return Sprite.Create(tex,new Rect(0,0,w,h),V2(0.5f,0.5f));
    }
    static Sprite MkRR(int r=10, int w=128, int h=64)
    {
        var tex=new Texture2D(w,h,TextureFormat.RGBA32,false); tex.wrapMode=TextureWrapMode.Clamp;
        for(int y=0;y<h;y++) for(int x=0;x<w;x++){
            int cx=Mathf.Clamp(x,r,w-r-1),cy=Mathf.Clamp(y,r,h-r-1);
            float d=Mathf.Sqrt((x-cx)*(x-cx)+(y-cy)*(y-cy));
            tex.SetPixel(x,y,d<=r?Color.white:Color.clear);}
        tex.Apply();
        return Sprite.Create(tex,new Rect(0,0,w,h),V2(0.5f,0.5f),100f,0,
            SpriteMeshType.FullRect,new Vector4(r,r,r,r));
    }
    static Sprite MkCircle(int s=24)
    {
        var tex=new Texture2D(s,s,TextureFormat.RGBA32,false); tex.wrapMode=TextureWrapMode.Clamp;
        Vector2 c=V2(s/2f,s/2f); float r=s/2f-1f;
        for(int y=0;y<s;y++) for(int x=0;x<s;x++){
            float a=Mathf.Clamp01(r-Mathf.Max(0,Vector2.Distance(V2(x,y),c)-r+1));
            tex.SetPixel(x,y,new Color(1,1,1,a));}
        tex.Apply(); return Sprite.Create(tex,new Rect(0,0,s,s),V2(0.5f,0.5f));
    }
}
