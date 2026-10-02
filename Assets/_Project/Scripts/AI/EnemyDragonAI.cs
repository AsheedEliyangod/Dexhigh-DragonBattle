using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyDragonAI : MonoBehaviour
{
    private enum AIState
    {
        Idle,
        Chase,
        TailAttack,
        FireAttack
    }

    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private DragonHealth health;
    [SerializeField] private TailAttackAbility tailAttack;
    [SerializeField] private FireAttackAbility fireAttack;

    [Header("Player References")]
    [SerializeField] private FlyAttackAbility playerFlyAttack;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 7f;
    [SerializeField] private float gravity = -20f;

    [Header("Ranges")]
    [SerializeField] private float detectionRange = 20f;

    [Tooltip("Enemy prefers Tail Attack inside this range.")]
    [SerializeField] private float tailAttackRange = 2.6f;

    [Tooltip("Enemy can use Fire Attack inside this range.")]
    [SerializeField] private float fireAttackRange = 6f;

    [Header("Decision Timing")]
    [SerializeField] private float decisionInterval = 0.15f;

    [Header("Animation")]
    [SerializeField] private float animationSmoothTime = 0.12f;

    private CharacterController controller;

    private AIState currentState =
        AIState.Idle;

    private bool movementEnabled = true;

    private float verticalVelocity;
    private float decisionTimer;

    private static readonly int SpeedHash =
        Animator.StringToHash("Speed");

    private void Awake()
    {
        controller =
            GetComponent<CharacterController>();

        if (health == null)
        {
            health =
                GetComponent<DragonHealth>();
        }

        if (tailAttack == null)
        {
            tailAttack =
                GetComponent<TailAttackAbility>();
        }

        if (fireAttack == null)
        {
            fireAttack =
                GetComponent<FireAttackAbility>();
        }

        // Automatically find the Player's
        // FlyAttackAbility if not assigned manually.
        if (playerFlyAttack == null &&
            player != null)
        {
            playerFlyAttack =
                player.GetComponent<FlyAttackAbility>();

            if (playerFlyAttack == null)
            {
                playerFlyAttack =
                    player.GetComponentInParent<
                        FlyAttackAbility>();
            }
        }
    }

    private void Update()
    {
        if (health != null &&
            health.IsDead)
        {
            UpdateAnimation(0f);
            ApplyGravityOnly();
            return;
        }

        if (player == null)
        {
            currentState = AIState.Idle;

            UpdateAnimation(0f);
            ApplyGravityOnly();

            return;
        }

        // Ability scripts disable movement
        // while an attack is playing.
        if (!movementEnabled)
        {
            FacePlayer();
            UpdateAnimation(0f);
            ApplyGravityOnly();
            return;
        }

        // Prevent overlapping attacks.
        if (IsAnyAttackActive())
        {
            FacePlayer();
            UpdateAnimation(0f);
            ApplyGravityOnly();
            return;
        }

        decisionTimer -= Time.deltaTime;

        if (decisionTimer <= 0f)
        {
            decisionTimer =
                decisionInterval;

            EvaluateState();
        }

        ExecuteCurrentState();
    }

    private void EvaluateState()
    {
        float distance =
            GetFlatDistanceToPlayer();

        if (distance > detectionRange)
        {
            currentState =
                AIState.Idle;

            return;
        }

        // =================================
        // PLAYER IS AIRBORNE
        // =================================

        bool playerIsAirborne =
            playerFlyAttack != null &&
            playerFlyAttack.IsAirborne;

        if (playerIsAirborne)
        {
            EvaluateAirbornePlayer(
                distance
            );

            return;
        }

        // =================================
        // PLAYER IS ON THE GROUND
        // =================================

        // CLOSE RANGE = TAIL
        if (distance <= tailAttackRange)
        {
            if (tailAttack != null &&
                tailAttack.IsReady)
            {
                currentState =
                    AIState.TailAttack;
            }
            else
            {
                currentState =
                    AIState.Idle;
            }

            return;
        }

        // MID RANGE = FIRE
        if (distance <= fireAttackRange)
        {
            if (fireAttack != null &&
                fireAttack.IsReady)
            {
                currentState =
                    AIState.FireAttack;
            }
            else
            {
                currentState =
                    AIState.Chase;
            }

            return;
        }

        // TOO FAR = CHASE
        currentState =
            AIState.Chase;
    }

    private void EvaluateAirbornePlayer(
        float distance)
    {
        // ---------------------------------
        // NEVER use Tail while Player flies
        // ---------------------------------

        // Player is in Fire range.
        if (distance <= fireAttackRange)
        {
            // Fire is available.
            if (fireAttack != null &&
                fireAttack.IsReady)
            {
                currentState =
                    AIState.FireAttack;
            }
            else
            {
                // Fire is cooling down.
                // Hold position, face Player,
                // and wait for landing/cooldown.
                currentState =
                    AIState.Idle;
            }

            return;
        }

        // Player is flying but too far away
        // for Fire, so move closer.
        currentState =
            AIState.Chase;
    }

    private void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case AIState.Idle:
                HandleIdle();
                break;

            case AIState.Chase:
                HandleChase();
                break;

            case AIState.TailAttack:
                HandleTailAttack();
                break;

            case AIState.FireAttack:
                HandleFireAttack();
                break;
        }
    }

    private void HandleIdle()
    {
        FacePlayer();

        UpdateAnimation(0f);

        ApplyGravityOnly();
    }

    private void HandleChase()
    {
        Vector3 direction =
            GetFlatDirectionToPlayer();

        if (direction.sqrMagnitude <=
            0.001f)
        {
            UpdateAnimation(0f);
            ApplyGravityOnly();
            return;
        }

        SmoothRotate(direction);

        Vector3 velocity =
            direction *
            moveSpeed;

        ApplyGravity(
            ref velocity
        );

        controller.Move(
            velocity *
            Time.deltaTime
        );

        UpdateAnimation(0.5f);
    }

    private void HandleTailAttack()
    {
        // Extra safety:
        // If Player suddenly takes off
        // before Tail starts, cancel it.
        if (playerFlyAttack != null &&
            playerFlyAttack.IsAirborne)
        {
            currentState =
                AIState.Idle;

            return;
        }

        FacePlayer();

        UpdateAnimation(0f);

        ApplyGravityOnly();

        if (tailAttack != null &&
            tailAttack.TryUse())
        {
            currentState =
                AIState.Idle;
        }
        else
        {
            currentState =
                AIState.Idle;
        }
    }

    private void HandleFireAttack()
    {
        FacePlayer();

        UpdateAnimation(0f);

        ApplyGravityOnly();

        if (fireAttack != null &&
            fireAttack.TryUse())
        {
            currentState =
                AIState.Idle;
        }
        else
        {
            currentState =
                AIState.Chase;
        }
    }

    private void FacePlayer()
    {
        Vector3 direction =
            GetFlatDirectionToPlayer();

        if (direction.sqrMagnitude <=
            0.001f)
        {
            return;
        }

        SmoothRotate(direction);
    }

    private void SmoothRotate(
        Vector3 direction)
    {
        Quaternion targetRotation =
            Quaternion.LookRotation(
                direction,
                Vector3.up
            );

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

    private Vector3
        GetFlatDirectionToPlayer()
    {
        Vector3 direction =
            player.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude >
            0.001f)
        {
            direction.Normalize();
        }

        return direction;
    }

    private float
        GetFlatDistanceToPlayer()
    {
        Vector3 difference =
            player.position -
            transform.position;

        difference.y = 0f;

        return difference.magnitude;
    }

    private bool IsAnyAttackActive()
    {
        bool tailActive =
            tailAttack != null &&
            tailAttack.IsAttacking;

        bool fireActive =
            fireAttack != null &&
            fireAttack.IsAttacking;

        return
            tailActive ||
            fireActive;
    }

    private void ApplyGravity(
        ref Vector3 velocity)
    {
        if (controller.isGrounded &&
            verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity +=
            gravity *
            Time.deltaTime;

        velocity.y =
            verticalVelocity;
    }

    private void ApplyGravityOnly()
    {
        if (controller.isGrounded &&
            verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity +=
            gravity *
            Time.deltaTime;

        controller.Move(
            Vector3.up *
            verticalVelocity *
            Time.deltaTime
        );
    }

    private void UpdateAnimation(
        float targetSpeed)
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
        bool enabled)
    {
        movementEnabled = enabled;

        if (!enabled)
        {
            UpdateAnimation(0f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            tailAttackRange
        );

        Gizmos.DrawWireSphere(
            transform.position,
            fireAttackRange
        );

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange
        );
    }
}