using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerPostureController : MonoBehaviour
{
    [Header("Input")]
    public KeyCode crouchKey = KeyCode.C;
    public KeyCode proneKey = KeyCode.X;

    [Header("Controller Heights")]
    public float standingHeight = 1.8f;
    public float crouchHeight = 1.1f;
    public float proneHeight = 0.55f;

    [Header("Animator")]
    public Animator animator;
    public string crouchBool = "IsCrouching";
    public string proneBool = "IsProne";

    private CharacterController characterController;
    private Vector3 standingCenter;
    private bool isCrouching;
    private bool isProne;

    public bool IsCrouching => isCrouching;
    public bool IsProne => isProne;


    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        standingCenter = characterController.center;

        if (standingHeight <= 0f)
        {
            standingHeight = characterController.height;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            ToggleCrouch();
        }

        if (Input.GetKeyDown(proneKey))
        {
            ToggleProne();
        }
    }

    public void ToggleCrouch()
    {
        SetPosture(!isCrouching, false);
    }

    public void ToggleProne()
    {
        SetPosture(false, !isProne);
    }

    public void Stand()
    {
        SetPosture(false, false);
    }

    void SetPosture(bool crouch, bool prone)
    {
        isCrouching = crouch;
        isProne = prone;

        if (isProne)
        {
            ApplyHeight(proneHeight);
        }
        else if (isCrouching)
        {
            ApplyHeight(crouchHeight);
        }
        else
        {
            ApplyHeight(standingHeight);
        }

        if (animator != null)
        {
            animator.SetBool(crouchBool, isCrouching);
            animator.SetBool(proneBool, isProne);
        }
    }

    void ApplyHeight(float height)
    {
        characterController.height = height;
        characterController.center = new Vector3(
            standingCenter.x,
            standingCenter.y - (standingHeight - height) * 0.5f,
            standingCenter.z);
    }
}
