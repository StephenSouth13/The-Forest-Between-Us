#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class DevSceneSetupTool
{
    [MenuItem("Tools/Forest Between Us/Create Developer Dashboard Scene")]
    public static void CreateDevScene()
    {
        // Tạo Scene Mới
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Tạo Đối Tượng Đầu Não
        GameObject directorObj = new GameObject("GAME_DIRECTOR_BRAIN");
        directorObj.AddComponent<GameDirector>();
        
        // Save Scene
        string scenePath = "Assets/_GAME/Scenes/Developer_Control_Center.unity";
        
        if (!System.IO.Directory.Exists("Assets/_GAME/Scenes"))
        {
            System.IO.Directory.CreateDirectory("Assets/_GAME/Scenes");
        }

        bool saved = EditorSceneManager.SaveScene(newScene, scenePath);
        
        if (saved)
        {
            Debug.Log($"🎉 [Forest Between Us] Đã tạo thành công Scene Đầu Não tại {scenePath}");
            EditorUtility.DisplayDialog("Thành Công", "Đã tạo Scene 'Developer_Control_Center'.\n\nHãy mở Scene này lên, chọn GameObject 'GAME_DIRECTOR_BRAIN' để cài đặt Cốt truyện, Quái, và các thông số Giao Thương (Cửa hàng) bằng Inspector!", "Tuyệt Vời");
        }
        else
        {
            Debug.LogError("Failed to save the Developer Scene.");
        }
    }
}
#endif
