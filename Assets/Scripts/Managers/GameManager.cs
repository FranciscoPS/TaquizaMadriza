using System.Collections;
using TaquizaMadriza.Audio;
using UnityEngine;

namespace TaquizaMadriza.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [Header("Game Settings")]
        [SerializeField]
        private float readyFightDelay = 1f;
        
        private bool gameStarted = false;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }
        
        private void Start()
        {
            StartCoroutine(StartGameSequence());
        }
        
        private IEnumerator StartGameSequence()
        {
            yield return new WaitForSeconds(readyFightDelay);
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayReadyFightVoice();
            }
            
            gameStarted = true;
        }
        
        public bool IsGameStarted() => gameStarted;
    }
}
