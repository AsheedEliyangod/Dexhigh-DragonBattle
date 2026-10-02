using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilityInput : MonoBehaviour
{
    [Header("Abilities")]
    [SerializeField] private TailAttackAbility tailAttack;
    [SerializeField] private FireAttackAbility fireAttack;
    [SerializeField] private FlyAttackAbility flyAttack;

    private DragonHealth health;

    private void Awake()
    {
        health = GetComponent<DragonHealth>();
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (health != null && health.IsDead)
            return;

        // Prevent starting another ability
        // while any current ability is active.
        if (IsAnyAbilityActive())
            return;

        HandleTailAttack();
        HandleFireAttack();
        HandleFlyAttack();
    }

    private bool IsAnyAbilityActive()
    {
        if (tailAttack != null && tailAttack.IsAttacking)
            return true;

        if (fireAttack != null && fireAttack.IsAttacking)
            return true;

        if (flyAttack != null && flyAttack.IsAttacking)
            return true;

        return false;
    }

    private void HandleTailAttack()
    {
        bool pressed =
            Keyboard.current.digit1Key.wasPressedThisFrame ||
            Keyboard.current.numpad1Key.wasPressedThisFrame;

        if (!pressed)
            return;

        if (tailAttack != null)
        {
            tailAttack.TryUse();
        }
    }

    private void HandleFireAttack()
    {
        bool pressed =
            Keyboard.current.digit2Key.wasPressedThisFrame ||
            Keyboard.current.numpad2Key.wasPressedThisFrame;

        if (!pressed)
            return;

        if (fireAttack != null)
        {
            fireAttack.TryUse();
        }
    }

    private void HandleFlyAttack()
    {
        bool pressed =
            Keyboard.current.digit3Key.wasPressedThisFrame ||
            Keyboard.current.numpad3Key.wasPressedThisFrame;

        if (!pressed)
            return;

        if (flyAttack != null)
        {
            flyAttack.TryUse();
        }
    }
}