using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Patrol,
    Investigate,
    Chase,
    Attack
}

public class EnemyAI : MonoBehaviour
{
    [Header("Detection Settings")]
    public float viewRadius = 12f;
    [Range(0, 360)] public float viewAngle = 90f;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [Header("Sound Detection Radii")]
    public float soundRadiusStanding = 15f;
    public float soundRadiusCrouching = 6f;
    public float soundRadiusProne = 2f;

    [Header("Movement & Waypoints")]
    public List<Transform> patrolWaypoints = new List<Transform>();
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4.5f;
    public float attackRange = 1.8f;
    public float attackCooldown = 1.5f;

    [Header("State")]
    public AIState currentState = AIState.Patrol;

    private NavMeshAgent agent;
    private Transform playerTransform;
    private PlayerPostureController playerPosture;
    private int currentWaypointIndex;
    private Vector3 lastKnownPlayerPos;
    private float attackTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerPosture = playerObj.GetComponent<PlayerPostureController>();
        }

        if (agent != null)
        {
            agent.speed = patrolSpeed;
        }

        GoToNextWaypoint();
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        DetectPlayer();

        switch (currentState)
        {
            case AIState.Patrol:
                UpdatePatrol();
                break;
            case AIState.Investigate:
                UpdateInvestigate();
                break;
            case AIState.Chase:
                UpdateChase();
                break;
            case AIState.Attack:
                UpdateAttack();
                break;
        }
    }

    void DetectPlayer()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // 1. Sight Detection
        if (distanceToPlayer <= viewRadius)
        {
            Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2f)
            {
                if (!Physics.Raycast(transform.position + Vector3.up * 1.5f, dirToPlayer, distanceToPlayer, obstacleMask))
                {
                    // Player spotted in vision cone!
                    lastKnownPlayerPos = playerTransform.position;
                    currentState = AIState.Chase;
                    return;
                }
            }
        }

        // 2. Hearing Detection (Posture Dependent)
        float noiseRadius = soundRadiusStanding;
        if (playerPosture != null)
        {
            if (playerPosture.IsProne) noiseRadius = soundRadiusProne;
            else if (playerPosture.IsCrouching) noiseRadius = soundRadiusCrouching;
        }

        // Only make noise if moving
        CharacterController cc = playerTransform.GetComponent<CharacterController>();
        bool isMoving = cc != null && cc.velocity.sqrMagnitude > 0.1f;

        if (isMoving && distanceToPlayer <= noiseRadius)
        {
            lastKnownPlayerPos = playerTransform.position;
            if (currentState != AIState.Chase)
            {
                currentState = AIState.Investigate;
            }
        }
    }

    void UpdatePatrol()
    {
        if (patrolWaypoints.Count == 0) return;

        if (agent != null)
        {
            agent.speed = patrolSpeed;
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                GoToNextWaypoint();
            }
        }
        else
        {
            // Simple fallback without NavMesh
            Transform target = patrolWaypoints[currentWaypointIndex];
            transform.position = Vector3.MoveTowards(transform.position, target.position, patrolSpeed * Time.deltaTime);
            transform.LookAt(target.position);
            if (Vector3.Distance(transform.position, target.position) < 0.5f)
            {
                GoToNextWaypoint();
            }
        }
    }

    void GoToNextWaypoint()
    {
        if (patrolWaypoints.Count == 0) return;

        if (agent != null)
        {
            agent.SetDestination(patrolWaypoints[currentWaypointIndex].position);
        }

        currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Count;
    }

    void UpdateInvestigate()
    {
        if (agent != null)
        {
            agent.speed = patrolSpeed;
            agent.SetDestination(lastKnownPlayerPos);

            if (agent.remainingDistance < 0.8f)
            {
                // Lost track, resume patrol
                currentState = AIState.Patrol;
                GoToNextWaypoint();
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, lastKnownPlayerPos, patrolSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, lastKnownPlayerPos) < 0.8f)
            {
                currentState = AIState.Patrol;
            }
        }
    }

    void UpdateChase()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= attackRange)
        {
            currentState = AIState.Attack;
            return;
        }

        if (distance > viewRadius * 1.5f)
        {
            // Player escaped
            currentState = AIState.Investigate;
            lastKnownPlayerPos = playerTransform.position;
            return;
        }

        if (agent != null)
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

    void UpdateAttack()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance > attackRange)
        {
            currentState = AIState.Chase;
            return;
        }

        transform.LookAt(playerTransform.position);

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            Debug.Log($"Enemy attacked player! Dealt damage.");
            // Can call Player health logic here if implemented
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw vision cone
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 fwdAngleA = DirectionFromAngle(-viewAngle / 2f, false);
        Vector3 fwdAngleB = DirectionFromAngle(viewAngle / 2f, false);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + fwdAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + fwdAngleB * viewRadius);
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
