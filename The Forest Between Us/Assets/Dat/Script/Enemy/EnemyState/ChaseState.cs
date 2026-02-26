using UnityEngine;

public class ChaseState : EnemyState
{
    public ChaseState(EnemyAIController controller) : base(controller){}

    
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