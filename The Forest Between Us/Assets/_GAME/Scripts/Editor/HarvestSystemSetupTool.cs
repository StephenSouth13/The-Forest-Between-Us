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
        wildBerry.spoilTimeSeconds = 180f;
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

        ItemData rawMeat = GetOrCreateItem("Item_RawMeat", "Thịt Sống", ItemType.Food, true, 20, true, 15f, 0f, -25f, 0.5f);
        rawMeat.isRawFood = true;
        rawMeat.canSpoil = true;
        rawMeat.spoilTimeSeconds = 120f;
        rawMeat.spoiledItemResult = spoiledFood;
        EditorUtility.SetDirty(rawMeat);

        ItemData cookedMeat = GetOrCreateItem("Item_CookedMeat", "Thịt Nướng Chín", ItemType.Food, true, 20, true, 60f, 0f, 25f, 0.5f);

        ItemData herbalPlant = GetOrCreateItem("Item_HerbalPlant", "Thảo Dược Rừng", ItemType.Consumable, true, 30, true, 5f, 10f, 15f, 0.1f);
        ItemData mushroom = GetOrCreateItem("Item_ForestMushroom", "Nấm Rừng Tươi", ItemType.Food, true, 30, true, 15f, 5f, 5f, 0.1f);

        ItemData heartyStew = GetOrCreateItem("Item_HeartyStew", "Thịt Hầm Thảo Mộc", ItemType.Consumable, true, 10, true, 75f, 20f, 45f, 0.8f);
        ItemData detoxTea = GetOrCreateItem("Item_DetoxTea", "Trà Cảm Cúm Gừng & Thảo Dược", ItemType.Consumable, true, 10, true, 0f, 55f, 30f, 0.4f);

        // 💊 Thuốc Kháng Sinh & Thuốc Chống Muỗi
        ItemData diyAntibiotic = GetOrCreateItem("Item_DIYAntibiotic", "Thuốc Kháng Sinh Tự Chế", ItemType.Consumable, true, 10, true, 0f, 10f, 35f, 0.2f);
        ItemData mosquitoRepellent = GetOrCreateItem("Item_MosquitoRepellent", "Thuốc Bôi Chống Muỗi", ItemType.Consumable, true, 5, true, 0f, 0f, 0f, 0.3f);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 3. Setup Managers
        GameObject managerObj = GameObject.Find("GameManagers");
        if (managerObj == null) managerObj = new GameObject("GameManagers");
        if (managerObj.GetComponent<FoodSpoilageManager>() == null) managerObj.AddComponent<FoodSpoilageManager>();
        if (managerObj.GetComponent<PlayerDiseaseManager>() == null) managerObj.AddComponent<PlayerDiseaseManager>();

        // 4. Tìm vị trí Player
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

        // 💧 Bể Nước Suối
        CreateWaterCollector(container.transform, "WaterCollector_Sample", centerPos + new Vector3(0f, 0f, 5f), emptyBottle, fullWaterBottle);

        // 🌾 Ô Đất Trồng Trọt
        CreateFarmingPlot(container.transform, "FarmingPlot_Sample", centerPos + new Vector3(-4f, 0f, -2f), berrySeed, wildBerry);

        // 🔥 Đống Lửa Trại & 🍲 Nồi Nấu Ăn (Có Thuốc Kháng Sinh)
        Campfire fireComp = CreateCampfire(container.transform, "Campfire_Sample", centerPos + new Vector3(2f, 0f, -3f), woodItem, rawMeat, cookedMeat);
        CreateCookingPot(container.transform, "CookingPot_Sample", centerPos + new Vector3(2f, 0.6f, -3f), fireComp, fullWaterBottle, rawMeat, herbalPlant, mushroom, heartyStew, detoxTea, diyAntibiotic);

        // 🦟 Ổ Muỗi (Mosquito Zone)
        CreateMosquitoZone(container.transform, "MosquitoZone_Sample", centerPos + new Vector3(7f, 0f, 2f));

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log("<b>[Forest Between Us] SUCCESS!</b> Configured Mosquito Diseases, Cold/Flu, DIY Antibiotic Crafting & Repellent System!");
        EditorUtility.DisplayDialog("Thành Công!",
            "Đã cài đặt thành công Trọn Bộ Hệ Thống Bệnh Tật & Dược Phẩm Kháng Sinh Tự Chế:\n\n" +
            "1. 🦟 Bệnh Sốt Rét Do Muỗi Đốt (Mosquito Disease): Gặp ổ muỗi không bôi thuốc sẽ bị sốt rét, trừ máu (-1.2 HP/s) & kiệt sức.\n" +
            "2. 🤒 Bệnh Cảm Cúm (Flu / Cold): Bị dầm mưa / sương đêm sẽ ho tiếng ho audio & mất máu liên tục.\n" +
            "3. 💊 Thuốc Kháng Sinh Tự Chế (DIY Antibiotic): Nấu 2x Thảo Dược + 1x Nấm Rừng + 1x Nước Suối trong Nồi Nấu để ĐẶC TRỊ KHỎI SỐT RÉT!\n" +
            "4. 🍵 Trà Cảm Cúm Gừng & Thảo Dược: Nấu đun sôi trong Nồi Nấu để CHỮA KHỎI CẢM CÚM!\n" +
            "5. 🧴 Thuốc Bôi Chống Muỗi (Repellent): Bôi lên người để kháng muỗi đốt trong 5 phút.\n\n" +
            "Ổ Muỗi Mẫu 'MosquitoZone_Sample' đã được đặt tại vùng đầm lầy cạnh nhân vật!", "OK");
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

    static void CreateCookingPot(Transform parent, string goName, Vector3 pos, Campfire campfire, ItemData water, ItemData rawMeat, ItemData herb, ItemData mushroom, ItemData stew, ItemData tea, ItemData antibiotic)
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
            rend.sharedMaterial.color = new Color(0.15f, 0.15f, 0.18f);
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
        pot.antibioticItem = antibiotic;

        EditorUtility.SetDirty(pot);
    }

    static void CreateMosquitoZone(Transform parent, string goName, Vector3 pos)
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
        go.transform.localScale = new Vector3(8f, 4f, 8f);

        SphereCollider col = go.GetComponent<SphereCollider>();
        if (col != null) col.isTrigger = true;

        Renderer rend = go.GetComponent<Renderer>();
        if (rend != null)
        {
            // Tắt hiển thị MeshRenderer của Vùng Muỗi để tránh quả cầu tím khổng lồ che mắt
            rend.enabled = false;
        }

        MosquitoZone mZone = go.GetComponent<MosquitoZone>();
        if (mZone == null) mZone = go.AddComponent<MosquitoZone>();

        mZone.zoneName = "Vùng Đầm Lầy Sương Ẩm (Ổ Muỗi)";
        mZone.biteIntervalSeconds = 4f;

        EditorUtility.SetDirty(mZone);
    }
}
