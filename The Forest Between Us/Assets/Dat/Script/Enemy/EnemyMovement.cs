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
// hàm này có thể dùng để kiểm tra nếu đã đến điểm cuối cùng của đường đi, không phải chỉ là điểm dừng hiện tại
    public bool ReachedDestination()
    {
        if (!agent.enabled) return true;

        // Nếu agent chưa tính xong path thì chưa coi là đến
        if (agent.pathPending) return false;

        // Nếu khoảng cách còn lại nhỏ hơn hoặc bằng stoppingDistance
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            // Nếu agent không còn path hoặc tốc độ rất nhỏ (gần như đứng yên)
            if (!agent.hasPath || agent.velocity.magnitude < 0.1f)
            {
                return true;
            }
        }

        return false;
    }

}
