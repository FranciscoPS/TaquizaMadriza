using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TaquizaMadriza.Characters;
using TaquizaMadriza.Combat;

namespace TaquizaMadriza.Characters
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerStateManager))]
    [RequireComponent(typeof(PlayerCombat))]
    [RequireComponent(typeof(PlayerHealth))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movimiento")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float airControl = 0.5f;
        
        [Header("Salto")]
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float maxFallSpeed = -15f;
        
        [Header("Detección de Suelo")]
        [SerializeField] private float groundCheckDistance = 1.2f;
        
        [Header("Configuración")]
        [SerializeField] private int playerNumber = 1;
        
        private Transform groundCheck;
        private Rigidbody rb;
        private PlayerStateManager stateManager;
        private PlayerCombat combat;
        private PlayerHealth health;
        private PlayerInput playerInput;
        
        private Vector2 moveInput;
        private bool isGrounded;
        private bool jumpRequested;
        private int facingDirection = 1;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            stateManager = GetComponent<PlayerStateManager>();
            combat = GetComponent<PlayerCombat>();
            health = GetComponent<PlayerHealth>();
            playerInput = GetComponent<PlayerInput>();
            
            groundCheck = transform.Find("GroundCheck");
            if (groundCheck == null)
            {
                GameObject gc = new GameObject("GroundCheck");
                gc.transform.SetParent(transform);
                gc.transform.localPosition = new Vector3(0, 0.1f, 0);
                groundCheck = gc.transform;
            }
            
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.useGravity = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
        
        private void Start()
        {
            health.Initialize(playerNumber);
            combat.Initialize(playerNumber);
            
            if (playerInput != null)
            {
                var actions = playerInput.actions;
                
                actions["Movement"].performed += OnMove;
                actions["Movement"].canceled += OnMove;
                actions["Jump"].performed += OnJump;
                actions["Punch"].performed += context => combat.Punch(context);
                actions["Kick"].performed += context => combat.Kick(context);
            }
        }
        
        private void OnDestroy()
        {
            if (playerInput != null)
            {
                var actions = playerInput.actions;
                
                actions["Movement"].performed -= OnMove;
                actions["Movement"].canceled -= OnMove;
                actions["Jump"].performed -= OnJump;
                actions["Punch"].performed -= context => combat.Punch(context);
                actions["Kick"].performed -= context => combat.Kick(context);
            }
        }
        
        private void Update()
        {
            CheckGrounded();
        }
        
        private void FixedUpdate()
        {
            // Siempre aplicar gravedad
            ApplyGravity();
            
            // Movimiento: permitido incluso si es invulnerable, pero no en knockback
            if (stateManager.CanMove())
            {
                HandleMovement();
            }
            
            // Salto: solo si puede actuar completamente (no invulnerable en knockback)
            if (stateManager.CanAct())
            {
                HandleJump();
            }
        }
        
        private void CheckGrounded()
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
            
            if (isGrounded && stateManager.CurrentState == PlayerState.Jumping)
            {
                stateManager.ChangeState(PlayerState.Idle);
                combat.OnLanded();
            }
        }
        
        private void HandleMovement()
        {
            if (moveInput.sqrMagnitude < 0.01f)
            {
                Vector3 velocity = rb.linearVelocity;
                velocity.x = 0;
                velocity.z = 0;
                rb.linearVelocity = velocity;
                
                if (isGrounded && stateManager.CurrentState == PlayerState.Moving)
                {
                    stateManager.ChangeState(PlayerState.Idle);
                }
                return;
            }
            
            Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            float currentSpeed = isGrounded ? moveSpeed : moveSpeed * airControl;
            
            Vector3 targetVelocity = moveDirection * currentSpeed;
            targetVelocity.y = rb.linearVelocity.y;
            
            rb.linearVelocity = targetVelocity;
            
            if (isGrounded && stateManager.CurrentState == PlayerState.Idle)
            {
                stateManager.ChangeState(PlayerState.Moving);
            }
            
            if (moveDirection.sqrMagnitude > 0.01f)
            {
                // Actualizar dirección horizontal: usar el componente X del moveDirection en mundo
                // Esto asegura que la dirección se base en el movimiento real, no solo en input
                if (Mathf.Abs(moveDirection.x) > 0.2f)
                {
                    facingDirection = moveDirection.x > 0 ? 1 : -1;
                }
                
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            }
        }
        
        private void HandleJump()
        {
            if (jumpRequested && isGrounded)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
                stateManager.ChangeState(PlayerState.Jumping);
            }
            
            jumpRequested = false;
        }
        
        private void ApplyGravity()
        {
            if (!isGrounded)
            {
                Vector3 velocity = rb.linearVelocity;
                velocity.y += gravity * Time.fixedDeltaTime;
                velocity.y = Mathf.Max(velocity.y, maxFallSpeed);
                rb.linearVelocity = velocity;
            }
        }
        
        private void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed || context.started)
            {
                moveInput = context.ReadValue<Vector2>();
            }
            else if (context.canceled)
            {
                moveInput = Vector2.zero;
            }
        }
        
        private void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                jumpRequested = true;
            }
        }
        
        public int GetFacingDirection()
        {
            return facingDirection;
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Vector3 start = Application.isPlaying ? transform.position : transform.position;
            Gizmos.DrawLine(start, start + Vector3.down * groundCheckDistance);
        }
    }
}
