using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// Menu: Tools/Forest Between Us/Setup Scene Transition
// Dựng Canvas loading screen (ảnh nền, vignette, spinner, thanh tiến trình, tip, cinematic bars),
// gắn SceneTransitionManager, rồi lưu thành prefab trong 1 thư mục Resources để hệ thống
// tự nạp lúc chạy (SceneTransitionManager.Instance). Chạy lại an toàn, sẽ ghi đè prefab cũ.
public static class SceneTransitionSetupTool
{
    const string ResourcesFolder = "Assets/_GAME/Resources";
    const string PrefabPath = ResourcesFolder + "/SceneTransitionManager.prefab";
    const string SettingsFolder = "Assets/_GAME/Data/Transitions";
    const string SettingsPath = SettingsFolder + "/Default Transition Settings.asset";

    [MenuItem("Tools/Forest Between Us/Setup Scene Transition")]
    public static void Setup()
    {
        SceneTransitionSettings settings = GetOrCreateSettings();

        GameObject root = BuildHierarchy();
        SceneTransitionManager manager = root.GetComponent<SceneTransitionManager>();
        manager.settings = settings;

        EnsureFolder(ResourcesFolder);
        PrefabUtility.SaveAsPrefabAsset(root, PrefabPath, out bool success);

        Object.DestroyImmediate(root);

        if (success)
        {
            Debug.Log($"[SceneTransitionSetupTool] Đã lưu prefab tại {PrefabPath}. " +
                      "Gọi SceneTransitionManager.Instance.LoadScene(\"TenScene\") ở bất kỳ đâu để dùng.");
        }
        else
        {
            Debug.LogError("[SceneTransitionSetupTool] Lưu prefab thất bại.");
        }
    }

    static SceneTransitionSettings GetOrCreateSettings()
    {
        SceneTransitionSettings existing = AssetDatabase.LoadAssetAtPath<SceneTransitionSettings>(SettingsPath);
        if (existing != null) return existing;

        EnsureFolder(SettingsFolder);

        SceneTransitionSettings settings = ScriptableObject.CreateInstance<SceneTransitionSettings>();
        settings.tips = new System.Collections.Generic.List<string>
        {
            "Mẹo: Nhấn F để tương tác với vật thể xung quanh.",
            "Mẹo: Nhấn B để mở balo và xem vật phẩm đã thu thập.",
            "Khu rừng luôn thay đổi... hãy quan sát kỹ mọi ngóc ngách.",
            "Đang chuẩn bị cảnh tiếp theo...",
        };

        AssetDatabase.CreateAsset(settings, SettingsPath);
        AssetDatabase.SaveAssets();

        return settings;
    }

    static GameObject BuildHierarchy()
    {
        GameObject root = new GameObject("SceneTransitionManager",
            typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster),
            typeof(CanvasGroup), typeof(SceneTransitionManager));

        Canvas canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 5000;

        CanvasScaler scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        SceneTransitionManager manager = root.GetComponent<SceneTransitionManager>();
        manager.fadeGroup = root.GetComponent<CanvasGroup>();

        manager.colorBackdrop = CreateStretchImage(root.transform, "ColorBackdrop", Color.black);

        manager.background = CreateStretchImage(root.transform, "Background", Color.white);
        manager.background.preserveAspect = false;
        manager.background.enabled = false;

        manager.vignette = CreateStretchImage(root.transform, "Vignette", Color.white);

        manager.topBar = CreateBar(root.transform, "TopBar", true);
        manager.bottomBar = CreateBar(root.transform, "BottomBar", false);

        manager.spinner = CreateAnchoredImage(root.transform, "Spinner", new Vector2(0.5f, 0.24f), new Vector2(72, 72));
        manager.spinner.color = Color.white;

        GameObject barBg = CreateAnchoredImage(root.transform, "ProgressBar_BG", new Vector2(0.5f, 0.14f), new Vector2(560, 14)).gameObject;
        Image barBgImage = barBg.GetComponent<Image>();
        barBgImage.color = new Color(1f, 1f, 1f, 0.2f);

        GameObject fillGO = new GameObject("ProgressBar_Fill", typeof(RectTransform), typeof(Image));
        fillGO.transform.SetParent(barBg.transform, false);
        RectTransform fillRect = fillGO.GetComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0f, 0f);
        fillRect.anchorMax = new Vector2(0f, 1f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        Image fillImage = fillGO.GetComponent<Image>();
        fillImage.color = Color.white;
        fillImage.raycastTarget = false;
        manager.progressFill = fillImage;

        manager.progressText = CreateText(root.transform, "ProgressText", new Vector2(0.5f, 0.17f), new Vector2(200, 40), 24, TextAlignmentOptions.Center);
        manager.progressText.text = "0%";

        manager.tipText = CreateText(root.transform, "TipText", new Vector2(0.5f, 0.08f), new Vector2(1200, 80), 26, TextAlignmentOptions.Center);
        manager.tipText.text = "";

        return root;
    }

    static Image CreateStretchImage(Transform parent, string name, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = go.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;

        return image;
    }

    static RectTransform CreateBar(Transform parent, string name, bool top)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, top ? 1f : 0f);
        rect.anchorMax = new Vector2(1f, top ? 1f : 0f);
        rect.pivot = new Vector2(0.5f, top ? 1f : 0f);
        rect.sizeDelta = new Vector2(0f, 0f);
        rect.anchoredPosition = Vector2.zero;

        Image image = go.GetComponent<Image>();
        image.color = Color.black;
        image.raycastTarget = false;

        return rect;
    }

    static Image CreateAnchoredImage(Transform parent, string name, Vector2 anchor, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;

        Image image = go.GetComponent<Image>();
        image.raycastTarget = false;

        return image;
    }

    static TextMeshProUGUI CreateText(Transform parent, string name, Vector2 anchor, Vector2 size, float fontSize, TextAlignmentOptions alignment)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;

        TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.raycastTarget = false;

        return text;
    }

    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;

        string[] parts = path.Split('/');
        string current = parts[0];

        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
            {
                AssetDatabase.CreateFolder(current, parts[i]);
            }
            current = next;
        }
    }
}
