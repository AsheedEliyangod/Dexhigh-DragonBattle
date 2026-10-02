using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerDragonMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 5f;

    [Tooltip("Higher = faster turning. Lower = softer turning.")]
    [SerializeField] private float rotationSpeed = 7f;

    [Header("Grounding")]
    [SerializeField] private float gravity = -20f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private float animationSmoothTime = 0.12f;

    private CharacterController controller;

    private float verticalVelocity;
    private bool movementEnabled = true;

    private static readonly int SpeedHash =
        Animator.StringToHash("Speed");

    public bool CanMove => movementEnabled;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (!movementEnabled)
        {
            ApplyGravity();
            UpdateAnimation(0f);
            return;
        }

        if (Keyboard.current == null)
        {
            ApplyGravity();
            UpdateAnimation(0f);
            return;
        }

        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            input.y += 1f;

        if (Keyboard.current.sKey.isPressed)
            input.y -= 1f;

        if (Keyboard.current.dKey.isPressed)
            input.x += 1f;

        if (Keyboard.current.aKey.isPressed)
            input.x -= 1f;

        input = Vector2.ClampMagnitude(input, 1f);

        Vector3 moveDirection =
            new Vector3(
                input.x,
                0f,
                input.y
            );

        bool isMoving =
            moveDirection.sqrMagnitude > 0.01f;

        bool isRunning =
            isMoving &&
            Keyboard.current.leftShiftKey.isPressed;

        float currentSpeed =
            isRunning
                ? runSpeed
                : walkSpeed;

        // -------------------------
        // SMOOTH ROTATION
        // -------------------------
        if (isMoving)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(
                    moveDirection,
                    Vector3.up
                );

            // Frame-rate-independent smooth turning.
            float rotationAmount =
                1f -
                Mathf.Exp(
                    -rotationSpeed *
                    Time.deltaTime
                );

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationAmount
                );
        }

        // -------------------------
        // MOVEMENT
        // -------------------------
        Vector3 velocity =
            moveDirection * currentSpeed;

        if (controller.isGrounded &&
            verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity +=
            gravity * Time.deltaTime;

        velocity.y = verticalVelocity;

        controller.Move(
            velocity * Time.deltaTime
        );

        // -------------------------
        // ANIMATION
        // -------------------------
        float targetAnimationSpeed = 0f;

        if (isMoving)
        {
            targetAnimationSpeed =
                isRunning ? 1f : 0.5f;
        }

        UpdateAnimation(
            targetAnimationSpeed
        );
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded &&
            verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity +=
            gravity * Time.deltaTime;

        Vector3 gravityMovement =
            Vector3.up *
            verticalVelocity *
            Time.deltaTime;

        controller.Move(
            gravityMovement
        );
    }

    private void UpdateAnimation(
        float targetSpeed
    )
    {
        if (animator == null)
            return;

        animator.SetFloat(
            SpeedHash,
            targetSpeed,
            animationSmoothTime,
            Time.deltaTime
        );
    }

    public void SetMovementEnabled(
        bool enabled
    )
    {
        movementEnabled = enabled;

        if (!enabled)
        {
            UpdateAnimation(0f);
        }
    }
}