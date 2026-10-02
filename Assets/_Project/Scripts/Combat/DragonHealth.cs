using System;
using UnityEngine;

public enum DragonTeam
{
    Player,
    Enemy
}

public class DragonHealth : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string dragonName = "Dragon";
    [SerializeField] private DragonTeam team;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    public string DragonName => dragonName;
    public DragonTeam Team => team;

    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }

    public bool IsDead { get; private set; }

    public float HealthNormalized =>
        maxHealth > 0f
            ? CurrentHealth / maxHealth
            : 0f;

    public event Action<DragonHealth> HealthChanged;
    public event Action<DragonHealth> Died;

    private static readonly int HitHash =
        Animator.StringToHash("Hit");

    private static readonly int DeathHash =
        Animator.StringToHash("Death");

    private void Awake()
    {
        CurrentHealth = maxHealth;
        IsDead = false;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead || damage <= 0f)
            return;

        CurrentHealth -= damage;

        CurrentHealth = Mathf.Clamp(
            CurrentHealth,
            0f,
            maxHealth
        );

        HealthChanged?.Invoke(this);

        // -------------------------
        // DEATH
        // -------------------------
        if (CurrentHealth <= 0f)
        {
            Die();
            return;
        }

        // -------------------------
        // HIT REACTION
        // -------------------------
        PlayHitReaction();
    }

    private void PlayHitReaction()
    {
        if (animator == null)
            return;

        // Make sure Death can never accidentally
        // be interrupted by Hit.
        if (IsDead)
            return;

        animator.SetTrigger(HitHash);
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f)
            return;

        CurrentHealth += amount;

        CurrentHealth = Mathf.Clamp(
            CurrentHealth,
            0f,
            maxHealth
        );

        HealthChanged?.Invoke(this);
    }

    private void Die()
    {
        if (IsDead)
            return;

        IsDead = true;

        // Prevent a queued Hit trigger from interfering
        // with the Death animation.
        if (animator != null)
        {
            animator.ResetTrigger(HitHash);
            animator.SetTrigger(DeathHash);
        }

        Debug.Log(
            $"{dragonName} has been defeated."
        );

        Died?.Invoke(this);
    }

    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
        IsDead = false;

        if (animator != null)
        {
            animator.ResetTrigger(HitHash);
            animator.ResetTrigger(DeathHash);
        }

        HealthChanged?.Invoke(this);
    }
}