using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.4f;
    public float jumpHeight = 2f;
    public float gravity = -20f;
    public float lookSensitivity = 1f;
    public Transform cameraTransform;

    private CharacterController controller;
    public PlayerInput input;
    private float cameraPitch = 0f;
    private float verticalVelocity = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        input.actions["Attack"].performed += _ => OnFire();
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        input.actions["Attack"].performed -= _ => OnFire();
    }

    private void Update()
    {
        // Sprint
        bool isSprinting = input.actions["Sprint"] != null && input.actions["Sprint"].ReadValue<float>() > 0.1f;
        float currentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;

        // Movement
        Vector2 moveInput = input.actions["Move"].ReadValue<Vector2>();
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move.Normalize();

        // Jump & Gravity
        if (controller.isGrounded)
        {
            verticalVelocity = -2f; // Small downward force to keep grounded
            if (input.actions["Jump"] != null && input.actions["Jump"].triggered)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 velocity = move * currentSpeed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);

        // Look
        Vector2 lookInput = input.actions["Look"].ReadValue<Vector2>();
        float mouseX = lookInput.x * lookSensitivity;
        float mouseY = lookInput.y * lookSensitivity;

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void OnFire()
    {
        Debug.Log("Fire event triggered!");
    }
}