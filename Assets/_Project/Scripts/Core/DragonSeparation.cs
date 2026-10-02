using UnityEngine;

public class DragonSeparation : MonoBehaviour
{
    [Header("Dragons")]
    [SerializeField] private Transform playerDragon;
    [SerializeField] private Transform enemyDragon;

    [Header("Controllers")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private CharacterController enemyController;

    [Header("Health")]
    [SerializeField] private DragonHealth playerHealth;
    [SerializeField] private DragonHealth enemyHealth;

    [Header("Separation")]
    [SerializeField] private float minimumDistance = 2.4f;

    private void LateUpdate()
    {
        if (playerDragon == null ||
            enemyDragon == null ||
            playerController == null ||
            enemyController == null)
        {
            return;
        }

        // Battle is basically finished once one dies.
        if ((playerHealth != null && playerHealth.IsDead) ||
            (enemyHealth != null && enemyHealth.IsDead))
        {
            return;
        }

        Vector3 difference =
            enemyDragon.position - playerDragon.position;

        // Only care about ground-plane distance.
        difference.y = 0f;

        float distance = difference.magnitude;

        if (distance >= minimumDistance)
            return;

        if (distance <= 0.001f)
            return;

        Vector3 direction =
            difference.normalized;

        float overlap =
            minimumDistance - distance;

        // Move each dragon half of the required distance.
        Vector3 correction =
            direction * (overlap * 0.5f);

        playerController.Move(-correction);
        enemyController.Move(correction);
    }
}