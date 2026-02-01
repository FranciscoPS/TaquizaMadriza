using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TaquizaMadriza.Combat;

namespace TaquizaMadriza.Characters
{
    /// <summary>
    /// Maneja efectos visuales del jugador: parpadeo de invulnerabilidad y bajo HP
    /// </summary>
    [RequireComponent(typeof(PlayerHealth))]
    public class PlayerVisualEffects : MonoBehaviour
    {
        [Header("Parpadeo de Invulnerabilidad")]
        [SerializeField] private float invulnerabilityBlinkRate = 0.1f;
        [SerializeField] private Color invulnerabilityColor = Color.white;
        
        [Header("Parpadeo de Vida Baja")]
        [SerializeField] private float lowHealthThreshold = 0.15f; // 15%
        [SerializeField] private float lowHealthBlinkRate = 0.3f;
        [SerializeField] private Color lowHealthColor = Color.red;
        
        private PlayerHealth health;
        private List<MeshRenderer> playerRenderers = new List<MeshRenderer>();
        private Dictionary<MeshRenderer, Color> originalColors = new Dictionary<MeshRenderer, Color>();
        
        private Coroutine invulnerabilityBlinkCoroutine;
        private Coroutine lowHealthBlinkCoroutine;
        
        private void Awake()
        {
            health = GetComponent<PlayerHealth>();
            
            // Obtener todos los MeshRenderers del jugador (excluyendo hitboxes)
            MeshRenderer[] allRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in allRenderers)
            {
                // Excluir hitboxes y cualquier objeto con "Hitbox" en el nombre
                if (!renderer.gameObject.name.Contains("Hitbox") && 
                    !renderer.gameObject.name.Contains("hitbox") &&
                    renderer.gameObject.layer != LayerMask.NameToLayer("Hitbox"))
                {
                    playerRenderers.Add(renderer);
                    
                    // Guardar color original
                    if (renderer.material != null)
                    {
                        originalColors[renderer] = renderer.material.color;
                    }
                }
            }
        }
        
        private void Start()
        {
            // Suscribirse a eventos de salud
            health.OnInvulnerabilityChanged += HandleInvulnerabilityChanged;
            health.OnHealthChanged += HandleHealthChanged;
        }
        
        private void OnDestroy()
        {
            if (health != null)
            {
                health.OnInvulnerabilityChanged -= HandleInvulnerabilityChanged;
                health.OnHealthChanged -= HandleHealthChanged;
            }
        }
        
        private void HandleInvulnerabilityChanged(bool isInvulnerable)
        {
            if (isInvulnerable)
            {
                StartInvulnerabilityBlink();
            }
            else
            {
                StopInvulnerabilityBlink();
            }
        }
        
        private void HandleHealthChanged(float currentHealth, float maxHealth)
        {
            float healthPercentage = currentHealth / maxHealth;
            
            if (healthPercentage <= lowHealthThreshold)
            {
                // Solo iniciar parpadeo de vida baja si no está ya activo
                if (lowHealthBlinkCoroutine == null)
                {
                    StartLowHealthBlink();
                }
            }
            else
            {
                StopLowHealthBlink();
            }
        }
        
        private void StartInvulnerabilityBlink()
        {
            if (invulnerabilityBlinkCoroutine != null)
            {
                StopCoroutine(invulnerabilityBlinkCoroutine);
            }
            invulnerabilityBlinkCoroutine = StartCoroutine(BlinkRoutine(invulnerabilityBlinkRate, invulnerabilityColor));
        }
        
        private void StopInvulnerabilityBlink()
        {
            if (invulnerabilityBlinkCoroutine != null)
            {
                StopCoroutine(invulnerabilityBlinkCoroutine);
                invulnerabilityBlinkCoroutine = null;
            }
            RestoreOriginalColors();
        }
        
        private void StartLowHealthBlink()
        {
            if (lowHealthBlinkCoroutine != null)
            {
                StopCoroutine(lowHealthBlinkCoroutine);
            }
            lowHealthBlinkCoroutine = StartCoroutine(BlinkRoutine(lowHealthBlinkRate, lowHealthColor));
        }
        
        private void StopLowHealthBlink()
        {
            if (lowHealthBlinkCoroutine != null)
            {
                StopCoroutine(lowHealthBlinkCoroutine);
                lowHealthBlinkCoroutine = null;
            }
            RestoreOriginalColors();
        }
        
        private IEnumerator BlinkRoutine(float blinkRate, Color blinkColor)
        {
            while (true)
            {
                // Cambiar a color de parpadeo
                SetRenderersColor(blinkColor);
                yield return new WaitForSeconds(blinkRate);
                
                // Restaurar color original
                RestoreOriginalColors();
                yield return new WaitForSeconds(blinkRate);
            }
        }
        
        private void SetRenderersColor(Color color)
        {
            foreach (var renderer in playerRenderers)
            {
                if (renderer != null && renderer.material != null)
                {
                    renderer.material.color = color;
                }
            }
        }
        
        private void RestoreOriginalColors()
        {
            foreach (var kvp in originalColors)
            {
                if (kvp.Key != null && kvp.Key.material != null)
                {
                    kvp.Key.material.color = kvp.Value;
                }
            }
        }
    }
}
