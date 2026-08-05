using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingUIController : MonoBehaviour
{
    public static CraftingUIController instance;

    [Header("UI Panels")]
    public GameObject craftingPanel;
    public Transform recipeContainer;
    
    [Header("Selected Recipe Display")]
    public TextMeshProUGUI recipeTitleText;
    public TextMeshProUGUI recipeDescText;
    public Image recipeIconImage;
    public TextMeshProUGUI ingredientsText;
    public Button craftButton;

    [Header("Key Bindings")]
    public KeyCode toggleKey = KeyCode.K;

    private RecipeData selectedRecipe;
    private List<GameObject> activeButtons = new List<GameObject>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (craftingPanel == null) craftingPanel = CreateNotebookPanel();
        if (craftingPanel != null) craftingPanel.SetActive(false);

        if (CraftingManager.instance != null)
        {
            CraftingManager.instance.OnCraftStarted += OnCraftStarted;
            CraftingManager.instance.OnCraftCompleted += OnCraftCompleted;
        }

        if (craftButton != null) craftButton.onClick.AddListener(OnCraftButtonClicked);

        PopulateRecipeList();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) ToggleCraftingWindow();
    }

    public void ToggleCraftingWindow()
    {
        if (craftingPanel == null) return;
        bool active = !craftingPanel.activeSelf;
        craftingPanel.SetActive(active);

        if (active)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PopulateRecipeList();
            UpdateDetailsPanel();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void PopulateRecipeList()
    {
        if (recipeContainer == null || CraftingManager.instance == null) return;

        foreach (var b in activeButtons) Destroy(b);
        activeButtons.Clear();

        foreach (RecipeData recipe in CraftingManager.instance.knownRecipes)
        {
            GameObject btnObj = new GameObject("RecipeBtn", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(recipeContainer, false);
            activeButtons.Add(btnObj);

            RectTransform rt = btnObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(300, 40);
            btnObj.GetComponent<Image>().color = new Color(0.8f, 0.7f, 0.5f); // Màu giấy cũ

            GameObject txtGO = new GameObject("Txt", typeof(RectTransform), typeof(TextMeshProUGUI));
            txtGO.transform.SetParent(btnObj.transform, false);
            RectTransform txtRT = txtGO.GetComponent<RectTransform>();
            txtRT.anchorMin = Vector2.zero; txtRT.anchorMax = Vector2.one;
            txtRT.offsetMin = new Vector2(10, 0); txtRT.offsetMax = Vector2.zero;

            TextMeshProUGUI txt = txtGO.GetComponent<TextMeshProUGUI>();
            txt.text = recipe.recipeName;
            txt.color = Color.black;
            txt.alignment = TextAlignmentOptions.Left;
            txt.fontSize = 16f;

            Button btn = btnObj.GetComponent<Button>();
            RecipeData rData = recipe;
            btn.onClick.AddListener(() => SelectRecipe(rData));
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
            if (recipeTitleText != null) recipeTitleText.text = "Chọn một công thức...";
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
            string ingStr = "<b>Nguyên Liệu Cần Thiết:</b>\n";
            foreach (Ingredient ing in selectedRecipe.ingredients)
            {
                if (ing.item == null) continue;
                int currentAmt = (InventoryManager.instance != null) ? InventoryManager.instance.GetItemCount(ing.item) : 0;
                bool hasEnough = currentAmt >= ing.amount;
                string colorHex = hasEnough ? "#006400" : "#8B0000"; // Xanh đậm, Đỏ đậm cho nền giấy
                ingStr += $"<color={colorHex}>• {ing.item.itemName}: {currentAmt}/{ing.amount}</color>\n";
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

    GameObject CreateNotebookPanel()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return null;

        // Main Panel
        GameObject p = new GameObject("Notebook_Panel(Runtime)", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        p.transform.SetParent(canvas.transform, false);
        RectTransform rect = p.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.15f, 0.15f);
        rect.anchorMax = new Vector2(0.85f, 0.85f);
        rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
        p.GetComponent<Image>().color = new Color(0.9f, 0.85f, 0.7f, 1f); // Màu giấy da

        // Left Page (Danh sách)
        GameObject leftPage = new GameObject("LeftPage", typeof(RectTransform));
        leftPage.transform.SetParent(p.transform, false);
        RectTransform leftRect = leftPage.GetComponent<RectTransform>();
        leftRect.anchorMin = new Vector2(0.02f, 0.05f); leftRect.anchorMax = new Vector2(0.48f, 0.95f);
        leftRect.offsetMin = Vector2.zero; leftRect.offsetMax = Vector2.zero;

        GameObject titleGO = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
        titleGO.transform.SetParent(leftPage.transform, false);
        RectTransform tr = titleGO.GetComponent<RectTransform>();
        tr.anchorMin = new Vector2(0, 0.9f); tr.anchorMax = new Vector2(1, 1);
        tr.offsetMin = Vector2.zero; tr.offsetMax = Vector2.zero;
        TextMeshProUGUI titleTxt = titleGO.GetComponent<TextMeshProUGUI>();
        titleTxt.text = "📖 SỔ TAY CÔNG THỨC"; titleTxt.fontSize = 24; titleTxt.color = new Color(0.2f, 0.1f, 0f); titleTxt.fontStyle = FontStyles.Bold; titleTxt.alignment = TextAlignmentOptions.Center;

        GameObject scrollGO = new GameObject("ScrollContainer", typeof(RectTransform));
        scrollGO.transform.SetParent(leftPage.transform, false);
        RectTransform sRect = scrollGO.GetComponent<RectTransform>();
        sRect.anchorMin = new Vector2(0, 0); sRect.anchorMax = new Vector2(1, 0.85f);
        sRect.offsetMin = Vector2.zero; sRect.offsetMax = Vector2.zero;
        VerticalLayoutGroup vlg = scrollGO.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 5; vlg.childControlHeight = false; vlg.childControlWidth = true;
        recipeContainer = scrollGO.transform;

        // Right Page (Chi tiết)
        GameObject rightPage = new GameObject("RightPage", typeof(RectTransform));
        rightPage.transform.SetParent(p.transform, false);
        RectTransform rightRect = rightPage.GetComponent<RectTransform>();
        rightRect.anchorMin = new Vector2(0.52f, 0.05f); rightRect.anchorMax = new Vector2(0.98f, 0.95f);
        rightRect.offsetMin = Vector2.zero; rightRect.offsetMax = Vector2.zero;

        // Line Seperator
        GameObject line = new GameObject("Line", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        line.transform.SetParent(p.transform, false);
        RectTransform lineR = line.GetComponent<RectTransform>();
        lineR.anchorMin = new Vector2(0.5f, 0.05f); lineR.anchorMax = new Vector2(0.505f, 0.95f);
        lineR.offsetMin = Vector2.zero; lineR.offsetMax = Vector2.zero;
        line.GetComponent<Image>().color = new Color(0.3f, 0.2f, 0.1f, 0.5f);

        // Right Page Content
        GameObject rTitleGO = new GameObject("RecipeTitle", typeof(RectTransform), typeof(TextMeshProUGUI));
        rTitleGO.transform.SetParent(rightPage.transform, false);
        RectTransform rtRect = rTitleGO.GetComponent<RectTransform>();
        rtRect.anchorMin = new Vector2(0, 0.8f); rtRect.anchorMax = new Vector2(1, 0.95f);
        rtRect.offsetMin = Vector2.zero; rtRect.offsetMax = Vector2.zero;
        recipeTitleText = rTitleGO.GetComponent<TextMeshProUGUI>();
        recipeTitleText.text = "Chọn một công thức..."; recipeTitleText.fontSize = 22; recipeTitleText.color = Color.black; recipeTitleText.fontStyle = FontStyles.Bold; recipeTitleText.alignment = TextAlignmentOptions.Center;

        GameObject rDescGO = new GameObject("RecipeDesc", typeof(RectTransform), typeof(TextMeshProUGUI));
        rDescGO.transform.SetParent(rightPage.transform, false);
        RectTransform rdRect = rDescGO.GetComponent<RectTransform>();
        rdRect.anchorMin = new Vector2(0, 0.6f); rdRect.anchorMax = new Vector2(1, 0.8f);
        rdRect.offsetMin = Vector2.zero; rdRect.offsetMax = Vector2.zero;
        recipeDescText = rDescGO.GetComponent<TextMeshProUGUI>();
        recipeDescText.text = ""; recipeDescText.fontSize = 16; recipeDescText.color = new Color(0.2f, 0.2f, 0.2f); recipeDescText.textWrappingMode = TextWrappingModes.Normal;

        GameObject rIngGO = new GameObject("Ingredients", typeof(RectTransform), typeof(TextMeshProUGUI));
        rIngGO.transform.SetParent(rightPage.transform, false);
        RectTransform riRect = rIngGO.GetComponent<RectTransform>();
        riRect.anchorMin = new Vector2(0, 0.2f); riRect.anchorMax = new Vector2(1, 0.6f);
        riRect.offsetMin = Vector2.zero; riRect.offsetMax = Vector2.zero;
        ingredientsText = rIngGO.GetComponent<TextMeshProUGUI>();
        ingredientsText.text = ""; ingredientsText.fontSize = 18; ingredientsText.color = Color.black;

        // Craft Button
        GameObject cBtnGO = new GameObject("CraftBtn", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        cBtnGO.transform.SetParent(rightPage.transform, false);
        RectTransform cbRect = cBtnGO.GetComponent<RectTransform>();
        cbRect.anchorMin = new Vector2(0.2f, 0.05f); cbRect.anchorMax = new Vector2(0.8f, 0.15f);
        cbRect.offsetMin = Vector2.zero; cbRect.offsetMax = Vector2.zero;
        cBtnGO.GetComponent<Image>().color = new Color(0.2f, 0.6f, 0.3f);
        craftButton = cBtnGO.GetComponent<Button>();

        GameObject cTxtGO = new GameObject("Txt", typeof(RectTransform), typeof(TextMeshProUGUI));
        cTxtGO.transform.SetParent(cBtnGO.transform, false);
        RectTransform ctRect = cTxtGO.GetComponent<RectTransform>();
        ctRect.anchorMin = Vector2.zero; ctRect.anchorMax = Vector2.one;
        ctRect.offsetMin = Vector2.zero; ctRect.offsetMax = Vector2.zero;
        TextMeshProUGUI ctTxt = cTxtGO.GetComponent<TextMeshProUGUI>();
        ctTxt.text = "CHẾ TẠO"; ctTxt.fontSize = 20; ctTxt.color = Color.white; ctTxt.alignment = TextAlignmentOptions.Center; ctTxt.fontStyle = FontStyles.Bold;

        return p;
    }
}
