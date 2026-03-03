using UnityEngine;

public class EnemyPerception : MonoBehaviour
{
    [Header("Target")]
    public Transform player; // Tham chiếu đến mục tiêu (người chơi)

    [Header("Vision Settings")]
    public float detectionRange = 15f; // Khoảng cách tầm nhìn
    [Range(0, 180)]
    public float fieldOfViewAngle = 100f; // Góc tầm nhìn
    public Transform visionOrigin; // Điểm xuất phát tầm nhìn (thường là vị trí mắt kẻ địch)

    [Header("Awareness Settings")]
    public float awarenessLevel = 0f; // Mức độ nhận thức hiện tại
    public float awarenessIncreaseRate = 3f; // Tốc độ tăng mức độ nhận thức khi thấy mục tiêu
    public float awarenessDecreaseRate = 0.2f; // Tốc độ giảm mức độ nhận thức khi không thấy mục tiêu
    [Header("Distance Scaling")]
    public float distanceExponent = 1.5f; // Hệ số mũ để điều chỉnh ảnh hưởng của khoảng cách đến mức độ nhận thức

    [Header("RunTime Variables")]
    public Vector3 lastSeenPosition; // Vị trí cuối cùng biết của mục tiêu
    public bool lockAwareness = false; // Khóa mức độ nhận thức // bật khi chaseState
    [Header("Vision Obstacle")]
    public LayerMask obstacleMask;
    void Update()
    {
        if(lockAwareness)
        {
            awarenessLevel = 1f;
            return;
        }
        bool canSeePlayer = CheckPlayerVision();
        if(canSeePlayer)
        {
            float distance = Vector3.Distance(visionOrigin.position, player.position);
            // Tăng mức độ nhận thức dựa trên khoảng cách (gần hơn thì tăng nhanh hơn)
            if(distance < detectionRange * 0.4f) // Nếu rất gần cố định mức độ nhận thức ở 1
            {
                awarenessLevel = 1f;
                lastSeenPosition = player.position; // Cập nhật vị trí cuối cùng biết của mục
            }
            else if(distance < detectionRange * 0.8f) // Nếu ở khoảng giữa thì tăng mức độ nhận thức hẳn lên 0.3 rồi tăng tiếp dựa vào tuyen tính khoảng cách
            {
                awarenessLevel = Mathf.Max(awarenessLevel, 0.3f); // Đảm bảo mức độ nhận thức ít nhất là 0.3 khi ở khoảng này
                float distanceFactor = 1f - ((distance - detectionRange * 0.4f) / (detectionRange * 0.35f)); // Tỷ lệ khoảng cách trong khoảng giữa (0 khi ở rìa, 1 khi ở gần)
                distanceFactor = Mathf.Clamp01(distanceFactor); // Đảm bảo giá trị trong khoảng 0-1
                distanceFactor = Mathf.Pow(distanceFactor, distanceExponent); // Điều chỉnh bằng hệ số mũ để tăng cường ảnh hưởng của khoảng cách
                float scaledIncrease = awarenessIncreaseRate * distanceFactor; // Tăng tốc độ nhận thức khi gần hơn
                awarenessLevel += scaledIncrease * Time.deltaTime; 
                lastSeenPosition = player.position; // Cập nhật vị trí cuối cùng biết của mục tiêu
            }
            else
            {
                float distanceFactor = 1f - (distance / detectionRange); // Tỷ lệ khoảng cách (0 khi ở rìa, 1 khi ở gần)
                distanceFactor = Mathf.Clamp01(distanceFactor); // Đảm bảo giá trị trong khoảng 0-1
                distanceFactor = Mathf.Pow(distanceFactor, distanceExponent); // Điều chỉnh bằng hệ số mũ để tăng cường ảnh hưởng của khoảng cách
                float scaledIncrease = awarenessIncreaseRate * distanceFactor; // Tăng tốc độ nhận thức khi gần hơn
                awarenessLevel += scaledIncrease * Time.deltaTime; 
                lastSeenPosition = player.position; // Cập nhật vị trí cuối cùng biết của mục tiêu
            }
        }
        else
        {
            awarenessLevel -= awarenessDecreaseRate * Time.deltaTime;
        }
        awarenessLevel = Mathf.Clamp01(awarenessLevel);

    }
    bool CheckPlayerVision()
    {
        if(player == null && visionOrigin == null) return false;

        Vector3 toPlayer = player.position - visionOrigin.position;
        float distance = toPlayer.magnitude; // magnitude trả về độ dài vector
        if(distance > detectionRange) return false; // Nếu ngoài tầm nhìn thì không cần kiểm tra góc
        toPlayer.Normalize(); // Chuẩn hóa vector để chỉ giữ hướng
        Vector3 forward = visionOrigin.forward; // forward là hướng nhìn về phía trước của transform
        forward.Normalize();
        float angle = Vector3.Angle(forward, toPlayer); // Tính góc giữa hai vector
        if(angle > fieldOfViewAngle * 0.5f) return false; // Nếu ngoài góc nhìn thì không thấy
        return true;
    }
    void OnDrawGizmosSelected()
    {
        if (visionOrigin == null) return;

        // Màu xanh cho vùng nhìn
        Gizmos.color = Color.green;

        // Vẽ đường thẳng từ mắt enemy đến player (nếu có)
        if (player != null)
        {
            Gizmos.DrawLine(visionOrigin.position, player.position);
        }

        // Vẽ hình quạt (field of view)
        Vector3 origin = visionOrigin.position;
        Vector3 forward = visionOrigin.forward;

        // Góc trái và phải của tầm nhìn
        Quaternion leftRotation = Quaternion.AngleAxis(-fieldOfViewAngle * 0.5f, Vector3.up);
        Quaternion rightRotation = Quaternion.AngleAxis(fieldOfViewAngle * 0.5f, Vector3.up);

        Vector3 leftDir = leftRotation * forward;
        Vector3 rightDir = rightRotation * forward;

        Gizmos.DrawRay(origin, leftDir * detectionRange);
        Gizmos.DrawRay(origin, rightDir * detectionRange);

        // Vẽ vòng cung tầm nhìn
        int segments = 20;
        float step = fieldOfViewAngle / segments;
        Vector3 lastPoint = origin + (leftDir * detectionRange);
        for (int i = 1; i <= segments; i++)
        {
            Quaternion rot = Quaternion.AngleAxis(-fieldOfViewAngle * 0.5f + step * i, Vector3.up);
            Vector3 nextPoint = origin + (rot * forward * detectionRange);
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
    }

    // [Header("Detection Ranges")]
    // public float awarenessRange = 10f; // Khoảng cách để phát hiện người chơi
    // public float detectionRange = 5f; // Khoảng cách để xác nhận người chơi
    // public float attackRange = 2f; // Khoảng cách để tấn công người chơi

    // private Transform playerTransform;

    // void Awake()
    // {
    //     playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    // }

    // public float DistanceToPlayer() // Hàm trả về khoảng cách từ kẻ địch đến người chơi
    // {
    //     if(playerTransform == null) return Mathf.Infinity;
    //     return Vector3.Distance(transform.position, playerTransform.position);
    // }
    // public bool IsPlayerInAwarenessRange() // Kiểm tra nếu người chơi trong phạm vi nhận thức
    // {
    //     return DistanceToPlayer() <= awarenessRange;
    // }
    // public bool IsPlayerInDetectionRange() // Kiểm tra nếu người chơi trong phạm vi phát hiện
    // {
    //     return DistanceToPlayer() <= detectionRange;
    // }
    // public bool IsPlayerInAttackRange() // Kiểm tra nếu người chơi trong phạm vi tấn công
    // {
    //     return DistanceToPlayer() <= attackRange;
    // }
    // public Transform GetPlayerTransform() // Lấy Transform của người chơi
    // {
    //     return playerTransform;
    // }
    // void OnDrawGizmosSelected()
    // {
    //     // Vẽ vòng awareness (xanh lá)
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawWireSphere(transform.position, awarenessRange);

    //     // Vẽ vòng detection (vàng)
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawWireSphere(transform.position, detectionRange);

    //     // Vẽ vòng attack (đỏ)
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(transform.position, attackRange);
    // }

}
