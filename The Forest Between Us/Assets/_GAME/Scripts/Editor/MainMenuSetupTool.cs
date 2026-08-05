using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Editor tool: Tools > Forest Between Us > Setup Main Menu
/// Tự động gắn MainMenuController vào scene Home,
/// xoá Canvas cũ và đảm bảo EventSystem tồn tại.
/// </summary>
public class MainMenuSetupTool
{
    [MenuItem("Tools/Forest Between Us/🎬 Setup Main Menu (Auto)")]
    public static void SetupMainMenu()
    {
        // 1. Tìm hoặc tạo GameObject gốc "Home"
        GameObject homeGO = GameObject.Find("Home");
        if (homeGO == null)
        {
            homeGO = new GameObject("Home");
            Undo.RegisterCreatedObjectUndo(homeGO, "Create Home");
            Debug.Log("[MainMenuSetup] Created new 'Home' GameObject.");
        }

        // 2. Xoá Canvas cũ (nếu có) để tránh trùng lặp UI
        Canvas[] oldCanvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        int removedCount = 0;
        foreach (Canvas c in oldCanvases)
        {
            // Chỉ xoá canvas tên "Canvas" hoặc "MainMenu_Canvas" (không xoá canvas của Transition Manager)
            if (c.gameObject.name == "Canvas" || c.gameObject.name == "MainMenu_Canvas")
            {
                Undo.DestroyObjectImmediate(c.gameObject);
                removedCount++;
            }
        }
        if (removedCount > 0)
            Debug.Log($"[MainMenuSetup] Removed {removedCount} old Canvas(es).");

        // 3. Đảm bảo EventSystem tồn tại
        EventSystem es = Object.FindFirstObjectByType<EventSystem>();
        if (es == null)
        {
            GameObject esGO = new GameObject("EventSystem",
                typeof(EventSystem),
                typeof(StandaloneInputModule));
            Undo.RegisterCreatedObjectUndo(esGO, "Create EventSystem");
            Debug.Log("[MainMenuSetup] Created EventSystem.");
        }

        // 4. Gắn MainMenuController (xoá cái cũ nếu đã có)
        MainMenuController existingCtrl = homeGO.GetComponent<MainMenuController>();
        if (existingCtrl != null)
        {
            Undo.DestroyObjectImmediate(existingCtrl);
        }

        MainMenuController ctrl = Undo.AddComponent<MainMenuController>(homeGO);
        ctrl.gameplaySceneName = "GamePlay";

        // 5. Đánh dấu scene là dirty để Unity biết cần save
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        // 6. Thông báo thành công
        EditorUtility.DisplayDialog(
            "✅  Setup Thành Công!",
            "MainMenuController đã được gắn vào GameObject 'Home'.\n\n" +
            "Bước tiếp theo:\n" +
            "1. Nhấn ▶ PLAY để xem menu động\n" +
            "2. Inspector của 'Home' → kiểm tra tên scene GamePlay\n" +
            "3. Bạn có thể kéo AudioClip nhạc nền vào 'Menu Music Clip' nếu có\n\n" +
            "Toàn bộ UI, nhạc nền và hiệu ứng sẽ tự dựng khi Play!",
            "OK - Tuyệt vời!");

        // 7. Select Home để dễ xem Inspector
        Selection.activeGameObject = homeGO;
    }

    // Validate: chỉ enable khi đang ở scene Home
    [MenuItem("Tools/Forest Between Us/🎬 Setup Main Menu (Auto)", true)]
    public static bool ValidateSetupMainMenu()
    {
        return true; // luôn enable
    }
}
