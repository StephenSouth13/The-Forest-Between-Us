using UnityEngine;

public class SearchState : EnemyState
{
    Vector3 targetPosition;

    public SearchState(EnemyAIController controller)
        : base(controller) { }

    public override void Enter()
    {
        targetPosition = controller.lastSeenPosition;
        Debug.Log("Enter Search");

        // controller.movement.MoveTo(targetPosition);
    }

    public override void Update()
    {
        // nếu tới nơi rồi:
        // controller.movement.Stop();
    }

    public override void Exit()
    {
        Debug.Log("Exit Search");
    }
}
