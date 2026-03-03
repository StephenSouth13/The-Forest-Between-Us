using UnityEngine;

public class SearchState : EnemyState
{
    public bool IsSearchComplete {get; private set;}
    EnemyMovement movement;
    Vector3 targetPosition;
    float searchTime = 0f;
    float searchDuration = 5f;
    bool reachedPoint = false;
    public SearchState(EnemyAIController controller)
        : base(controller)
    {
        movement = controller.GetComponent<EnemyMovement>();
    }

    public override void Enter()
    {
        IsSearchComplete = false; 
        targetPosition = controller.TargetPosition; // Lấy vị trí cuối cùng của người chơi từ EnemyAIController
        Debug.Log("Enter Search");
        reachedPoint = false;
        searchTime = 0f;
        movement.MoveTo(targetPosition);
        // controller.movement.MoveTo(targetPosition);
    }

    public override void Update()
    {
        // nếu tới nơi rồi:
        if (!reachedPoint)
        {
            if (movement.ReachedDestination())
            {
                reachedPoint = true;
                movement.StopMovement();
            }
        }
        else
        {
            // look around tại chỗ cuối cùng 
            searchTime += Time.deltaTime;
            movement.RotateTowards(targetPosition); // có thể thêm hiệu ứng look around ở đây
            if( searchTime >= searchDuration)
            {
                Debug.Log("Search complete, returning to patrol");
                // sau khi look around xong mà vẫn không thấy player thì quay về patrol
                controller.ForcePatrol();
                IsSearchComplete = true;
            }
        }
    }

    public override void Exit()
    {
        Debug.Log("Exit Search");
    }
}
