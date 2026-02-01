using System.Collections;
using UnityEngine;

namespace TaquizaMadriza.Combat
{
    [RequireComponent(typeof(Collider))]
    public class Hitbox : MonoBehaviour
    {
        [Header("Configuración")]
        [SerializeField]
        private int ownerPlayerNumber = 1;

        private AttackData currentAttackData;
        private bool isActive = false;

        public int OwnerPlayerNumber => ownerPlayerNumber;

        public void Initialize(int playerNumber)
        {
            ownerPlayerNumber = playerNumber;
        }

        public void ActivateHitbox(AttackData attackData)
        {
            currentAttackData = attackData;
            isActive = true;
            gameObject.SetActive(true);

            StartCoroutine(DeactivateAfterDelay(attackData.hitboxDuration));
        }

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
            if (!isActive)
                return;

            var targetHealth = other.GetComponent<PlayerHealth>();
            if (targetHealth != null && targetHealth.PlayerNumber != ownerPlayerNumber)
            {
                Vector3 knockbackDirection = (
                    other.transform.position - transform.parent.position
                ).normalized;
                knockbackDirection.y = 0;

                targetHealth.TakeDamage(
                    currentAttackData.damage,
                    knockbackDirection * currentAttackData.knockbackForce,
                    currentAttackData.hitstunDuration,
                    currentAttackData.appliesKnockback
                );
            }
        }
    }
}
