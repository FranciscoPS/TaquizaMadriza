using UnityEngine;
using System.Collections;

namespace TaquizaMadriza.Combat
{
    /// <summary>
    /// Componente de la hitbox que detecta colisiones con otros jugadores
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Hitbox : MonoBehaviour
    {
        [Header("Configuración")]
        [SerializeField] private int ownerPlayerNumber = 1;  // Número del jugador dueño de esta hitbox
        
        private AttackData currentAttackData;
        private bool isActive = false;
        
        public int OwnerPlayerNumber => ownerPlayerNumber;
        
        public void Initialize(int playerNumber)
        {
            ownerPlayerNumber = playerNumber;
        }
        
        /// <summary>
        /// Activa la hitbox con los datos del ataque
        /// </summary>
        public void ActivateHitbox(AttackData attackData)
        {
            currentAttackData = attackData;
            isActive = true;
            gameObject.SetActive(true);
            
            Debug.Log($"[Hitbox] Activado - Owner: {ownerPlayerNumber}, Position: {transform.position}, AppliesKnockback: {attackData.appliesKnockback}");
            
            // Auto-desactivar después del tiempo configurado
            StartCoroutine(DeactivateAfterDelay(attackData.hitboxDuration));
        }
        
        /// <summary>
        /// Desactiva la hitbox
        /// </summary>
        public void DeactivateHitbox()
        {
            isActive = false;
            gameObject.SetActive(false);
        }
        
        private IEnumerator DeactivateAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            DeactivateHitbox();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"[Hitbox] OnTriggerEnter - isActive: {isActive}, collider: {other.name}");
            
            if (!isActive) return;
            
            // Verificar si golpeó a otro jugador
            var targetHealth = other.GetComponent<PlayerHealth>();
            if (targetHealth != null && targetHealth.PlayerNumber != ownerPlayerNumber)
            {
                // Calcular dirección del knockback desde el atacante (el padre del hitbox)
                Vector3 knockbackDirection = (other.transform.position - transform.parent.position).normalized;
                knockbackDirection.y = 0;
                
                Debug.Log($"[Hitbox] Jugador {ownerPlayerNumber} golpeó a Jugador {targetHealth.PlayerNumber}! Knockback: {currentAttackData.appliesKnockback}, Fuerza: {currentAttackData.knockbackForce}");
                
                // Aplicar daño
                targetHealth.TakeDamage(
                    currentAttackData.damage,
                    knockbackDirection * currentAttackData.knockbackForce,
                    currentAttackData.hitstunDuration,
                    currentAttackData.appliesKnockback
                );
            }
            else
            {
                Debug.Log($"[Hitbox] No es un jugador válido: targetHealth={targetHealth}, playerNumber={targetHealth?.PlayerNumber}");
            }
        }
    }
}
