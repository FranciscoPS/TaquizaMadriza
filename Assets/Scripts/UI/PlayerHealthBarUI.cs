using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TaquizaMadriza.Combat;

namespace TaquizaMadriza.UI
{
    /// <summary>
    /// Controla la UI de la barra de vida de un jugador individual
    /// </summary>
    public class PlayerHealthBarUI : MonoBehaviour
    {
        [Header("Referencias UI")]
        [SerializeField] private Image healthBarFill;
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private Image playerIcon;
        
        [Header("Configuración")]
        [SerializeField] private Color healthFullColor = Color.green;
        [SerializeField] private Color healthMidColor = Color.yellow;
        [SerializeField] private Color healthLowColor = Color.red;
        [SerializeField] private float lowHealthThreshold = 0.35f;
        [SerializeField] private float midHealthThreshold = 0.65f;
        
        private PlayerHealth playerHealth;
        
        /// <summary>
        /// Inicializa la barra de vida para un jugador específico
        /// </summary>
        public void Initialize(PlayerHealth health, Sprite iconSprite = null)
        {
            playerHealth = health;
            
            // Configurar nombre del jugador
            if (playerNameText != null)
            {
                playerNameText.text = $"Player {playerHealth.PlayerNumber}";
            }
            
            // Configurar icono si se proporciona
            if (playerIcon != null && iconSprite != null)
            {
                playerIcon.sprite = iconSprite;
            }
            
            // Suscribirse al evento de cambio de salud
            playerHealth.OnHealthChanged += UpdateHealthBar;
            
            // Actualizar barra inicial
            UpdateHealthBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        }
        
        private void OnDestroy()
        {
            // Desuscribirse del evento
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged -= UpdateHealthBar;
            }
        }
        
        /// <summary>
        /// Actualiza la barra de vida visualmente
        /// </summary>
        private void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            if (healthBarFill == null) return;
            
            // Calcular porcentaje de vida
            float healthPercentage = currentHealth / maxHealth;
            
            // Actualizar fill amount
            healthBarFill.fillAmount = healthPercentage;
            
            // Actualizar color según porcentaje
            healthBarFill.color = GetHealthColor(healthPercentage);
        }
        
        /// <summary>
        /// Obtiene el color según el porcentaje de vida
        /// </summary>
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
