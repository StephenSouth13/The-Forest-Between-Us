using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueChoiceUIController : MonoBehaviour
{
    public static DialogueChoiceUIController instance;

    public GameObject panel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI messageText;
    public Button option1Btn;
    public TextMeshProUGUI option1Text;
    public Button option2Btn;
    public TextMeshProUGUI option2Text;

    private Action onOpt1Callback;
    private Action onOpt2Callback;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (panel == null) panel = CreateFallbackPanel();
        if (panel != null) panel.SetActive(false);

        if (option1Btn != null) option1Btn.onClick.AddListener(OnOption1);
        if (option2Btn != null) option2Btn.onClick.AddListener(OnOption2);
    }

    public void ShowChoices(string speaker, string message, string opt1Str, Action opt1Action, string opt2Str, Action opt2Action)
    {
        if (panel == null) return;

        speakerText.text = speaker;
        messageText.text = message;
        
        option1Text.text = "1. " + opt1Str;
        onOpt1Callback = opt1Action;

        if (string.IsNullOrEmpty(opt2Str))
        {
            option2Btn.gameObject.SetActive(false);
            onOpt2Callback = null;
        }
        else
        {
            option2Btn.gameObject.SetActive(true);
            option2Text.text = "2. " + opt2Str;
            onOpt2Callback = opt2Action;
        }

        panel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnOption1()
    {
        panel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        onOpt1Callback?.Invoke();
    }

    void OnOption2()
    {
        panel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        onOpt2Callback?.Invoke();
    }

    GameObject CreateFallbackPanel()
    {
        Canvas canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return null;

        GameObject p = new GameObject("DialogueChoice_Panel(Runtime)", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        p.transform.SetParent(canvas.transform, false);
        RectTransform rect = p.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.2f, 0.1f);
        rect.anchorMax = new Vector2(0.8f, 0.4f);
        rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
        p.GetComponent<Image>().color = new Color(0.05f, 0.08f, 0.12f, 0.95f);

        // Header
        GameObject sGO = new GameObject("SpeakerTxt", typeof(RectTransform), typeof(TextMeshProUGUI));
        sGO.transform.SetParent(p.transform, false);
        RectTransform sr = sGO.GetComponent<RectTransform>();
        sr.anchorMin = new Vector2(0.05f, 0.8f); sr.anchorMax = new Vector2(0.95f, 0.95f);
        sr.offsetMin = Vector2.zero; sr.offsetMax = Vector2.zero;
        speakerText = sGO.GetComponent<TextMeshProUGUI>();
        speakerText.fontSize = 22f; speakerText.fontStyle = FontStyles.Bold;
        speakerText.color = new Color(1f, 0.7f, 0.2f);

        // Message
        GameObject mGO = new GameObject("MsgTxt", typeof(RectTransform), typeof(TextMeshProUGUI));
        mGO.transform.SetParent(p.transform, false);
        RectTransform mr = mGO.GetComponent<RectTransform>();
        mr.anchorMin = new Vector2(0.05f, 0.5f); mr.anchorMax = new Vector2(0.95f, 0.8f);
        mr.offsetMin = Vector2.zero; mr.offsetMax = Vector2.zero;
        messageText = mGO.GetComponent<TextMeshProUGUI>();
        messageText.fontSize = 18f; messageText.color = Color.white;
        messageText.textWrappingMode = TextWrappingModes.Normal;

        // Button 1
        GameObject b1 = new GameObject("Btn1", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        b1.transform.SetParent(p.transform, false);
        RectTransform b1r = b1.GetComponent<RectTransform>();
        b1r.anchorMin = new Vector2(0.05f, 0.1f); b1r.anchorMax = new Vector2(0.48f, 0.35f);
        b1r.offsetMin = Vector2.zero; b1r.offsetMax = Vector2.zero;
        b1.GetComponent<Image>().color = new Color(0.2f, 0.4f, 0.3f, 1f);
        option1Btn = b1.GetComponent<Button>();
        
        GameObject t1 = new GameObject("T1", typeof(RectTransform), typeof(TextMeshProUGUI));
        t1.transform.SetParent(b1.transform, false);
        RectTransform t1r = t1.GetComponent<RectTransform>();
        t1r.anchorMin = Vector2.zero; t1r.anchorMax = Vector2.one;
        t1r.offsetMin = Vector2.zero; t1r.offsetMax = Vector2.zero;
        option1Text = t1.GetComponent<TextMeshProUGUI>();
        option1Text.alignment = TextAlignmentOptions.Center; option1Text.fontSize = 16f; option1Text.color = Color.white;

        // Button 2
        GameObject b2 = new GameObject("Btn2", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        b2.transform.SetParent(p.transform, false);
        RectTransform b2r = b2.GetComponent<RectTransform>();
        b2r.anchorMin = new Vector2(0.52f, 0.1f); b2r.anchorMax = new Vector2(0.95f, 0.35f);
        b2r.offsetMin = Vector2.zero; b2r.offsetMax = Vector2.zero;
        b2.GetComponent<Image>().color = new Color(0.4f, 0.25f, 0.2f, 1f);
        option2Btn = b2.GetComponent<Button>();
        
        GameObject t2 = new GameObject("T2", typeof(RectTransform), typeof(TextMeshProUGUI));
        t2.transform.SetParent(b2.transform, false);
        RectTransform t2r = t2.GetComponent<RectTransform>();
        t2r.anchorMin = Vector2.zero; t2r.anchorMax = Vector2.one;
        t2r.offsetMin = Vector2.zero; t2r.offsetMax = Vector2.zero;
        option2Text = t2.GetComponent<TextMeshProUGUI>();
        option2Text.alignment = TextAlignmentOptions.Center; option2Text.fontSize = 16f; option2Text.color = Color.white;

        return p;
    }
}
