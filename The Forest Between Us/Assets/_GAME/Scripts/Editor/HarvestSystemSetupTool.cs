using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class HarvestSystemSetupTool
{
    [MenuItem("Tools/Forest Between Us/Setup Harvest & Resource System")]
    public static void SetupHarvestSystem()
    {
        // 1. Tạo thư mục chứa Asset Data
        if (!AssetDatabase.IsValidFolder("Assets/_GAME/Data"))
        {
            AssetDatabase.CreateFolder("Assets/_GAME", "Data");
        }
        if (!AssetDatabase.IsValidFolder("Assets/_GAME/Data/Items"))
        {
            AssetDatabase.CreateFolder("Assets/_GAME/Data", "Items");
        }

        // 2. Khởi tạo / Tìm ItemData Assets
        ItemData spoiledFood = GetOrCreateItem("Item_SpoiledFood", "Trái Cây / Đồ Ăn Thối", ItemType.Food, true, 20, true, -15f, 0f, -20f, 0.2f);
        ItemData wildBerry = GetOrCreateItem("Item_WildBerry", "Trái Cây Rừng", ItemType.Food, true, 20, true, 25f, 15f, 10f, 0.2f);
        wildBerry.canSpoil = true;
        wildBerry.spoilTimeSeconds = 180f; // 3 phút thối hỏng
        wildBerry.spoiledItemResult = spoiledFood;
        EditorUtility.SetDirty(wildBerry);

        ItemData woodItem = GetOrCreateItem("Item_WoodLog", "Khúc Gỗ Rừng", ItemType.Resource, true, 99, false, 0, 0, 0, 3.0f);
        ItemData stoneItem = GetOrCreateItem("Item_Stone", "Đá Cuội Rừng", ItemType.Resource, true, 99, false, 0, 0, 0, 2.5f);

        ItemData berrySeed = GetOrCreateItem("Item_BerrySeed", "Hạt Giống Trái Cây", ItemType.Resource, true, 50, false, 0, 0, 0, 0.05f);
        berrySeed.isSeed = true;
        berrySeed.cropHarvestResult = wildBerry;
        EditorUtility.SetDirty(berrySeed);

        ItemData emptyBottle = GetOrCreateItem("Item_EmptyBottle", "Bình Nước Rỗng", ItemType.Resource, true, 5, false, 0, 0, 0, 0.3f);
        emptyBottle.isWaterContainer = true;
        emptyBottle.isFullWaterBottle = false;

        ItemData fullWaterBottle = GetOrCreateItem("Item_FullWaterBottle", "Nước Suối Trong", ItemType.Consumable, true, 5, true, 0f, 60f, 5f, 1.3f);
        fullWaterBottle.isWaterContainer = true;
        fullWaterBottle.isFullWaterBottle = true;
        fullWaterBottle.emptyBottleVariant = emptyBottle;

        emptyBottle.fullBottleVariant = fullWaterBottle;
        EditorUtility.SetDirty(emptyBottle);
        EditorUtility.SetDirty(fullWaterBottle);

        // Raw & Cooked Food
        ItemData rawMeat = GetOrCreateItem("Item_RawMeat", "Thịt Sống", ItemType.Food, true, 20, true, 15f, 0f, -25f, 0.5f);
        rawMeat.isRawFood = true; // Ăn sống bị ngộ độc
        rawMeat.canSpoil = true;
        rawMeat.spoilTimeSeconds = 120f;
        rawMeat.spoiledItemResult = spoiledFood;
        EditorUtility.SetDirty(rawMeat);

        ItemData cookedMeat = GetOrCreateItem("Item_CookedMeat", "Thịt Nướng Chín", ItemType.Food, true, 20, true, 60f, 0f, 25f, 0.5f);

        // Herbs & Culinary Ingredients
        ItemData herbalPlant = GetOrCreateItem("Item_HerbalPlant", "Thảo Dược Rừng", ItemType.Consumable, true, 30, true, 5f, 10f, 15f, 0.1f);
        ItemData mushroom = GetOrCreateItem("Item_ForestMushroom", "Nấm Rừng tươi", ItemType.Food, true, 30, true, 15f, 5f, 5f, 0.1f);

        // Advanced Culinary Dishes (Súp & Trà)
        ItemData heartyStew = GetOrCreateItem("Item_HeartyStew", "Thịt Hầm Thảo Mộc", ItemType.Consumable, true, 10, true, 75f, 20f, 45f, 0.8f);
        ItemData detoxTea = GetOrCreateItem("Item_DetoxTea", "Trà Thảo Dược Giải Độc", ItemType.Consumable, true, 10, true, 0f, 55f, 30f, 0.4f);

        // Cooking Pot Fixture Item
        ItemData cookingPotItem = GetOrCreateItem("Item_CookingPot", "Nồi Nấu Ăn Kim Loại", ItemType.Tool, false, 1, false, 0, 0, 0, 4.0f);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 3. Setup Manager Spoilage
        GameObject managerObj = GameObject.Find("GameManagers");
        if (managerObj == null) managerObj = new GameObject("GameManagers");
        if (managerObj.GetComponent<FoodSpoilageManager>() == null)
        {
            managerObj.AddComponent<FoodSpoilageManager>();
        }

        // 4. Tìm vị trí Player để đặt các điểm mẫu
        Vector3 centerPos = Vector3.zero;
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null && Camera.main != null) player = Camera.main.gameObject;
        if (player != null) centerPos = player.transform.position;

        GameObject container = GameObject.Find("HarvestSystem_Container");
        if (container == null)
        {
            container = new GameObject("HarvestSystem_Container");
            Undo.RegisterCreatedObjectUndo(container, "Create HarvestSystem_Container");
        }

        // 🌳 Cây Gỗ Mẫu
        CreateResourceNode(container.transform, "ResourceNode_Tree_Sample", centerPos + new Vector3(3f, 0f, 4f),
            ResourceType.Tree, "Cây Gỗ Rừng", 3, woodItem, 2, 4, 90f, PrimitiveType.Cylinder, new Vector3(0.8f, 3.5f, 0.8f), new Color(0.4f, 0.25f, 0.1f));

        // 🍓 Bụi Quả Rừng
        CreateResourceNode(container.transform, "ResourceNode_BerryBush_Sample", centerPos + new Vector3(-3f, 0f, 3f),
            ResourceType.BerryBush, "Bụi Quả Rừng", 1, wildBerry, 1, 3, 45f, PrimitiveType.Sphere, new Vector3(1.2f, 1.2f, 1.2f), new Color(0.8f, 0.1f, 0.2f));

        // 💧 Bể Tích Trữ Nước Suối (Water Collector & Water Bottle Refill)
        CreateWaterCollector(container.transform, "WaterCollector_Sample", centerPos + new Vector3(0f, 0f, 5f), emptyBottle, fullWaterBottle);

        // 🌾 Ô Đất Trồng Trọt (Farming Plot)
        CreateFarmingPlot(container.transform, "FarmingPlot_Sample", centerPos + new Vector3(-4f, 0f, -2f), berrySeed, wildBerry);

        // 🔥 Đống Lửa Trại (Campfire) + 🍲 Nồi Nấu Ăn (Cooking Pot)
        Campfire fireComp = CreateCampfire(container.transform, "Campfire_Sample", centerPos + new Vector3(2f, 0f, -3f), woodItem, rawMeat, cookedMeat);
        CreateCookingPot(container.transform, "CookingPot_Sample", centerPos + new Vector3(2f, 0.6f, -3f), fireComp, fullWaterBottle, rawMeat, herbalPlant, mushroom, heartyStew, detoxTea);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log("<b>[Forest Between Us] SUCCESS!</b> Configured Full Survival, Culinary Cooking & Recipe Book Integration!");
        EditorUtility.DisplayDialog("Thành Công!",
            "Đã cài đặt thành công Trọn Bộ Hệ Thống Nấu Ăn & Ẩm Thực Sinh Tồn:\n\n" +
            "1. 🥩 Thịt Sống: Ăn trực tiếp bị tác dụng phụ ngộ độc (-25 Máu / -20 Thể Lực).\n" +
            "2. 🍲 Nồi Nấu Ăn (Cooking Pot): Đặt trên đống lửa trại để mở khóa các công thức hầm súp cao cấp.\n" +
            "3. 📖 Thư Viện (Phím L): Đã cập nhật công thức Nồi Nấu, Súp Nấm (+40 Đói), Thịt Hầm Thảo Mộc (+75 Đói), Trà Giải Độc (+50 Khát).\n" +
            "4. 🎒 Giới Hạn Trọng Lượng Balo (30kg) & Thực Phẩm Thối Rữa (Food Spoilage).\n\n" +
            "Tất cả các node mẫu (Nồi Nấu, Lửa Trại, Bể Nước, Ô Đất) đã sẵn sàng trong Scene!", "OK");
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

    static void CreateResourceNode(Transform parent, string goName, Vector3 pos, ResourceType type, string rName, int maxHits, ItemData dropItem, int minDrop, int maxDrop, float respawnTime, PrimitiveType primType, Vector3 scale, Color color)
    {
        GameObject nodeGO = GameObject.Find(goName);
        if (nodeGO == null)
        {
            nodeGO = GameObject.CreatePrimitive(primType);
            nodeGO.name = goName;
            Undo.RegisterCreatedObjectUndo(nodeGO, $"Create {goName}");
        }

        nodeGO.transform.SetParent(parent);
        nodeGO.transform.position = pos;
        nodeGO.transform.localScale = scale;

        Renderer rend = nodeGO.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.sharedMaterial = new Material(Shader.Find("Standard"));
            rend.sharedMaterial.color = color;
        }

        ResourceNode resNode = nodeGO.GetComponent<ResourceNode>();
        if (resNode == null) resNode = nodeGO.AddComponent<ResourceNode>();

        resNode.resourceType = type;
        resNode.resourceName = rName;
        resNode.maxHits = maxHits;
        resNode.currentHits = maxHits;
        resNode.dropItemData = dropItem;
        resNode.dropAmountMin = minDrop;
        resNode.dropAmountMax = maxDrop;
        resNode.autoRespawn = true;
        resNode.respawnTimeSeconds = respawnTime;
        resNode.directToInventory = true;

        EditorUtility.SetDirty(resNode);
    }

    static void CreateWaterCollector(Transform parent, string goName, Vector3 pos, ItemData emptyBottle, ItemData fullBottle)
    {
        GameObject go = GameObject.Find(goName);
        if (go == null)
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = goName;
            Undo.RegisterCreatedObjectUndo(go, $"Create {goName}");
        }
        go.transform.SetParent(parent);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(1.8f, 0.3f, 1.8f);

        Renderer rend = go.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.sharedMaterial = new Material(Shader.Find("Standard"));
            rend.sharedMaterial.color = new Color(0.1f, 0.6f, 1.0f);
        }

        WaterCollector collector = go.GetComponent<WaterCollector>();
        if (collector == null) collector = go.AddComponent<WaterCollector>();

        collector.collectorName = "Bể Tích Trữ Nước Suối";
        collector.currentWaterUnits = 10;
        collector.emptyBottleItem = emptyBottle;
        collector.fullBottleItem = fullBottle;
        EditorUtility.SetDirty(collector);
    }

    static void CreateFarmingPlot(Transform parent, string goName, Vector3 pos, ItemData seed, ItemData result)
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
        go.transform.localScale = new Vector3(2.5f, 0.2f, 2.5f);

        Renderer rend = go.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.sharedMaterial = new Material(Shader.Find("Standard"));
            rend.sharedMaterial.color = new Color(0.35f, 0.2f, 0.1f);
        }

        FarmingPlot plot = go.GetComponent<FarmingPlot>();
        if (plot == null) plot = go.AddComponent<FarmingPlot>();

        plot.plotName = "Ô Đất Trồng Trọt Rừng";
        plot.seedItem = seed;
        plot.harvestResultItem = result;
        plot.growthDurationSeconds = 30f;
        EditorUtility.SetDirty(plot);
    }

    static Campfire CreateCampfire(Transform parent, string goName, Vector3 pos, ItemData wood, ItemData rawFood, ItemData cookedFood)
    {
        GameObject go = GameObject.Find(goName);
        if (go == null)
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = goName;
            Undo.RegisterCreatedObjectUndo(go, $"Create {goName}");
        }
        go.transform.SetParent(parent);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(1.2f, 0.5f, 1.2f);

        Renderer rend = go.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.sharedMaterial = new Material(Shader.Find("Standard"));
            rend.sharedMaterial.color = new Color(0.2f, 0.15f, 0.1f);
        }

        Campfire fire = go.GetComponent<Campfire>();
        if (fire == null) fire = go.AddComponent<Campfire>();

        fire.fireName = "Đống Lửa Trại Rừng";
        fire.woodFuelItem = wood;
        fire.rawFoodItem = rawFood;
        fire.cookedFoodItem = cookedFood;

        Light l = go.GetComponentInChildren<Light>();
        if (l == null)
        {
            GameObject lightGO = new GameObject("FireLight", typeof(Light));
            lightGO.transform.SetParent(go.transform, false);
            lightGO.transform.localPosition = Vector3.up * 0.5f;
            l = lightGO.GetComponent<Light>();
            l.type = LightType.Point;
            l.color = new Color(1.0f, 0.5f, 0.1f);
            l.intensity = 3.5f;
            l.range = 8f;
            l.enabled = false;
        }
        fire.fireLight = l;
        EditorUtility.SetDirty(fire);

        return fire;
    }

    static void CreateCookingPot(Transform parent, string goName, Vector3 pos, Campfire campfire, ItemData water, ItemData rawMeat, ItemData herb, ItemData mushroom, ItemData stew, ItemData tea)
    {
        GameObject go = GameObject.Find(goName);
        if (go == null)
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = goName;
            Undo.RegisterCreatedObjectUndo(go, $"Create {goName}");
        }
        go.transform.SetParent(parent);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(0.8f, 0.4f, 0.8f);

        Renderer rend = go.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.sharedMaterial = new Material(Shader.Find("Standard"));
            rend.sharedMaterial.color = new Color(0.15f, 0.15f, 0.18f); // Dark Iron Pot
        }

        CookingPot pot = go.GetComponent<CookingPot>();
        if (pot == null) pot = go.AddComponent<CookingPot>();

        pot.potName = "Nồi Nấu Ăn Kim Loại";
        pot.parentCampfire = campfire;
        pot.cleanWaterItem = water;
        pot.rawMeatItem = rawMeat;
        pot.herbalItem = herb;
        pot.mushroomItem = mushroom;
        pot.heartyStewItem = stew;
        pot.detoxTeaItem = tea;

        EditorUtility.SetDirty(pot);
    }
}
