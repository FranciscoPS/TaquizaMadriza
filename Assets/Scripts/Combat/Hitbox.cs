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
            if (!isActive) return;
            
            // Verificar si golpeó a otro jugador
            var targetHealth = other.GetComponent<PlayerHealth>();
            if (targetHealth != null && targetHealth.PlayerNumber != ownerPlayerNumber)
            {
                // Calcular dirección del knockback
                Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                knockbackDirection.y = 0; // Solo knockback horizontal
                
                // Aplicar daño
                targetHealth.TakeDamage(
                    currentAttackData.damage,
                    knockbackDirection * currentAttackData.knockbackForce,
                    currentAttackData.hitstunDuration
                );
            }
        }
    }
}
