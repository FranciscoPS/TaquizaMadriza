using TaquizaMadriza.Characters;
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

        [SerializeField]
        private bool keepInitialRotation = true;

        [Header("Sprite Flip")]
        [SerializeField]
        private bool flipSpriteWithMovement = true;

        private Camera mainCamera;
        private Quaternion initialRotation;
        private PlayerController playerController;
        private Vector3 initialScale;

        private void Start()
        {
            mainCamera = Camera.main;
            initialRotation = transform.rotation;
            initialScale = transform.localScale;
            
            playerController = GetComponentInParent<PlayerController>();
        }

        private void LateUpdate()
        {
            if (keepInitialRotation)
            {
                transform.rotation = initialRotation;
            }
            else if (mainCamera != null)
            {
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

            if (flipSpriteWithMovement && playerController != null)
            {
                int facingDir = playerController.GetFacingDirection();
                Vector3 newScale = initialScale;
                newScale.x = Mathf.Abs(initialScale.x) * facingDir;
                transform.localScale = newScale;
            }
        }
    }
}
