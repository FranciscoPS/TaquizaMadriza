using UnityEngine;
using TaquizaMadriza.Combat;

namespace TaquizaMadriza.UI
{
    public class GameUIManager : MonoBehaviour
    {
        [Header("Referencias de Barras de Vida")]
        [SerializeField] private PlayerHealthBarUI player1HealthBar;
        [SerializeField] private PlayerHealthBarUI player2HealthBar;
        
        [Header("Sprites de Jugadores (Opcional)")]
        [SerializeField] private Sprite player1Icon;
        [SerializeField] private Sprite player2Icon;
        
        [Header("Referencias de Jugadores")]
        [SerializeField] private PlayerHealth player1Health;
        [SerializeField] private PlayerHealth player2Health;
        
        private void Start()
        {
            if (player1Health == null || player2Health == null)
            {
                FindPlayers();
            }

            InitializeHealthBars();
        }

        private void FindPlayers()
        {
            PlayerHealth[] players = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);
            
            foreach (var player in players)
            {
                if (player.PlayerNumber == 1 && player1Health == null)
                {
                    player1Health = player;
                }
                else if (player.PlayerNumber == 2 && player2Health == null)
                {
                    player2Health = player;
                }
            }
            
            if (player1Health == null)
            {
                Debug.LogWarning("[GameUIManager] No se encontró Player 1");
            }
            
            if (player2Health == null)
            {
                Debug.LogWarning("[GameUIManager] No se encontró Player 2");
            }
        }

        private void InitializeHealthBars()
        {
            if (player1HealthBar != null && player1Health != null)
            {
                player1HealthBar.Initialize(player1Health, player1Icon);
            }
            else
            {
                Debug.LogWarning("[GameUIManager] No se pudo inicializar la barra de vida del Player 1");
            }
            
            if (player2HealthBar != null && player2Health != null)
            {
                player2HealthBar.Initialize(player2Health, player2Icon);
            }
            else
            {
                Debug.LogWarning("[GameUIManager] No se pudo inicializar la barra de vida del Player 2");
            }
        }
    }
}
