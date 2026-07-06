using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject statusPanel;
    public KeyCode toggleKey = KeyCode.Tab;
    public bool startHidden = true;
    public bool createFallbackPanel = true;

    [Header("Mock Values")]
    [Range(0f, 100f)] public float health = 100f;
    [Range(0f, 100f)] public float stamina = 100f;
    [Range(0f, 100f)] public float hunger = 85f;
    [Range(0f, 100f)] public float thirst = 80f;

    [Header("Text")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI staminaText;
    public TextMeshProUGUI hungerText;
    public TextMeshProUGUI thirstText;

    [Header("Sliders")]
    public Slider healthSlider;
    public Slider staminaSlider;
    public Slider hungerSlider;
    public Slider thirstSlider;

    void Start()
    {
        if (statusPanel == null && createFallbackPanel)
        {
            statusPanel = CreateFallbackPanel();
        }

        if (statusPanel != null) statusPanel.SetActive(!startHidden);
        Refresh();
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
        if (statusPanel == null) return;

        statusPanel.SetActive(!statusPanel.activeSelf);
        Refresh();
    }

    public void Refresh()
    {
        SetText(healthText, "Health", health);
        SetText(staminaText, "Stamina", stamina);
        SetText(hungerText, "Hunger", hunger);
        SetText(thirstText, "Thirst", thirst);

        SetSlider(healthSlider, health);
        SetSlider(staminaSlider, stamina);
        SetSlider(hungerSlider, hunger);
        SetSlider(thirstSlider, thirst);
    }

    void SetText(TextMeshProUGUI label, string name, float value)
    {
        if (label != null) label.text = $"{name}: {Mathf.RoundToInt(value)}";
    }

    void SetSlider(Slider slider, float value)
    {
        if (slider == null) return;

        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.value = value;
    }

    GameObject CreateFallbackPanel()
    {
        GameObject panel = new GameObject("Player Status Panel (Runtime)", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(transform, false);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.05f, 0.55f);
        panelRect.anchorMax = new Vector2(0.35f, 0.9f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = panel.GetComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.65f);

        healthText = CreateFallbackLabel(panel.transform, "Health", 0);
        staminaText = CreateFallbackLabel(panel.transform, "Stamina", 1);
        hungerText = CreateFallbackLabel(panel.transform, "Hunger", 2);
        thirstText = CreateFallbackLabel(panel.transform, "Thirst", 3);

        return panel;
    }

    TextMeshProUGUI CreateFallbackLabel(Transform parent, string label, int index)
    {
        GameObject textObject = new GameObject(label, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.08f, 0.76f - index * 0.2f);
        rectTransform.anchorMax = new Vector2(0.92f, 0.94f - index * 0.2f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.fontSize = 22f;
        text.color = Color.white;
        text.text = label;

        return text;
    }
}
