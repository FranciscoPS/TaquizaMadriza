using UnityEngine;

namespace TaquizaMadriza.Utils
{
    public class Billboard : MonoBehaviour
    {
        [Header("Configuración")]
        [SerializeField]
        private bool freezeXRotation = true;

        [SerializeField]
        private bool freezeYRotation = false;

        [SerializeField]
        private bool freezeZRotation = true;

        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (mainCamera == null)
                return;

            Vector3 directionToCamera = mainCamera.transform.position - transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

            Vector3 euler = targetRotation.eulerAngles;

            if (freezeXRotation)
                euler.x = 0;
            if (freezeYRotation)
                euler.y = 0;
            if (freezeZRotation)
                euler.z = 0;

            transform.rotation = Quaternion.Euler(euler);
        }
    }
}
