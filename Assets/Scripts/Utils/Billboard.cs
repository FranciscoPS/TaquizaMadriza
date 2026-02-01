using UnityEngine;

namespace TaquizaMadriza.Utils
{
    /// <summary>
    /// Hace que un sprite 2D siempre mire hacia la cámara (billboard effect)
    /// Útil para usar sprites 2D en un mundo 3D
    /// </summary>
    public class Billboard : MonoBehaviour
    {
        [Header("Configuración")]
        [SerializeField] private bool freezeXRotation = true;
        [SerializeField] private bool freezeYRotation = false;
        [SerializeField] private bool freezeZRotation = true;
        
        private Camera mainCamera;
        
        private void Start()
        {
            mainCamera = Camera.main;
        }
        
        private void LateUpdate()
        {
            if (mainCamera == null) return;
            
            // Hacer que el sprite mire a la cámara
            Vector3 directionToCamera = mainCamera.transform.position - transform.position;
            
            // Calcular la rotación hacia la cámara
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
            
            // Aplicar restricciones de rotación si están configuradas
            Vector3 euler = targetRotation.eulerAngles;
            
            if (freezeXRotation) euler.x = 0;
            if (freezeYRotation) euler.y = 0;
            if (freezeZRotation) euler.z = 0;
            
            transform.rotation = Quaternion.Euler(euler);
        }
    }
}
