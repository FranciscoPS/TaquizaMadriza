using UnityEngine;
using TaquizaMadriza.Characters;
using TaquizaMadriza.Combat;

namespace TaquizaMadriza.Environment
{
    /// <summary>
    /// Script para cuerdas del ring.
    /// Rebota a los jugadores que están en knockback (volando).
    /// Agregar a un GameObject con Cube y Rigidbody.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class RopeCollider : MonoBehaviour
    {
        [Header("Configuración de Rebote")]
        [SerializeField] private float bounceForce = 1.5f; // Multiplicador de la fuerza de rebote
        [SerializeField] private float minVerticalVelocity = 3f; // Velocidad vertical mínima al rebotar
        [SerializeField] private float bounceCooldown = 0.3f; // Tiempo entre rebotes
        
        private PhysicsMaterial bouncyMaterial;
        private float lastBounceTime = -999f;
        
        private void Awake()
        {
            // Configurar el Rigidbody de la cuerda (estático)
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            
            // Asegurar que el collider NO es trigger (debe ser sólido)
            Collider col = GetComponent<Collider>();
            col.isTrigger = false;
            
            // Crear material físico con rebote
            bouncyMaterial = new PhysicsMaterial("RopeMaterial");
            bouncyMaterial.bounciness = 0.6f;
            bouncyMaterial.frictionCombine = PhysicsMaterialCombine.Minimum;
            bouncyMaterial.bounceCombine = PhysicsMaterialCombine.Maximum;
            bouncyMaterial.dynamicFriction = 0.05f;
            bouncyMaterial.staticFriction = 0.05f;
            
            col.material = bouncyMaterial;
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            // Cooldown para evitar rebotes múltiples
            if (Time.time - lastBounceTime < bounceCooldown)
                return;
            
            // Verificar si es un jugador
            PlayerStateManager stateManager = collision.gameObject.GetComponent<PlayerStateManager>();
            if (stateManager == null) return;
            
            // Solo rebotar si está en knockback (volando)
            if (stateManager.CurrentState != PlayerState.Knockback) return;
            
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb == null) return;
            
            // Obtener la normal de la superficie de colisión
            Vector3 normal = collision.contacts[0].normal;
            
            // Calcular velocidad reflejada (rebote) con fuerza extra
            Vector3 currentVelocity = rb.linearVelocity;
            Vector3 reflectedVelocity = Vector3.Reflect(currentVelocity, normal) * bounceForce;
            
            // Asegurar que mantiene elevación vertical
            if (reflectedVelocity.y < minVerticalVelocity)
            {
                reflectedVelocity.y = minVerticalVelocity;
            }
            
            // Aplicar nueva velocidad de rebote
            rb.linearVelocity = reflectedVelocity;
            
            // Empujar al jugador ligeramente fuera del collider para evitar solapamiento
            collision.gameObject.transform.position += normal * 0.1f;
            
            // Actualizar tiempo del último rebote
            lastBounceTime = Time.time;
            
            Debug.Log($"[Rope] Jugador rebotado - Velocidad original: {currentVelocity.magnitude:F1}, Rebote: {reflectedVelocity.magnitude:F1}");
        }
    }
}
