using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TaquizaMadriza.Characters;

namespace TaquizaMadriza.Combat
{
    [RequireComponent(typeof(PlayerStateManager))]
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Configuración de Ataques")]
        [SerializeField] private AttackData punchAttack = new AttackData();
        [SerializeField] private AttackData kickAttack = new AttackData();
        
        [Header("Sistema de Combo")]
        [SerializeField] private int maxComboCount = 3;
        [SerializeField] private float comboResetTime = 0.5f;
        
        private PlayerStateManager stateManager;
        private Rigidbody rb;
        private Transform punchHitbox;
        private Transform kickHitbox;
        
        private bool hasUsedAirAttack = false;
        private int currentComboCount = 0;
        private float lastAttackTime = 0f;
        private bool isAttacking = false;
        private Coroutine attackCoroutine;
        private int playerNumber = 1;
        
        private void Awake()
        {
            stateManager = GetComponent<PlayerStateManager>();
            rb = GetComponent<Rigidbody>();
            
            // Buscar o crear hitboxes
            SetupHitboxes();
        }
        
        public void Initialize(int playerNum)
        {
            playerNumber = playerNum;
            
            // Inicializar hitboxes con el número de jugador
            var punchHitboxComp = punchHitbox?.GetComponent<Hitbox>();
            if (punchHitboxComp != null)
                punchHitboxComp.Initialize(playerNumber);
            
            var kickHitboxComp = kickHitbox?.GetComponent<Hitbox>();
            if (kickHitboxComp != null)
                kickHitboxComp.Initialize(playerNumber);
        }
        
        private void SetupHitboxes()
        {
            // Buscar hitboxes existentes
            punchHitbox = transform.Find("PunchHitbox");
            kickHitbox = transform.Find("KickHitbox");
            
            // Crear PunchHitbox si no existe
            if (punchHitbox == null)
            {
                GameObject punch = GameObject.CreatePrimitive(PrimitiveType.Cube);
                punch.name = "PunchHitbox";
                punch.transform.SetParent(transform);
                punch.transform.localPosition = new Vector3(0.8f, 0, 0);
                punch.transform.localScale = new Vector3(0.6f, 0.8f, 0.6f);
                punch.layer = LayerMask.NameToLayer("Hitbox");
                
                var collider = punch.GetComponent<BoxCollider>();
                collider.isTrigger = true;
                
                Destroy(punch.GetComponent<MeshRenderer>());
                punch.AddComponent<Hitbox>();
                punch.SetActive(false);
                
                punchHitbox = punch.transform;
            }
            
            // Crear KickHitbox si no existe
            if (kickHitbox == null)
            {
                GameObject kick = GameObject.CreatePrimitive(PrimitiveType.Cube);
                kick.name = "KickHitbox";
                kick.transform.SetParent(transform);
                kick.transform.localPosition = new Vector3(1.0f, -0.3f, 0);
                kick.transform.localScale = new Vector3(0.8f, 0.6f, 0.6f);
                kick.layer = LayerMask.NameToLayer("Hitbox");
                
                var collider = kick.GetComponent<BoxCollider>();
                collider.isTrigger = true;
                
                Destroy(kick.GetComponent<MeshRenderer>());
                kick.AddComponent<Hitbox>();
                kick.SetActive(false);
                
                kickHitbox = kick.transform;
            }
            
            // Asignar hitboxes a los attack data
            punchAttack.hitboxObject = punchHitbox.gameObject;
            kickAttack.hitboxObject = kickHitbox.gameObject;
            
            // Configurar valores por defecto si no están configurados
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
            // Resetear combo si pasó mucho tiempo
            if (Time.time - lastAttackTime > comboResetTime && currentComboCount > 0)
            {
                ResetCombo();
            }
        }
        
        /// <summary>
        /// Ejecuta un golpe (ataque rápido)
        /// </summary>
        public void Punch(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            // Verificar si puede atacar
            if (!CanAttack()) return;
            
            // Ataque aéreo
            if (stateManager.CurrentState == PlayerState.Jumping)
            {
                if (!hasUsedAirAttack)
                {
                    PerformAirAttack(punchAttack);
                }
                return;
            }
            
            // Ataque en tierra
            PerformGroundAttack(punchAttack);
        }
        
        /// <summary>
        /// Ejecuta una patada (ataque lento)
        /// </summary>
        public void Kick(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            if (!CanAttack()) return;
            
            // Ataque aéreo
            if (stateManager.CurrentState == PlayerState.Jumping)
            {
                if (!hasUsedAirAttack)
                {
                    PerformAirAttack(kickAttack);
                }
                return;
            }
            
            // Ataque en tierra - La patada siempre resetea el combo
            ResetCombo();
            PerformGroundAttack(kickAttack);
        }
        
        private bool CanAttack()
        {
            // No puede atacar si está en hitstun, knockback, muerto, etc
            if (!stateManager.CanAct()) return false;
            
            // No puede atacar si ya está atacando
            if (isAttacking) return false;
            
            return true;
        }
        
        private void PerformGroundAttack(AttackData attackData)
        {
            // Incrementar combo solo para golpes
            if (attackData == punchAttack)
            {
                currentComboCount++;
                
                // Si alcanzó el máximo de combo, el siguiente golpe manda a volar
                if (currentComboCount >= maxComboCount)
                {
                    attackData.knockbackForce *= 2f; // Doble knockback en el tercer golpe
                }
            }
            
            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);
            
            attackCoroutine = StartCoroutine(AttackRoutine(attackData));
        }
        
        private void PerformAirAttack(AttackData attackData)
        {
            hasUsedAirAttack = true;
            
            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);
            
            attackCoroutine = StartCoroutine(AttackRoutine(attackData));
        }
        
        private IEnumerator AttackRoutine(AttackData attackData)
        {
            isAttacking = true;
            stateManager.ChangeState(PlayerState.Attacking);
            lastAttackTime = Time.time;
            
            // Activar hitbox
            if (attackData.hitboxObject != null)
            {
                var hitbox = attackData.hitboxObject.GetComponent<Hitbox>();
                if (hitbox != null)
                {
                    hitbox.ActivateHitbox(attackData);
                }
            }
            
            // Esperar a que termine la animación del ataque
            yield return new WaitForSeconds(attackData.attackCooldown);
            
            isAttacking = false;
            
            // Volver a estado idle o jumping según corresponda
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
            
            // Si completó el combo, resetearlo
            if (currentComboCount >= maxComboCount)
            {
                ResetCombo();
            }
        }
        
        private void ResetCombo()
        {
            currentComboCount = 0;
        }
        
        /// <summary>
        /// Llamar cuando el jugador toca el suelo para resetear el ataque aéreo
        /// </summary>
        public void OnLanded()
        {
            hasUsedAirAttack = false;
        }
        
        private bool IsGrounded()
        {
            // Simple raycast hacia abajo
            return Physics.Raycast(transform.position, Vector3.down, 1.2f);
        }
        
        // Para debug en el inspector
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1.2f);
        }
    }
}
