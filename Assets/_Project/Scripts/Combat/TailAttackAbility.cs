using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailAttackAbility : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private DragonHealth ownerHealth;
    [SerializeField] private Transform attackPoint;

    [Header("Tail Attack")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private float attackRadius = 3.3f;

    [Header("Timing")]
    [SerializeField] private float cooldown = 2.5f;
    [SerializeField] private float damageDelay = 0.48f;
    [SerializeField] private float damageWindowDuration = 0.25f;
    [SerializeField] private float attackDuration = 1.45f;

    [Header("Detection")]
    [SerializeField] private LayerMask hitLayers = ~0;

    [Header("Hit VFX")]
    [SerializeField] private GameObject hitImpactVFX;

    [SerializeField] private Vector3 hitVFXOffset =
        new Vector3(0f, 1f, 0f);

    [SerializeField] private float hitVFXLifetime = 2f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip tailSwingSFX;
    [SerializeField] private AudioClip tailHitSFX;

    [Range(0f, 1f)]
    [SerializeField] private float swingVolume = 0.8f;

    [Range(0f, 1f)]
    [SerializeField] private float hitVolume = 1f;

    [Header("Camera Shake")]
    [SerializeField] private CameraShake cameraShake;

    [SerializeField] private float shakeDuration = 0.10f;
    [SerializeField] private float shakeStrength = 0.08f;

    private PlayerDragonMovement playerMovement;
    private EnemyDragonAI enemyAI;

    private bool isAttacking;
    private float cooldownRemaining;

    private static readonly int TailAttackHash =
        Animator.StringToHash("TailAttack");

    public float Damage => damage;
    public float CooldownDuration => cooldown;
    public float CooldownRemaining => cooldownRemaining;

    public float CooldownNormalized =>
        cooldown > 0f
            ? Mathf.Clamp01(cooldownRemaining / cooldown)
            : 0f;

    public bool IsReady =>
        cooldownRemaining <= 0f &&
        !isAttacking &&
        ownerHealth != null &&
        !ownerHealth.IsDead;

    public bool IsAttacking => isAttacking;

    private void Awake()
    {
        playerMovement =
            GetComponent<PlayerDragonMovement>();

        enemyAI =
            GetComponent<EnemyDragonAI>();

        if (audioSource == null)
        {
            audioSource =
                GetComponent<AudioSource>();
        }

        // Automatically find CameraShake.
        if (cameraShake == null &&
            Camera.main != null)
        {
            cameraShake =
                Camera.main.GetComponent<CameraShake>();
        }
    }

    private void Update()
    {
        if (cooldownRemaining > 0f)
        {
            cooldownRemaining -= Time.deltaTime;

            if (cooldownRemaining < 0f)
            {
                cooldownRemaining = 0f;
            }
        }
    }

    public bool TryUse()
    {
        if (!IsReady)
            return false;

        StartCoroutine(PerformTailAttack());

        return true;
    }

    private IEnumerator PerformTailAttack()
    {
        isAttacking = true;
        cooldownRemaining = cooldown;

        SetMovementEnabled(false);

        if (animator != null)
        {
            animator.SetTrigger(TailAttackHash);
        }

        PlaySound(
            tailSwingSFX,
            swingVolume
        );

        yield return new WaitForSeconds(
            damageDelay
        );

        HashSet<DragonHealth> damagedTargets =
            new HashSet<DragonHealth>();

        float elapsedWindow = 0f;

        while (elapsedWindow < damageWindowDuration)
        {
            CheckForTailHits(
                damagedTargets
            );

            elapsedWindow += Time.deltaTime;

            yield return null;
        }

        CheckForTailHits(
            damagedTargets
        );

        float remainingTime =
            Mathf.Max(
                0f,
                attackDuration -
                damageDelay -
                damageWindowDuration
            );

        yield return new WaitForSeconds(
            remainingTime
        );

        SetMovementEnabled(true);

        isAttacking = false;
    }

    private void CheckForTailHits(
        HashSet<DragonHealth> damagedTargets)
    {
        if (attackPoint == null ||
            ownerHealth == null)
        {
            return;
        }

        Collider[] hits =
            Physics.OverlapSphere(
                attackPoint.position,
                attackRadius,
                hitLayers,
                QueryTriggerInteraction.Ignore
            );

        foreach (Collider hit in hits)
        {
            DragonHealth target =
                hit.GetComponentInParent<DragonHealth>();

            if (target == null)
                continue;

            if (target == ownerHealth)
                continue;

            if (target.Team == ownerHealth.Team)
                continue;

            if (target.IsDead)
                continue;

            FlyAttackAbility targetFlyAttack =
                target.GetComponent<FlyAttackAbility>();

            if (targetFlyAttack != null &&
                targetFlyAttack.IsAirborne)
            {
                continue;
            }

            if (!damagedTargets.Add(target))
                continue;

            // Damage
            target.TakeDamage(damage);

            // Hit VFX
            SpawnHitVFX(target);

            // Hit sound
            PlaySound(
                tailHitSFX,
                hitVolume
            );

            // Camera shake only when Tail actually hits.
            if (cameraShake != null)
            {
                cameraShake.Shake(
                    shakeDuration,
                    shakeStrength
                );
            }

            Debug.Log(
                $"{ownerHealth.DragonName} hit " +
                $"{target.DragonName} with Tail Attack " +
                $"for {damage} damage."
            );
        }
    }

    private void PlaySound(
        AudioClip clip,
        float volume)
    {
        if (audioSource == null ||
            clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(
            clip,
            volume
        );
    }

    private void SpawnHitVFX(
        DragonHealth target)
    {
        if (hitImpactVFX == null ||
            target == null)
        {
            return;
        }

        Vector3 spawnPosition =
            target.transform.position +
            hitVFXOffset;

        GameObject impact =
            Instantiate(
                hitImpactVFX,
                spawnPosition,
                Quaternion.identity
            );

        Destroy(
            impact,
            hitVFXLifetime
        );
    }

    private void SetMovementEnabled(
        bool enabled)
    {
        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(
                enabled
            );
        }

        if (enemyAI != null)
        {
            enemyAI.SetMovementEnabled(
                enabled
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRadius
        );
    }
}