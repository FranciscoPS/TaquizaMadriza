using UnityEngine;

namespace TaquizaMadriza.Characters
{
    [RequireComponent(typeof(PlayerStateManager))]
    public class PlayerAnimationController : MonoBehaviour
    {
        private Animator animator;
        private PlayerStateManager stateManager;
        private PlayerController controller;

        private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
        private static readonly int IsIdleHash = Animator.StringToHash("IsIdle");
        private static readonly int KickTriggerHash = Animator.StringToHash("Kick");
        private static readonly int PunchTriggerHash = Animator.StringToHash("Punch");
        private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
        private static readonly int IsHitHash = Animator.StringToHash("IsHit");
        private static readonly int IsDeadHash = Animator.StringToHash("IsDead");

        private void Awake()
        {
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
                    break;
            }
        }

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
