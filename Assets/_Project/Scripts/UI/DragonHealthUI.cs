using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DragonHealthUI : MonoBehaviour
{
    [Header("Dragon")]
    [SerializeField] private DragonHealth dragonHealth;

    [Header("UI")]
    [SerializeField] private Image healthFill;
    [SerializeField] private TMP_Text healthText;

    [Header("Settings")]
    [SerializeField] private bool showHealthText = true;

    private void Start()
    {
        UpdateHealthUI();
    }

    private void Update()
    {
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (dragonHealth == null)
            return;

        float currentHealth = dragonHealth.CurrentHealth;
        float maxHealth = dragonHealth.MaxHealth;

        float normalizedHealth = 0f;

        if (maxHealth > 0f)
        {
            normalizedHealth =
                Mathf.Clamp01(currentHealth / maxHealth);
        }

        // Shrink the colored HP fill.
        if (healthFill != null)
        {
            healthFill.fillAmount = normalizedHealth;
        }

        // Optional numbers: 75 / 100
        if (healthText != null)
        {
            if (showHealthText)
            {
                healthText.text =
                    $"{Mathf.CeilToInt(currentHealth)} / " +
                    $"{Mathf.CeilToInt(maxHealth)}";
            }
            else
            {
                healthText.text = "";
            }
        }
    }
}