using System.Collections;
using UnityEngine;

public class BattleEndController : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private DragonHealth playerHealth;
    [SerializeField] private DragonHealth enemyHealth;

    [Header("Player")]
    [SerializeField] private PlayerDragonMovement playerMovement;
    [SerializeField] private PlayerAbilityInput playerAbilityInput;

    [SerializeField] private TailAttackAbility playerTailAttack;
    [SerializeField] private FireAttackAbility playerFireAttack;
    [SerializeField] private FlyAttackAbility playerFlyAttack;

    [Header("Enemy")]
    [SerializeField] private EnemyDragonAI enemyAI;

    [SerializeField] private TailAttackAbility enemyTailAttack;
    [SerializeField] private FireAttackAbility enemyFireAttack;

    [Header("Result UI")]
    [SerializeField] private BattleResultUI resultUI;

    [Header("Battle End Timing")]
    [SerializeField] private float resultDelay = 2.5f;

    private bool battleEnded;

    public bool BattleEnded => battleEnded;

    private void Update()
    {
        if (battleEnded)
            return;

        if (playerHealth == null || enemyHealth == null)
            return;

        // Both dragons died
        if (playerHealth.IsDead && enemyHealth.IsDead)
        {
            StartCoroutine(
                EndBattleSequence(BattleResult.Draw)
            );

            return;
        }

        // Player died
        if (playerHealth.IsDead)
        {
            StartCoroutine(
                EndBattleSequence(BattleResult.EnemyWin)
            );

            return;
        }

        // Enemy died
        if (enemyHealth.IsDead)
        {
            StartCoroutine(
                EndBattleSequence(BattleResult.PlayerWin)
            );
        }
    }

    private IEnumerator EndBattleSequence(
        BattleResult result)
    {
        // Prevent this sequence from starting again.
        battleEnded = true;

        // Stop all combat immediately.
        StopBattleGameplay();

        // Give the death animation time to play.
        yield return new WaitForSeconds(resultDelay);

        // Show the result only after the delay.
        if (resultUI == null)
            yield break;

        switch (result)
        {
            case BattleResult.PlayerWin:
                resultUI.ShowPlayerWin();
                break;

            case BattleResult.EnemyWin:
                resultUI.ShowEnemyWin();
                break;

            case BattleResult.Draw:
                resultUI.ShowDraw();
                break;
        }
    }

    private void StopBattleGameplay()
    {
        // -------------------------
        // PLAYER
        // -------------------------

        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(false);
            playerMovement.enabled = false;
        }

        if (playerAbilityInput != null)
        {
            playerAbilityInput.enabled = false;
        }

        StopAbility(playerTailAttack);
        StopAbility(playerFireAttack);
        StopAbility(playerFlyAttack);

        // -------------------------
        // ENEMY
        // -------------------------

        if (enemyAI != null)
        {
            enemyAI.SetMovementEnabled(false);
            enemyAI.enabled = false;
        }

        StopAbility(enemyTailAttack);
        StopAbility(enemyFireAttack);
    }

    private void StopAbility(
        MonoBehaviour ability)
    {
        if (ability == null)
            return;

        ability.StopAllCoroutines();
        ability.enabled = false;
    }

    private enum BattleResult
    {
        PlayerWin,
        EnemyWin,
        Draw
    }
}