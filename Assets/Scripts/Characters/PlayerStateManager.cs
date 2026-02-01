using UnityEngine;
using System;

namespace TaquizaMadriza.Characters
{
    public class PlayerStateManager : MonoBehaviour
    {
        [Header("Estado Actual")]
        [SerializeField] private PlayerState currentState = PlayerState.Idle;
        
        public event Action<PlayerState, PlayerState> OnStateChanged;
        
        public PlayerState CurrentState => currentState;
        
        public void ChangeState(PlayerState newState)
        {
            if (currentState == newState) return;
            
            PlayerState previousState = currentState;
            currentState = newState;
            
            OnStateChanged?.Invoke(previousState, newState);
        }
        
        public bool CanAct()
        {
            return currentState != PlayerState.Hit &&
                   currentState != PlayerState.Knockback &&
                   currentState != PlayerState.Grounded &&
                   currentState != PlayerState.Dead;
        }
        
        /// <summary>
        /// Verifica si el jugador puede moverse (caminar). 
        /// No puede moverse en Knockback (volando) o estados de stun, pero SÍ puede moverse cuando está invulnerable en el suelo.
        /// </summary>
        public bool CanMove()
        {
            return currentState != PlayerState.Hit &&
                   currentState != PlayerState.Knockback &&
                   currentState != PlayerState.Grounded &&
                   currentState != PlayerState.GettingUp &&
                   currentState != PlayerState.Dead;
        }
        
        public bool IsGrounded()
        {
            return currentState != PlayerState.Jumping;
        }
        
        public bool IsAttacking()
        {
            return currentState == PlayerState.Attacking;
        }
        
        public bool IsInvulnerable()
        {
            return currentState == PlayerState.GettingUp;
        }
    }
}
