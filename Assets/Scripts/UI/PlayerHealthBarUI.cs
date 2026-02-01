using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TaquizaMadriza.Combat;

namespace TaquizaMadriza.UI
{
    public class PlayerHealthBarUI : MonoBehaviour
    {
        [Header("Referencias UI")]
        [SerializeField] private Image healthBarFill;
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private Image playerIcon;
        
        [Header("Configuracion")]
        [SerializeField] private Color healthFullColor = Color.green;
        [SerializeField] private Color healthMidColor = Color.yellow;
        [SerializeField] private Color healthLowColor = Color.red;
        [SerializeField] private float lowHealthThreshold = 0.35f;
        [SerializeField] private float midHealthThreshold = 0.65f;
        
        private PlayerHealth playerHealth;

        public void Initialize(PlayerHealth health, Sprite iconSprite = null)
        {
            playerHealth = health;

            if (playerNameText != null)
            {
                playerNameText.text = $"Player {playerHealth.PlayerNumber}";
            }

            if (playerIcon != null && iconSprite != null)
            {
                playerIcon.sprite = iconSprite;
            }

            playerHealth.OnHealthChanged += UpdateHealthBar;

            UpdateHealthBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        }
        
        private void OnDestroy()
        {
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged -= UpdateHealthBar;
            }
        }

        private void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            if (healthBarFill == null) return;

            float healthPercentage = currentHealth / maxHealth;

            healthBarFill.fillAmount = healthPercentage;

            healthBarFill.color = GetHealthColor(healthPercentage);
        }

        private Color GetHealthColor(float healthPercentage)
        {
            if (healthPercentage <= lowHealthThreshold)
            {
                return healthLowColor;
            }
            else if (healthPercentage <= midHealthThreshold)
            {
                return healthMidColor;
            }
            else
            {
                return healthFullColor;
            }
        }
    }
}
