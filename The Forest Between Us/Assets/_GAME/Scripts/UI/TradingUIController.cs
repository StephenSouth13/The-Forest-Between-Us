using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradingUIController : MonoBehaviour
{
    public static TradingUIController instance;

    public GameObject panel;
    public Transform itemsContainer;
    public TextMeshProUGUI playerCurrencyText;
    public Button closeBtn;

    private List<GameObject> activeSlots = new List<GameObject>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (panel == null) panel = CreateFallbackPanel();
        if (panel != null) panel.SetActive(false);
        if (closeBtn != null) closeBtn.onClick.AddListener(CloseStore);
    }

    public void OpenStore()
    {
        if (panel == null || GameDirector.instance == null) return;
        panel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        RefreshUI();
    }

    public void CloseStore()
    {
        if (panel != null) panel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void RefreshUI()
    {
        // Xóa slot cũ
        foreach (GameObject g in activeSlots) Destroy(g);
        activeSlots.Clear();

        // Hiển thị số dư Vỏ Sò
        int playerShells = 0;
        ItemData currency = GameDirector.instance.globalCurrencyItem;
        if (currency != null && InventoryManager.instance != null)
        {
            playerShells = InventoryManager.instance.GetItemCount(currency);
        }
        if (playerCurrencyText != null) playerCurrencyText.text = $"Số dư Vỏ Sò: {playerShells}";

        // Tạo slot mới
        foreach (var listing in GameDirector.instance.merchantInventory)
        {
            if (listing.itemToSell == null) continue;

            GameObject slot = new GameObject("TradeSlot", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            slot.transform.SetParent(itemsContainer, false);
            activeSlots.Add(slot);

            RectTransform sRect = slot.GetComponent<RectTransform>();
            sRect.sizeDelta = new Vector2(350f, 60f);
            slot.GetComponent<Image>().color = new Color(0.1f, 0.15f, 0.2f, 1f);

            // Tên Item
            GameObject txtGO = new GameObject("Txt", typeof(RectTransform), typeof(TextMeshProUGUI));
            txtGO.transform.SetParent(slot.transform, false);
            RectTransform tRect = txtGO.GetComponent<RectTransform>();
            tRect.anchorMin = new Vector2(0.05f, 0f); tRect.anchorMax = new Vector2(0.6f, 1f);
            tRect.offsetMin = Vector2.zero; tRect.offsetMax = Vector2.zero;
            TextMeshProUGUI txt = txtGO.GetComponent<TextMeshProUGUI>();
            txt.text = $"{listing.itemToSell.itemName} - Giá: {listing.priceInCurrency}";
            txt.alignment = TextAlignmentOptions.Left; txt.color = Color.white; txt.fontSize = 16f;

            // Nút Mua
            GameObject btnGO = new GameObject("BuyBtn", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnGO.transform.SetParent(slot.transform, false);
            RectTransform bRect = btnGO.GetComponent<RectTransform>();
            bRect.anchorMin = new Vector2(0.7f, 0.1f); bRect.anchorMax = new Vector2(0.95f, 0.9f);
            bRect.offsetMin = Vector2.zero; bRect.offsetMax = Vector2.zero;
            btnGO.GetComponent<Image>().color = new Color(0.2f, 0.6f, 0.3f, 1f);
            
            Button btn = btnGO.GetComponent<Button>();
            
            GameObject btTxt = new GameObject("BTxt", typeof(RectTransform), typeof(TextMeshProUGUI));
            btTxt.transform.SetParent(btnGO.transform, false);
            RectTransform btRect = btTxt.GetComponent<RectTransform>();
            btRect.anchorMin = Vector2.zero; btRect.anchorMax = Vector2.one;
            btRect.offsetMin = Vector2.zero; btRect.offsetMax = Vector2.zero;
            TextMeshProUGUI btxt = btTxt.GetComponent<TextMeshProUGUI>();
            btxt.text = "MUA"; btxt.alignment = TextAlignmentOptions.Center; btxt.color = Color.white;

            btn.onClick.AddListener(() =>
            {
                BuyItem(listing);
            });
        }
    }

    void BuyItem(GameDirector.TradeListing listing)
    {
        ItemData currency = GameDirector.instance.globalCurrencyItem;
        if (currency == null || InventoryManager.instance == null) return;

        int currentShells = InventoryManager.instance.GetItemCount(currency);
        if (currentShells >= listing.priceInCurrency)
        {
            // Trừ tiền
            InventoryManager.instance.RemoveItem(currency, listing.priceInCurrency);
            // Thêm đồ
            InventoryManager.instance.PickUpItem(listing.itemToSell, 1);
            Debug.Log($"🛒 Đã mua thành công: {listing.itemToSell.itemName} với giá {listing.priceInCurrency} vỏ sò.");
            RefreshUI();
        }
        else
        {
            Debug.LogWarning("❌ Không đủ Vỏ Sò để mua vật phẩm này!");
        }
    }

    GameObject CreateFallbackPanel()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return null;

        GameObject p = new GameObject("Trading_Panel(Runtime)", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        p.transform.SetParent(canvas.transform, false);
        RectTransform rect = p.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.2f, 0.1f); rect.anchorMax = new Vector2(0.8f, 0.9f);
        rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
        p.GetComponent<Image>().color = new Color(0.08f, 0.1f, 0.15f, 0.98f);

        // Header
        GameObject hdr = new GameObject("Header", typeof(RectTransform), typeof(TextMeshProUGUI));
        hdr.transform.SetParent(p.transform, false);
        RectTransform hRect = hdr.GetComponent<RectTransform>();
        hRect.anchorMin = new Vector2(0f, 0.9f); hRect.anchorMax = new Vector2(1f, 1f);
        hRect.offsetMin = Vector2.zero; hRect.offsetMax = Vector2.zero;
        TextMeshProUGUI hTxt = hdr.GetComponent<TextMeshProUGUI>();
        hTxt.text = "CỬA HÀNG BẢN ĐỊA"; hTxt.fontSize = 24f; hTxt.alignment = TextAlignmentOptions.Center; hTxt.color = new Color(1f, 0.8f, 0.3f);

        // Currency
        GameObject cur = new GameObject("CurrencyTxt", typeof(RectTransform), typeof(TextMeshProUGUI));
        cur.transform.SetParent(p.transform, false);
        RectTransform cRect = cur.GetComponent<RectTransform>();
        cRect.anchorMin = new Vector2(0.05f, 0.85f); cRect.anchorMax = new Vector2(0.5f, 0.9f);
        cRect.offsetMin = Vector2.zero; cRect.offsetMax = Vector2.zero;
        playerCurrencyText = cur.GetComponent<TextMeshProUGUI>();
        playerCurrencyText.fontSize = 18f; playerCurrencyText.color = Color.green;

        // Container
        GameObject cont = new GameObject("ItemsContainer", typeof(RectTransform));
        cont.transform.SetParent(p.transform, false);
        RectTransform coRect = cont.GetComponent<RectTransform>();
        coRect.anchorMin = new Vector2(0.05f, 0.1f); coRect.anchorMax = new Vector2(0.95f, 0.82f);
        coRect.offsetMin = Vector2.zero; coRect.offsetMax = Vector2.zero;
        
        VerticalLayoutGroup vlg = cont.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 10f; vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlHeight = false; vlg.childControlWidth = false;
        itemsContainer = cont.transform;

        // Close Button
        GameObject cl = new GameObject("CloseBtn", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        cl.transform.SetParent(p.transform, false);
        RectTransform clRect = cl.GetComponent<RectTransform>();
        clRect.anchorMin = new Vector2(0.4f, 0.02f); clRect.anchorMax = new Vector2(0.6f, 0.08f);
        clRect.offsetMin = Vector2.zero; clRect.offsetMax = Vector2.zero;
        cl.GetComponent<Image>().color = new Color(0.8f, 0.2f, 0.2f, 1f);
        closeBtn = cl.GetComponent<Button>();

        GameObject ct = new GameObject("Txt", typeof(RectTransform), typeof(TextMeshProUGUI));
        ct.transform.SetParent(cl.transform, false);
        RectTransform ctr = ct.GetComponent<RectTransform>();
        ctr.anchorMin = Vector2.zero; ctr.anchorMax = Vector2.one;
        ctr.offsetMin = Vector2.zero; ctr.offsetMax = Vector2.zero;
        TextMeshProUGUI clTxt = ct.GetComponent<TextMeshProUGUI>();
        clTxt.text = "ĐÓNG"; clTxt.alignment = TextAlignmentOptions.Center; clTxt.color = Color.white;

        return p;
    }
}
