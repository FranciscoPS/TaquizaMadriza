using TaquizaMadriza.Combat;
using UnityEngine;

namespace TaquizaMadriza.Environment
{
    [RequireComponent(typeof(Collider))]
    public class OutOfBoundsZone : MonoBehaviour
    {
        [Header("Configuración de Penalización")]
        [SerializeField]
        private float outOfBoundsDamage = 10f;

        [SerializeField]
        private float respawnInvulnerabilityDuration = 2f;

        [Header("Punto de Respawn")]
        [SerializeField]
        private Transform respawnPoint;

        [SerializeField]
        private Vector3 respawnPosition = new Vector3(0, 1, 0);

        [Header("Debug")]
        [SerializeField]
        private bool showDebugGizmos = true;

        private void Awake()
        {
            Collider col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == null)
                return;

            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth == null)
                return;

            if (playerHealth.IsDead())
                return;

            playerHealth.TakeDamageFromEnvironment(outOfBoundsDamage);

            if (!playerHealth.IsDead())
            {
                Vector3 spawnPos = respawnPoint != null ? respawnPoint.position : respawnPosition;
                playerHealth.RespawnAtPosition(spawnPos, respawnInvulnerabilityDuration);
            }
        }

        private void OnDrawGizmos()
        {
            if (!showDebugGizmos)
                return;

            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);

            Collider col = GetComponent<Collider>();
            if (col is BoxCollider box)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(box.center, box.size);
            }

            Vector3 spawnPos = respawnPoint != null ? respawnPoint.position : respawnPosition;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnPos, 0.5f);
            Gizmos.DrawLine(spawnPos, spawnPos + Vector3.up * 2f);
        }
    }
}
