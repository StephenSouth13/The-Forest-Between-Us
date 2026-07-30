using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RadioDialogueUIController : MonoBehaviour
{
    public static RadioDialogueUIController instance;

    [Header("UI Panel References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueMessageText;
    public TextMeshProUGUI promptHintText;
    public Button nextButton;
    public CanvasGroup canvasGroup;

    [Header("Typewriter Speed")]
    public float charactersPerSecond = 35f;

    [Header("Audio Feedback")]
    public bool playAudioBeep = true;

    private List<DialogueLine> currentLines = new List<DialogueLine>();
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private string fullCurrentText = "";
    private AudioSource audioSource;

    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        public string messageText;
    }

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
        if (dialoguePanel == null)
        {
            dialoguePanel = CreateFallbackDialoguePanel();
        }

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (nextButton != null) nextButton.onClick.AddListener(OnNextClicked);
    }

    void Update()
    {
        if (dialoguePanel != null && dialoguePanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            {
                OnNextClicked();
            }
        }
    }

    public void StartRadioDialogueSequence()
    {
        List<DialogueLine> lines = new List<DialogueLine>()
        {
            new DialogueLine
            {
                speakerName = "📻 MAI AN TIÊM (Tín Hiệu Đài Vô Tuyến 01)",
                messageText = "rè... kè... 'Nếu bạn nhận được tần số này... hãy cẩn thận với Sương Mù và Trăng Máu ở Vùng Đứt Gãy. Hãy thu thập Dưa Hấu Hạt Đen để duy trì sức khỏe...'"
            },
            new DialogueLine
            {
                speakerName = "🌐 THÔNG BÁO HỆ THỐNG",
                messageText = "Tín hiệu Radio đã được kết nối thành công! Chuẩn bị tải dữ liệu Intro Kịch Bản Ngày 1: Sự Im Lặng Của Rừng..."
            }
        };

        PlayDialogue(lines);
    }

    public void PlayDialogue(List<DialogueLine> lines)
    {
        currentLines = lines;
        currentLineIndex = 0;

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (canvasGroup != null) canvasGroup.alpha = 1f;

        ShowLine(currentLineIndex);
    }

    void ShowLine(int index)
    {
        if (index >= currentLines.Count)
        {
            FinishDialogueSequence();
            return;
        }

        DialogueLine line = currentLines[index];
        if (speakerNameText != null) speakerNameText.text = line.speakerName;
        fullCurrentText = line.messageText;

        StopAllCoroutines();
        StartCoroutine(TypewriterRoutine(fullCurrentText));
    }

    IEnumerator TypewriterRoutine(string targetText)
    {
        isTyping = true;
        if (dialogueMessageText != null) dialogueMessageText.text = "";
        if (promptHintText != null) promptHintText.text = "Nhấn [Space] / [Click] để tiếp tục...";

        float delay = 1f / Mathf.Max(1f, charactersPerSecond);

        for (int i = 0; i <= targetText.Length; i++)
        {
            if (dialogueMessageText != null) dialogueMessageText.text = targetText.Substring(0, i);

            if (playAudioBeep && i % 3 == 0 && audioSource != null)
            {
                AudioClip clip = CreateRadioBeep(650f + Random.Range(-50f, 50f), 0.04f, 0.15f);
                audioSource.PlayOneShot(clip);
            }

            yield return new WaitForSecondsRealtime(delay);
        }

        isTyping = false;
    }

    void OnNextClicked()
    {
        if (isTyping)
        {
            // Skip typing and show full text immediately
            StopAllCoroutines();
            if (dialogueMessageText != null) dialogueMessageText.text = fullCurrentText;
            isTyping = false;
        }
        else
        {
            currentLineIndex++;
            ShowLine(currentLineIndex);
        }
    }

    void FinishDialogueSequence()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        Debug.Log("Radio Dialogue Finished -> Transitioning to Day 1 Intro!");

        // Update Quest and Save Progress
        QuestManager.instance?.AdvanceStep(StepType.ReachTarget, 1);
        SaveSystem.SaveProgress(1, 50, true);

        // Trigger Scene Transition / Fade to Day 1
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene("DemoScene");
        }
    }

    AudioClip CreateRadioBeep(float freq, float duration, float volume)
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
        AudioClip clip = AudioClip.Create("RadioBeep", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    GameObject CreateFallbackDialoguePanel()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return null;

        // Bottom Center Dialogue Panel
        GameObject panel = new GameObject("RadioDialogue_Panel (Runtime)", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
        panel.transform.SetParent(canvas.transform, false);

        RectTransform pRect = panel.GetComponent<RectTransform>();
        pRect.anchorMin = new Vector2(0.18f, 0.05f); // BOTTOM CENTER
        pRect.anchorMax = new Vector2(0.82f, 0.28f);
        pRect.offsetMin = Vector2.zero;
        pRect.offsetMax = Vector2.zero;

        Image bg = panel.GetComponent<Image>();
        bg.color = new Color(0.04f, 0.07f, 0.11f, 0.94f); // Sleek Cyber Charcoal

        canvasGroup = panel.GetComponent<CanvasGroup>();

        // Speaker Name Header
        GameObject nameGO = new GameObject("SpeakerName", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        nameGO.transform.SetParent(panel.transform, false);
        RectTransform nRect = nameGO.GetComponent<RectTransform>();
        nRect.anchorMin = new Vector2(0.04f, 0.72f);
        nRect.anchorMax = new Vector2(0.96f, 0.95f);
        nRect.offsetMin = Vector2.zero;
        nRect.offsetMax = Vector2.zero;

        speakerNameText = nameGO.GetComponent<TextMeshProUGUI>();
        speakerNameText.fontSize = 18f;
        speakerNameText.fontStyle = FontStyles.Bold;
        speakerNameText.color = new Color(0.2f, 1f, 0.6f); // Neon Cyan
        speakerNameText.text = "📻 MAI AN TIÊM (Đài Vô Tuyến)";

        // Dialogue Message Text
        GameObject msgGO = new GameObject("DialogueMessage", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        msgGO.transform.SetParent(panel.transform, false);
        RectTransform mRect = msgGO.GetComponent<RectTransform>();
        mRect.anchorMin = new Vector2(0.04f, 0.25f);
        mRect.anchorMax = new Vector2(0.96f, 0.70f);
        mRect.offsetMin = Vector2.zero;
        mRect.offsetMax = Vector2.zero;

        dialogueMessageText = msgGO.GetComponent<TextMeshProUGUI>();
        dialogueMessageText.fontSize = 16f;
        dialogueMessageText.color = Color.white;
        dialogueMessageText.text = "...";

        // Click / Space Hint Text
        GameObject hintGO = new GameObject("PromptHint", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        hintGO.transform.SetParent(panel.transform, false);
        RectTransform hRect = hintGO.GetComponent<RectTransform>();
        hRect.anchorMin = new Vector2(0.04f, 0.04f);
        hRect.anchorMax = new Vector2(0.96f, 0.22f);
        hRect.offsetMin = Vector2.zero;
        hRect.offsetMax = Vector2.zero;

        promptHintText = hintGO.GetComponent<TextMeshProUGUI>();
        promptHintText.fontSize = 13f;
        promptHintText.alignment = TextAlignmentOptions.Right;
        promptHintText.color = new Color(1f, 0.85f, 0.2f);
        promptHintText.text = "Nhấn [Space] / [Click] để tiếp tục...";

        // Overlay Button for Full Area Click
        GameObject btnGO = new GameObject("FullAreaButton", typeof(RectTransform), typeof(Button));
        btnGO.transform.SetParent(panel.transform, false);
        RectTransform bRect = btnGO.GetComponent<RectTransform>();
        bRect.anchorMin = Vector2.zero;
        bRect.anchorMax = Vector2.one;
        bRect.offsetMin = Vector2.zero;
        bRect.offsetMax = Vector2.zero;
        nextButton = btnGO.GetComponent<Button>();

        return panel;
    }
}
