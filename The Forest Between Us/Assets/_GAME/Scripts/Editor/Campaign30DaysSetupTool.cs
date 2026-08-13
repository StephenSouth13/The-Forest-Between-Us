using UnityEngine;
using UnityEditor;

public class Campaign30DaysSetupTool
{
    [MenuItem("Tools/Forest Between Us/Setup 30-Day Campaign & Story Data")]
    public static void SetupCampaign()
    {
        // 1. Tạo thư mục Data Story nếu chưa có
        if (!AssetDatabase.IsValidFolder("Assets/_GAME/Data"))
        {
            AssetDatabase.CreateFolder("Assets/_GAME", "Data");
        }
        if (!AssetDatabase.IsValidFolder("Assets/_GAME/Data/Story"))
        {
            AssetDatabase.CreateFolder("Assets/_GAME/Data", "Story");
        }

        // 2. Khởi tạo / Tìm StoryLoreDatabase Asset
        string storyDbPath = "Assets/_GAME/Data/Story/StoryLoreDatabase.asset";
        StoryLoreDatabase storyDb = AssetDatabase.LoadAssetAtPath<StoryLoreDatabase>(storyDbPath);
        if (storyDb == null)
        {
            storyDb = ScriptableObject.CreateInstance<StoryLoreDatabase>();
            storyDb.prologueLore = "<b>[CHƯƠNG I: NHẬT KÝ RỪNG THIÊNG]</b>\nNgày 01: Bạn thức dậy sau một cơn bão từ kỳ lạ. Chiếc máy thu phát vô tuyến phát ra những tần số đứt đoạn cùng giọng nói xa xưa của Mai An Tiêm về Hạt Giống Cổ Đại.";
            storyDb.day1Lore = "<b>[CHƯƠNG II: SƯƠNG MÙ VÙNG ĐỨT GÃY]</b>\nNgày 07: Sương độc bao phủ khu rừng. Những sinh thể biến dạng bắt đầu đi tìm kiếm nguồn năng lượng từ trại của bạn.";
            storyDb.day2Lore = "<b>[CHƯƠNG III: BÍ MẬT TRẠM PHÁT SỐNG]</b>\nNgày 15: Bạn tìm thấy tàn tích Trạm Vô Tuyến Siêu Cấp. Thương nhân bí ẩn xuất hiện đề nghị đổi Cổ Vật lấy linh kiện giải mã.";
            storyDb.day3Lore = "<b>[CHƯƠNG IV: QUYẾT ĐỊNH VẬN MỆNH (5 ENDINGS)]</b>\nNgày 30: Năng lượng Rift đạt đỉnh điểm. 5 Cánh Cổng Thời Không mở ra — Lựa chọn của bạn sẽ quyết định số phận của hai thế giới!";
            
            AssetDatabase.CreateAsset(storyDb, storyDbPath);
            EditorUtility.SetDirty(storyDb);
            Debug.Log("<b>[Forest Between Us]</b> Created StoryLoreDatabase Asset at " + storyDbPath);
        }

        // 3. Setup Campaign Manager GameObjects trong Scene
        GameObject managerObj = GameObject.Find("GameManagers");
        if (managerObj == null)
        {
            managerObj = new GameObject("GameManagers");
            Undo.RegisterCreatedObjectUndo(managerObj, "Create GameManagers");
        }

        // Add Campaign30DaysManager
        if (managerObj.GetComponent<Campaign30DaysManager>() == null)
        {
            managerObj.AddComponent<Campaign30DaysManager>();
        }

        // Add DayManager
        if (managerObj.GetComponent<DayManager>() == null)
        {
            managerObj.AddComponent<DayManager>();
        }

        // Add CombatManager
        if (managerObj.GetComponent<CombatManager>() == null)
        {
            managerObj.AddComponent<CombatManager>();
        }

        // Add EndingManager
        if (managerObj.GetComponent<EndingManager>() == null)
        {
            managerObj.AddComponent<EndingManager>();
        }

        // Add MissionManager
        if (managerObj.GetComponent<MissionManager>() == null)
        {
            managerObj.AddComponent<MissionManager>();
        }

        // Add QuestManager
        if (managerObj.GetComponent<QuestManager>() == null)
        {
            managerObj.AddComponent<QuestManager>();
        }

        AssetDatabase.SaveAssets();
        Selection.activeGameObject = managerObj;
        Debug.Log("<b>[Forest Between Us] SUCCESS!</b> Successfully configured 30-Day Campaign & Story Database in Scene!");
        EditorUtility.DisplayDialog("Setup Complete", "Đã khởi tạo & kết nối thành công Data Cốt Truyện + Bảng Thống Kê Chỉ Số 5 Kết Thúc vào Scene!\n\nBạn chỉ cần nhấn PLAY trong Unity để trải nghiệm.", "OK");
    }
}
