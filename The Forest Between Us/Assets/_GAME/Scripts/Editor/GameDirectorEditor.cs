#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(GameDirector))]
public class GameDirectorEditor : Editor
{
    private int currentTab = 0;
    private string[] tabs = { "🌍 WORLD", "🧍 PLAYER", "💀 DEATH & COMBAT", "🛒 ECONOMY", "🎬 SCENES" };

    // Properties
    SerializedProperty walkSpeed, runSpeed, jumpHeight;
    SerializedProperty hungerDepletion, thirstDepletion, staminaDepletion, dayNightSpeed;
    SerializedProperty enemyDmgMul, respawnHealth, dropItems, karmaPenalty;
    SerializedProperty mainQuestDialogues;
    SerializedProperty globalCurrencyItem, merchantInventory;

    private Texture2D headerBg;

    void OnEnable()
    {
        walkSpeed = serializedObject.FindProperty("walkSpeed");
        runSpeed = serializedObject.FindProperty("runSpeed");
        jumpHeight = serializedObject.FindProperty("jumpHeight");
        
        hungerDepletion = serializedObject.FindProperty("globalHungerDepletionRate");
        thirstDepletion = serializedObject.FindProperty("globalThirstDepletionRate");
        staminaDepletion = serializedObject.FindProperty("globalStaminaDepletionRate");
        dayNightSpeed = serializedObject.FindProperty("dayNightCycleSpeed");

        enemyDmgMul = serializedObject.FindProperty("enemyDamageMultiplier");
        respawnHealth = serializedObject.FindProperty("respawnHealthPercentage");
        dropItems = serializedObject.FindProperty("dropItemsOnDeath");
        karmaPenalty = serializedObject.FindProperty("respawnKarmaPenalty");

        mainQuestDialogues = serializedObject.FindProperty("mainQuestDialogues");
        
        globalCurrencyItem = serializedObject.FindProperty("globalCurrencyItem");
        merchantInventory = serializedObject.FindProperty("merchantInventory");

        headerBg = MakeTex(1, 1, new Color(0.1f, 0.15f, 0.2f, 1f));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Header Styling
        GUIStyle headerStyle = new GUIStyle(GUI.skin.box);
        headerStyle.normal.background = headerBg;
        headerStyle.normal.textColor = Color.white;
        headerStyle.fontSize = 20;
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.alignment = TextAnchor.MiddleCenter;
        
        GUILayout.Space(10);
        GUILayout.Box("DEVELOPER CONTROL CENTER\n<size=12>BRAIN ENGINE</size>", headerStyle, GUILayout.ExpandWidth(true), GUILayout.Height(60));
        GUILayout.Space(10);

        // Tabs
        GUIStyle tabStyle = new GUIStyle(GUI.skin.button);
        tabStyle.fontSize = 12;
        tabStyle.fontStyle = FontStyle.Bold;
        tabStyle.fixedHeight = 35;

        GUILayout.BeginHorizontal();
        for (int i = 0; i < tabs.Length; i++)
        {
            if (GUILayout.Toggle(currentTab == i, tabs[i], tabStyle, GUILayout.ExpandWidth(true)))
            {
                currentTab = i;
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        // Content
        EditorGUI.BeginChangeCheck();

        switch (currentTab)
        {
            case 0: DrawWorldTab(); break;
            case 1: DrawPlayerTab(); break;
            case 2: DrawDeathCombatTab(); break;
            case 3: DrawEconomyTab(); break;
            case 4: DrawScenesTab(); break;
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawWorldTab()
    {
        EditorGUILayout.HelpBox("Các thông số cân bằng thế giới. Áp dụng toàn cầu.", MessageType.Info);
        
        EditorGUILayout.LabelField("Day & Night Cycle", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(dayNightSpeed, new GUIContent("Tốc Độ Trôi Thời Gian"));
        
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Survival Drain Rates", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(hungerDepletion, new GUIContent("Độ Tụt Đói"));
        EditorGUILayout.PropertyField(thirstDepletion, new GUIContent("Độ Tụt Khát"));
        EditorGUILayout.PropertyField(staminaDepletion, new GUIContent("Độ Tụt Thể Lực"));

        GUILayout.Space(15);
        EditorGUILayout.LabelField("Story Dialogues", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(mainQuestDialogues, true);
    }

    private void DrawPlayerTab()
    {
        EditorGUILayout.HelpBox("Tinh chỉnh cảm giác điều khiển nhân vật (Game Feel).", MessageType.Info);
        
        EditorGUILayout.PropertyField(walkSpeed, new GUIContent("Tốc Độ Đi Bộ"));
        EditorGUILayout.PropertyField(runSpeed, new GUIContent("Tốc Độ Chạy (Shift)"));
        EditorGUILayout.PropertyField(jumpHeight, new GUIContent("Lực Nhảy"));
    }

    private void DrawDeathCombatTab()
    {
        EditorGUILayout.HelpBox("Thiết lập luật khi chiến đấu và khi nhân vật bị chết.", MessageType.Warning);

        EditorGUILayout.LabelField("Combat Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(enemyDmgMul, new GUIContent("Hệ Số Sát Thương Quái"));

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Respawn Penalties", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(respawnHealth, new GUIContent("% Máu Khi Hồi Sinh"));
        EditorGUILayout.PropertyField(karmaPenalty, new GUIContent("Trừ Bao Nhiêu Karma"));
        EditorGUILayout.PropertyField(dropItems, new GUIContent("Rớt Đồ Khi Chết?"));
    }

    private void DrawEconomyTab()
    {
        EditorGUILayout.HelpBox("Quản lý tiền tệ và các mặt hàng thương nhân bán.", MessageType.Info);
        
        EditorGUILayout.PropertyField(globalCurrencyItem, new GUIContent("Tiền Tệ Chính (VD: Vỏ Sò)"));
        GUILayout.Space(10);
        EditorGUILayout.PropertyField(merchantInventory, new GUIContent("Danh Sách Hàng Bán"), true);
    }

    private void DrawScenesTab()
    {
        EditorGUILayout.HelpBox("Chuyển đổi nhanh giữa các Scene mà không cần tìm file trong Project.", MessageType.Info);

        GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
        btnStyle.fontSize = 14; btnStyle.fixedHeight = 40;

        if (GUILayout.Button("🎬 Mở Scene: GAMEPLAY (Rừng Sương Mù)", btnStyle))
        {
            OpenScene("Assets/_GAME/Scenes/GamePlay.unity");
        }
        GUILayout.Space(5);
        if (GUILayout.Button("🏠 Mở Scene: TRANG CHỦ (Home Menu)", btnStyle))
        {
            OpenScene("Assets/_GAME/Scenes/Home.unity");
        }
        GUILayout.Space(5);
        if (GUILayout.Button("🧠 Lưu & Tải Lại Scene Đầu Não (Brain)", btnStyle))
        {
            OpenScene("Assets/_GAME/Scenes/Developer_Control_Center.unity");
        }
    }

    private void OpenScene(string path)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(path);
        }
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++) pix[i] = col;
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
#endif
