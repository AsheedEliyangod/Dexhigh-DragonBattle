using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FlyAttackAbility : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private DragonHealth ownerHealth;

    [Tooltip("The Visual object containing the animated dragon.")]
    [SerializeField] private Transform visualRoot;

    [SerializeField] private Transform attackPoint;

    [Header("Damage")]
    [SerializeField] private float initialDamage = 25f;
    [SerializeField] private float extraDamagePerTick = 2f;
    [SerializeField] private int maximumExtraTicks = 3;

    [SerializeField] private float damageTickInterval = 0.12f;
    [SerializeField] private float impactRadius = 2.2f;

    [Header("Flight")]
    [SerializeField] private float flightHeight = 2.5f;
    [SerializeField] private float riseDuration = 0.8f;
    [SerializeField] private float hoverDuration = 0.35f;
    [SerializeField] private float landingDuration = 0.75f;

    [Header("Forward Fly Attack")]
    [SerializeField] private float dashDistance = 4.5f;
    [SerializeField] private float dashDuration = 0.65f;

    [Header("Attack Timing")]
    [SerializeField] private float impactDelay = 0.15f;

    [Header("Cooldown")]
    [SerializeField] private float cooldown = 7f;

    [Header("Detection")]
    [SerializeField] private LayerMask hitLayers = ~0;

    [Header("Main Impact VFX")]
    [SerializeField] private VisualEffect impactVFX;
    [SerializeField] private float impactVFXDuration = 2.5f;

    [Header("Hit VFX")]
    [SerializeField] private GameObject hitImpactVFX;

    [SerializeField] private Vector3 hitVFXOffset =
        new Vector3(0f, 1f, 0f);

    [SerializeField] private float hitVFXLifetime = 2f;

    [Header("Audio")]
    [SerializeField] private AudioSource attackAudioSource;
    [SerializeField] private AudioSource flyAudioSource;

    [SerializeField] private AudioClip takeoffSFX;
    [SerializeField] private AudioClip flyWhooshSFX;
    [SerializeField] private AudioClip impactSFX;
    [SerializeField] private AudioClip landingSFX;

    [Range(0f, 1f)]
    [SerializeField] private float takeoffVolume = 0.7f;

    [Range(0f, 1f)]
    [SerializeField] private float flyVolume = 0.7f;

    [Range(0f, 1f)]
    [SerializeField] private float impactVolume = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float landingVolume = 0.7f;

    [Header("Camera Shake")]
    [SerializeField] private CameraShake cameraShake;

    [SerializeField] private float impactShakeDuration = 0.30f;
    [SerializeField] private float impactShakeStrength = 0.18f;

    private PlayerDragonMovement playerMovement;
    private EnemyDragonAI enemyAI;
    private CharacterController characterController;

    private bool isAttacking;
    private float cooldownRemaining;

    private Vector3 originalVisualLocalPosition;

    private static readonly int TakeoffHash =
        Animator.StringToHash("Takeoff");

    private static readonly int AirAttackHash =
        Animator.StringToHash("AirAttack");

    private static readonly int LandingHash =
        Animator.StringToHash("Landing");

    public float Damage =>
        initialDamage +
        (extraDamagePerTick * maximumExtraTicks);

    public float CooldownDuration => cooldown;
    public float CooldownRemaining => cooldownRemaining;

    public float CooldownNormalized =>
        cooldown > 0f
            ? Mathf.Clamp01(cooldownRemaining / cooldown)
            : 0f;

    public bool IsAttacking => isAttacking;

    public bool IsAirborne { get; private set; }

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

        characterController =
            GetComponent<CharacterController>();

        IsAirborne = false;

        if (visualRoot != null)
        {
            originalVisualLocalPosition =
                visualRoot.localPosition;
        }

        if (impactVFX != null)
        {
            impactVFX.Stop();
        }

        if (flyAudioSource != null)
        {
            flyAudioSource.playOnAwake = false;
            flyAudioSource.loop = true;
        }

        // Automatically find the CameraShake.
        if (cameraShake == null &&
            Camera.main != null)
        {
            cameraShake =
                Camera.main.GetComponent<CameraShake>();
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

        StartCoroutine(PerformFlyAttack());

        return true;
    }

    private IEnumerator PerformFlyAttack()
    {
        isAttacking = true;
        IsAirborne = true;

        cooldownRemaining = cooldown;

        SetMovementEnabled(false);

        // -------------------------
        // TAKEOFF
        // -------------------------

        if (animator != null)
        {
            animator.SetTrigger(TakeoffHash);
        }

        PlayOneShot(
            takeoffSFX,
            takeoffVolume
        );

        yield return StartCoroutine(
            MoveVisualVertically(
                originalVisualLocalPosition.y,
                originalVisualLocalPosition.y + flightHeight,
                riseDuration
            )
        );

        // -------------------------
        // HOVER
        // -------------------------

        yield return new WaitForSeconds(
            hoverDuration
        );

        // -------------------------
        // AIR ATTACK
        // -------------------------

        if (animator != null)
        {
            animator.SetTrigger(AirAttackHash);
        }

        yield return new WaitForSeconds(
            impactDelay
        );

        Vector3 dashDirection =
            transform.forward;

        dashDirection.y = 0f;

        if (dashDirection.sqrMagnitude > 0.001f)
        {
            dashDirection.Normalize();
        }

        StartFlySound();

        yield return StartCoroutine(
            PerformForwardDash(
                dashDirection
            )
        );

        StopFlySound();

        // -------------------------
        // LANDING
        // -------------------------

        if (animator != null)
        {
            animator.SetTrigger(LandingHash);
        }

        PlayOneShot(
            landingSFX,
            landingVolume
        );

        yield return StartCoroutine(
            MoveVisualVertically(
                originalVisualLocalPosition.y + flightHeight,
                originalVisualLocalPosition.y,
                landingDuration
            )
        );

        if (visualRoot != null)
        {
            visualRoot.localPosition =
                originalVisualLocalPosition;
        }

        IsAirborne = false;

        SetMovementEnabled(true);

        isAttacking = false;
    }

    private IEnumerator PerformForwardDash(
        Vector3 direction)
    {
        float elapsed = 0f;

        float speed =
            dashDistance /
            Mathf.Max(
                0.01f,
                dashDuration
            );

        bool initialHitDone = false;

        int extraTicksDone = 0;

        float nextExtraTickTime = 0f;

        HashSet<DragonHealth> initialHitTargets =
            new HashSet<DragonHealth>();

        while (elapsed < dashDuration)
        {
            Vector3 movement =
                direction *
                speed *
                Time.deltaTime;

            if (characterController != null)
            {
                characterController.Move(
                    movement
                );
            }
            else
            {
                transform.position += movement;
            }

            if (impactVFX != null &&
                attackPoint != null)
            {
                impactVFX.transform.position =
                    attackPoint.position;
            }

            List<DragonHealth> targets =
                FindTargetsInImpactArea();

            foreach (DragonHealth target in targets)
            {
                // -------------------------
                // FIRST CONTACT
                // -------------------------

                if (!initialHitTargets.Contains(target))
                {
                    initialHitTargets.Add(target);

                    // Main damage
                    target.TakeDamage(
                        initialDamage
                    );

                    // Hit VFX
                    SpawnHitVFX(target);

                    // Strong impact sound
                    PlayOneShot(
                        impactSFX,
                        impactVolume
                    );

                    // Strong camera shake
                    if (cameraShake != null)
                    {
                        cameraShake.Shake(
                            impactShakeDuration,
                            impactShakeStrength
                        );
                    }

                    initialHitDone = true;

                    if (impactVFX != null)
                    {
                        impactVFX.Reinit();
                        impactVFX.Play();
                    }

                    Debug.Log(
                        $"{ownerHealth.DragonName} hit " +
                        $"{target.DragonName} with Fly Attack " +
                        $"for {initialDamage} damage."
                    );

                    nextExtraTickTime =
                        elapsed +
                        damageTickInterval;
                }

                // -------------------------
                // EXTRA CONTACT DAMAGE
                // -------------------------

                if (initialHitDone &&
                    extraTicksDone <
                    maximumExtraTicks &&
                    elapsed >=
                    nextExtraTickTime)
                {
                    target.TakeDamage(
                        extraDamagePerTick
                    );

                    extraTicksDone++;

                    SpawnHitVFX(target);

                    nextExtraTickTime =
                        elapsed +
                        damageTickInterval;

                    Debug.Log(
                        $"{ownerHealth.DragonName} dealt " +
                        $"{extraDamagePerTick} extra Fly damage " +
                        $"to {target.DragonName}."
                    );
                }
            }

            elapsed += Time.deltaTime;

            yield return null;
        }

        if (impactVFX != null)
        {
            StartCoroutine(
                StopImpactVFXAfterDelay()
            );
        }
    }

    private List<DragonHealth> FindTargetsInImpactArea()
    {
        List<DragonHealth> targets =
            new List<DragonHealth>();

        if (attackPoint == null ||
            ownerHealth == null)
        {
            return targets;
        }

        Collider[] hits =
            Physics.OverlapSphere(
                attackPoint.position,
                impactRadius,
                hitLayers,
                QueryTriggerInteraction.Ignore
            );

        HashSet<DragonHealth> uniqueTargets =
            new HashSet<DragonHealth>();

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

            if (!uniqueTargets.Add(target))
                continue;

            targets.Add(target);
        }

        return targets;
    }

    private void StartFlySound()
    {
        if (flyAudioSource == null ||
            flyWhooshSFX == null)
        {
            return;
        }

        flyAudioSource.clip =
            flyWhooshSFX;

        flyAudioSource.volume =
            flyVolume;

        flyAudioSource.loop = true;

        flyAudioSource.Play();
    }

    private void StopFlySound()
    {
        if (flyAudioSource == null)
            return;

        flyAudioSource.Stop();
    }

    private void PlayOneShot(
        AudioClip clip,
        float volume)
    {
        if (attackAudioSource == null ||
            clip == null)
        {
            return;
        }

        attackAudioSource.PlayOneShot(
            clip,
            volume
        );
    }

    private IEnumerator StopImpactVFXAfterDelay()
    {
        yield return new WaitForSeconds(
            impactVFXDuration
        );

        if (impactVFX != null)
        {
            impactVFX.Stop();
        }
    }

    private IEnumerator MoveVisualVertically(
        float startY,
        float endY,
        float duration)
    {
        if (visualRoot == null)
            yield break;

        Vector3 position =
            visualRoot.localPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t =
                Mathf.Clamp01(
                    elapsed / duration
                );

            t = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            position.y =
                Mathf.Lerp(
                    startY,
                    endY,
                    t
                );

            visualRoot.localPosition =
                position;

            elapsed += Time.deltaTime;

            yield return null;
        }

        position.y = endY;

        visualRoot.localPosition =
            position;
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

    private void OnDisable()
    {
        IsAirborne = false;

        StopFlySound();
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            impactRadius
        );
    }
}