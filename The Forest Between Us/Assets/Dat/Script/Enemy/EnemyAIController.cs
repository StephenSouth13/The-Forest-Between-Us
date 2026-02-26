using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    public EnemyState currentState;
    
    [Header("DataEnemy")]
    public EnemyMovement enemyMovement;
    public EnemyPerception enemyPerception;
    public EnemyStateMachine stateMachine;

    [Header("State Machine tree")]
    public PatrolState patrolState;
    public SuspiciousState suspiciousState;
    public SearchState searchState;
    public ChaseState chaseState;

    bool wasChasing = false;
    public Vector3 lastSeenPosition;

    // [Header("Patrol Settings")]
    // public float patrolRadius = 5f;

    void Awake()
    {
        patrolState = new PatrolState(this);
        suspiciousState = new SuspiciousState(this);
        searchState = new SearchState(this);
        chaseState = new ChaseState(this);
        stateMachine = GetComponent<EnemyStateMachine>();
        enemyMovement = GetComponent<EnemyMovement>();
        enemyPerception = GetComponent<EnemyPerception>();
    }
    void Start()
    {
        stateMachine.Initialize(patrolState);
    }
    void Update()
    {
        HandleTransitions();
        stateMachine.UpdateState();

    }
    void HandleTransitions()
    {
        float awareness = enemyPerception.awarenessLevel;
        if(awareness >= 1f)
        {
            stateMachine.ChangeState(chaseState);
            wasChasing = true;
        }
        else if(wasChasing && awareness < 0.7f)
        {
            stateMachine.ChangeState(searchState);
            wasChasing = false;
        }
        else if(awareness >= 0.3f)
        {
            stateMachine.ChangeState(suspiciousState);

        }
        else
        {
            stateMachine.ChangeState(patrolState);
        }
    }
}
// using UnityEngine;

// public enum EnemyState
// {
//     Patrol,
//     Chase
// }
// public class EnemyAIController : MonoBehaviour
// {
//     public EnemyState currentState;
    
//     [Header("DataEnemy")]
//     public EnemyMovement enemyMovement;
//     public EnemyPerception enemyPerception;

//     private Vector3 spawnPosition;
//     private Vector3 patrolTargetPosition;
//     [Header("Patrol Settings")]
//     public float patrolRadius = 5f;

//     void Awake()
//     {
//         spawnPosition = transform.position;
//         enemyMovement = GetComponent<EnemyMovement>();
//         enemyPerception = GetComponent<EnemyPerception>();
//     }

//     void Start()
//     {
//         currentState = EnemyState.Patrol;
//         SetNewPatrolPoint();
//     }
//     void Update()
//     {
//         switch (currentState)
//         {
//             case EnemyState.Patrol:
//                 UpdatePatrol();
//                 break;
//             case EnemyState.Chase:
//                 UpdateChase();
//                 break;
//         }
//     }
//     void UpdatePatrol()
//     {
//         enemyMovement.MoveTo(patrolTargetPosition);
//         if (enemyMovement.ReachedDestination())
//         {
//             SetNewPatrolPoint();
//         }
//         if (enemyPerception.IsPlayerInDetectionRange())
//         {
//             currentState = EnemyState.Chase;
//         }
//     }
//     void UpdateChase()
//     {
//         Transform player = enemyPerception.GetPlayerTransform();
//         if(player == null)
//         {
//             currentState = EnemyState.Patrol;
//             SetNewPatrolPoint();
//             return;
//         }
//         enemyMovement.MoveTo(player.position);
//         enemyMovement.RotateTowards(player.position);
//         if(enemyPerception.IsPlayerInDetectionRange() == false)
//         {
//             currentState = EnemyState.Patrol;
//             SetNewPatrolPoint();
//         }

//     }
//     void SetNewPatrolPoint() // Hàm thiết lập điểm tuần tra mới
//     {
//         Vector3 randomPoint = Random.insideUnitSphere * patrolRadius;
//         patrolTargetPosition = spawnPosition + new Vector3(randomPoint.x, 0, randomPoint.z);
//     }
// }
