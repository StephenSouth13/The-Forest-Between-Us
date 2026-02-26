using UnityEngine;

public class PatrolState : EnemyState
{
    public PatrolState(EnemyAIController controller) 
        : base(controller) { }

    public override void Enter()
    {
        Debug.Log("Enter Patrol");
        // controller.movement.StartPatrol();
    }

    public override void Update()
    {
        // controller.movement.PatrolUpdate();
    }

    public override void Exit()
    {
        Debug.Log("Exit Patrol");
    }

    
    // public PatrolState(EnemyAIController controller) : base(controller){}

    // private Vector3 patrolTargetPosition;
    // private float patrolRadius = 5f; // Bán kính tuần tra

    // public override void Enter()
    // {
    //     Debug.Log("Entering Patrol State");
    //     SetNewPatrolPoint();
    // }
    // public override void Update()
    // {
    //     controller.enemyMovement.MoveTo(patrolTargetPosition);
    //     // Kiểm tra nếu đã đến điểm tuần tra
    //     if (controller.enemyMovement.ReachedDestination())
    //     {
    //         SetNewPatrolPoint();
    //     }
    //     // Kiểm tra nếu phát hiện người chơi
    //     // if (controller.enemyPerception.IsPlayerInDetectionRange())
    //     // {
    //     //     controller.ChangeState(new ChaseState(controller));
    //     // }

    // }
    // public void SetNewPatrolPoint()
    // {
    //     Debug.Log("Setting new patrol point");
    //     Vector3 randomPoint = Random.insideUnitCircle * patrolRadius;
    //     randomPoint.y = 0f;
    //     patrolTargetPosition = controller.transform.position + randomPoint;
    // }
}
