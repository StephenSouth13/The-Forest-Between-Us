using UnityEngine;
using UnityEngine.AI;

public enum AnimalType
{
    Prey,     // Động vật ăn cỏ (Thỏ, Hươu) -> Gặp người chơi là CHẠY TRỐN
    Predator  // Đã thú (Lợn rừng, Sói) -> Gặp người chơi là RƯỢT ĐUỔI & TẤN CÔNG
}

public enum AnimalState
{
    Wander,    // Đi dạo / Quan sát
    Flee,      // Bị rượt đuổi / Chạy trốn
    Chase,     // Rượt đuổi kẻ thù
    Attack,    // Tấn công
    Tamed      // Đã thuần hóa (Đi theo bảo vệ người chơi)
}

public class AnimalAI : MonoBehaviour, Interactable
{
    [Header("Animal Config")]
    public string animalName = "Thỏ Rừng";
    public AnimalType animalType = AnimalType.Prey;
    public AnimalState currentState = AnimalState.Wander;

    [Header("Stats")]
    public float maxHealth = 30f;
    public float currentHealth = 30f;
    public float wanderSpeed = 2f;
    public float runSpeed = 5.5f;
    public float detectRadius = 8f;
    public float attackRange = 1.5f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;

    [Header("Taming (Thuần Hóa)")]
    public bool isTamed = false;
    public ItemData favoriteFood; // Món ăn ưa thích để thuần hóa (ví dụ: Trái cây)
    public int tamingProgress = 0;
    public int tamingRequired = 3;

    [Header("Loot Drop")]
    public ItemData dropMeatItem;
    public int dropMeatAmount = 2;

    private NavMeshAgent agent;
    private Transform playerTransform;
    private float attackTimer;
    private Vector3 wanderTarget;
    private float wanderTimer;

    void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.speed = wanderSpeed;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        SetNewWanderTarget();
    }

    void Update()
    {
        attackTimer += Time.deltaTime;
        wanderTimer += Time.deltaTime;

        if (isTamed)
        {
            UpdateTamedState();
            return;
        }

        float distToPlayer = playerTransform != null ? Vector3.Distance(transform.position, playerTransform.position) : 999f;

        switch (animalType)
        {
            case AnimalType.Prey:
                if (distToPlayer <= detectRadius) currentState = AnimalState.Flee;
                else if (currentState == AnimalState.Flee && distToPlayer > detectRadius * 1.8f) currentState = AnimalState.Wander;
                break;

            case AnimalType.Predator:
                if (distToPlayer <= attackRange) currentState = AnimalState.Attack;
                else if (distToPlayer <= detectRadius) currentState = AnimalState.Chase;
                else currentState = AnimalState.Wander;
                break;
        }

        switch (currentState)
        {
            case AnimalState.Wander: UpdateWander(); break;
            case AnimalState.Flee: UpdateFlee(); break;
            case AnimalState.Chase: UpdateChase(); break;
            case AnimalState.Attack: UpdateAttack(); break;
        }
    }

    void UpdateWander()
    {
        if (wanderTimer >= 5f)
        {
            SetNewWanderTarget();
            wanderTimer = 0f;
        }

        MoveTo(wanderTarget, wanderSpeed);
    }

    void UpdateFlee()
    {
        if (playerTransform == null) return;
        Vector3 fleeDir = (transform.position - playerTransform.position).normalized;
        Vector3 targetPos = transform.position + fleeDir * 6f;
        MoveTo(targetPos, runSpeed);
    }

    void UpdateChase()
    {
        if (playerTransform == null) return;
        MoveTo(playerTransform.position, runSpeed);
    }

    void UpdateAttack()
    {
        if (playerTransform == null) return;
        transform.LookAt(playerTransform.position);

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            if (PlayerStatsManager.instance != null)
            {
                PlayerStatsManager.instance.TakeDamage(attackDamage);
                Debug.LogWarning($"💥 {animalName} đã tấn công người chơi! Gây {attackDamage} sát thương.");
            }
        }
    }

    void UpdateTamedState()
    {
        if (playerTransform == null) return;
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist > 3f)
        {
            MoveTo(playerTransform.position, wanderSpeed * 1.3f);
        }
    }

    void MoveTo(Vector3 pos, float speed)
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.speed = speed;
            agent.SetDestination(pos);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, pos, speed * Time.deltaTime);
            transform.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
        }
    }

    void SetNewWanderTarget()
    {
        wanderTarget = transform.position + new Vector3(Random.Range(-8f, 8f), 0, Random.Range(-8f, 8f));
    }

    public string GetInteractPrompt()
    {
        if (isTamed) return $"{animalName} (Đã Thuần Hóa - Đang đi theo bạn)";
        if (favoriteFood != null) return $"Cho {animalName} ăn {favoriteFood.itemName} để Thuần Hóa (Phím F)";
        return $"Tương tác với {animalName}";
    }

    public void OnInteract()
    {
        if (isTamed)
        {
            Debug.Log($"🐾 {animalName} cọ đầu vào bạn tỏ vẻ thân thiện!");
            return;
        }

        // Cho ăn để Thuần hóa
        if (favoriteFood != null && InventoryManager.instance != null && InventoryManager.instance.HasItem(favoriteFood, 1))
        {
            if (InventoryManager.instance.RemoveItem(favoriteFood, 1))
            {
                tamingProgress++;
                Debug.Log($"🍎 Đã cho {animalName} ăn {favoriteFood.itemName}! (Tiến độ Thuần Hóa: {tamingProgress}/{tamingRequired})");

                if (tamingProgress >= tamingRequired)
                {
                    isTamed = true;
                    currentState = AnimalState.Tamed;
                    Debug.Log($"🎉 BẠN ĐÃ THUẦN HÓA THÀNH CÔNG {animalName.ToUpper()}! Nó sẽ luôn đi theo bảo vệ bạn.");
                }
            }
        }
        else if (favoriteFood != null)
        {
            Debug.LogWarning($"⚠️ Bạn cần {favoriteFood.itemName} trong Balo để thuần hóa {animalName}!");
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"🗡️ {animalName} bị chém (-{damage} Máu)! Còn {currentHealth}/{maxHealth}");
        if (currentHealth <= 0f) Die();
    }

    void Die()
    {
        Debug.Log($"💀 {animalName} đã bị hạ gục!");
        if (dropMeatItem != null && InventoryManager.instance != null)
        {
            InventoryManager.instance.PickUpItem(dropMeatItem, dropMeatAmount);
        }
        Destroy(gameObject);
    }
}
