using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class BackpackUIController : MonoBehaviour
{
    [Header("Panel")]
    public GameObject backpackPanel;
    public KeyCode toggleKey = KeyCode.B;
    public bool startHidden = true;
    public bool createFallbackPanel = true;
    public bool controlCursor = true;
    public bool playAudioOnToggle = true;

    private AudioSource audioSource;
    private Vector3 originalPanelScale = Vector3.one;

    void Awake()
    {
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

        if (backpackPanel == null && createFallbackPanel)
        {
            backpackPanel = CreateFallbackPanel();
        }

        if (backpackPanel != null)
        {
            originalPanelScale = backpackPanel.transform.localScale;
            backpackPanel.SetActive(!startHidden);
        }
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
        if (backpackPanel == null) return;

        bool isOpening = !backpackPanel.activeSelf;
        backpackPanel.SetActive(isOpening);

        if (isOpening)
        {
            InventoryManager.instance?.RefreshSlots();
            AnimateOpen();

            if (controlCursor)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (playAudioOnToggle) PlayBackpackSFX(true);
        }
        else
        {
            if (controlCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (playAudioOnToggle) PlayBackpackSFX(false);
        }
    }

    void AnimateOpen()
    {
        if (backpackPanel == null) return;
        backpackPanel.transform.localScale = originalPanelScale * 0.92f;
        StartCoroutine(PopScaleRoutine());
    }

    System.Collections.IEnumerator PopScaleRoutine()
    {
        float duration = 0.15f;
        float t = 0f;
        Vector3 start = originalPanelScale * 0.92f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(t / duration);
            float eased = Mathf.Sin(progress * Mathf.PI * 0.5f);
            backpackPanel.transform.localScale = Vector3.Lerp(start, originalPanelScale, eased);
            yield return null;
        }

        backpackPanel.transform.localScale = originalPanelScale;
    }

    void PlayBackpackSFX(bool open)
    {
        if (audioSource == null) return;
        float freqStart = open ? 400f : 600f;
        float freqEnd = open ? 750f : 350f;
        AudioClip clip = CreateZipClip(freqStart, freqEnd, 0.12f, 0.2f);
        audioSource.PlayOneShot(clip);
    }

    AudioClip CreateZipClip(float startFreq, float endFreq, float duration, float volume)
    {
        int sampleRate = 44100;
        int samples = (int)(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float norm = t / duration;
            float freq = Mathf.Lerp(startFreq, endFreq, norm);
            float envelope = Mathf.Sin(norm * Mathf.PI);
            data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * envelope * volume;
        }

        AudioClip clip = AudioClip.Create("BackpackZipSFX", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    GameObject CreateFallbackPanel()
    {
        GameObject panel = new GameObject("Backpack Panel (Runtime)", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(transform, false);

        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.65f, 0.1f);
        rectTransform.anchorMax = new Vector2(0.95f, 0.85f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Image image = panel.GetComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.65f);

        return panel;
    }
}

