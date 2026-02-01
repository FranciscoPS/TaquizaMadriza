using UnityEngine;
using TaquizaMadriza.Combat;

namespace TaquizaMadriza.Environment
{
    /// <summary>
    /// Zona invisible que detecta cuando un jugador sale del ring.
    /// Aplica daño y respawnea al jugador en el centro con invulnerabilidad.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class OutOfBoundsZone : MonoBehaviour
    {
        [Header("Configuración de Penalización")]
        [SerializeField] private float outOfBoundsDamage = 10f;
        [SerializeField] private float respawnInvulnerabilityDuration = 2f;
        
        [Header("Punto de Respawn")]
        [SerializeField] private Transform respawnPoint;
        [SerializeField] private Vector3 respawnPosition = new Vector3(0, 1, 0);
        
        [Header("Debug")]
        [SerializeField] private bool showDebugGizmos = true;
        
        private void Awake()
        {
            // Asegurar que el collider ES un trigger
            Collider col = GetComponent<Collider>();
            col.isTrigger = true;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            // Verificar si es un jugador
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth == null) return;
            
            // Evitar que se active si ya está muerto
            if (playerHealth.IsDead()) return;
            
            Debug.Log($"[OutOfBounds] Jugador {playerHealth.PlayerNumber} salió del ring!");
            
            // Aplicar daño
            playerHealth.TakeDamageFromEnvironment(outOfBoundsDamage);
            
            // Respawnear al jugador
            Vector3 spawnPos = respawnPoint != null ? respawnPoint.position : respawnPosition;
            playerHealth.RespawnAtPosition(spawnPos, respawnInvulnerabilityDuration);
        }
        
        private void OnDrawGizmos()
        {
            if (!showDebugGizmos) return;
            
            // Dibujar el collider en color rojo semi-transparente
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            
            Collider col = GetComponent<Collider>();
            if (col is BoxCollider box)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(box.center, box.size);
            }
            
            // Dibujar punto de respawn
            Vector3 spawnPos = respawnPoint != null ? respawnPoint.position : respawnPosition;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnPos, 0.5f);
            Gizmos.DrawLine(spawnPos, spawnPos + Vector3.up * 2f);
        }
    }
}
