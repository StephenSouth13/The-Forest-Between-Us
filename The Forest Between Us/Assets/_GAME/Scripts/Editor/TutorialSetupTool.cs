using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;
using TobyFredson;

public static class TutorialSetupTool
{
    [MenuItem("Tools/Forest Between Us/Setup Full Tutorial & UI")]
    public static void SetupFullTutorialAndUI()
    {
        Canvas canvas = GetOrCreateCanvas();
        EnsureEventSystem();

        // 1. Setup PlayerStatsManager (Real Vitals & Persistence)
        PlayerStatsManager statsManager = GetOrCreateComponent<PlayerStatsManager>("PlayerStatsManager");

        // 2. Setup FPSController Invert Y & Persistent Settings
        FPSController fpsController = Object.FindFirstObjectByType<FPSController>();
        if (fpsController != null)
        {
            fpsController.invertMouseY = false; // Mouse UP looks UP
            EditorUtility.SetDirty(fpsController);
        }

        // 3. Setup TutorialManager
        TutorialManager tutorialManager = GetOrCreateComponent<TutorialManager>("TutorialManager");

        // 3. Setup PlayerStatusUI (Tab Key)
        PlayerStatusUI statusUI = GetOrCreateUIComponent<PlayerStatusUI>("PlayerStatusUI", canvas.transform);
        statusUI.toggleKey = KeyCode.Tab;
        statusUI.controlCursor = true;

        // 4. Setup BackpackUIController (B Key)
        BackpackUIController backpackUI = GetOrCreateUIComponent<BackpackUIController>("BackpackUIController", canvas.transform);
        backpackUI.toggleKey = KeyCode.B;
        backpackUI.controlCursor = true;

        // 5. Setup Objective Waypoint Arrow (SM_Radio Navigation)
        ObjectiveWaypointArrow waypointArrow = GetOrCreateUIComponent<ObjectiveWaypointArrow>("ObjectiveWaypointArrow", canvas.transform);
        waypointArrow.targetObjectName = "SM_Radio";
        waypointArrow.labelText = "TÍN HIỆU RADIO";

        // 6. Setup Circular Survival HUD (Circular Minimap + Health, Thirst, Hunger, Sleep badges)
        CircularSurvivalHUD circularHUD = GetOrCreateUIComponent<CircularSurvivalHUD>("CircularSurvivalHUD", canvas.transform);

        // 7. Setup Full Map UI (M Key Map Popup)
        FullMapUIController fullMapUI = GetOrCreateUIComponent<FullMapUIController>("FullMapUIController", canvas.transform);
        fullMapUI.toggleKey = KeyCode.M;
        fullMapUI.controlCursor = true;

        // 8. Setup Pause Settings UI (ESC Key Menu)
        PauseSettingsUIController pauseUI = GetOrCreateUIComponent<PauseSettingsUIController>("PauseSettingsUIController", canvas.transform);
        pauseUI.toggleKey = KeyCode.Escape;

        // 9. Setup Radio Dialogue Subtitles Panel (Bottom Center)
        RadioDialogueUIController dialogueUI = GetOrCreateUIComponent<RadioDialogueUIController>("RadioDialogueUIController", canvas.transform);

        // 10. Setup Recipe Book UI (L Key Survival Library)
        RecipeBookUIController recipeBookUI = GetOrCreateUIComponent<RecipeBookUIController>("RecipeBookUIController", canvas.transform);
        recipeBookUI.toggleKey = KeyCode.L;

        // 11. Setup Player Respawn Manager (Checkpoint & Death Handling)
        PlayerRespawnManager respawnManager = GetOrCreateComponent<PlayerRespawnManager>("PlayerRespawnManager");

        // Link SM_Radio & Ensure Interactable Component
        GameObject radioGO = GameObject.Find("SM_Radio");
        if (radioGO != null)
        {
            if (radioGO.GetComponent<Collider>() == null)
            {
                radioGO.AddComponent<BoxCollider>();
            }

            if (radioGO.GetComponent<RadioInteractable>() == null)
            {
                radioGO.AddComponent<RadioInteractable>();
            }

            waypointArrow.targetTransform = radioGO.transform;
            fullMapUI.radioTransform = radioGO.transform;
            if (tutorialManager.goalTransform == null)
            {
                tutorialManager.goalTransform = radioGO.transform;
            }
        }

        EditorUtility.SetDirty(tutorialManager);
        EditorUtility.SetDirty(statusUI);
        EditorUtility.SetDirty(backpackUI);
        EditorUtility.SetDirty(waypointArrow);
        EditorUtility.SetDirty(circularHUD);
        EditorUtility.SetDirty(fullMapUI);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log("[TutorialSetupTool] HOÀN TẤT ĐỒNG BỘ 100% CÀI ĐẶT UI & ĐIỀU KHIỂN!\n" +
                  "1. Đã sửa lỗi Chuột Y: Di chuyển chuột lên -> Nhìn lên, kéo chuột xuống -> Nhìn xuống (Lưu vĩnh viễn qua PlayerPrefs).\n" +
                  "2. HUD Vòng Tròn Chỉ Số: Máu 💪, Độ Khát 💧, Độ Đói 🍗, Đuối Sức/Buồn Ngủ 🌙 + Dòng Nhiệm Vụ Banner.\n" +
                  "3. Phím M: Mở BẢN ĐỒ CHIẾN THUẬT (Tactical Fullscreen Map Popup).\n" +
                  "4. Phím TAB & B: Bảng Trạng Thái & Balô.");
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

    static T GetOrCreateUIComponent<T>(string gameObjectName, Transform canvasTransform) where T : Component
    {
        T existing = Object.FindFirstObjectByType<T>();
        if (existing != null)
        {
            if (existing.transform.parent != canvasTransform && existing.GetComponent<RectTransform>() != null)
            {
                existing.transform.SetParent(canvasTransform, false);
            }
            return existing;
        }

        GameObject go = new GameObject(gameObjectName, typeof(RectTransform), typeof(T));
        go.transform.SetParent(canvasTransform, false);
        return go.GetComponent<T>();
    }

    static T GetOrCreateComponent<T>(string gameObjectName) where T : Component
    {
        T existing = Object.FindFirstObjectByType<T>();
        if (existing != null) return existing;

        GameObject go = new GameObject(gameObjectName, typeof(T));
        return go.GetComponent<T>();
    }
}

