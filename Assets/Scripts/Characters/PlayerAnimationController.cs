using System.Collections;
using UnityEngine;

namespace TaquizaMadriza.Characters
{
    [RequireComponent(typeof(PlayerStateManager))]
    public class PlayerAnimationController : MonoBehaviour
    {
        private Animator animator;
        private PlayerStateManager stateManager;
        private Coroutine freezeAnimationCoroutine;
        private PlayerController controller;

        private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
        private static readonly int IsIdleHash = Animator.StringToHash("IsIdle");
        private static readonly int IsJumpingHash = Animator.StringToHash("IsJumping");
        private static readonly int KickTriggerHash = Animator.StringToHash("Kick");
        private static readonly int PunchTriggerHash = Animator.StringToHash("Punch");
        private static readonly int ComboCountHash = Animator.StringToHash("ComboCount");
        private static readonly int DamageTriggerHash = Animator.StringToHash("Damage");
        private static readonly int IsDownedHash = Animator.StringToHash("IsDowned");
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

            if (previousState == newState)
                return;

            switch (previousState)
            {
                case PlayerState.Idle:
                    animator.SetBool(IsIdleHash, false);
                    break;

                case PlayerState.Moving:
                    animator.SetBool(IsWalkingHash, false);
                    break;

                case PlayerState.Jumping:
                    animator.SetBool(IsJumpingHash, false);
                    break;

                case PlayerState.Knockback:
                case PlayerState.Grounded:
                    animator.SetBool(IsDownedHash, false);
                    break;

                case PlayerState.Dead:
                    animator.SetBool(IsDeadHash, false);
                    break;
            }

            switch (newState)
            {
                case PlayerState.Idle:
                    animator.SetBool(IsIdleHash, true);
                    animator.speed = 1f;
                    break;

                case PlayerState.Moving:
                    animator.SetBool(IsWalkingHash, true);
                    animator.speed = 1f;
                    break;

                case PlayerState.Hit:
                    animator.SetTrigger(DamageTriggerHash);
                    animator.speed = 1f;
                    break;

                case PlayerState.Jumping:
                    animator.SetBool(IsJumpingHash, true);
                    animator.speed = 1f;
                    break;

                case PlayerState.Knockback:
                    animator.SetBool(IsDownedHash, true);
                    animator.speed = 1f;
                    break;

                case PlayerState.Grounded:
                    if (freezeAnimationCoroutine != null)
                        StopCoroutine(freezeAnimationCoroutine);
                    freezeAnimationCoroutine = StartCoroutine(FreezeDownedAnimation());
                    break;

                case PlayerState.GettingUp:
                    if (freezeAnimationCoroutine != null)
                    {
                        StopCoroutine(freezeAnimationCoroutine);
                        freezeAnimationCoroutine = null;
                    }
                    animator.speed = 1f;
                    break;

                case PlayerState.Dead:
                    animator.SetBool(IsDeadHash, true);
                    animator.speed = 1f;
                    break;

                case PlayerState.Attacking:
                    break;
            }
        }

        public void TriggerPunchAnimation(int comboCount = 0)
        {
            if (animator != null)
            {
                animator.SetInteger(ComboCountHash, comboCount);
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

        private IEnumerator FreezeDownedAnimation()
        {
            yield return null;
            
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Downed"))
            {
                yield return null;
            }
            
            yield return new WaitForSeconds(0.1f);
            
            animator.Play("Downed", -1, 0.99f);
            animator.speed = 0f;
        }
    }
}
