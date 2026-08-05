using UnityEngine;

public class MobileInputManager : MonoBehaviour
{
    public static MobileInputManager instance;

    [Header("Joystick Input")]
    public Vector2 joystickInput;
    public Vector2 lookInput;

    [Header("Button States")]
    public bool isRunning;
    public bool isCrouching;
    public bool isJumping;

    [Header("Action Triggers")]
    public bool interactPressed;
    public bool attackPressed;
    public bool inventoryPressed;
    public bool mapPressed;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Các hàm này sẽ được gọi bằng EventTrigger trên UI Button
    public void SetJoystickInput(Vector2 input) { joystickInput = input; }
    public void SetLookInput(Vector2 input) { lookInput = input; }

    public void OnRunDown() { isRunning = true; }
    public void OnRunUp() { isRunning = false; }

    public void OnCrouchDown() { isCrouching = true; }
    public void OnCrouchUp() { isCrouching = false; }

    public void OnJumpDown() { isJumping = true; }
    public void OnJumpUp() { isJumping = false; }

    public void OnInteractClick() { interactPressed = true; }
    public void OnAttackClick() { attackPressed = true; }
    public void OnInventoryClick() { inventoryPressed = true; }
    public void OnMapClick() { mapPressed = true; }

    void LateUpdate()
    {
        // Reset Trigger trạng thái sau mỗi frame để giống Input.GetKeyDown
        interactPressed = false;
        attackPressed = false;
        inventoryPressed = false;
        mapPressed = false;
    }

    public static bool GetInteractDown()
    {
        if (instance != null && instance.interactPressed) return true;
        return Input.GetKeyDown(KeyCode.E);
    }

    public static bool GetMapDown()
    {
        if (instance != null && instance.mapPressed) return true;
        return Input.GetKeyDown(KeyCode.M);
    }

    public static bool GetInventoryDown()
    {
        if (instance != null && instance.inventoryPressed) return true;
        return Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.B);
    }
}
