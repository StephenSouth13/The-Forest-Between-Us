using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseSettingsUIController : MonoBehaviour
{
    public static PauseSettingsUIController instance;

    [Header("Panel & Key Controls")]
    public GameObject pausePanel;
    public KeyCode toggleKey = KeyCode.Escape;
    public bool isPaused { get; private set; }

    [Header("Custom UI Asset Slots (Kéo Asset Ảnh Của Bạn Vào Đây)")]
    public Sprite menuBackgroundSprite;
    public Sprite buttonBackgroundSprite;
    public Sprite sliderFillSprite;
    public Sprite sliderHandleSprite;

    [Header("Audio Settings")]
    public Slider masterVolumeSlider;
    public TextMeshProUGUI masterVolumeText;

    [Header("Controls Settings")]
    public Toggle invertYToggle;
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityText;

    [Header("Buttons")]
    public Button resumeButton;
    public Button quitToMenuButton;

    private AudioSource audioSource;
    private TobyFredson.FPSController fpsController;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

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
        fpsController = Object.FindFirstObjectByType<TobyFredson.FPSController>();

        if (pausePanel == null)
        {
            pausePanel = CreateFallbackPausePanel();
        }

        if (pausePanel != null) pausePanel.SetActive(false);

        LoadSavedSettings();
        SetupButtonListeners();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (pausePanel != null) pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PlaySFX(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PlaySFX(false);
        }
    }

    public void ResumeGame()
    {
        if (isPaused) TogglePause();
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene("Home");
        }
        else
        {
            SceneManager.LoadScene("Home");
        }
    }

    public void OnMasterVolumeChanged(float val)
    {
        AudioListener.volume = val;
        PlayerPrefs.SetFloat("Settings_MasterVolume", val);
        PlayerPrefs.Save();
        if (masterVolumeText != null) masterVolumeText.text = $"{Mathf.RoundToInt(val * 100f)}%";
    }

    public void OnInvertYChanged(bool invert)
    {
        if (fpsController != null) fpsController.SetInvertY(invert);
        PlayerPrefs.SetInt("Settings_InvertMouseY", invert ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnSensitivityChanged(float val)
    {
        if (fpsController != null) fpsController.lookSpeed = val;
        PlayerPrefs.SetFloat("Settings_LookSpeed", val);
        PlayerPrefs.Save();
        if (sensitivityText != null) sensitivityText.text = val.ToString("F1");
    }

    void LoadSavedSettings()
    {
        float vol = PlayerPrefs.GetFloat("Settings_MasterVolume", 1f);
        AudioListener.volume = vol;
        if (masterVolumeSlider != null) masterVolumeSlider.value = vol;
        if (masterVolumeText != null) masterVolumeText.text = $"{Mathf.RoundToInt(vol * 100f)}%";

        bool invY = PlayerPrefs.GetInt("Settings_InvertMouseY", 0) == 1;
        if (invertYToggle != null) invertYToggle.isOn = invY;

        float sens = PlayerPrefs.GetFloat("Settings_LookSpeed", 2f);
        if (sensitivitySlider != null) sensitivitySlider.value = sens;
        if (sensitivityText != null) sensitivityText.text = sens.ToString("F1");
    }

    void SetupButtonListeners()
    {
        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (quitToMenuButton != null) quitToMenuButton.onClick.AddListener(QuitToMainMenu);

        if (masterVolumeSlider != null) masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        if (invertYToggle != null) invertYToggle.onValueChanged.AddListener(OnInvertYChanged);
        if (sensitivitySlider != null) sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    void PlaySFX(bool open)
    {
        if (audioSource == null) return;
        AudioClip clip = CreateToneClip(open ? 600f : 400f, 0.1f, 0.2f);
        audioSource.PlayOneShot(clip);
    }

    AudioClip CreateToneClip(float freq, float duration, float volume)
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
        AudioClip clip = AudioClip.Create("PauseSFX", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    GameObject CreateFallbackPausePanel()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return null;

        GameObject panel = new GameObject("PauseSettings_Panel (Runtime)", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(canvas.transform, false);

        RectTransform pRect = panel.GetComponent<RectTransform>();
        pRect.anchorMin = new Vector2(0.25f, 0.15f);
        pRect.anchorMax = new Vector2(0.75f, 0.85f);
        pRect.offsetMin = Vector2.zero;
        pRect.offsetMax = Vector2.zero;

        Image bg = panel.GetComponent<Image>();
        if (menuBackgroundSprite != null) bg.sprite = menuBackgroundSprite;
        bg.color = new Color(0.05f, 0.08f, 0.12f, 0.95f);

        // Header Title
        GameObject titleGO = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        titleGO.transform.SetParent(panel.transform, false);
        RectTransform tRect = titleGO.GetComponent<RectTransform>();
        tRect.anchorMin = new Vector2(0.05f, 0.85f);
        tRect.anchorMax = new Vector2(0.95f, 0.96f);
        tRect.offsetMin = Vector2.zero;
        tRect.offsetMax = Vector2.zero;
        TextMeshProUGUI tText = titleGO.GetComponent<TextMeshProUGUI>();
        tText.fontSize = 24f;
        tText.fontStyle = FontStyles.Bold;
        tText.alignment = TextAlignmentOptions.Center;
        tText.color = new Color(0.2f, 1f, 0.6f);
        tText.text = "⚙️ CÀI ĐẶT & TẠM DỪNG (PAUSE)";

        // Resume Button
        resumeButton = CreateButton(panel.transform, "Btn_Resume", "TIẾP TỤC TRÒ CHƠI", new Vector2(0.2f, 0.25f), new Vector2(0.8f, 0.38f));
        quitToMenuButton = CreateButton(panel.transform, "Btn_Quit", "THOÁT RA MENU MÀN HÌNH CHÍNH", new Vector2(0.2f, 0.08f), new Vector2(0.8f, 0.21f));

        return panel;
    }

    Button CreateButton(Transform parent, string name, string label, Vector2 minAnchor, Vector2 maxAnchor)
    {
        GameObject btnGO = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        btnGO.transform.SetParent(parent, false);

        RectTransform rect = btnGO.GetComponent<RectTransform>();
        rect.anchorMin = minAnchor;
        rect.anchorMax = maxAnchor;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image img = btnGO.GetComponent<Image>();
        if (buttonBackgroundSprite != null) img.sprite = buttonBackgroundSprite;
        img.color = new Color(0.1f, 0.18f, 0.25f, 1f);

        GameObject txtGO = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        txtGO.transform.SetParent(btnGO.transform, false);
        RectTransform txtRect = txtGO.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.offsetMin = Vector2.zero;
        txtRect.offsetMax = Vector2.zero;

        TextMeshProUGUI txt = txtGO.GetComponent<TextMeshProUGUI>();
        txt.fontSize = 18f;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.white;
        txt.text = label;

        return btnGO.GetComponent<Button>();
    }
}
