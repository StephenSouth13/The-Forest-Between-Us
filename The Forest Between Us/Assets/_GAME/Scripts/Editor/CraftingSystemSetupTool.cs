using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CraftingSystemSetupTool
{
    [MenuItem("Tools/Forest Between Us/Setup Repair, Upgrade & Salvage Workbenches")]
    public static void SetupCraftingWorkbenches()
    {
        // 1. Tải nguyên liệu
        ItemData woodItem = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/_GAME/Data/Items/Item_WoodLog.asset");
        ItemData stoneItem = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/_GAME/Data/Items/Item_Stone.asset");

        // 2. Tạo Item Rìu Gãy, Rìu Thường & Rìu Thép Nâng Cấp
        ItemData brokenAxe = GetOrCreateItem("Item_BrokenAxe", "Rìu Gỗ Cũ (Bị Gãy)", ItemType.Tool, false, 1, false, 0, 0, 0, 1.5f);
        ItemData normalAxe = GetOrCreateItem("Item_Axe", "Rìu Gỗ Săn Bắt (Lvl 1)", ItemType.Tool, false, 1, false, 0, 0, 0, 2.0f);
        ItemData steelAxe = GetOrCreateItem("Item_SteelAxe", "Rìu Thép Cường Hóa (Lvl 2)", ItemType.Tool, false, 1, false, 0, 0, 0, 2.5f);

        // Cấu hình Nâng Cấp, Sửa Chữa & Đập Bỏ
        normalAxe.hasDurability = true;
        normalAxe.maxDurability = 100f;
        normalAxe.repairIngredient = stoneItem;
        normalAxe.repairIngredientAmount = 1;
        normalAxe.brokenItemVariant = brokenAxe;

        normalAxe.itemLevel = 1;
        normalAxe.upgradedItemVariant = steelAxe;
        normalAxe.upgradeMaterial = stoneItem;
        normalAxe.upgradeMaterialAmount = 2;

        normalAxe.canDismantle = true;
        normalAxe.salvageReturnItem = woodItem;
        normalAxe.salvageReturnAmount = 1; // Thu lại 1 khúc gỗ (~50%)

        brokenAxe.brokenItemVariant = normalAxe; // Sửa rìu gãy -> Rìu lành
        brokenAxe.repairIngredient = stoneItem;
        brokenAxe.repairIngredientAmount = 1;

        steelAxe.itemLevel = 2;
        steelAxe.salvageReturnItem = stoneItem;
        steelAxe.salvageReturnAmount = 1;

        EditorUtility.SetDirty(brokenAxe);
        EditorUtility.SetDirty(normalAxe);
        EditorUtility.SetDirty(steelAxe);

        AssetDatabase.SaveAssets();

        // 3. Spawn các Bàn Chế Tạo trong Scene
        Vector3 centerPos = Vector3.zero;
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null && Camera.main != null) player = Camera.main.gameObject;
        if (player != null) centerPos = player.transform.position;

        GameObject container = GameObject.Find("Workbenches_Container");
        if (container == null)
        {
            container = new GameObject("Workbenches_Container");
            Undo.RegisterCreatedObjectUndo(container, "Create Workbenches_Container");
        }

        // 🔧 Bàn Sửa Chữa (Repair Station)
        CreateStation<RepairStation>(container.transform, "RepairStation_Sample", centerPos + new Vector3(-3f, 0f, -4f),
            "Bàn Sửa Chữa Trang Bị", new Color(0.2f, 0.4f, 0.6f));

        // ⭐ Bàn Nâng Cấp (Upgrade Station)
        CreateStation<UpgradeStation>(container.transform, "UpgradeStation_Sample", centerPos + new Vector3(0f, 0f, -4f),
            "Bàn Nâng Cấp Trang Bị", new Color(0.8f, 0.6f, 0.1f));

        // 🔨 Bàn Tinh Giản Đập Bỏ (Salvage Station)
        CreateStation<SalvageStation>(container.transform, "SalvageStation_Sample", centerPos + new Vector3(3f, 0f, -4f),
            "Bàn Tinh Giản & Tháo Đập Trang Bị", new Color(0.6f, 0.2f, 0.2f));

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log("<b>[Forest Between Us] SUCCESS!</b> Configured Workbenches for Repair, Upgrade & Salvage (50% refund)!");
        EditorUtility.DisplayDialog("Thành Công!",
            "Đã cài đặt thành công Trọn Bộ 3 Bàn Thao Tác Chế Tạo:\n\n" +
            "1. 🔧 Bàn Sửa Chữa (RepairStation): Sửa chữa vật phẩm hỏng/gãy về 100% bằng nguyên liệu tiêu hao.\n" +
            "2. ⭐ Bàn Nâng Cấp (UpgradeStation): Nâng cấp Rìu Gỗ Lvl 1 ➔ Rìu Thép Lvl 2 tăng lực chặt & độ bền.\n" +
            "3. 🔨 Bàn Tinh Giản (SalvageStation): Tháo đập trang bị không dùng để hoàn lại 50% nguyên liệu ban đầu!\n\n" +
            "Tất cả các Script đều có ô '🎨 Custom Asset Slots' trong Inspector để bạn kéo thả Model/VFX/SFX tùy ý!", "OK");
    }

    static ItemData GetOrCreateItem(string assetName, string itemName, ItemType category, bool stackable, int maxStack, bool consumable, float hunger = 0, float thirst = 0, float health = 0, float weight = 1.0f)
    {
        string path = $"Assets/_GAME/Data/Items/{assetName}.asset";
        ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
        if (item == null)
        {
            item = ScriptableObject.CreateInstance<ItemData>();
            item.itemName = itemName;
            item.category = category;
            item.isStackable = stackable;
            item.maxStackSize = maxStack;
            item.isConsumable = consumable;
            item.hungerRestore = hunger;
            item.thirstRestore = thirst;
            item.healthRestore = health;
            item.itemWeight = weight;

            AssetDatabase.CreateAsset(item, path);
        }
        else
        {
            item.itemName = itemName;
            item.category = category;
            item.isConsumable = consumable;
            item.hungerRestore = hunger;
            item.thirstRestore = thirst;
            item.healthRestore = health;
            item.itemWeight = weight;
            EditorUtility.SetDirty(item);
        }
        return item;
    }

    static void CreateStation<T>(Transform parent, string goName, Vector3 pos, string sName, Color color) where T : MonoBehaviour
    {
        GameObject go = GameObject.Find(goName);
        if (go == null)
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = goName;
            Undo.RegisterCreatedObjectUndo(go, $"Create {goName}");
        }

        go.transform.SetParent(parent);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(1.8f, 0.9f, 1.2f);

        Renderer rend = go.GetComponent<Renderer>();
        if (rend != null)
        {
            Shader defaultShader = Shader.Find("HDRP/Lit");
            if (defaultShader == null) defaultShader = Shader.Find("Universal Render Pipeline/Lit");
            if (defaultShader == null) defaultShader = Shader.Find("Standard");

            Material mat = new Material(defaultShader);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            else if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);

            rend.sharedMaterial = mat;
        }

        T station = go.GetComponent<T>();
        if (station == null) station = go.AddComponent<T>();

        EditorUtility.SetDirty(station);
    }
}
