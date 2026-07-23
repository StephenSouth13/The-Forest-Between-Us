using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

// Menu: Tools/Forest Between Us/Setup Inventory UI
// Dựng (hoặc bổ sung nếu đã có) Canvas > Backpack Panel > Slot_Grid với các InventorySlot,
// tạo InventoryManager + BackpackUIController trong scene đang mở và gắn tham chiếu.
// Chạy lại nhiều lần an toàn: chỉ tạo thêm phần còn thiếu, không tạo trùng.
public static class InventorySetupTool
{
    const int SlotCount = 24;

    [MenuItem("Tools/Forest Between Us/Setup Inventory UI")]
    public static void SetupInventoryUI()
    {
        Canvas canvas = GetOrCreateCanvas();
        EnsureEventSystem();

        GameObject panel = GetOrCreatePanel(canvas.transform);
        GameObject grid = GetOrCreateGrid(panel.transform);
        EnsureSlots(grid.transform);

        InventoryManager manager = GetOrCreateComponent<InventoryManager>("InventoryManager");
        manager.slotContainer = grid.transform;
        manager.RefreshSlots();

        BackpackUIController controller = GetOrCreateComponent<BackpackUIController>("BackpackUIController");
        controller.backpackPanel = panel;
        controller.createFallbackPanel = false;

        EditorUtility.SetDirty(manager);
        EditorUtility.SetDirty(controller);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log($"[InventorySetupTool] Xong: {grid.transform.childCount} slot dưới '{GetPath(grid.transform)}'. " +
                  $"InventoryManager.slotContainer và BackpackUIController.backpackPanel đã được gắn.");
    }

    static Canvas GetOrCreateCanvas()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null) return canvas;

        GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        return canvas;
    }

    static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null) return;

        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    }

    static GameObject GetOrCreatePanel(Transform canvasTransform)
    {
        Transform existing = canvasTransform.Find("Backpack Panel");
        if (existing != null) return existing.gameObject;

        GameObject panel = new GameObject("Backpack Panel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvasTransform, false);

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.62f, 0.08f);
        rect.anchorMax = new Vector2(0.98f, 0.92f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        panel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);
        panel.SetActive(false); // BackpackUIController.startHidden = true theo mặc định

        return panel;
    }

    static GameObject GetOrCreateGrid(Transform panelTransform)
    {
        Transform existing = panelTransform.Find("Slot_Grid");
        if (existing != null) return existing.gameObject;

        GameObject grid = new GameObject("Slot_Grid", typeof(RectTransform), typeof(GridLayoutGroup));
        grid.transform.SetParent(panelTransform, false);

        RectTransform rect = grid.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(20, 20);
        rect.offsetMax = new Vector2(-20, -20);

        GridLayoutGroup layout = grid.GetComponent<GridLayoutGroup>();
        layout.cellSize = new Vector2(90, 90);
        layout.spacing = new Vector2(10, 10);

        return grid;
    }

    static void EnsureSlots(Transform gridTransform)
    {
        int existingCount = gridTransform.GetComponentsInChildren<InventorySlot>(true).Length;

        for (int i = existingCount; i < SlotCount; i++)
        {
            CreateSlot(gridTransform, i);
        }
    }

    static void CreateSlot(Transform parent, int index)
    {
        GameObject slotGO = new GameObject($"Slot_{index}", typeof(RectTransform), typeof(Image), typeof(InventorySlot));
        slotGO.transform.SetParent(parent, false);
        slotGO.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.15f);

        GameObject iconGO = new GameObject("Icon", typeof(RectTransform), typeof(Image));
        iconGO.transform.SetParent(slotGO.transform, false);
        SetStretch(iconGO.GetComponent<RectTransform>(), 6);
        Image icon = iconGO.GetComponent<Image>();
        icon.enabled = false;
        icon.raycastTarget = false;
        icon.preserveAspect = true;

        GameObject textGO = new GameObject("Count", typeof(RectTransform), typeof(TextMeshProUGUI));
        textGO.transform.SetParent(slotGO.transform, false);
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0f);
        textRect.anchorMax = new Vector2(1f, 0.4f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI text = textGO.GetComponent<TextMeshProUGUI>();
        text.fontSize = 20;
        text.alignment = TextAlignmentOptions.BottomRight;
        text.raycastTarget = false;
        text.text = "";

        InventorySlot slot = slotGO.GetComponent<InventorySlot>();
        slot.iconDisplay = icon;
        slot.countText = text;
    }

    static void SetStretch(RectTransform rect, float padding)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(padding, padding);
        rect.offsetMax = new Vector2(-padding, -padding);
    }

    static T GetOrCreateComponent<T>(string gameObjectName) where T : Component
    {
        T existing = Object.FindFirstObjectByType<T>();
        if (existing != null) return existing;

        GameObject go = new GameObject(gameObjectName, typeof(T));
        return go.GetComponent<T>();
    }

    static string GetPath(Transform t)
    {
        return t.parent == null ? t.name : GetPath(t.parent) + "/" + t.name;
    }
}
