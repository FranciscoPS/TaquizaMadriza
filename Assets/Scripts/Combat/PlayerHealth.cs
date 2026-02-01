using UnityEngine;
using System;
using System.Collections;
using TaquizaMadriza.Characters;

namespace TaquizaMadriza.Combat
{
    /// <summary>
    /// Maneja la vida, daño, invulnerabilidad y muerte del jugador
    /// </summary>
    [RequireComponent(typeof(PlayerStateManager))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Configuración de Vida")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private int playerNumber = 1;
        
        [Header("Knockback")]
        [SerializeField] private float knockbackDuration = 0.5f;
        [SerializeField] private float groundedDuration = 1f;
        [SerializeField] private float invulnerabilityDuration = 1f;
        
        // Referencias (auto-asignadas)
        private PlayerStateManager stateManager;
        private Rigidbody rb;
        private PlayerCombat playerCombat;
        
        // Estado
        private float currentHealth;
        
        // Eventos
        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;
        public event Action<float> OnDamageTaken;
        
        // Estado interno
        private bool isInvulnerable = false;
        private Coroutine invulnerabilityCoroutine;
        
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public int PlayerNumber => playerNumber;
        public bool IsInvulnerable => isInvulnerable;
        
        private void Awake()
        {
            stateManager = GetComponent<PlayerStateManager>();
            rb = GetComponent<Rigidbody>();
            playerCombat = GetComponent<PlayerCombat>();
            
            currentHealth = maxHealth;
        }
        
        public void Initialize(int playerNum)
        {
            playerNumber = playerNum;
            currentHealth = maxHealth;
        }
        
        /// <summary>
        /// Aplica daño al jugador
        /// </summary>
        public void TakeDamage(float damage, Vector3 knockbackVelocity, float hitstunDuration)
        {
            // No recibir daño si está invulnerable o muerto
            if (isInvulnerable || stateManager.CurrentState == PlayerState.Dead)
            {
                return;
            }
            
            // Reducir vida
            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);
            
            // Notificar cambio de vida
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnDamageTaken?.Invoke(damage);
            
            // Verificar muerte
            if (currentHealth <= 0)
            {
                Die();
                return;
            }
            
            // Aplicar knockback y hitstun
            StartCoroutine(HitstunRoutine(knockbackVelocity, hitstunDuration));
        }
        
        private IEnumerator HitstunRoutine(Vector3 knockbackVelocity, float hitstunDuration)
        {
            // Cambiar a estado Hit
            stateManager.ChangeState(PlayerState.Hit);
            
            // Aplicar knockback
            rb.linearVelocity = new Vector3(knockbackVelocity.x, rb.linearVelocity.y, knockbackVelocity.z);
            
            // Esperar el hitstun
            yield return new WaitForSeconds(hitstunDuration);
            
            // Cambiar a knockback (siendo empujado)
            stateManager.ChangeState(PlayerState.Knockback);
            
            // Esperar a que termine el knockback
            yield return new WaitForSeconds(knockbackDuration);
            
            // Caer al suelo
            stateManager.ChangeState(PlayerState.Grounded);
            
            // Esperar en el suelo
            yield return new WaitForSeconds(groundedDuration);
            
            // Levantarse con invulnerabilidad
            stateManager.ChangeState(PlayerState.GettingUp);
            StartInvulnerability(invulnerabilityDuration);
            
            // Después de levantarse, volver a idle
            yield return new WaitForSeconds(0.5f);
            
            if (stateManager.CurrentState == PlayerState.GettingUp)
            {
                stateManager.ChangeState(PlayerState.Idle);
            }
        }
        
        private void StartInvulnerability(float duration)
        {
            if (invulnerabilityCoroutine != null)
                StopCoroutine(invulnerabilityCoroutine);
            
            invulnerabilityCoroutine = StartCoroutine(InvulnerabilityRoutine(duration));
        }
        
        private IEnumerator InvulnerabilityRoutine(float duration)
        {
            isInvulnerable = true;
            
            yield return new WaitForSeconds(duration);
            
            isInvulnerable = false;
        }
        
        private void Die()
        {
            stateManager.ChangeState(PlayerState.Dead);
            OnDeath?.Invoke();
            
            // Desactivar físicas
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        
        /// <summary>
        /// Restaura la vida al máximo (para reiniciar partida)
        /// </summary>
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            isInvulnerable = false;
            rb.isKinematic = false;
            stateManager.ChangeState(PlayerState.Idle);
            
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }
}
