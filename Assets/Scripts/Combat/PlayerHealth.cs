using System;
using System.Collections;
using TaquizaMadriza.Characters;
using UnityEngine;

namespace TaquizaMadriza.Combat
{
    [RequireComponent(typeof(PlayerStateManager))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Configuración de Vida")]
        [SerializeField]
        private float maxHealth = 100f;

        [SerializeField]
        private int playerNumber = 1;

        [Header("Knockback")]
        [SerializeField]
        private float groundedDuration = 0.5f;

        [SerializeField]
        private float invulnerabilityDuration = 1f;

        private PlayerStateManager stateManager;
        private Rigidbody rb;
        private PlayerCombat playerCombat;

        private float currentHealth;

        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;
        public event Action<float> OnDamageTaken;
        public event Action<bool> OnInvulnerabilityChanged;

        private bool isInvulnerable = false;
        private Coroutine invulnerabilityCoroutine;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public int PlayerNumber => playerNumber;
        public bool IsInvulnerable => isInvulnerable;

        public bool IsDead() => stateManager.CurrentState == PlayerState.Dead;

        private void Awake()
        {
            stateManager = GetComponent<PlayerStateManager>();
            rb = GetComponent<Rigidbody>();
            playerCombat = GetComponent<PlayerCombat>();

            currentHealth = maxHealth;
        }

        public void Initialize(int playerNum)
        {
            playerNumber = playerNum;
            currentHealth = maxHealth;
        }

        public void TakeDamage(
            float damage,
            Vector3 knockbackVelocity,
            float hitstunDuration,
            bool applyKnockback = true
        )
        {
            if (isInvulnerable || stateManager.CurrentState == PlayerState.Dead)
            {
                return;
            }

            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);

            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnDamageTaken?.Invoke(damage);

            if (currentHealth <= 0)
            {
                Die();
                return;
            }

            StartCoroutine(
                HitstunRoutine(applyKnockback ? knockbackVelocity : Vector3.zero, hitstunDuration)
            );
        }

        private IEnumerator HitstunRoutine(Vector3 knockbackVelocity, float hitstunDuration)
        {
            if (rb.isKinematic)
            {
                rb.isKinematic = false;
            }

            stateManager.ChangeState(PlayerState.Hit);

            if (knockbackVelocity.sqrMagnitude > 0.01f)
            {
                Vector3 finalKnockback = knockbackVelocity;
                finalKnockback.y = 8f;
                rb.linearVelocity = finalKnockback;

                yield return new WaitForSeconds(hitstunDuration);

                stateManager.ChangeState(PlayerState.Knockback);

                float airTime = 0f;
                float maxAirTime = 2f;
                while (
                    !Physics.Raycast(transform.position, Vector3.down, 1.2f) && airTime < maxAirTime
                )
                {
                    airTime += Time.deltaTime;
                    yield return null;
                }

                stateManager.ChangeState(PlayerState.Grounded);
                rb.linearVelocity = Vector3.zero;

                yield return new WaitForSeconds(groundedDuration);

                stateManager.ChangeState(PlayerState.GettingUp);
                StartInvulnerability(invulnerabilityDuration);

                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                rb.linearVelocity = Vector3.zero;

                yield return new WaitForSeconds(hitstunDuration);
            }

            if (
                stateManager.CurrentState == PlayerState.GettingUp
                || stateManager.CurrentState == PlayerState.Hit
            )
            {
                stateManager.ChangeState(PlayerState.Idle);
            }
        }

        private void StartInvulnerability(float duration)
        {
            if (invulnerabilityCoroutine != null)
                StopCoroutine(invulnerabilityCoroutine);

            invulnerabilityCoroutine = StartCoroutine(InvulnerabilityRoutine(duration));
        }

        private IEnumerator InvulnerabilityRoutine(float duration)
        {
            isInvulnerable = true;
            OnInvulnerabilityChanged?.Invoke(true);

            yield return new WaitForSeconds(duration);

            isInvulnerable = false;
            OnInvulnerabilityChanged?.Invoke(false);
        }

        private void Die()
        {
            if (rb.isKinematic)
            {
                rb.isKinematic = false;
            }

            stateManager.ChangeState(PlayerState.Dead);
            OnDeath?.Invoke();

            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        public void ResetHealth()
        {
            currentHealth = maxHealth;
            isInvulnerable = false;
            rb.isKinematic = false;
            stateManager.ChangeState(PlayerState.Idle);

            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void TakeDamageFromEnvironment(float damage)
        {
            if (stateManager.CurrentState == PlayerState.Dead)
                return;

            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);

            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnDamageTaken?.Invoke(damage);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void RespawnAtPosition(Vector3 position, float invulnerabilityTime)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.isKinematic = false;

            transform.position = position;

            stateManager.ChangeState(PlayerState.Idle);

            StartInvulnerability(invulnerabilityTime);
        }
    }
}
