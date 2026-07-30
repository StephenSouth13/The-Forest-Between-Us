using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FullMapUIController : MonoBehaviour
{
    public static FullMapUIController instance;

    [Header("Controls")]
    public GameObject mapPanel;
    public KeyCode toggleKey = KeyCode.M;
    public bool startHidden = true;
    public bool controlCursor = true;
    public bool playAudioOnToggle = true;

    [Header("Map Elements")]
    public RectTransform playerMapIcon;
    public RectTransform radioMapIcon;
    public TextMeshProUGUI zoneTitleText;
    public TextMeshProUGUI coordinatesText;
    public Transform playerTransform;
    public Transform radioTransform;

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
        if (playerTransform == null && Camera.main != null) playerTransform = Camera.main.transform;
        if (radioTransform == null)
        {
            GameObject r = GameObject.Find("SM_Radio");
            if (r != null) radioTransform = r.transform;
        }

        if (mapPanel == null)
        {
            mapPanel = CreateFallbackMapPanel();
        }

        if (mapPanel != null)
        {
            originalScale = mapPanel.transform.localScale;
            mapPanel.SetActive(!startHidden);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            Toggle();
        }

        if (mapPanel != null && mapPanel.activeSelf)
        {
            UpdateMapCoordinates();
        }
    }

    public void Toggle()
    {
        if (mapPanel == null) return;

        bool isOpening = !mapPanel.activeSelf;
        mapPanel.SetActive(isOpening);

        if (isOpening)
        {
            AnimateOpen();
            UpdateMapCoordinates();

            if (controlCursor)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (playAudioOnToggle) PlayMapSFX(true);
        }
        else
        {
            if (controlCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (playAudioOnToggle) PlayMapSFX(false);
        }
    }

    void AnimateOpen()
    {
        if (mapPanel == null) return;
        mapPanel.transform.localScale = originalScale * 0.90f;
        StartCoroutine(PopScaleRoutine());
    }

    System.Collections.IEnumerator PopScaleRoutine()
    {
        float duration = 0.18f;
        float t = 0f;
        Vector3 start = originalScale * 0.90f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(t / duration);
            float eased = Mathf.Sin(progress * Mathf.PI * 0.5f);
            mapPanel.transform.localScale = Vector3.Lerp(start, originalScale, eased);
            yield return null;
        }

        mapPanel.transform.localScale = originalScale;
    }

    void UpdateMapCoordinates()
    {
        if (playerTransform == null && Camera.main != null) playerTransform = Camera.main.transform;
        if (playerTransform == null) return;

        Vector3 pos = playerTransform.position;
        if (coordinatesText != null)
        {
            coordinatesText.text = $"GPS COORDS: X: {Mathf.RoundToInt(pos.x)} | Y: {Mathf.RoundToInt(pos.y)} | Z: {Mathf.RoundToInt(pos.z)}\n" +
                                   $"ZONE: RỪNG SƯƠNG MÙ VÙNG ĐỨT GÃY (SECTOR 01)";
        }

        if (playerMapIcon != null)
        {
            float rot = playerTransform.eulerAngles.y;
            playerMapIcon.localRotation = Quaternion.Euler(0f, 0f, -rot);
        }
    }

    void PlayMapSFX(bool open)
    {
        if (audioSource == null) return;
        AudioClip clip = CreateMapRadarClip(open ? 950f : 500f, 0.15f, 0.25f);
        audioSource.PlayOneShot(clip);
    }

    AudioClip CreateMapRadarClip(float freq, float duration, float volume)
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
        AudioClip clip = AudioClip.Create("MapSFX", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    GameObject CreateFallbackMapPanel()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return null;

        GameObject panel = new GameObject("FullMap_Panel (Runtime)", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.1f, 0.08f);
        panelRect.anchorMax = new Vector2(0.9f, 0.92f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image bg = panel.GetComponent<Image>();
        bg.color = new Color(0.04f, 0.07f, 0.11f, 0.94f); // Sleek Tactical Dark Cyber blue

        // Header Title
        GameObject headerGO = new GameObject("HeaderTitle", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        headerGO.transform.SetParent(panel.transform, false);
        RectTransform headerRect = headerGO.GetComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0.05f, 0.90f);
        headerRect.anchorMax = new Vector2(0.95f, 0.98f);
        headerRect.offsetMin = Vector2.zero;
        headerRect.offsetMax = Vector2.zero;
        zoneTitleText = headerGO.GetComponent<TextMeshProUGUI>();
        zoneTitleText.fontSize = 24f;
        zoneTitleText.fontStyle = FontStyles.Bold;
        zoneTitleText.color = new Color(0.2f, 1f, 0.6f);
        zoneTitleText.text = "🗺️ BẢN ĐỒ CHIẾN THUẬT VÙNG ĐỨT GÃY (SECTOR 01)";

        // Footer Coords
        GameObject footerGO = new GameObject("FooterCoords", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        footerGO.transform.SetParent(panel.transform, false);
        RectTransform footerRect = footerGO.GetComponent<RectTransform>();
        footerRect.anchorMin = new Vector2(0.05f, 0.02f);
        footerRect.anchorMax = new Vector2(0.95f, 0.10f);
        footerRect.offsetMin = Vector2.zero;
        footerRect.offsetMax = Vector2.zero;
        coordinatesText = footerGO.GetComponent<TextMeshProUGUI>();
        coordinatesText.fontSize = 16f;
        coordinatesText.color = Color.white;

        // Player Icon on Map
        GameObject pIconGO = new GameObject("PlayerMapIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        pIconGO.transform.SetParent(panel.transform, false);
        playerMapIcon = pIconGO.GetComponent<RectTransform>();
        playerMapIcon.anchorMin = new Vector2(0.48f, 0.48f);
        playerMapIcon.anchorMax = new Vector2(0.52f, 0.52f);
        playerMapIcon.offsetMin = Vector2.zero;
        playerMapIcon.offsetMax = Vector2.zero;
        TextMeshProUGUI pTxt = pIconGO.GetComponent<TextMeshProUGUI>();
        pTxt.fontSize = 32f;
        pTxt.alignment = TextAlignmentOptions.Center;
        pTxt.color = new Color(1f, 0.85f, 0.2f);
        pTxt.text = "▲";

        // Radio Objective Marker Icon on Map
        GameObject rIconGO = new GameObject("RadioMapIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        rIconGO.transform.SetParent(panel.transform, false);
        radioMapIcon = rIconGO.GetComponent<RectTransform>();
        radioMapIcon.anchorMin = new Vector2(0.65f, 0.70f);
        radioMapIcon.anchorMax = new Vector2(0.75f, 0.80f);
        radioMapIcon.offsetMin = Vector2.zero;
        radioMapIcon.offsetMax = Vector2.zero;
        TextMeshProUGUI rTxt = rIconGO.GetComponent<TextMeshProUGUI>();
        rTxt.fontSize = 24f;
        rTxt.alignment = TextAlignmentOptions.Center;
        rTxt.color = new Color(1f, 0.3f, 0.3f);
        rTxt.text = "📻\n<size=12>RADIO SIGNAL</size>";

        return panel;
    }
}
