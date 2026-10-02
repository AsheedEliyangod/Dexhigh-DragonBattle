using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAttackAbility : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private DragonHealth ownerHealth;
    [SerializeField] private ParticleSystem fireVFX;
    [SerializeField] private BoxCollider fireDamageCollider;

    [Header("Fire Damage")]
    [SerializeField] private float damagePerTick = 5f;
    [SerializeField] private float damageTickInterval = 0.3f;
    [SerializeField] private int maximumSuccessfulTicks = 4;

    [Header("Fire Hitbox")]
    [SerializeField] private float maximumRange = 2.5f;
    [SerializeField] private float fireWidth = 0.30f;
    [SerializeField] private float fireHeight = 0.30f;
    [SerializeField] private float fireGrowTime = 0.4f;

    [Header("Timing")]
    [SerializeField] private float cooldown = 4f;
    [SerializeField] private float vfxDelay = 0.65f;
    [SerializeField] private float fireDuration = 1.2f;
    [SerializeField] private float attackDuration = 2.8f;

    [Header("Detection")]
    [SerializeField] private LayerMask hitLayers = ~0;

    [Header("Hit VFX")]
    [SerializeField] private GameObject hitImpactVFX;

    [SerializeField] private Vector3 hitVFXOffset =
        new Vector3(0f, 1f, 0f);

    [SerializeField] private float hitVFXLifetime = 2f;

    [Header("Audio")]
    [SerializeField] private AudioSource fireAudioSource;
    [SerializeField] private AudioSource attackAudioSource;

    [SerializeField] private AudioClip fireBreathSFX;
    [SerializeField] private AudioClip fireHitSFX;

    [Range(0f, 1f)]
    [SerializeField] private float fireBreathVolume = 0.8f;

    [Range(0f, 1f)]
    [SerializeField] private float fireHitVolume = 0.45f;

    [Header("Camera Shake")]
    [SerializeField] private CameraShake cameraShake;

    [SerializeField] private float shakeDuration = 0.10f;
    [SerializeField] private float shakeStrength = 0.04f;

    private PlayerDragonMovement playerMovement;
    private EnemyDragonAI enemyAI;

    private bool isAttacking;
    private float cooldownRemaining;

    private bool cameraShakeTriggered;

    private static readonly int FireAttackHash =
        Animator.StringToHash("FireAttack");

    public float CooldownDuration => cooldown;
    public float CooldownRemaining => cooldownRemaining;

    public float CooldownNormalized =>
        cooldown > 0f
            ? Mathf.Clamp01(cooldownRemaining / cooldown)
            : 0f;

    public bool IsAttacking => isAttacking;

    public bool IsReady =>
        cooldownRemaining <= 0f &&
        !isAttacking &&
        ownerHealth != null &&
        !ownerHealth.IsDead;

    private void Awake()
    {
        playerMovement =
            GetComponent<PlayerDragonMovement>();

        enemyAI =
            GetComponent<EnemyDragonAI>();

        if (cameraShake == null &&
            Camera.main != null)
        {
            cameraShake =
                Camera.main.GetComponent<CameraShake>();
        }

        ResetFireDamageVolume();

        if (fireDamageCollider != null)
        {
            fireDamageCollider.enabled = false;
        }

        if (fireVFX != null)
        {
            fireVFX.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }

        if (fireAudioSource != null)
        {
            fireAudioSource.playOnAwake = false;
            fireAudioSource.loop = true;
        }
    }

    private void Update()
    {
        if (cooldownRemaining <= 0f)
            return;

        cooldownRemaining -= Time.deltaTime;

        if (cooldownRemaining < 0f)
        {
            cooldownRemaining = 0f;
        }
    }

    public bool TryUse()
    {
        if (!IsReady)
            return false;

        StartCoroutine(PerformFireAttack());

        return true;
    }

    private IEnumerator PerformFireAttack()
    {
        isAttacking = true;
        cooldownRemaining = cooldown;

        cameraShakeTriggered = false;

        SetMovementEnabled(false);

        if (animator != null)
        {
            animator.SetTrigger(FireAttackHash);
        }

        yield return new WaitForSeconds(vfxDelay);

        ResetFireDamageVolume();

        if (fireVFX != null)
        {
            fireVFX.Clear(true);
            fireVFX.Play(true);
        }

        StartFireSound();

        if (fireDamageCollider != null)
        {
            fireDamageCollider.enabled = true;
        }

        float elapsed = 0f;
        float nextDamageTick = 0f;

        int successfulDamageTicks = 0;

        while (elapsed < fireDuration)
        {
            float growth =
                Mathf.Clamp01(
                    elapsed / fireGrowTime
                );

            float currentLength =
                Mathf.Lerp(
                    0.05f,
                    maximumRange,
                    growth
                );

            UpdateFireDamageVolume(
                currentLength
            );

            if (elapsed >= nextDamageTick &&
                successfulDamageTicks <
                maximumSuccessfulTicks)
            {
                bool damagedSomething =
                    DealDamageInsideCurrentFire();

                if (damagedSomething)
                {
                    successfulDamageTicks++;
                }

                nextDamageTick +=
                    damageTickInterval;
            }

            elapsed += Time.deltaTime;

            yield return null;
        }

        if (fireVFX != null)
        {
            fireVFX.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting
            );
        }

        StopFireSound();

        if (fireDamageCollider != null)
        {
            fireDamageCollider.enabled = false;
        }

        ResetFireDamageVolume();

        float remainingAttackTime =
            Mathf.Max(
                0f,
                attackDuration -
                vfxDelay -
                fireDuration
            );

        yield return new WaitForSeconds(
            remainingAttackTime
        );

        SetMovementEnabled(true);

        isAttacking = false;
    }

    private bool DealDamageInsideCurrentFire()
    {
        if (fireDamageCollider == null ||
            ownerHealth == null)
        {
            return false;
        }

        Transform colliderTransform =
            fireDamageCollider.transform;

        Vector3 worldCenter =
            colliderTransform.TransformPoint(
                fireDamageCollider.center
            );

        Vector3 scale =
            colliderTransform.lossyScale;

        Vector3 halfExtents =
            Vector3.Scale(
                fireDamageCollider.size * 0.5f,
                new Vector3(
                    Mathf.Abs(scale.x),
                    Mathf.Abs(scale.y),
                    Mathf.Abs(scale.z)
                )
            );

        Collider[] hits =
            Physics.OverlapBox(
                worldCenter,
                halfExtents,
                colliderTransform.rotation,
                hitLayers,
                QueryTriggerInteraction.Ignore
            );

        HashSet<DragonHealth> damagedTargets =
            new HashSet<DragonHealth>();

        bool damagedSomething = false;

        foreach (Collider hit in hits)
        {
            DragonHealth target =
                hit.GetComponentInParent<DragonHealth>();

            if (target == null)
                continue;

            if (target == ownerHealth)
                continue;

            if (target.Team ==
                ownerHealth.Team)
                continue;

            if (target.IsDead)
                continue;

            if (!damagedTargets.Add(target))
                continue;

            target.TakeDamage(
                damagePerTick
            );

            SpawnHitVFX(target);

            PlayFireHitSound();

            // Only shake once during
            // one complete Fire Breath.
            if (!cameraShakeTriggered &&
                cameraShake != null)
            {
                cameraShake.Shake(
                    shakeDuration,
                    shakeStrength
                );

                cameraShakeTriggered = true;
            }

            damagedSomething = true;
        }

        return damagedSomething;
    }

    private void StartFireSound()
    {
        if (fireAudioSource == null ||
            fireBreathSFX == null)
            return;

        fireAudioSource.clip =
            fireBreathSFX;

        fireAudioSource.volume =
            fireBreathVolume;

        fireAudioSource.loop = true;

        fireAudioSource.Play();
    }

    private void StopFireSound()
    {
        if (fireAudioSource != null)
        {
            fireAudioSource.Stop();
        }
    }

    private void PlayFireHitSound()
    {
        if (attackAudioSource == null ||
            fireHitSFX == null)
            return;

        attackAudioSource.PlayOneShot(
            fireHitSFX,
            fireHitVolume
        );
    }

    private void UpdateFireDamageVolume(
        float length)
    {
        if (fireDamageCollider == null)
            return;

        length =
            Mathf.Max(0.05f, length);

        fireDamageCollider.size =
            new Vector3(
                fireWidth,
                fireHeight,
                length
            );

        fireDamageCollider.center =
            new Vector3(
                0f,
                0f,
                length * 0.5f
            );
    }

    private void ResetFireDamageVolume()
    {
        if (fireDamageCollider == null)
            return;

        fireDamageCollider.size =
            new Vector3(
                fireWidth,
                fireHeight,
                0.05f
            );

        fireDamageCollider.center =
            new Vector3(
                0f,
                0f,
                0.025f
            );
    }

    private void SpawnHitVFX(
        DragonHealth target)
    {
        if (hitImpactVFX == null ||
            target == null)
            return;

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

    private void OnDisable()
    {
        StopFireSound();

        if (fireDamageCollider != null)
        {
            fireDamageCollider.enabled = false;
        }

        if (fireVFX != null)
        {
            fireVFX.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }
    }
}