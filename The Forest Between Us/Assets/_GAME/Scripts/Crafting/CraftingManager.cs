using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager instance;

    [Header("Available Recipes")]
    public List<RecipeData> knownRecipes = new List<RecipeData>();

    public event Action<RecipeData> OnCraftStarted;
    public event Action<RecipeData> OnCraftCompleted;
    public event Action<string> OnCraftFailed;

    private bool isCrafting;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public bool CanCraft(RecipeData recipe)
    {
        if (recipe == null || recipe.resultItem == null) return false;
        if (isCrafting) return false;

        if (InventoryManager.instance == null)
        {
            Debug.LogWarning("InventoryManager missing. Cannot check recipe ingredients.");
            return false;
        }

        foreach (Ingredient ing in recipe.ingredients)
        {
            if (ing.item != null && !InventoryManager.instance.HasItem(ing.item, ing.amount))
            {
                return false;
            }
        }

        return true;
    }

    public void StartCrafting(RecipeData recipe)
    {
        if (isCrafting)
        {
            OnCraftFailed?.Invoke("Already crafting an item!");
            return;
        }

        if (!CanCraft(recipe))
        {
            OnCraftFailed?.Invoke("Missing required ingredients!");
            return;
        }

        StartCoroutine(CraftRoutine(recipe));
    }

    private IEnumerator CraftRoutine(RecipeData recipe)
    {
        isCrafting = true;
        OnCraftStarted?.Invoke(recipe);

        // Deduct ingredients upfront
        foreach (Ingredient ing in recipe.ingredients)
        {
            if (ing.item != null)
            {
                InventoryManager.instance.RemoveItem(ing.item, ing.amount);
            }
        }

        float duration = Mathf.Max(0.1f, recipe.craftTimeSeconds);
        yield return new WaitForSeconds(duration);

        // Add result item
        InventoryManager.instance.PickUpItem(recipe.resultItem, recipe.resultAmount);
        
        isCrafting = false;
        OnCraftCompleted?.Invoke(recipe);

        // Advance quest if relevant
        if (QuestManager.instance != null)
        {
            QuestManager.instance.AdvanceStep(StepType.Collect, recipe.resultAmount);
        }

        Debug.Log($"Successfully crafted: {recipe.recipeName} x{recipe.resultAmount}");
    }

    public bool IsCrafting() => isCrafting;
}
