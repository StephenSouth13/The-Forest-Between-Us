using UnityEngine;

public enum BossState
{
    Normal, // trạng thái bình thường
    Combat // trong trạng thái tấn công
}
public class Move : MonoBehaviour
{
    [Header("Preferences")]
    CharacterController ctl;
    public Transform GroundCheck;

    AnimControll animControl;
    [Header("Settings")]
    public float speed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    public LayerMask groundLayer;
    public float groundDistance = 0.4f;

    Vector3 velocity;
    bool isGrounded;
    public BossState currentState = BossState.Normal;
    public float rotationY = 0f;
    void Awake()
    {
        ctl = GetComponent<CharacterController>();
        Debug.Log("groundCheck pos at: " + GroundCheck.position);
        if(animControl == null) animControl = GetComponentInChildren<AnimControll>();
    }

    void Update()
    {
        HandleMovement();
        SetCombatState();
        TestAnimTrigger();
    }

    void HandleMovement()
    {
        isGrounded = Physics.CheckSphere(GroundCheck.position, groundDistance, groundLayer);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        PlayAnimation(x, z);
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.rotation = Quaternion.LookRotation(Vector3.left);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.LookRotation(Vector3.right);
        }

        Vector3 move = transform.right * x + transform.forward * z;
        ctl.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        ctl.Move(velocity * Time.deltaTime);
    }

    public void SetCombatState()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            currentState = BossState.Combat;
            animControl.PlayBoolAnimation("IsIdle_Combat", true);
        }
        if(Input.GetKeyDown(KeyCode.N))
        {
            currentState = BossState.Normal;
            animControl.PlayTriggerAnimation("IdleTrigger");
            animControl.PlayBoolAnimation("IsIdle_Combat", false);
        }
    }
    public void TestAnimTrigger()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            animControl.PlayTriggerAnimation("TestTrigger");
        }
    }
    void PlayAnimation(float x, float z)
    {
        switch (currentState)
        {
            case BossState.Normal:
                if (x != 0 || z != 0)
                {
                    animControl.EnableRootMotion(0);
                    rotationY = Mathf.Atan2(x, z) * Mathf.Rad2Deg;
                    animControl.SetUpRotation(rotationY);
                    animControl.PlayBoolAnimation("IsWalking", true);
                }
                else
                {
           
                    animControl.EnableRootMotion(1);
                    animControl.PlayBoolAnimation("IsWalking", false);
                }
                break;
            case BossState.Combat:
                if (x != 0 || z != 0)
                {
                    animControl.EnableRootMotion(0);
                    rotationY = Mathf.Atan2(x, z) * Mathf.Rad2Deg;
                    animControl.SetUpRotation(rotationY);
                    animControl.PlayBoolAnimation("Iswalk_combat", true);
                }
                else
                {
                    
                    animControl.EnableRootMotion(1);
                    animControl.PlayBoolAnimation("Iswalk_combat", false);
                }
                // Thêm logic hoạt ảnh cho trạng thái Combat nếu cần
                break;
            // Xử lý các trạng thái khác nếu cần
        }
    }
}
