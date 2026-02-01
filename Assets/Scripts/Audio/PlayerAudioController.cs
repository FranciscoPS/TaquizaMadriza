using TaquizaMadriza.Characters;
using TaquizaMadriza.Combat;
using UnityEngine;

namespace TaquizaMadriza.Audio
{
    [RequireComponent(typeof(PlayerHealth))]
    [RequireComponent(typeof(PlayerCombat))]
    [RequireComponent(typeof(PlayerController))]
    public class PlayerAudioController : MonoBehaviour
    {
        [Header("Character SFX Data")]
        [SerializeField]
        private CharacterSFXData characterSFX;
        
        [Header("Audio Source")]
        [SerializeField]
        private AudioSource characterAudioSource;
        
        [Header("Volume Settings")]
        [Range(0f, 1f)]
        [SerializeField]
        private float voiceVolume = 1f;
        
        [Range(0f, 1f)]
        [SerializeField]
        private float attackVolume = 0.8f;
        
        [Range(0f, 1f)]
        [SerializeField]
        private float impactVolume = 0.9f;
        
        private PlayerHealth health;
        private PlayerCombat combat;
        private PlayerController controller;
        private PlayerStateManager stateManager;
        
        private float lastDamageSoundTime;
        private const float damageSoundCooldown = 0.1f;
        
        private void Awake()
        {
            health = GetComponent<PlayerHealth>();
            combat = GetComponent<PlayerCombat>();
            controller = GetComponent<PlayerController>();
            stateManager = GetComponent<PlayerStateManager>();
            
            if (characterAudioSource == null)
            {
                characterAudioSource = gameObject.AddComponent<AudioSource>();
                characterAudioSource.playOnAwake = false;
                characterAudioSource.spatialBlend = 0f;
            }
        }
        
        private void OnEnable()
        {
            if (health != null)
            {
                health.OnDamageTaken += HandleDamageTaken;
                health.OnDeath += HandleDeath;
            }
            
            if (stateManager != null)
            {
                stateManager.OnStateChanged += HandleStateChanged;
            }
        }
        
        private void OnDisable()
        {
            if (health != null)
            {
                health.OnDamageTaken -= HandleDamageTaken;
                health.OnDeath -= HandleDeath;
            }
            
            if (stateManager != null)
            {
                stateManager.OnStateChanged -= HandleStateChanged;
            }
        }
        
        private void HandleDamageTaken(float damage)
        {
            if (Time.time - lastDamageSoundTime < damageSoundCooldown)
                return;
            
            lastDamageSoundTime = Time.time;
            
            if (stateManager.CurrentState == PlayerState.Knockback)
            {
                PlayHeavyDamageSound();
            }
            else
            {
                PlayDamageSound();
            }
        }
        
        private void HandleDeath()
        {
            PlayDeathSound();
        }
        
        private void HandleStateChanged(PlayerState previousState, PlayerState newState)
        {
            if (newState == PlayerState.Grounded && previousState == PlayerState.Knockback)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayFloorDownSound();
                }
            }
        }
        
        public void PlayDamageSound()
        {
            if (characterSFX != null)
            {
                AudioClip clip = characterSFX.GetRandomClip(characterSFX.sfxDamage);
                if (clip != null)
                {
                    characterAudioSource.PlayOneShot(clip, voiceVolume);
                }
            }
        }
        
        public void PlayHeavyDamageSound()
        {
            if (characterSFX != null && characterSFX.sfxHeavyDamage != null)
            {
                characterAudioSource.PlayOneShot(characterSFX.sfxHeavyDamage, voiceVolume);
            }
        }
        
        public void PlayDeathSound()
        {
            if (characterSFX != null && characterSFX.sfxDeath != null)
            {
                characterAudioSource.PlayOneShot(characterSFX.sfxDeath, voiceVolume);
            }
        }
        
        public void PlayHitImpactSound()
        {
            if (characterSFX != null)
            {
                AudioClip clip = characterSFX.GetRandomClip(characterSFX.sfxHit);
                if (clip != null)
                {
                    characterAudioSource.PlayOneShot(clip, impactVolume);
                }
            }
        }
        
        public void PlayPunchSound()
        {
            if (characterSFX != null)
            {
                AudioClip clip = characterSFX.GetRandomClip(characterSFX.sfxPunch);
                if (clip != null)
                {
                    characterAudioSource.PlayOneShot(clip, attackVolume);
                }
            }
        }
        
        public void PlayKickSound()
        {
            if (characterSFX != null)
            {
                AudioClip clip = characterSFX.GetRandomClip(characterSFX.sfxKick);
                if (clip != null)
                {
                    characterAudioSource.PlayOneShot(clip, attackVolume);
                }
            }
        }
        
        public void SetVoiceVolume(float volume)
        {
            voiceVolume = Mathf.Clamp01(volume);
        }
        
        public void SetAttackVolume(float volume)
        {
            attackVolume = Mathf.Clamp01(volume);
        }
        
        public void SetImpactVolume(float volume)
        {
            impactVolume = Mathf.Clamp01(volume);
        }
    }
}
