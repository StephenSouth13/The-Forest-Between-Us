using UnityEngine;

public enum EnemyState
{
    Patrol,
    Chase
}
public class EnemyAIController : MonoBehaviour
{
    public EnemyState currentState;
    
    [Header("DataEnemy")]
    public EnemyMovement enemyMovement;
    public EnemyPerception enemyPerception;

    private Vector3 spawnPosition;
    private Vector3 patrolTargetPosition;
    [Header("Patrol Settings")]
    public float patrolRadius = 5f;

    void Awake()
    {
        spawnPosition = transform.position;
        enemyMovement = GetComponent<EnemyMovement>();
        enemyPerception = GetComponent<EnemyPerception>();
    }

    void Start()
    {
        currentState = EnemyState.Patrol;
        SetNewPatrolPoint();
    }
    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                UpdatePatrol();
                break;
            case EnemyState.Chase:
                UpdateChase();
                break;
        }
    }
    void UpdatePatrol()
    {
        enemyMovement.MoveTo(patrolTargetPosition);
        if (enemyMovement.ReachedDestination())
        {
            SetNewPatrolPoint();
        }
        if (enemyPerception.IsPlayerInDetectionRange())
        {
            currentState = EnemyState.Chase;
        }
    }
    void UpdateChase()
    {
        Transform player = enemyPerception.GetPlayerTransform();
        if(player == null)
        {
            currentState = EnemyState.Patrol;
            SetNewPatrolPoint();
            return;
        }
        enemyMovement.MoveTo(player.position);
        enemyMovement.RotateTowards(player.position);
        if(enemyPerception.IsPlayerInDetectionRange() == false)
        {
            currentState = EnemyState.Patrol;
            SetNewPatrolPoint();
        }

    }
    void SetNewPatrolPoint() // Hàm thiết lập điểm tuần tra mới
    {
        Vector3 randomPoint = Random.insideUnitSphere * patrolRadius;
        patrolTargetPosition = spawnPosition + new Vector3(randomPoint.x, 0, randomPoint.z);
    }
}
