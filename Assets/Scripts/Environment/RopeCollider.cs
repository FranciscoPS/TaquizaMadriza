using TaquizaMadriza.Characters;
using TaquizaMadriza.Combat;
using UnityEngine;

namespace TaquizaMadriza.Environment
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class RopeCollider : MonoBehaviour
    {
        [Header("Configuración de Rebote")]
        [SerializeField]
        private float bounceForce = 1.5f;

        [SerializeField]
        private float minVerticalVelocity = 3f;

        [SerializeField]
        private float bounceCooldown = 0.3f;

        private PhysicsMaterial bouncyMaterial;
        private float lastBounceTime = -999f;

        private void Awake()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            Collider col = GetComponent<Collider>();
            col.isTrigger = false;

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
            if (Time.time - lastBounceTime < bounceCooldown)
                return;

            PlayerStateManager stateManager =
                collision.gameObject.GetComponent<PlayerStateManager>();
            if (stateManager == null)
                return;

            if (stateManager.CurrentState != PlayerState.Knockback)
                return;

            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb == null)
                return;

            Vector3 normal = collision.contacts[0].normal;

            Vector3 currentVelocity = rb.linearVelocity;
            Vector3 reflectedVelocity = Vector3.Reflect(currentVelocity, normal) * bounceForce;

            if (reflectedVelocity.y < minVerticalVelocity)
            {
                reflectedVelocity.y = minVerticalVelocity;
            }

            rb.linearVelocity = reflectedVelocity;

            collision.gameObject.transform.position += normal * 0.1f;

            lastBounceTime = Time.time;
        }
    }
}
