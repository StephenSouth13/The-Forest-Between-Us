using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New_Recipe", menuName = "Forest Between Us/Crafting Recipe")]
public class RecipeData : ScriptableObject
{
    [Header("Recipe Info")]
    public string recipeName;
    public Sprite recipeIcon;
    [TextArea(2, 5)]
    public string description;

    [Header("Crafting Requirements")]
    public List<Ingredient> ingredients = new List<Ingredient>();

    [Header("Result")]
    public ItemData resultItem;
    public int resultAmount = 1;
    public float craftTimeSeconds = 2f;
}

[System.Serializable]
public class Ingredient
{
    public ItemData item;
    public int amount = 1;
}
