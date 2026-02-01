using UnityEngine;

namespace TaquizaMadriza.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        [Header("SFX Data")]
        [SerializeField]
        private GeneralSFXData generalSFX;
        
        [Header("Audio Sources")]
        [SerializeField]
        private AudioSource sfxSource;
        
        [SerializeField]
        private AudioSource voiceSource;
        
        [SerializeField]
        private AudioSource uiSource;
        
        [Header("Volume Settings")]
        [Range(0f, 1f)]
        [SerializeField]
        private float sfxVolume = 1f;
        
        [Range(0f, 1f)]
        [SerializeField]
        private float voiceVolume = 1f;
        
        [Range(0f, 1f)]
        [SerializeField]
        private float uiVolume = 0.7f;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            SetupAudioSources();
        }
        
        private void SetupAudioSources()
        {
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
                sfxSource.volume = sfxVolume;
            }
            
            if (voiceSource == null)
            {
                voiceSource = gameObject.AddComponent<AudioSource>();
                voiceSource.playOnAwake = false;
                voiceSource.volume = voiceVolume;
            }
            
            if (uiSource == null)
            {
                uiSource = gameObject.AddComponent<AudioSource>();
                uiSource.playOnAwake = false;
                uiSource.volume = uiVolume;
            }
        }
        
        public void PlayButtonSound()
        {
            if (generalSFX != null && generalSFX.sfxButton != null)
            {
                uiSource.PlayOneShot(generalSFX.sfxButton, uiVolume);
            }
        }
        
        public void PlayFloorDownSound()
        {
            if (generalSFX != null)
            {
                AudioClip clip = generalSFX.GetRandomClip(generalSFX.sfxFloorDown);
                if (clip != null)
                {
                    sfxSource.PlayOneShot(clip, sfxVolume);
                }
            }
        }
        
        public void PlayJumpSound()
        {
            if (generalSFX != null)
            {
                AudioClip clip = generalSFX.GetRandomClip(generalSFX.sfxJump);
                if (clip != null)
                {
                    sfxSource.PlayOneShot(clip, sfxVolume * 0.8f);
                }
            }
        }
        
        public void PlayLandSound()
        {
            if (generalSFX != null)
            {
                AudioClip clip = generalSFX.GetRandomClip(generalSFX.sfxLand);
                if (clip != null)
                {
                    sfxSource.PlayOneShot(clip, sfxVolume * 0.7f);
                }
            }
        }
        
        public void PlayStepSound()
        {
            if (generalSFX != null)
            {
                AudioClip clip = generalSFX.GetRandomClip(generalSFX.sfxSteps);
                if (clip != null)
                {
                    sfxSource.PlayOneShot(clip, sfxVolume * 0.5f);
                }
            }
        }
        
        public void PlayIntroVoice()
        {
            if (generalSFX != null && generalSFX.sfxIntro != null)
            {
                voiceSource.PlayOneShot(generalSFX.sfxIntro, voiceVolume);
            }
        }
        
        public void PlayKOVoice()
        {
            if (generalSFX != null && generalSFX.sfxKO != null)
            {
                voiceSource.PlayOneShot(generalSFX.sfxKO, voiceVolume);
            }
        }
        
        public void PlayReadyFightVoice()
        {
            if (generalSFX != null && generalSFX.sfxReadyFight != null)
            {
                voiceSource.PlayOneShot(generalSFX.sfxReadyFight, voiceVolume);
            }
        }
        
        public void PlayPlayerWinsVoice(int playerNumber)
        {
            if (generalSFX != null && generalSFX.sfxPlayerWins != null)
            {
                int index = Mathf.Clamp(playerNumber - 1, 0, generalSFX.sfxPlayerWins.Length - 1);
                
                if (index >= 0 && index < generalSFX.sfxPlayerWins.Length)
                {
                    AudioClip clip = generalSFX.sfxPlayerWins[index];
                    if (clip != null)
                    {
                        voiceSource.PlayOneShot(clip, voiceVolume);
                    }
                }
            }
        }
        
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            if (sfxSource != null)
                sfxSource.volume = sfxVolume;
        }
        
        public void SetVoiceVolume(float volume)
        {
            voiceVolume = Mathf.Clamp01(volume);
            if (voiceSource != null)
                voiceSource.volume = voiceVolume;
        }
        
        public void SetUIVolume(float volume)
        {
            uiVolume = Mathf.Clamp01(volume);
            if (uiSource != null)
                uiSource.volume = uiVolume;
        }
    }
}
