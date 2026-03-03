using UnityEngine;

public class ChaseState : EnemyState
{
    EnemyMovement movement;
    public ChaseState(EnemyAIController controller) : base(controller)
    {
        movement = controller.GetComponent<EnemyMovement>();
    }
    public override void Enter()
    {
        Debug.Log("Entering Chase State");
    }
    public override void Update()
    {
        Vector3 target = controller.TargetPosition;
        movement.MoveTo(target);
        movement.RotateTowards(target);
    }
    public override void Exit()
    {
        Debug.Log("Exiting Chase State");
        movement.StopMovement();
    }
    
    // public override void Enter()
    // {
    //     Transform player = controller.enemyPerception.GetPlayerTransform();
    //     if(player == null)
    //     {
    //         // Nếu không tìm thấy người chơi khi vào trạng thái Chase, quay lại trạng thái Patrol
    //         controller.ChangeState(new PatrolState(controller));
    //         return;
    //     }
    //     // Lưu vị trí người chơi lần cuối
    //     controller.lastSeenPosition = player.position;
    //     // Di chuyển về phía người chơi ngay khi vào trạng thái Chase
    //     controller.enemyMovement.MoveTo(player.position);
    //     controller.enemyMovement.RotateTowards(player.position);
    // }
}