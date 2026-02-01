using UnityEngine;
using TaquizaMadriza.Characters;
using TaquizaMadriza.Combat;

namespace TaquizaMadriza.Utils
{
    /// <summary>
    /// Setup automático del jugador - Ejecuta esto en el editor para configurar todo
    /// </summary>
    [ExecuteInEditMode]
    public class PlayerSetup : MonoBehaviour
    {
        [Header("Configuración")]
        [SerializeField] private int playerNumber = 1;
        
        [ContextMenu("Setup Player")]
        private void Setup()
        {
            Debug.Log($"[PlayerSetup] Configurando Jugador {playerNumber}...");
            
            // Asegurar que tiene Rigidbody
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
            
            // Asegurar componentes necesarios
            if (GetComponent<PlayerStateManager>() == null)
                gameObject.AddComponent<PlayerStateManager>();
            
            if (GetComponent<PlayerHealth>() == null)
                gameObject.AddComponent<PlayerHealth>();
            
            if (GetComponent<PlayerCombat>() == null)
                gameObject.AddComponent<PlayerCombat>();
            
            if (GetComponent<PlayerController>() == null)
                gameObject.AddComponent<PlayerController>();
            
            // Configurar número de jugador en el PlayerController
            var controller = GetComponent<PlayerController>();
            if (controller != null)
            {
                var so = new UnityEditor.SerializedObject(controller);
                so.FindProperty("playerNumber").intValue = playerNumber;
                so.ApplyModifiedProperties();
            }
            
            // Configurar número de jugador en PlayerHealth
            var health = GetComponent<PlayerHealth>();
            if (health != null)
            {
                var so = new UnityEditor.SerializedObject(health);
                so.FindProperty("playerNumber").intValue = playerNumber;
                so.ApplyModifiedProperties();
            }
            
            Debug.Log($"[PlayerSetup] ✓ Jugador {playerNumber} configurado!");
        }
    }
}
