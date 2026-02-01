using UnityEngine;

namespace TaquizaMadriza.Combat
{
    [System.Serializable]
    public class AttackData
    {
        [Header("Configuración del Ataque")]
        public string attackName = "Ataque";
        public float damage = 10f;
        public float hitboxDuration = 0.2f;
        public float attackCooldown = 0.3f;
        public float knockbackForce = 5f;

        [Header("Hitstun")]
        public float hitstunDuration = 0.3f;

        [HideInInspector]
        public bool appliesKnockback = false;
    }
}
