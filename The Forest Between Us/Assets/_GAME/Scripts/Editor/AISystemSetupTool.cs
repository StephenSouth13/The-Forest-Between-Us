using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class AISystemSetupTool
{
    [MenuItem("Tools/Forest Between Us/Setup AI, Animals, Natives & Traps")]
    public static void SetupAISystem()
    {
        // 1. Tải các Item cần thiết cho AI
        ItemData berryItem = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/_GAME/Data/Items/Item_WildBerry.asset");
        ItemData rawMeat = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/_GAME/Data/Items/Item_RawMeat.asset");
        ItemData cookedMeat = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/_GAME/Data/Items/Item_CookedMeat.asset");
        ItemData detoxTea = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/_GAME/Data/Items/Item_DetoxTea.asset");

        // 2. Tìm vị trí Player
        Vector3 centerPos = Vector3.zero;
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null && Camera.main != null) player = Camera.main.gameObject;
        if (player != null) centerPos = player.transform.position;

        GameObject container = GameObject.Find("AISystem_Container");
        if (container == null)
        {
            container = new GameObject("AISystem_Container");
            Undo.RegisterCreatedObjectUndo(container, "Create AISystem_Container");
        }

        // 🐇 Thỏ Rừng (Prey - Thuần Hóa & Chạy Trốn)
        CreateAnimal(container.transform, "Animal_Rabbit_Sample", centerPos + new Vector3(-5f, 0f, 4f),
            "Thỏ Rừng Hiền Lành", AnimalType.Prey, berryItem, rawMeat, new Color(0.9f, 0.85f, 0.7f), new Vector3(0.5f, 0.5f, 0.5f));

        // 🐗 Lợn Rừng (Predator - Rượt Đổi & Tấn Công)
        CreateAnimal(container.transform, "Animal_Boar_Sample", centerPos + new Vector3(6f, 0f, 6f),
            "Lợn Rừng Dã Thú", AnimalType.Predator, rawMeat, rawMeat, new Color(0.35f, 0.2f, 0.1f), new Vector3(1.2f, 0.9f, 1.4f));

        // 🗿 Thổ Dân K'Nu (Native NPC - Giao Tiếp & Bị Hắc Hóa)
        CreateNativeNPC(container.transform, "Native_KNu_Sample", centerPos + new Vector3(4f, 0f, -4f), detoxTea);

        // 🪤 Bẫy Săn Bắt (Animal Trap)
        CreateAnimalTrap(container.transform, "Trap_Sample", centerPos + new Vector3(-3f, 0f, 2f), rawMeat);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log("<b>[Forest Between Us] SUCCESS!</b> Configured AI Animals, Natives, Taming, Traps & Night Corruption!");
        EditorUtility.DisplayDialog("Thành Công!",
            "Đã cài đặt thành công Trọn Bộ Hệ Thống AI Sinh Thể:\n\n" +
            "1. 🐇 Thỏ Rừng (Prey): Thấy người chơi là Chạy trốn. Cho ăn Trái cây 3 lần để THUẦN HÓA làm pet đi theo bảo vệ!\n" +
            "2. 🐗 Lợn Rừng Dã Thú (Predator): Rượt đuổi & Tấn công người chơi nếu đến gần.\n" +
            "3. 🗿 Thổ Dân K'Nu (Native NPC): Ban ngày nói chuyện & truyền dạy công thức. BAN ĐÊM BỊ HẮC HÓA biến thành chiến binh bóng đêm cuồng sát!\n" +
            "4. 🪤 Bẫy Săn Bắt (Animal Trap): Động vật/Quái dẫm vào bẫy sẽ bị bắt sập bẫy!\n\n" +
            "Tất cả các sinh thể mẫu đã sẵn sàng trong 'AISystem_Container'!", "OK");
    }

    static void CreateAnimal(Transform parent, string goName, Vector3 pos, string aName, AnimalType type, ItemData favFood, ItemData dropMeat, Color color, Vector3 scale)
    {
        GameObject go = GameObject.Find(goName);
        if (go == null)
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = goName;
            Undo.RegisterCreatedObjectUndo(go, $"Create {goName}");
        }

        go.transform.SetParent(parent);
        go.transform.position = pos;
        go.transform.localScale = scale;

        Renderer rend = go.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.sharedMaterial = new Material(Shader.Find("Standard"));
            rend.sharedMaterial.color = color;
        }

        AnimalAI animal = go.GetComponent<AnimalAI>();
        if (animal == null) animal = go.AddComponent<AnimalAI>();

        animal.animalName = aName;
        animal.animalType = type;
        animal.favoriteFood = favFood;
        animal.dropMeatItem = dropMeat;
        animal.dropMeatAmount = 2;

        EditorUtility.SetDirty(animal);
    }

    static void CreateNativeNPC(Transform parent, string goName, Vector3 pos, ItemData gift)
    {
        GameObject go = GameObject.Find(goName);
        if (go == null)
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = goName;
            Undo.RegisterCreatedObjectUndo(go, $"Create {goName}");
        }

        go.transform.SetParent(parent);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(1f, 1.8f, 1.f);

        Renderer rend = go.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.sharedMaterial = new Material(Shader.Find("Standard"));
            rend.sharedMaterial.color = new Color(0.8f, 0.5f, 0.3f);
        }

        NativeNPC native = go.GetComponent<NativeNPC>();
        if (native == null) native = go.AddComponent<NativeNPC>();

        native.npcName = "Già Làng Thổ Dân K'Nu";
        native.giftItemReward = gift;

        EditorUtility.SetDirty(native);
    }

    static void CreateAnimalTrap(Transform parent, string goName, Vector3 pos, ItemData loot)
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
        go.transform.localScale = new Vector3(1.2f, 0.4f, 1.2f);

        BoxCollider col = go.GetComponent<BoxCollider>();
        if (col != null) col.isTrigger = true;

        Renderer rend = go.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.sharedMaterial = new Material(Shader.Find("Standard"));
            rend.sharedMaterial.color = new Color(0.3f, 0.3f, 0.35f);
        }

        AnimalTrap trap = go.GetComponent<AnimalTrap>();
        if (trap == null) trap = go.AddComponent<AnimalTrap>();

        trap.trapName = "Bẫy Lồng Săn Bắt";
        trap.capturedLootItem = loot;
        trap.isArmed = true;

        EditorUtility.SetDirty(trap);
    }
}
