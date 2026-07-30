using UnityEngine;
using UnityEngine.AI;

public class NativeNPC : MonoBehaviour, Interactable
{
    [Header("Native Info")]
    public string npcName = "Già Làng Thổ Dân K'Nu";
    public bool isCorrupted = false;

    [Header("Dialogue & Teaching")]
    [TextArea(2, 4)]
    public string friendlyDialogue = "Chào người đến từ vùng đất khác! Rừng rậm này chứa đầy nguy hiểm, ta có thể chỉ dạy cho bạn cách nhóm lửa và nấu súp giải độc.";
    public ItemData giftItemReward; // Quà tặng / Chỉ dạy

    [Header("Corrupted Form (Hắc Hóa Ban Đêm)")]
    public float chaseSpeed = 5.0f;
    public float attackRange = 1.8f;
    public float attackDamage = 22f;
    public float attackCooldown = 1.2f;
    public Color normalColor = new Color(0.8f, 0.5f, 0.3f);
    public Color corruptedColor = new Color(0.2f, 0.05f, 0.05f);

    [Header("Stats")]
    public float maxHealth = 80f;
    public float currentHealth = 80f;

    private NavMeshAgent agent;
    private Transform playerTransform;
    private Renderer npcRenderer;
    private float attackTimer;
    private bool learnedFromNPC = false;

    void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        npcRenderer = GetComponent<Renderer>();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        if (npcRenderer != null) npcRenderer.material.color = normalColor;
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        // Tự động kiểm tra trạng thái Hắc Hóa ban đêm
        bool nightTime = DayManager.instance != null && DayManager.instance.IsNight();
        if (nightTime && !isCorrupted)
        {
            TransformToCorrupted(true);
        }
        else if (!nightTime && isCorrupted)
        {
            TransformToCorrupted(false);
        }

        if (isCorrupted && playerTransform != null)
        {
            UpdateCorruptedBehavior();
        }
    }

    void TransformToCorrupted(bool corrupted)
    {
        isCorrupted = corrupted;
        if (npcRenderer != null)
        {
            npcRenderer.material.color = corrupted ? corruptedColor : normalColor;
        }

        if (corrupted)
        {
            Debug.LogWarning($"👹 ⚠️ {npcName} ĐÃ BỊ HẮC HÓA BỞI SƯƠNG MÙ BÓNG ĐÊM! Chuyển sang tấn công cuồng sát!");
        }
        else
        {
            Debug.Log($"🕊️ {npcName} đã bình tĩnh trở lại khi bình minh lên.");
        }
    }

    void UpdateCorruptedBehavior()
    {
        float dist = Vector3.Distance(transform.position, playerTransform.position);

        if (dist <= attackRange)
        {
            transform.LookAt(playerTransform.position);
            if (attackTimer >= attackCooldown)
            {
                attackTimer = 0f;
                if (PlayerStatsManager.instance != null)
                {
                    PlayerStatsManager.instance.TakeDamage(attackDamage);
                    Debug.LogWarning($"💥 {npcName} Hắc Hóa đã quật trúng người chơi! Gây {attackDamage} sát thương!");
                }
            }
        }
        else if (dist <= 15f)
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.speed = chaseSpeed;
                agent.SetDestination(playerTransform.position);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, chaseSpeed * Time.deltaTime);
                transform.LookAt(playerTransform.position);
            }
        }
    }

    public string GetInteractPrompt()
    {
        if (isCorrupted) return $"⚠️ {npcName} (ĐÃ BỊ HẮC HÓA - ĐANG TẤN CÔNG!)";
        if (!learnedFromNPC) return $"Trò chuyện & Học kỹ năng với {npcName} (Phím F)";
        return $"Nói chuyện với {npcName}";
    }

    public void OnInteract()
    {
        if (isCorrupted) return;

        // Hiển thị thoại Subtitle
        if (RadioDialogueUIController.instance != null)
        {
            RadioDialogueUIController.instance.ShowSubtitle(npcName, friendlyDialogue, 6f);
        }
        else
        {
            Debug.Log($"💬 [{npcName}]: {friendlyDialogue}");
        }

        // Chỉ dạy kỹ năng & Tặng quà
        if (!learnedFromNPC && giftItemReward != null && InventoryManager.instance != null)
        {
            learnedFromNPC = true;
            InventoryManager.instance.PickUpItem(giftItemReward, 1);
            Debug.Log($"🎓 {npcName} đã truyền dạy cho bạn công thức mới và truyền trao {giftItemReward.itemName}!");
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"🗡️ Đã tấn công {npcName} (-{damage} HP)! Còn {currentHealth}/{maxHealth}");
        if (currentHealth <= 0f)
        {
            Debug.Log($"💀 {npcName} đã gục ngã!");
            Destroy(gameObject);
        }
    }
}
