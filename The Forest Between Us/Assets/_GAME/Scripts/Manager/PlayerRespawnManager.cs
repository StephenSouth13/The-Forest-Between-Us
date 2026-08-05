using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRespawnManager : MonoBehaviour
{
    public static PlayerRespawnManager instance;

    [Header("Checkpoint Position")]
    public Vector3 lastCheckpointPos;
    public string lastCheckpointName = "Trạm Khởi Đầu";
    public bool hasCustomCheckpoint = false;

    [Header("Player & Death UI")]
    public Transform playerTransform;
    public GameObject deathPanel;
    public TextMeshProUGUI deathText;
    public CanvasGroup deathCanvasGroup;

    private bool isRespawning = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        LoadCheckpointFromPrefs();
    }

    void Start()
    {
        FindPlayer();
        EnsureDeathUI();
    }

    void FindPlayer()
    {
        if (playerTransform == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) playerTransform = p.transform;
            else if (Camera.main != null) playerTransform = Camera.main.transform;
        }

        if (playerTransform != null && !hasCustomCheckpoint)
        {
            lastCheckpointPos = playerTransform.position;
        }
    }

    void Update()
    {
        if (playerTransform == null) FindPlayer();

        // Check if player died
        if (!isRespawning && PlayerStatsManager.instance != null && PlayerStatsManager.instance.currentHealth <= 0f)
        {
            TriggerPlayerDeath();
        }
    }

    public void SetCheckpoint(Vector3 pos, string name)
    {
        lastCheckpointPos = pos;
        lastCheckpointName = name;
        hasCustomCheckpoint = true;

        PlayerPrefs.SetFloat("Checkpoint_X", pos.x);
        PlayerPrefs.SetFloat("Checkpoint_Y", pos.y);
        PlayerPrefs.SetFloat("Checkpoint_Z", pos.z);
        PlayerPrefs.SetString("Checkpoint_Name", name);
        PlayerPrefs.SetInt("Checkpoint_HasSaved", 1);
        PlayerPrefs.Save();

        Debug.Log($"[RespawnManager] Saved Checkpoint: {name} at {pos}");
    }

    public void TriggerPlayerDeath()
    {
        if (isRespawning) return;
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        // Disable Player Control
        TobyFredson.FPSController fpsController = Object.FindFirstObjectByType<TobyFredson.FPSController>();
        if (fpsController != null) fpsController.canMove = false;

        // Show Death Screen Fade
        if (deathPanel != null) deathPanel.SetActive(true);
        if (deathText != null) deathText.text = $"💀 BẠN ĐÃ GỤC NGÃ TRONG SƯƠNG MÙ...\n<size=14>Đang hồi sinh tại {lastCheckpointName}</size>";

        float elapsed = 0f;
        while (elapsed < 1.5f)
        {
            elapsed += Time.unscaledDeltaTime;
            if (deathCanvasGroup != null) deathCanvasGroup.alpha = Mathf.Clamp01(elapsed / 1.5f);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);

        // Teleport Player to Checkpoint
        if (playerTransform != null)
        {
            CharacterController cc = playerTransform.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            playerTransform.position = lastCheckpointPos;

            if (cc != null) cc.enabled = true;
        }

        // Restore Vitals
        if (PlayerStatsManager.instance != null)
        {
            float hpPercent = 100f;
            if (GameDirector.instance != null)
            {
                hpPercent = GameDirector.instance.respawnHealthPercentage;
                if (GameDirector.instance.dropItemsOnDeath && InventoryManager.instance != null)
                {
                    // Giả lập rớt đồ bằng cách làm rỗng túi (hoặc rơi 1 số slot, ở đây làm đơn giản là clear)
                    InventoryManager.instance.ClearInventory();
                    Debug.LogWarning("💀 Bạn đã đánh rơi tất cả vật phẩm khi chết!");
                }
                PlayerStatsManager.instance.karma -= GameDirector.instance.respawnKarmaPenalty;
            }

            PlayerStatsManager.instance.currentHealth = PlayerStatsManager.instance.maxHealth * (hpPercent / 100f);
            PlayerStatsManager.instance.currentStamina = PlayerStatsManager.instance.maxStamina;
            PlayerStatsManager.instance.currentThirst = 80f;
            PlayerStatsManager.instance.currentHunger = 80f;
            PlayerStatsManager.instance.SaveStats();
        }

        // Fade Out Death Screen
        elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.unscaledDeltaTime;
            if (deathCanvasGroup != null) deathCanvasGroup.alpha = Mathf.Clamp01(1f - elapsed / 1f);
            yield return null;
        }

        if (deathPanel != null) deathPanel.SetActive(false);
        if (fpsController != null) fpsController.canMove = true;

        isRespawning = false;
        Debug.Log($"[RespawnManager] Player respawned successfully at {lastCheckpointName}");
    }

    void LoadCheckpointFromPrefs()
    {
        if (PlayerPrefs.GetInt("Checkpoint_HasSaved", 0) == 1)
        {
            float x = PlayerPrefs.GetFloat("Checkpoint_X", 0f);
            float y = PlayerPrefs.GetFloat("Checkpoint_Y", 0f);
            float z = PlayerPrefs.GetFloat("Checkpoint_Z", 0f);
            lastCheckpointPos = new Vector3(x, y, z);
            lastCheckpointName = PlayerPrefs.GetString("Checkpoint_Name", "Trạm Khởi Đầu");
            hasCustomCheckpoint = true;
        }
    }

    void EnsureDeathUI()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        Transform existing = canvas.transform.Find("PlayerDeath_Panel");
        if (existing != null)
        {
            deathPanel = existing.gameObject;
            deathCanvasGroup = deathPanel.GetComponent<CanvasGroup>();
            return;
        }

        deathPanel = new GameObject("PlayerDeath_Panel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
        deathPanel.transform.SetParent(canvas.transform, false);

        RectTransform pRect = deathPanel.GetComponent<RectTransform>();
        pRect.anchorMin = Vector2.zero;
        pRect.anchorMax = Vector2.one;
        pRect.offsetMin = Vector2.zero;
        pRect.offsetMax = Vector2.zero;

        Image img = deathPanel.GetComponent<Image>();
        img.color = new Color(0.1f, 0.02f, 0.02f, 0.95f);

        deathCanvasGroup = deathPanel.GetComponent<CanvasGroup>();
        deathCanvasGroup.alpha = 0f;

        GameObject txtGO = new GameObject("DeathText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        txtGO.transform.SetParent(deathPanel.transform, false);
        RectTransform tRect = txtGO.GetComponent<RectTransform>();
        tRect.anchorMin = new Vector2(0.1f, 0.3f);
        tRect.anchorMax = new Vector2(0.9f, 0.7f);
        tRect.offsetMin = Vector2.zero;
        tRect.offsetMax = Vector2.zero;

        deathText = txtGO.GetComponent<TextMeshProUGUI>();
        deathText.fontSize = 28f;
        deathText.fontStyle = FontStyles.Bold;
        deathText.alignment = TextAlignmentOptions.Center;
        deathText.color = new Color(1f, 0.25f, 0.25f);
        deathText.text = "💀 BẠN ĐÃ GỤC NGÃ TRONG SƯƠNG MÙ...";

        deathPanel.SetActive(false);
    }
}
