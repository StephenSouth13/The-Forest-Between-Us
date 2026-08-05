#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public static class ItemAndRecipeGeneratorTool
{
    private const string ITEM_PATH = "Assets/_GAME/Data/Items";
    private const string RECIPE_PATH = "Assets/_GAME/Data/Recipes";

    [MenuItem("Tools/Forest Between Us/Generate Items & Recipes")]
    public static void GenerateItemsAndRecipes()
    {
        EnsureDirectories();

        // 1. Tạo Items
        ItemData rawMeat = CreateItem("Raw_Meat", "Thịt Sống", ItemType.Food, true, true, 10f, 0f, 0f, 0f);
        ItemData cookedMeat = CreateItem("Cooked_Meat", "Thịt Nướng", ItemType.Food, true, false, 40f, 0f, 20f, 10f);
        ItemData wildVegetable = CreateItem("Wild_Vegetable", "Rau Rừng", ItemType.Food, true, true, 15f, 5f, 5f, 0f);
        ItemData grilledVegetable = CreateItem("Grilled_Vegetable", "Rau Xào", ItemType.Food, true, false, 30f, 5f, 10f, 0f);
        ItemData waterBottle = CreateItem("Water_Bottle", "Bình Nước Suối", ItemType.Consumable, true, false, 0f, 50f, 5f, 15f);
        ItemData healthPotion = CreateItem("Super_Health_Potion", "Thuốc Hồi Huyết", ItemType.Consumable, true, false, 0f, 10f, 100f, 20f);
        ItemData energyDrink = CreateItem("Super_Energy_Drink", "Nước Tăng Lực", ItemType.Consumable, true, false, 0f, 20f, 10f, 100f);
        ItemData seashell = CreateItem("Item_Seashell", "Vỏ Sò Biển (Tiền Tệ)", ItemType.Resource, false, false, 0f, 0f, 0f, 0f);
        
        ItemData ironSword = CreateItem("Iron_Sword", "Kiếm Sắt", ItemType.Weapon, false, false, 0f, 0f, 0f, 0f);
        ItemData leatherArmor = CreateItem("Leather_Armor", "Giáp Da", ItemType.Tool, false, false, 0f, 0f, 0f, 0f);

        // 2. Tạo Recipes
        CreateRecipe("Recipe_Cooked_Meat", "Thịt Nướng", "Nướng thịt trên ngọn lửa hồng.", cookedMeat, 1, 3f, 
            new Ingredient { item = rawMeat, amount = 1 });

        CreateRecipe("Recipe_Grilled_Vegetable", "Rau Xào", "Rau xào đơn giản giúp ấm bụng.", grilledVegetable, 1, 2f, 
            new Ingredient { item = wildVegetable, amount = 1 });

        CreateRecipe("Recipe_Health_Potion", "Thuốc Hồi Huyết", "Thuốc chiết xuất từ rau rừng và nước suối, hồi lượng lớn máu.", healthPotion, 1, 5f, 
            new Ingredient { item = wildVegetable, amount = 1 },
            new Ingredient { item = waterBottle, amount = 1 });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("🎉 Đã tạo xong toàn bộ Vật phẩm & Công thức chế tạo tại Assets/_GAME/Data/");
    }

    private static void EnsureDirectories()
    {
        if (!Directory.Exists(ITEM_PATH)) Directory.CreateDirectory(ITEM_PATH);
        if (!Directory.Exists(RECIPE_PATH)) Directory.CreateDirectory(RECIPE_PATH);
    }

    private static ItemData CreateItem(string fileName, string itemName, ItemType type, bool consumable, bool raw, float hunger, float thirst, float health, float stamina)
    {
        string path = $"{ITEM_PATH}/{fileName}.asset";
        ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
        
        if (item == null)
        {
            item = ScriptableObject.CreateInstance<ItemData>();
            AssetDatabase.CreateAsset(item, path);
        }

        item.itemName = itemName;
        item.category = type;
        item.isConsumable = consumable;
        item.isRawFood = raw;
        item.hungerRestore = hunger;
        item.thirstRestore = thirst;
        item.healthRestore = health;
        item.staminaRestore = stamina;

        EditorUtility.SetDirty(item);
        return item;
    }

    private static void CreateRecipe(string fileName, string recipeName, string desc, ItemData result, int resultAmt, float craftTime, params Ingredient[] ingredients)
    {
        string path = $"{RECIPE_PATH}/{fileName}.asset";
        RecipeData recipe = AssetDatabase.LoadAssetAtPath<RecipeData>(path);
        
        if (recipe == null)
        {
            recipe = ScriptableObject.CreateInstance<RecipeData>();
            AssetDatabase.CreateAsset(recipe, path);
        }

        recipe.recipeName = recipeName;
        recipe.description = desc;
        recipe.resultItem = result;
        recipe.resultAmount = resultAmt;
        recipe.craftTimeSeconds = craftTime;
        
        recipe.ingredients.Clear();
        recipe.ingredients.AddRange(ingredients);

        EditorUtility.SetDirty(recipe);
    }
}
#endif
