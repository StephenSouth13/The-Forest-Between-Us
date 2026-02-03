using UnityEngine;

public class EnemyPerception : MonoBehaviour
{
    [Header("Detection Ranges")]
    public float awarenessRange = 10f; // Khoảng cách để phát hiện người chơi
    public float detectionRange = 5f; // Khoảng cách để xác nhận người chơi
    public float attackRange = 2f; // Khoảng cách để tấn công người chơi

    private Transform playerTransform;

    void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public float DistanceToPlayer() // Hàm trả về khoảng cách từ kẻ địch đến người chơi
    {
        if(playerTransform == null) return Mathf.Infinity;
        return Vector3.Distance(transform.position, playerTransform.position);
    }
    public bool IsPlayerInAwarenessRange() // Kiểm tra nếu người chơi trong phạm vi nhận thức
    {
        return DistanceToPlayer() <= awarenessRange;
    }
    public bool IsPlayerInDetectionRange() // Kiểm tra nếu người chơi trong phạm vi phát hiện
    {
        return DistanceToPlayer() <= detectionRange;
    }
    public bool IsPlayerInAttackRange() // Kiểm tra nếu người chơi trong phạm vi tấn công
    {
        return DistanceToPlayer() <= attackRange;
    }
    public Transform GetPlayerTransform() // Lấy Transform của người chơi
    {
        return playerTransform;
    }
}
