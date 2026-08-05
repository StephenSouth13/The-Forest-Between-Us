using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class AutoAssignTreesTool
{
    [MenuItem("Tools/Forest Between Us/Convert Selected Objects to Choppable Trees")]
    public static void ConvertSelectedToTrees()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects == null || selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("Thông Báo", "Vui lòng chọn 1 hoặc nhiều GameObject Cây 3D trong Hierarchy trước khi bấm nút này!", "OK");
            return;
        }

        ItemData woodItem = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/_GAME/Data/Items/Item_WoodLog.asset");
        int count = 0;

        foreach (GameObject go in selectedObjects)
        {
            Undo.RegisterCompleteObjectUndo(go, "Convert to Choppable Tree");

            // Đảm bảo có Collider
            if (go.GetComponent<Collider>() == null)
            {
                go.AddComponent<CapsuleCollider>();
            }

            // Gắn ResourceNode
            ResourceNode resNode = go.GetComponent<ResourceNode>();
            if (resNode == null) resNode = go.AddComponent<ResourceNode>();

            resNode.resourceType = ResourceType.Tree;
            resNode.resourceName = go.name;
            resNode.maxHits = 3;
            resNode.currentHits = 3;
            resNode.dropItemData = woodItem;
            resNode.dropAmountMin = 2;
            resNode.dropAmountMax = 4;
            resNode.autoRespawn = true;
            resNode.respawnTimeSeconds = 90f;
            resNode.directToInventory = true;

            EditorUtility.SetDirty(resNode);
            count++;
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"<b>[Forest Between Us] SUCCESS!</b> Successfully converted {count} selected 3D tree objects into Choppable Resource Trees!");
        EditorUtility.DisplayDialog("Thành Công!", $"Đã gắn thành công Script Chặt Cây Mọc Lại cho {count} đối tượng cây 3D được chọn!\n\nGiờ đây bạn có thể dùng Rìu chặt các cây này trong game.", "OK");
    }

    [MenuItem("Tools/Forest Between Us/Auto Detect & Convert ALL Trees in Scene")]
    public static void AutoDetectAllTreesInScene()
    {
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        ItemData woodItem = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/_GAME/Data/Items/Item_WoodLog.asset");
        int count = 0;

        foreach (GameObject go in allObjects)
        {
            string lowerName = go.name.ToLower();
            if (lowerName.Contains("tree") || lowerName.Contains("pine") || lowerName.Contains("foliage_tree") || lowerName.Contains("broadleaf"))
            {
                // Bỏ qua nếu là container hoặc camera
                if (go.transform.childCount > 10) continue;

                if (go.GetComponent<Collider>() == null)
                {
                    go.AddComponent<CapsuleCollider>();
                }

                ResourceNode resNode = go.GetComponent<ResourceNode>();
                if (resNode == null) resNode = go.AddComponent<ResourceNode>();

                resNode.resourceType = ResourceType.Tree;
                resNode.resourceName = "Cây Rừng " + go.name;
                resNode.maxHits = 3;
                resNode.currentHits = 3;
                resNode.dropItemData = woodItem;
                resNode.dropAmountMin = 2;
                resNode.dropAmountMax = 4;
                resNode.autoRespawn = true;
                resNode.respawnTimeSeconds = 90f;
                resNode.directToInventory = true;

                EditorUtility.SetDirty(resNode);
                count++;
            }
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"<b>[Forest Between Us] SUCCESS!</b> Auto-detected and converted {count} trees in Scene!");
        EditorUtility.DisplayDialog("Tự Động Quét Thành Công!", $"Hệ thống đã tự động tìm thấy và gắn Script Chặt Cây cho {count} cây 3D trong Scene hiện tại!", "OK");
    }

    [MenuItem("Tools/Forest Between Us/Convert Selected Objects to Rocks")]
    public static void ConvertSelectedToRocks()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects == null || selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("Thông Báo", "Vui lòng chọn 1 hoặc nhiều GameObject Cục Đá trong Hierarchy!", "OK");
            return;
        }

        ItemData rockItem = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/_GAME/Data/Items/Item_Stone.asset");
        int count = 0;

        foreach (GameObject go in selectedObjects)
        {
            Undo.RegisterCompleteObjectUndo(go, "Convert to Rock");

            if (go.GetComponent<Collider>() == null) go.AddComponent<BoxCollider>();

            ResourceNode resNode = go.GetComponent<ResourceNode>();
            if (resNode == null) resNode = go.AddComponent<ResourceNode>();

            resNode.resourceType = ResourceType.Rock;
            resNode.resourceName = "Đá Tảng";
            resNode.maxHits = 4;
            resNode.currentHits = 4;
            resNode.dropItemData = rockItem;
            resNode.dropAmountMin = 1;
            resNode.dropAmountMax = 3;
            resNode.autoRespawn = false; // Đá thường không mọc lại
            resNode.directToInventory = true;

            EditorUtility.SetDirty(resNode);
            count++;
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"<b>[Forest Between Us] SUCCESS!</b> Converted {count} objects into Rocks.");
        EditorUtility.DisplayDialog("Thành Công!", $"Đã chuyển {count} đối tượng thành Đá (Khai thác).", "OK");
    }

    [MenuItem("Tools/Forest Between Us/Convert Selected Objects to Berry Bushes")]
    public static void ConvertSelectedToBushes()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects == null || selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("Thông Báo", "Vui lòng chọn 1 hoặc nhiều Bụi cỏ trong Hierarchy!", "OK");
            return;
        }

        ItemData berryItem = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/_GAME/Data/Items/Wild_Vegetable.asset"); // Dùng Rau Rừng đã tạo
        int count = 0;

        foreach (GameObject go in selectedObjects)
        {
            Undo.RegisterCompleteObjectUndo(go, "Convert to Berry Bush");

            if (go.GetComponent<Collider>() == null) go.AddComponent<SphereCollider>();

            ResourceNode resNode = go.GetComponent<ResourceNode>();
            if (resNode == null) resNode = go.AddComponent<ResourceNode>();

            resNode.resourceType = ResourceType.BerryBush;
            resNode.resourceName = "Bụi Rau Rừng";
            resNode.maxHits = 1;
            resNode.currentHits = 1;
            resNode.dropItemData = berryItem;
            resNode.dropAmountMin = 1;
            resNode.dropAmountMax = 2;
            resNode.autoRespawn = true;
            resNode.respawnTimeSeconds = 60f;
            resNode.directToInventory = true;

            EditorUtility.SetDirty(resNode);
            count++;
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"<b>[Forest Between Us] SUCCESS!</b> Converted {count} objects into Berry Bushes.");
        EditorUtility.DisplayDialog("Thành Công!", $"Đã chuyển {count} đối tượng thành Bụi Rau Rừng (Hái lượm).", "OK");
    }
}
