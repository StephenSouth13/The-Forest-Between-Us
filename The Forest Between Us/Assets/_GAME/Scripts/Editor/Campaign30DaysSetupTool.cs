using UnityEngine;
using UnityEditor;

public class Campaign30DaysSetupTool
{
    [MenuItem("Tools/Forest Between Us/Setup 30-Day Campaign")]
    public static void SetupCampaign()
    {
        // 1. Setup Campaign Manager GameObject
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

        Selection.activeGameObject = managerObj;
        Debug.Log("<b>[Forest Between Us] SUCCESS!</b> Successfully configured 30-Day Campaign Managers in Scene!");
        EditorUtility.DisplayDialog("Setup Complete", "Đã thiết lập thành công toàn bộ Hệ thống Manager 30 Ngày trong Scene!\n\nBạn chỉ cần nhấn PLAY trong Unity để trải nghiệm.", "OK");
    }
}
