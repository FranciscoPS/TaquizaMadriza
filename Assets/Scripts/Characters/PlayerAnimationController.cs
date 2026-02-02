using UnityEngine;

namespace TaquizaMadriza.Characters
{
    [RequireComponent(typeof(PlayerStateManager))]
    public class PlayerAnimationController : MonoBehaviour
    {
        private Animator animator;
        private PlayerStateManager stateManager;
        private PlayerController controller;

        // Nombres de parámetros del Animator
        private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
        private static readonly int IsIdleHash = Animator.StringToHash("IsIdle");
        private static readonly int KickTriggerHash = Animator.StringToHash("Kick");
        private static readonly int PunchTriggerHash = Animator.StringToHash("Punch");
        private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
        private static readonly int IsHitHash = Animator.StringToHash("IsHit");
        private static readonly int IsDeadHash = Animator.StringToHash("IsDead");

        private void Awake()
        {
            // Buscar el Animator en los hijos (VisualContainer)
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogError(
                    $"[PlayerAnimationController] No se encontró Animator en {gameObject.name} ni en sus hijos. Asegúrate de que VisualContainer tenga el componente Animator."
                );
            }
            
            stateManager = GetComponent<PlayerStateManager>();
            controller = GetComponent<PlayerController>();
        }

        private void Start()
        {
            // Suscribirse a cambios de estado
            if (stateManager != null)
            {
                stateManager.OnStateChanged += HandleStateChanged;
            }
        }

        private void OnDestroy()
        {
            if (stateManager != null)
            {
                stateManager.OnStateChanged -= HandleStateChanged;
            }
        }

        private void HandleStateChanged(PlayerState previousState, PlayerState newState)
        {
            if (animator == null)
                return;

            // Solo actualizar cuando el estado realmente cambia
            if (previousState == newState)
                return;

            Debug.Log($"[AnimController] {gameObject.name}: {previousState} -> {newState}");

            // Desactivar el estado anterior
            switch (previousState)
            {
                case PlayerState.Idle:
                case PlayerState.Grounded:
                    animator.SetBool(IsIdleHash, false);
                    break;

                case PlayerState.Moving:
                    animator.SetBool(IsWalkingHash, false);
                    break;
            }

            // Activar el nuevo estado
            switch (newState)
            {
                case PlayerState.Idle:
                case PlayerState.Grounded:
                    animator.SetBool(IsIdleHash, true);
                    break;

                case PlayerState.Moving:
                    animator.SetBool(IsWalkingHash, true);
                    break;

                case PlayerState.Attacking:
                case PlayerState.Hit:
                case PlayerState.Dead:
                    // Los triggers de ataque se manejan desde PlayerCombat
                    // Hit y Dead requieren parámetros adicionales en el Animator
                    break;
            }
        }

        // Método público para que PlayerCombat active animaciones de ataque
        public void TriggerPunchAnimation()
        {
            if (animator != null)
            {
                animator.SetTrigger(PunchTriggerHash);
            }
        }

        public void TriggerKickAnimation()
        {
            if (animator != null)
            {
                animator.SetTrigger(KickTriggerHash);
            }
        }
    }
}
