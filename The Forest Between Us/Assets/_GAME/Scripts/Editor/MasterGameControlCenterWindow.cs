using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MasterGameControlCenterWindow : EditorWindow
{
    private Vector2 scrollPosition;

    [MenuItem("Tools/Forest Between Us/🎮 MASTER CONTROL CENTER (BẢNG QUẢN LÝ DỰ ÁN)", false, 0)]
    public static void ShowWindow()
    {
        GetWindow<MasterGameControlCenterWindow>("Master Game Control Center");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        EditorGUILayout.LabelField("🌲 THE FOREST BETWEEN US - MASTER CONTROL CENTER 🌲", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Bảng quản lý tập trung toàn bộ Tools & Chuyển Cảnh AAA", EditorStyles.miniLabel);
        GUILayout.Space(10);

        // 🟢 CẢNH BÁO NẾU ĐANG Ở PLAY MODE (UNITY PLAY MODE GUARD)
        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox("⚠️ CẢNH BÁO: Unity đang ở chế độ PLAY MODE!\n\n" +
                                    "Nếu bạn chạy các Tool setup khi đang Play, các thay đổi trên Scene sẽ BỊ MẤT KHI STOP GAME.\n" +
                                    "Vui lòng nhấn nút STOP (▶) ở trên cùng Unity trước khi bấm chạy các Tool bên dưới.", MessageType.Warning);
            GUILayout.Space(5);
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // -------------------------------------------------------------
        // SECTION 1: HỆ THỐNG CHUYỂN CẢNH & LOADING SCENE AAA
        // -------------------------------------------------------------
        EditorGUILayout.BeginVertical("box");
        GUI.color = new Color(0.4f, 0.8f, 1f);
        EditorGUILayout.LabelField("1. HỆ THỐNG CHUYỂN CẢNH & LOADING SCREEN (AAA)", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.HelpBox("Tự động tạo Loading Screen Prefab nâng cấp với Cinematic Black Bars, Progressive Bar, Mẹo sinh tồn & Hiệu ứng Vignette mượt mà.", MessageType.Info);
        
        if (GUILayout.Button("🎬 Setup / Upgrade Loading Screen System (AAA)", GUILayout.Height(32)))
        {
            if (CheckAndConfirmPlayMode())
            {
                SceneTransitionSetupTool.Setup();
                AutoSaveActiveScene();
            }
        }
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        // -------------------------------------------------------------
        // SECTION 2: HỆ THỐNG CORE & CỐT TRUYỆN 30 NGÀY
        // -------------------------------------------------------------
        EditorGUILayout.BeginVertical("box");
        GUI.color = new Color(0.5f, 1f, 0.5f);
        EditorGUILayout.LabelField("2. HỆ THỐNG 30 NGÀY & CỐT TRUYỆN 5 ENDINGS", EditorStyles.boldLabel);
        GUI.color = Color.white;
        EditorGUILayout.HelpBox("Tự động nạp Data Cốt Truyện 4 Chương, Nhật ký rừng thiêng, Bảng Thống Kê Chỉ Số 5 Kết Thúc & các Managers vào Scene.", MessageType.Info);
        
        if (GUILayout.Button("📖 Setup 30-Day Campaign & Story Data", GUILayout.Height(30)))
        {
            if (CheckAndConfirmPlayMode())
            {
                Campaign30DaysSetupTool.SetupCampaign();
                AutoSaveActiveScene();
            }
        }
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        // -------------------------------------------------------------
        // SECTION 3: TÀI NGUYÊN, ITEM & CRAFTING
        // -------------------------------------------------------------
        EditorGUILayout.BeginVertical("box");
        GUI.color = new Color(1f, 0.8f, 0.4f);
        EditorGUILayout.LabelField("3. VẬT PHẨM, KHAI THÁC & CHẾ TẠO (ITEM & CRAFTING)", EditorStyles.boldLabel);
        GUI.color = Color.white;
        
        if (GUILayout.Button("🪓 Setup Harvest & Resource System", GUILayout.Height(26)))
        {
            if (CheckAndConfirmPlayMode())
            {
                HarvestSystemSetupTool.SetupHarvestSystem();
                AutoSaveActiveScene();
            }
        }
        if (GUILayout.Button("🔨 Setup Crafting System & Recipes", GUILayout.Height(26)))
        {
            if (CheckAndConfirmPlayMode())
            {
                CraftingSystemSetupTool.SetupCraftingWorkbenches();
                AutoSaveActiveScene();
            }
        }
        if (GUILayout.Button("🎒 Setup Inventory UI & Drag-Drop System", GUILayout.Height(26)))
        {
            if (CheckAndConfirmPlayMode())
            {
                InventorySetupTool.SetupInventoryUI();
                AutoSaveActiveScene();
            }
        }
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        // -------------------------------------------------------------
        // SECTION 4: QUÁI VẬT, BOT AI & CÂY CỐI TERRAIN
        // -------------------------------------------------------------
        EditorGUILayout.BeginVertical("box");
        GUI.color = new Color(1f, 0.5f, 0.5f);
        EditorGUILayout.LabelField("4. QUÁI VẬT AI & TERRAIN ENVIRONMENT", EditorStyles.boldLabel);
        GUI.color = Color.white;
        
        if (GUILayout.Button("👾 Setup Full AI & Monster Spawner System", GUILayout.Height(26)))
        {
            if (CheckAndConfirmPlayMode())
            {
                AISystemSetupTool.SetupAISystem();
                AutoSaveActiveScene();
            }
        }
        if (GUILayout.Button("🌳 Auto Detect & Convert ALL Trees in Scene", GUILayout.Height(26)))
        {
            if (CheckAndConfirmPlayMode())
            {
                AutoAssignTreesTool.AutoDetectAllTreesInScene();
                AutoSaveActiveScene();
            }
        }
        if (GUILayout.Button("🌲 Convert Selected Objects to Choppable Trees", GUILayout.Height(26)))
        {
            if (CheckAndConfirmPlayMode())
            {
                AutoAssignTreesTool.ConvertSelectedToTrees();
                AutoSaveActiveScene();
            }
        }
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        // -------------------------------------------------------------
        // SECTION 5: UI NỔI & TƯƠNG TÁC GIAO DIỆN
        // -------------------------------------------------------------
        EditorGUILayout.BeginVertical("box");
        GUI.color = new Color(0.8f, 0.6f, 1f);
        EditorGUILayout.LabelField("5. NÂNG CẤP GIAO DIỆN & TUTORIAL", EditorStyles.boldLabel);
        GUI.color = Color.white;
        
        if (GUILayout.Button("🖥️ Setup Main Menu Scene", GUILayout.Height(26)))
        {
            if (CheckAndConfirmPlayMode())
            {
                MainMenuSetupTool.SetupMainMenu();
                AutoSaveActiveScene();
            }
        }
        if (GUILayout.Button("📜 Setup Full Tutorial & UI", GUILayout.Height(26)))
        {
            if (CheckAndConfirmPlayMode())
            {
                TutorialSetupTool.SetupFullTutorialAndUI();
                AutoSaveActiveScene();
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndScrollView();
    }

    private bool CheckAndConfirmPlayMode()
    {
        if (Application.isPlaying)
        {
            return EditorUtility.DisplayDialog("Cảnh Báo Play Mode",
                "Unity đang ở chế độ PLAY MODE!\n\n" +
                "Nếu tiếp tục, những thiết lập mới này sẽ bị mất khi bạn bấm STOP game.\n\n" +
                "Bạn có muốn tiếp tục chạy không?", "Tiếp tục chạy", "Hủy bỏ");
        }
        return true;
    }

    private void AutoSaveActiveScene()
    {
        if (!Application.isPlaying)
        {
            var activeScene = EditorSceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(activeScene);
            EditorSceneManager.SaveScene(activeScene);
            AssetDatabase.SaveAssets();
            Debug.Log($"<b>[Master Control Center]</b> 💾 Auto-Saved Scene '{activeScene.name}' and Assets successfully!");
        }
    }
}
