using UnityEngine;

namespace TaquizaMadriza.Audio
{
    [CreateAssetMenu(fileName = "GeneralSFXData", menuName = "TaquizaMadriza/Audio/General SFX Data")]
    public class GeneralSFXData : ScriptableObject
    {
        [Header("=== GENERAL SFX (Movement) ===")]
        
        [Header("UI")]
        public AudioClip sfxButton;
        
        [Header("Floor Impact")]
        public AudioClip[] sfxFloorDown = new AudioClip[3];
        
        [Header("Jump")]
        public AudioClip[] sfxJump = new AudioClip[3];
        
        [Header("Land")]
        public AudioClip[] sfxLand = new AudioClip[3];
        
        [Header("Steps")]
        public AudioClip[] sfxSteps = new AudioClip[4];
        
        [Header("=== GENERAL SFX (Voice) ===")]
        
        [Header("Game Start")]
        public AudioClip sfxIntro;
        
        [Header("Match End")]
        public AudioClip sfxKO;
        
        [Header("Match Start")]
        public AudioClip sfxReadyFight;
        
        [Header("Victory Announcements")]
        public AudioClip[] sfxPlayerWins = new AudioClip[4];
        
        public AudioClip GetRandomClip(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0)
                return null;
            
            var validClips = System.Array.FindAll(clips, clip => clip != null);
            if (validClips.Length == 0)
                return null;
            
            return validClips[Random.Range(0, validClips.Length)];
        }
    }
}
