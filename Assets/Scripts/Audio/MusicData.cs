using UnityEngine;

namespace TaquizaMadriza.Audio
{
    [CreateAssetMenu(fileName = "MusicData", menuName = "Taquiza Madriza/Audio/Music Data")]
    public class MusicData : ScriptableObject
    {
        [Header("Menu Music")]
        public AudioClip menuInicio;
        public AudioClip menuModo;
        public AudioClip menuPersonajes;
        
        [Header("Game Music")]
        public AudioClip luchaM;
        public AudioClip gameOver;
        public AudioClip creditos;
    }
}
