using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Melee,
    RangedCrossbow,
    SignalBomb
}

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;

    [Header("Current Equipped Weapon")]
    public WeaponType equippedWeapon = WeaponType.Melee;
    public float attackDamage = 25f;
    public float attackRange = 2.5f;
    public float attackRate = 1f; // Attacks per second
    public LayerMask enemyLayer;

    [Header("Ranged Crossbow Settings")]
    public GameObject boltPrefab;
    public Transform firePoint;
    public float boltSpeed = 25f;

    [Header("Wave Defense Spawner")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public List<Transform> spawnPoints = new List<Transform>();

    private float nextAttackTime;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + (1f / attackRate);
            PerformAttack();
        }
    }

    void PerformAttack()
    {
        switch (equippedWeapon)
        {
            case WeaponType.Melee:
                MeleeAttack();
                break;
            case WeaponType.RangedCrossbow:
                RangedAttack();
                break;
        }
    }

    void MeleeAttack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward * 1.2f, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log($"Hit enemy: {enemy.name} for {attackDamage} damage.");
            // Call damage interface if present
        }
    }

    void RangedAttack()
    {
        if (boltPrefab == null || firePoint == null) return;

        GameObject bolt = Instantiate(boltPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bolt.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.forward * boltSpeed;
        }

        Debug.Log("Fired frequency crossbow bolt!");
    }

    public void TriggerEnemyWave(int enemyCount)
    {
        if (enemyPrefabs.Count == 0 || spawnPoints.Count == 0) return;

        Debug.Log($"[WAVE DEFENSE] Spawning wave of {enemyCount} enemies!");

        for (int i = 0; i < enemyCount; i++)
        {
            Transform spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)];
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            Instantiate(enemyPrefab, spawnPos.position, spawnPos.rotation);
        }
    }
}
