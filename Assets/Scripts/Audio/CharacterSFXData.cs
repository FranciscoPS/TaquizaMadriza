using UnityEngine;

namespace TaquizaMadriza.Audio
{
    [CreateAssetMenu(fileName = "CharacterSFXData", menuName = "TaquizaMadriza/Audio/Character SFX Data")]
    public class CharacterSFXData : ScriptableObject
    {
        [Header("Character Info")]
        public string characterName = "Character";
        public int characterID = 1;
        
        [Header("=== DAMAGE SOUNDS ===")]
        
        [Header("Light Damage")]
        public AudioClip[] sfxDamage = new AudioClip[2];
        
        [Header("Heavy Damage / Knockback")]
        public AudioClip sfxHeavyDamage;
        
        [Header("Death")]
        public AudioClip sfxDeath;
        
        [Header("=== ATTACK SOUNDS ===")]
        
        [Header("Punch")]
        public AudioClip[] sfxPunch = new AudioClip[3];
        
        [Header("Kick")]
        public AudioClip[] sfxKick = new AudioClip[2];
        
        [Header("=== HIT IMPACT SOUNDS ===")]
        
        [Header("Hit Impact")]
        public AudioClip[] sfxHit = new AudioClip[3];
        
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
