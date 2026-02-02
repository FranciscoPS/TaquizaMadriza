using TaquizaMadriza.Characters;
using TaquizaMadriza.Combat;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TaquizaMadriza.Utils
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
    public class PlayerSetup : MonoBehaviour
    {
        [Header("Configuración")]
        [SerializeField]
        private int playerNumber = 1;

        [ContextMenu("Setup Player")]
        private void Setup()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }

            rb.mass = 1f;
            rb.linearDamping = 0f;
            rb.angularDamping = 0.05f;
            rb.useGravity = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            if (GetComponent<PlayerStateManager>() == null)
                gameObject.AddComponent<PlayerStateManager>();

            if (GetComponent<PlayerHealth>() == null)
                gameObject.AddComponent<PlayerHealth>();

            if (GetComponent<PlayerCombat>() == null)
                gameObject.AddComponent<PlayerCombat>();

            if (GetComponent<PlayerController>() == null)
                gameObject.AddComponent<PlayerController>();

            var controller = GetComponent<PlayerController>();
            if (controller != null)
            {
                var so = new SerializedObject(controller);
                so.FindProperty("playerNumber").intValue = playerNumber;
                so.ApplyModifiedProperties();
            }

            var health = GetComponent<PlayerHealth>();
            if (health != null)
            {
                var so = new SerializedObject(health);
                so.FindProperty("playerNumber").intValue = playerNumber;
                so.ApplyModifiedProperties();
            }
        }
    }
#endif
}
