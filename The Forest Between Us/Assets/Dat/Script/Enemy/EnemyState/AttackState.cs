using UnityEngine;

public class AttackState : EnemyState
{
    EnemyMovement movement;

    enum AttackPhase
    {
        Idle,
        WindUp,
        Recover
    }

    AttackPhase currentPhase;

    float windUpDuration = 0.5f; // Thời gian chuẩn bị tấn công trước
    float recoverDuration = 0.7f; // Thời gian hồi sau khi tấn công xong (khoảng thời gian mà enemy không thể tấn công lại ngay lập tức)

    float timer;

    public AttackState(EnemyAIController controller)
        : base(controller)
    {
        movement = controller.GetComponent<EnemyMovement>();
    }

    public override void Enter()
    {
        Debug.Log("Enter Attack");
        movement.StopMovement();
        StartWindUp();
    }

    public override void Update()
    {
        Vector3 target = controller.TargetPosition;
        movement.RotateTowards(target);

        timer -= Time.deltaTime;

        switch (currentPhase)
        {
            case AttackPhase.WindUp:
                if (timer <= 0f)
                {
                    PerformHit();
                    StartRecover();
                }
                break;

            case AttackPhase.Recover:
                if (timer <= 0f)
                {
                    StartWindUp();
                }
                break;
        }
    }

    void StartWindUp() // Bắt đầu giai đoạn chuẩn bị tấn công
    {
        currentPhase = AttackPhase.WindUp;
        timer = windUpDuration;

        Debug.Log("WindUp...");
        // Trigger animation wind-up ở đây sau này
    }
     void PerformHit() // Thực hiện
    {
        Debug.Log("HIT!");

        // Sau này:
        // Raycast / OverlapSphere
        // Gây damage nếu player còn trong range
    }

    void StartRecover() // Bắt đầu giai đoạn hồi sau khi tấn công xong
    {
        currentPhase = AttackPhase.Recover;
        timer = recoverDuration;

        Debug.Log("Recover...");
    }

    public override void Exit()
    {
        Debug.Log("Exit Attack");
    }
}