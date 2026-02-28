using UnityEngine;

public class SuspiciousState : EnemyState
{
    EnemyMovement movement;

    public SuspiciousState(EnemyAIController controller)
        : base(controller)
    {
        movement = controller.GetComponent<EnemyMovement>();
    }

    public override void Enter()
    {
        Debug.Log("Enter Suspicious");
        movement.StopMovement();
    }

    public override void Update()
    {
        // Nếu còn nhìn thấy player → nhìn thẳng vào player
        if (controller.awareness > 0.3f)
        {
            Vector3 lookTarget = controller.lastSeenPosition;
            movement.RotateTowards(lookTarget);
        }
    }

    public override void Exit()
    {
        Debug.Log("Exit Suspicious");
    }
}