using System.Collections;
using TaquizaMadriza.Characters;
using TaquizaMadriza.Audio;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TaquizaMadriza.Combat
{
    [RequireComponent(typeof(PlayerStateManager))]
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Configuración de Ataques")]
        [SerializeField]
        private AttackData punchAttack = new AttackData();

        [SerializeField]
        private AttackData kickAttack = new AttackData();

        [Header("Sistema de Combo")]
        [SerializeField]
        private int maxComboCount = 3;

        [SerializeField]
        private float comboResetTime = 0.5f;

        private PlayerStateManager stateManager;
        private Rigidbody rb;
        private PlayerController playerController;
        private PlayerAudioController audioController;
        private PlayerAnimationController animationController;
        private Transform punchHitbox;
        private Transform kickHitbox;

        private bool hasUsedAirAttack = false;
        private int currentComboCount = 0;
        private float lastAttackTime = 0f;
        private bool isAttacking = false;
        private Coroutine attackCoroutine;
        private Coroutine comboResetCoroutine;
        private int playerNumber = 1;

        private void Awake()
        {
            stateManager = GetComponent<PlayerStateManager>();
            rb = GetComponent<Rigidbody>();
            audioController = GetComponent<PlayerAudioController>();
            playerController = GetComponent<PlayerController>();
            animationController = GetComponent<PlayerAnimationController>();

            SetupHitboxes();
        }

        public void Initialize(int playerNum)
        {
            playerNumber = playerNum;

            var punchHitboxComp = punchHitbox?.GetComponent<Hitbox>();
            if (punchHitboxComp != null)
                punchHitboxComp.Initialize(playerNumber);

            var kickHitboxComp = kickHitbox?.GetComponent<Hitbox>();
            if (kickHitboxComp != null)
                kickHitboxComp.Initialize(playerNumber);
        }

        private void SetupHitboxes()
        {
            punchHitbox = transform.Find("PunchHitbox");
            kickHitbox = transform.Find("KickHitbox");

            if (punchHitbox == null)
            {
                GameObject punch = GameObject.CreatePrimitive(PrimitiveType.Cube);
                punch.name = "PunchHitbox";
                punch.transform.SetParent(transform);
                punch.transform.localPosition = Vector3.zero;
                punch.transform.localRotation = Quaternion.identity;
                punch.transform.localScale = new Vector3(0.6f, 0.8f, 0.6f);
                punch.layer = LayerMask.NameToLayer("Hitbox");

                var collider = punch.GetComponent<BoxCollider>();
                collider.isTrigger = true;

                // Deshabilitar el MeshRenderer para evitar cubos rosas en build
                var punchRenderer = punch.GetComponent<MeshRenderer>();
                if (punchRenderer != null)
                {
                    punchRenderer.enabled = false;
                }

                punch.AddComponent<Hitbox>();
                punch.SetActive(false);

                punchHitbox = punch.transform;
            }

            if (kickHitbox == null)
            {
                GameObject kick = GameObject.CreatePrimitive(PrimitiveType.Cube);
                kick.name = "KickHitbox";
                kick.transform.SetParent(transform);
                kick.transform.localPosition = Vector3.zero;
                kick.transform.localRotation = Quaternion.identity;
                kick.transform.localScale = new Vector3(0.8f, 0.6f, 0.8f);
                kick.layer = LayerMask.NameToLayer("Hitbox");

                var collider = kick.GetComponent<BoxCollider>();
                collider.isTrigger = true;

                // Deshabilitar el MeshRenderer para evitar cubos rosas en build
                var kickRenderer = kick.GetComponent<MeshRenderer>();
                if (kickRenderer != null)
                {
                    kickRenderer.enabled = false;
                }

                kick.AddComponent<Hitbox>();
                kick.SetActive(false);

                kickHitbox = kick.transform;
            }

            if (punchAttack.damage == 0)
            {
                punchAttack.attackName = "Golpe";
                punchAttack.damage = 10f;
                punchAttack.hitboxDuration = 0.2f;
                punchAttack.attackCooldown = 0.3f;
                punchAttack.knockbackForce = 5f;
                punchAttack.hitstunDuration = 0.3f;
            }

            if (kickAttack.damage == 0)
            {
                kickAttack.attackName = "Patada";
                kickAttack.damage = 15f;
                kickAttack.hitboxDuration = 0.3f;
                kickAttack.attackCooldown = 0.5f;
                kickAttack.knockbackForce = 10f;
                kickAttack.hitstunDuration = 0.5f;
            }
        }

        private void Update()
        {
            if (Time.time - lastAttackTime > comboResetTime && currentComboCount > 0)
            {
                ResetCombo();
            }
        }

        public void Punch(InputAction.CallbackContext context)
        {
            if (this == null || !enabled)
                return;

            if (!context.performed)
                return;

            if (!CanAttack())
                return;

            if (stateManager.CurrentState == PlayerState.Jumping)
            {
                if (!hasUsedAirAttack)
                {
                    PerformAirAttack(punchAttack);
                    
                    if (audioController != null)
                    {
                        audioController.PlayPunchSound();
                    }
                }
                return;
            }

            PerformGroundAttack(punchAttack);

            if (animationController != null)
            {
                animationController.TriggerPunchAnimation(currentComboCount);
            }

            if (audioController != null)
            {
                audioController.PlayPunchSound();
            }
        }

        public void Kick(InputAction.CallbackContext context)
        {
            if (this == null || !enabled)
                return;

            if (!context.performed)
                return;

            if (!CanAttack())
                return;

            if (stateManager.CurrentState == PlayerState.Jumping)
            {
                if (!hasUsedAirAttack)
                {
                    PerformAirAttack(kickAttack);

                    if (audioController != null)
                    {
                        audioController.PlayKickSound();
                    }
                }
                return;
            }

            ResetCombo();
            PerformGroundAttack(kickAttack);

            if (animationController != null)
            {
                animationController.TriggerKickAnimation();
            }

            if (audioController != null)
            {
                audioController.PlayKickSound();
            }
        }

        private bool CanAttack()
        {
            if (!stateManager.CanAct())
                return false;

            if (isAttacking)
                return false;

            return true;
        }

        private void PerformGroundAttack(AttackData attackData)
        {
            if (Time.time - lastAttackTime > comboResetTime)
            {
                currentComboCount = 0;
            }

            if (attackData == punchAttack)
            {
                currentComboCount++;

                if (currentComboCount >= maxComboCount)
                {
                    attackData.appliesKnockback = true;
                    attackData.knockbackForce = 20f;
                }
                else
                {
                    attackData.appliesKnockback = false;
                    attackData.knockbackForce = 0f;
                }
            }
            else if (attackData == kickAttack)
            {
                attackData.appliesKnockback = true;
                attackData.knockbackForce = 15f;
            }

            if (attackCoroutine != null && this != null && gameObject != null)
                StopCoroutine(attackCoroutine);

            attackCoroutine = StartCoroutine(AttackRoutine(attackData));
        }

        private void PerformAirAttack(AttackData attackData)
        {
            hasUsedAirAttack = true;

            if (attackCoroutine != null && this != null && gameObject != null)
                StopCoroutine(attackCoroutine);

            attackCoroutine = StartCoroutine(AttackRoutine(attackData));
        }

        private IEnumerator AttackRoutine(AttackData attackData)
        {
            isAttacking = true;
            stateManager.ChangeState(PlayerState.Attacking);
            lastAttackTime = Time.time;

            Transform hitboxToUse = (attackData == punchAttack) ? punchHitbox : kickHitbox;

            if (hitboxToUse != null)
            {
                var hitbox = hitboxToUse.GetComponent<Hitbox>();
                if (hitbox != null)
                {
                    float distance = (attackData == punchAttack) ? 0.8f : 1.0f;
                    float heightOffset = (attackData == punchAttack) ? 0f : -0.3f;

                    int facingDir = playerController.GetFacingDirection();
                    Vector3 direction = facingDir > 0 ? Vector3.right : Vector3.left;

                    hitboxToUse.position =
                        transform.position + direction * distance + Vector3.up * heightOffset;

                    hitbox.ActivateHitbox(attackData);
                }
            }

            yield return new WaitForSeconds(attackData.attackCooldown);

            isAttacking = false;

            if (stateManager.CurrentState == PlayerState.Attacking)
            {
                if (rb.linearVelocity.y > 0.1f || !IsGrounded())
                {
                    stateManager.ChangeState(PlayerState.Jumping);
                }
                else
                {
                    stateManager.ChangeState(PlayerState.Idle);
                }
            }

            if (currentComboCount >= maxComboCount)
            {
                ResetCombo();
            }
        }

        private void OnDisable()
        {
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }

            if (comboResetCoroutine != null)
            {
                StopCoroutine(comboResetCoroutine);
                comboResetCoroutine = null;
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            attackCoroutine = null;
            comboResetCoroutine = null;
        }

        private void ResetCombo()
        {
            currentComboCount = 0;
        }

        public void OnLanded()
        {
            hasUsedAirAttack = false;
        }

        private bool IsGrounded()
        {
            return Physics.Raycast(transform.position, Vector3.down, 1.2f);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1.2f);
        }
    }
}
