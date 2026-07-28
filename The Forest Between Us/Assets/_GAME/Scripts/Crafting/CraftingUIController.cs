using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingUIController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject craftingPanel;
    public Transform recipeContainer;
    public GameObject recipeButtonPrefab;

    [Header("Selected Recipe Display")]
    public TextMeshProUGUI recipeTitleText;
    public TextMeshProUGUI recipeDescText;
    public Image recipeIconImage;
    public TextMeshProUGUI ingredientsText;
    public Button craftButton;
    public Image craftProgressBar;

    [Header("Key Bindings")]
    public KeyCode toggleKey = KeyCode.K;

    private RecipeData selectedRecipe;

    void Start()
    {
        if (craftingPanel != null) craftingPanel.SetActive(false);

        if (CraftingManager.instance != null)
        {
            CraftingManager.instance.OnCraftStarted += OnCraftStarted;
            CraftingManager.instance.OnCraftCompleted += OnCraftCompleted;
        }

        if (craftButton != null)
        {
            craftButton.onClick.AddListener(OnCraftButtonClicked);
        }

        PopulateRecipeList();
    }

    void OnDestroy()
    {
        if (CraftingManager.instance != null)
        {
            CraftingManager.instance.OnCraftStarted -= OnCraftStarted;
            CraftingManager.instance.OnCraftCompleted -= OnCraftCompleted;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleCraftingWindow();
        }
    }

    public void ToggleCraftingWindow()
    {
        if (craftingPanel == null) return;

        bool active = !craftingPanel.activeSelf;
        craftingPanel.SetActive(active);

        if (active)
        {
            PopulateRecipeList();
            UpdateDetailsPanel();
        }
    }

    public void PopulateRecipeList()
    {
        if (recipeContainer == null || recipeButtonPrefab == null || CraftingManager.instance == null) return;

        foreach (Transform child in recipeContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (RecipeData recipe in CraftingManager.instance.knownRecipes)
        {
            GameObject btnObj = Instantiate(recipeButtonPrefab, recipeContainer);
            Button btn = btnObj.GetComponent<Button>();
            TextMeshProUGUI txt = btnObj.GetComponentInChildren<TextMeshProUGUI>();

            if (txt != null) txt.text = recipe.recipeName;

            RecipeData rData = recipe;
            if (btn != null)
            {
                btn.onClick.AddListener(() => SelectRecipe(rData));
            }
        }
    }

    public void SelectRecipe(RecipeData recipe)
    {
        selectedRecipe = recipe;
        UpdateDetailsPanel();
    }

    public void UpdateDetailsPanel()
    {
        if (selectedRecipe == null)
        {
            if (recipeTitleText != null) recipeTitleText.text = "Select a Recipe";
            if (recipeDescText != null) recipeDescText.text = "";
            if (ingredientsText != null) ingredientsText.text = "";
            if (craftButton != null) craftButton.interactable = false;
            return;
        }

        if (recipeTitleText != null) recipeTitleText.text = selectedRecipe.recipeName;
        if (recipeDescText != null) recipeDescText.text = selectedRecipe.description;
        if (recipeIconImage != null && selectedRecipe.recipeIcon != null)
        {
            recipeIconImage.sprite = selectedRecipe.recipeIcon;
            recipeIconImage.gameObject.SetActive(true);
        }

        if (ingredientsText != null)
        {
            string ingStr = "<b>Ingredients:</b>\n";
            foreach (Ingredient ing in selectedRecipe.ingredients)
            {
                if (ing.item == null) continue;
                bool hasEnough = InventoryManager.instance != null && InventoryManager.instance.HasItem(ing.item, ing.amount);
                string colorHex = hasEnough ? "#00FF00" : "#FF0000";
                ingStr += $"<color={colorHex}>• {ing.item.itemName}: x{ing.amount}</color>\n";
            }
            ingredientsText.text = ingStr;
        }

        if (craftButton != null)
        {
            bool canCraft = CraftingManager.instance != null && CraftingManager.instance.CanCraft(selectedRecipe);
            craftButton.interactable = canCraft;
        }
    }

    private void OnCraftButtonClicked()
    {
        if (selectedRecipe != null && CraftingManager.instance != null)
        {
            CraftingManager.instance.StartCrafting(selectedRecipe);
        }
    }

    private void OnCraftStarted(RecipeData recipe)
    {
        if (craftButton != null) craftButton.interactable = false;
    }

    private void OnCraftCompleted(RecipeData recipe)
    {
        UpdateDetailsPanel();
    }
}
