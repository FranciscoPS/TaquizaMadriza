using UnityEngine;
using UnityEngine.SceneManagement;

namespace TaquizaMadriza.Audio
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }
        
        [Header("Music Data")]
        [SerializeField]
        private MusicData musicData;
        
        [Header("Audio Source")]
        [SerializeField]
        private AudioSource musicSource;
        
        [Header("Volume Settings")]
        [Range(0f, 1f)]
        [SerializeField]
        private float musicVolume = 0.6f;
        
        [Header("Fade Settings")]
        [SerializeField]
        private float fadeOutDuration = 1f;
        
        [SerializeField]
        private float fadeInDuration = 1f;
        
        private AudioClip currentClip;
        private bool isFading = false;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            SetupMusicSource();
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void Start()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            
            if (sceneName.Contains("MainMenu"))
            {
                PlayMusic(musicData.menuInicio, true);
            }
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void SetupMusicSource()
        {
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
            }
            
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            string sceneName = scene.name;
            
            if (sceneName.Contains("MainMenu"))
            {
                PlayMusic(musicData.menuInicio, true);
            }
            else if (sceneName.Contains("ModeSelect") || sceneName.Contains("Modo"))
            {
                PlayMusic(musicData.menuModo, true);
            }
            else if (sceneName.Contains("CharacterSelect") || sceneName.Contains("Personaje"))
            {
                PlayMusic(musicData.menuPersonajes, true);
            }
            else if (sceneName.Contains("ModelsSample") || sceneName.Contains("Game") || sceneName.Contains("Fight"))
            {
                PlayMusic(musicData.luchaM, true);
            }
            else if (sceneName.Contains("Credits") || sceneName.Contains("Creditos"))
            {
                PlayMusic(musicData.creditos, true);
            }
        }
        
        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (clip == null || clip == currentClip)
                return;
            
            if (musicSource.isPlaying)
            {
                StopMusic();
            }
            
            currentClip = clip;
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
        
        public void StopMusic()
        {
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
            }
            currentClip = null;
        }
        
        public void PauseMusic()
        {
            if (musicSource.isPlaying)
            {
                musicSource.Pause();
            }
        }
        
        public void ResumeMusic()
        {
            if (!musicSource.isPlaying && currentClip != null)
            {
                musicSource.UnPause();
            }
        }
        
        public void PlayGameOverMusic()
        {
            PlayMusic(musicData.gameOver, true);
        }
        
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            musicSource.volume = musicVolume;
        }
    }
}
