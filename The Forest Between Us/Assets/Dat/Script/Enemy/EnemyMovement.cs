using UnityEngine;
using UnityEngine.AI;
public class EnemyMovement : MonoBehaviour
{
    public Transform model3DEnemy; // Tham chiếu đến mô hình 3D
    private NavMeshAgent agent; // Tham chiếu đến NavMeshAgent

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    public void MoveTo(Vector3 targetPosition) // Hàm di chuyển kẻ địch đến vị trí mục tiêu
    {
        if(!agent.enabled) return;
        agent.isStopped = false;
        agent.SetDestination(targetPosition); // Di chuyển đến vị trí mục tiêu
    }
    public void StopMovement() // Hàm dừng di chuyển kẻ địch
    {
        if(!agent.enabled) return;
        agent.isStopped = true;
    }
    public void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position);
        direction.y = 0; // Giữ nguyên trục Y để tránh nghiêng
        if(direction.sqrMagnitude == 0) return;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        model3DEnemy.rotation = Quaternion.Slerp(
            model3DEnemy.rotation,
            targetRotation,
            Time.deltaTime * 5f
        );
    }
    public bool ReachedDestination()
    {
        if(!agent.enabled) return true;
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
