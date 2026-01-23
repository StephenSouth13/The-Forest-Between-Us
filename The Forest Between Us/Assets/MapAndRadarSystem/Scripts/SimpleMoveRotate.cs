using UnityEngine;
using UnityEngine.InputSystem;

namespace MapAndRadarSystem
{
    public class SimpleMoveRotate : MonoBehaviour
    {
        public CharacterController characterController;

        private float rotationX;
        private float rotationY;
        private Vector3 moveDirection;
        private float moveSpeed = 8f;
        private float gravity = 9.81f;
        private float verticalVelocity;

        private InputAction moveAction;
        private InputAction lookAction;

        void Awake()
        {
            // Move action (WASD + Left Stick)
            moveAction = new InputAction(type: InputActionType.Value, binding: "<Keyboard>/w");
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            moveAction.AddBinding("<Gamepad>/leftStick");

            // Look action (Mouse + Right Stick)
            lookAction = new InputAction(type: InputActionType.Value, binding: "<Mouse>/delta");
            lookAction.AddBinding("<Gamepad>/rightStick");

            moveAction.Enable();
            lookAction.Enable();
        }

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            Vector2 moveInput = moveAction.ReadValue<Vector2>();
            Vector2 lookInput = lookAction.ReadValue<Vector2>();

            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            moveDirection = move * moveSpeed;

            if (characterController.isGrounded)
            {
                verticalVelocity = -gravity * Time.deltaTime;
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime;
            }

            moveDirection.y = verticalVelocity;

            characterController.Move(moveDirection * Time.deltaTime);

            float mouseX = lookInput.x * 20 * Time.deltaTime;
            float mouseY = lookInput.y * 20 * Time.deltaTime;

            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);
            rotationY += mouseX;
            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
        }

        void OnDestroy()
        {
            moveAction?.Disable();
            lookAction?.Disable();
        }
    }
}