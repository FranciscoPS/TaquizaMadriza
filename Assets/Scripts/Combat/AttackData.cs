using UnityEngine;

namespace TaquizaMadriza.Combat
{
    /// <summary>
    /// Datos de configuración para cada tipo de ataque
    /// </summary>
    [System.Serializable]
    public class AttackData
    {
        [Header("Configuración del Ataque")]
        public string attackName = "Ataque";
        public float damage = 10f;
        public float hitboxDuration = 0.2f;      // Cuánto tiempo está activa la hitbox
        public float attackCooldown = 0.3f;       // Tiempo antes de poder atacar de nuevo
        public float knockbackForce = 5f;         // Fuerza del empuje
        
        [Header("Hitstun")]
        public float hitstunDuration = 0.3f;      // Tiempo que el enemigo queda en hitstun
        
        [Header("Referencias")]
        public GameObject hitboxObject;           // Referencia al GameObject de la hitbox
    }
}
