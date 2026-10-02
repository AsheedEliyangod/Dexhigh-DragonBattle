using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAbilityCooldownUI : MonoBehaviour
{
    [Header("Abilities")]
    [SerializeField] private TailAttackAbility tailAttack;
    [SerializeField] private FireAttackAbility fireAttack;
    [SerializeField] private FlyAttackAbility flyAttack;

    [Header("Tail Attack UI")]
    [SerializeField] private Image tailCooldownOverlay;
    [SerializeField] private TMP_Text tailCooldownText;

    [Header("Fire Attack UI")]
    [SerializeField] private Image fireCooldownOverlay;
    [SerializeField] private TMP_Text fireCooldownText;

    [Header("Fly Attack UI")]
    [SerializeField] private Image flyCooldownOverlay;
    [SerializeField] private TMP_Text flyCooldownText;

    private void Update()
    {
        UpdateTailAttackUI();
        UpdateFireAttackUI();
        UpdateFlyAttackUI();
    }

    private void UpdateTailAttackUI()
    {
        if (tailAttack == null)
            return;

        UpdateCooldownDisplay(
            tailAttack.CooldownRemaining,
            tailAttack.CooldownDuration,
            tailCooldownOverlay,
            tailCooldownText
        );
    }

    private void UpdateFireAttackUI()
    {
        if (fireAttack == null)
            return;

        UpdateCooldownDisplay(
            fireAttack.CooldownRemaining,
            fireAttack.CooldownDuration,
            fireCooldownOverlay,
            fireCooldownText
        );
    }

    private void UpdateFlyAttackUI()
    {
        if (flyAttack == null)
            return;

        UpdateCooldownDisplay(
            flyAttack.CooldownRemaining,
            flyAttack.CooldownDuration,
            flyCooldownOverlay,
            flyCooldownText
        );
    }

    private void UpdateCooldownDisplay(
        float remaining,
        float duration,
        Image overlay,
        TMP_Text cooldownText
    )
    {
        float normalizedCooldown = 0f;

        if (duration > 0f)
        {
            normalizedCooldown =
                Mathf.Clamp01(
                    remaining / duration
                );
        }

        if (overlay != null)
        {
            overlay.fillAmount =
                normalizedCooldown;

            overlay.enabled =
                remaining > 0.01f;
        }

        if (cooldownText != null)
        {
            if (remaining > 0.01f)
            {
                cooldownText.gameObject.SetActive(true);

                cooldownText.text =
                    Mathf.CeilToInt(remaining)
                    .ToString();
            }
            else
            {
                cooldownText.gameObject.SetActive(false);
            }
        }
    }
}