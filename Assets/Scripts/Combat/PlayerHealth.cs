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
        [SerializeField] private float groundedDuration = 0.5f;
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
        public void TakeDamage(float damage, Vector3 knockbackVelocity, float hitstunDuration, bool applyKnockback = true)
        {
            Debug.Log($"[Health] Jugador {playerNumber} recibe daño: {damage}, Knockback: {applyKnockback}, Velocidad: {knockbackVelocity.magnitude}");
            
            // No recibir daño si está invulnerable o muerto
            if (isInvulnerable || stateManager.CurrentState == PlayerState.Dead)
            {
                Debug.Log($"[Health] Jugador {playerNumber} ignora daño - invulnerable: {isInvulnerable}, muerto: {stateManager.CurrentState == PlayerState.Dead}");
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
            
            // Aplicar knockback y hitstun (o solo hitstun)
            StartCoroutine(HitstunRoutine(applyKnockback ? knockbackVelocity : Vector3.zero, hitstunDuration));
        }
        
        private IEnumerator HitstunRoutine(Vector3 knockbackVelocity, float hitstunDuration)
        {
            // Asegurar que el Rigidbody no sea kinematic
            if (rb.isKinematic)
            {
                rb.isKinematic = false;
            }
            
            // Cambiar a estado Hit
            stateManager.ChangeState(PlayerState.Hit);
            
            // Aplicar knockback si hay
            if (knockbackVelocity.sqrMagnitude > 0.01f)
            {
                // Agregar elevación al knockback para que se vea más dramático
                Vector3 finalKnockback = knockbackVelocity;
                finalKnockback.y = 4.4f; // Elevar al jugador
                rb.linearVelocity = finalKnockback;
                
                // Esperar el hitstun
                yield return new WaitForSeconds(hitstunDuration);
                
                // Cambiar a knockback (siendo empujado, la gravedad lo hará caer)
                stateManager.ChangeState(PlayerState.Knockback);
                
                // Esperar hasta que toque el suelo (la gravedad ya está actuando)
                float airTime = 0f;
                float maxAirTime = 2f;
                while (!Physics.Raycast(transform.position, Vector3.down, 1.2f) && airTime < maxAirTime)
                {
                    airTime += Time.deltaTime;
                    yield return null;
                }
                
                // Caer al suelo
                stateManager.ChangeState(PlayerState.Grounded);
                rb.linearVelocity = Vector3.zero;
                
                // Esperar en el suelo
                yield return new WaitForSeconds(groundedDuration);
                
                // Levantarse con invulnerabilidad
                stateManager.ChangeState(PlayerState.GettingUp);
                StartInvulnerability(invulnerabilityDuration);
                
                // Después de levantarse, volver a idle
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                // Solo hitstun, sin knockback (golpes 1 y 2)
                rb.linearVelocity = Vector3.zero; // Congelar al jugador
                
                // Esperar el hitstun
                yield return new WaitForSeconds(hitstunDuration);
            }
            
            if (stateManager.CurrentState == PlayerState.GettingUp || stateManager.CurrentState == PlayerState.Hit)
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
            // Asegurar que el Rigidbody no sea kinematic
            if (rb.isKinematic)
            {
                rb.isKinematic = false;
            }
            
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
