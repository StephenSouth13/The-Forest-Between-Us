using UnityEngine;
using UnityEngine.UI;

public class BackpackUIController : MonoBehaviour
{
    [Header("Panel")]
    public GameObject backpackPanel;
    public KeyCode toggleKey = KeyCode.B;
    public bool startHidden = true;
    public bool createFallbackPanel = true;

    void Start()
    {
        if (backpackPanel == null && createFallbackPanel)
        {
            backpackPanel = CreateFallbackPanel();
        }

        if (backpackPanel != null) backpackPanel.SetActive(!startHidden);
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

        backpackPanel.SetActive(!backpackPanel.activeSelf);
        InventoryManager.instance?.RefreshSlots();
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
