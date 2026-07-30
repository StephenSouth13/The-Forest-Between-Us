using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TobyFredson
{
[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Camera & Mouse Look")]
    public Camera playerCamera;
    public float lookSpeed = 2f;
    public float lookXLimit = 85f;
    public bool invertMouseY = false;

    [Header("Movement Speeds")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 15f;

    public bool canMove = true;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>();

        // Load Persistent Settings
        invertMouseY = PlayerPrefs.GetInt("Settings_InvertMouseY", invertMouseY ? 1 : 0) == 1;
        lookSpeed = PlayerPrefs.GetFloat("Settings_LookSpeed", lookSpeed);
        walkSpeed = PlayerPrefs.GetFloat("Settings_WalkSpeed", walkSpeed);
        runSpeed = PlayerPrefs.GetFloat("Settings_RunSpeed", runSpeed);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        #region Handles Movement
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        #endregion

        #region Handles Jumping
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        #endregion

        #region Handles Rotation
        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove && Cursor.lockState == CursorLockMode.Locked)
        {
            float mouseY = Input.GetAxis("Mouse Y") * (invertMouseY ? 1f : -1f);
            rotationX += mouseY * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            if (playerCamera != null)
                playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        #endregion
    }

    public void SetInvertY(bool invert)
    {
        invertMouseY = invert;
        PlayerPrefs.SetInt("Settings_InvertMouseY", invertMouseY ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetSpeed(float newWalk, float newRun)
    {
        walkSpeed = newWalk;
        runSpeed = newRun;
        PlayerPrefs.SetFloat("Settings_WalkSpeed", walkSpeed);
        PlayerPrefs.SetFloat("Settings_RunSpeed", runSpeed);
        PlayerPrefs.Save();
    }
}
}