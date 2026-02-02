using System.Collections;
using System.Collections.Generic;
using TaquizaMadriza.Combat;
using UnityEngine;

namespace TaquizaMadriza.Characters
{
    [RequireComponent(typeof(PlayerHealth))]
    public class PlayerVisualEffects : MonoBehaviour
    {
        [Header("Parpadeo de Invulnerabilidad")]
        [SerializeField]
        private float invulnerabilityBlinkRate = 0.1f;

        [SerializeField]
        private Color invulnerabilityColor = new Color(0.5f, 1f, 1f, 1f); // Cyan claro

        [Header("Parpadeo de Vida Baja")]
        [SerializeField]
        private float lowHealthThreshold = 0.35f;

        [SerializeField]
        private float lowHealthBlinkRate = 0.3f;

        [SerializeField]
        private Color lowHealthColor = Color.red;

        [Header("Diferenciación de Jugadores")]
        [SerializeField]
        private bool applyPlayerColorTint = true;

        [SerializeField]
        private Color playerTint = Color.white; // Tint de este jugador específico

        private PlayerHealth health;
        private List<Renderer> playerRenderers = new List<Renderer>();
        private Dictionary<Renderer, Color> originalColors =
            new Dictionary<Renderer, Color>();

        private Coroutine invulnerabilityBlinkCoroutine;
        private Coroutine lowHealthBlinkCoroutine;

        private void Awake()
        {
            health = GetComponent<PlayerHealth>();

            // Buscar MeshRenderers
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in meshRenderers)
            {
                if (
                    !renderer.gameObject.name.Contains("Hitbox")
                    && !renderer.gameObject.name.Contains("hitbox")
                    && renderer.gameObject.layer != LayerMask.NameToLayer("Hitbox")
                )
                {
                    playerRenderers.Add(renderer);
                    if (renderer.material != null)
                    {
                        originalColors[renderer] = renderer.material.color;
                    }
                }
            }

            // Buscar SpriteRenderers
            SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (var renderer in spriteRenderers)
            {
                if (
                    !renderer.gameObject.name.Contains("Hitbox")
                    && !renderer.gameObject.name.Contains("hitbox")
                    && renderer.gameObject.layer != LayerMask.NameToLayer("Hitbox")
                )
                {
                    playerRenderers.Add(renderer);
                    originalColors[renderer] = renderer.color;
                }
            }

            // Aplicar tint de color según el jugador
            if (applyPlayerColorTint)
            {
                ApplyPlayerTint();
            }
        }

        private void Start()
        {
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
                // Detener parpadeo de vida baja si existe, la invulnerabilidad tiene prioridad
                if (lowHealthBlinkCoroutine != null)
                {
                    StopLowHealthBlink();
                }
                StartInvulnerabilityBlink();
            }
            else
            {
                StopInvulnerabilityBlink();

                // Restaurar parpadeo de vida baja si corresponde
                float healthPercentage = health.CurrentHealth / health.MaxHealth;
                if (healthPercentage <= lowHealthThreshold && healthPercentage > 0)
                {
                    StartLowHealthBlink();
                }
            }
        }

        private void HandleHealthChanged(float currentHealth, float maxHealth)
        {
            float healthPercentage = currentHealth / maxHealth;

            if (healthPercentage <= lowHealthThreshold && healthPercentage > 0)
            {
                if (lowHealthBlinkCoroutine == null)
                {
                    StopInvulnerabilityBlink();
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
            invulnerabilityBlinkCoroutine = StartCoroutine(
                BlinkRoutine(invulnerabilityBlinkRate, invulnerabilityColor)
            );
        }

        private void StopInvulnerabilityBlink()
        {
            if (invulnerabilityBlinkCoroutine != null)
            {
                StopCoroutine(invulnerabilityBlinkCoroutine);
                invulnerabilityBlinkCoroutine = null;
            }
            if (lowHealthBlinkCoroutine == null)
            {
                RestoreOriginalColors();
            }
        }

        private void StartLowHealthBlink()
        {
            if (lowHealthBlinkCoroutine != null)
            {
                StopCoroutine(lowHealthBlinkCoroutine);
            }
            lowHealthBlinkCoroutine = StartCoroutine(
                BlinkRoutine(lowHealthBlinkRate, lowHealthColor)
            );
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
                SetRenderersColor(blinkColor);
                yield return new WaitForSeconds(blinkRate);

                RestoreOriginalColors();
                yield return new WaitForSeconds(blinkRate);
            }
        }

        private void SetRenderersColor(Color color)
        {
            foreach (var renderer in playerRenderers)
            {
                if (renderer == null)
                    continue;

                if (renderer is MeshRenderer meshRenderer && meshRenderer.material != null)
                {
                    meshRenderer.material.color = color;
                }
                else if (renderer is SpriteRenderer spriteRenderer)
                {
                    spriteRenderer.color = color;
                }
            }
        }

        private void RestoreOriginalColors()
        {
            foreach (var kvp in originalColors)
            {
                if (kvp.Key == null)
                    continue;

                if (kvp.Key is MeshRenderer meshRenderer && meshRenderer.material != null)
                {
                    meshRenderer.material.color = kvp.Value;
                }
                else if (kvp.Key is SpriteRenderer spriteRenderer)
                {
                    spriteRenderer.color = kvp.Value;
                }
            }
        }

        private void ApplyPlayerTint()
        {
            // Crear lista temporal para evitar modificar el diccionario durante la iteración
            List<KeyValuePair<Renderer, Color>> updates = new List<KeyValuePair<Renderer, Color>>();

            foreach (var kvp in originalColors)
            {
                if (kvp.Key == null)
                    continue;

                Color newColor = kvp.Value * playerTint;
                updates.Add(new KeyValuePair<Renderer, Color>(kvp.Key, newColor));

                if (kvp.Key is MeshRenderer meshRenderer && meshRenderer.material != null)
                {
                    meshRenderer.material.color = newColor;
                }
                else if (kvp.Key is SpriteRenderer spriteRenderer)
                {
                    spriteRenderer.color = newColor;
                }
            }

            // Actualizar el diccionario con los nuevos colores
            foreach (var update in updates)
            {
                originalColors[update.Key] = update.Value;
            }
        }
    }
}