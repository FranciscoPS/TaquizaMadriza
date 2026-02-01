using TaquizaMadriza.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WinScreen : MonoBehaviour
{
	[Header("UI")]
	[SerializeField] private GameObject winScreenPanel;
	[SerializeField] private TextMeshProUGUI winnerPlayerTxt;

	[Header("Players")]
	[SerializeField] private PlayerHealth player1Health;
	[SerializeField] private PlayerHealth player2Health;

	[Header("Settings")]
	[SerializeField] private float winDelay = 3f;

	private bool matchEnded = false;

	private void Awake()
	{
		winScreenPanel.SetActive(false);

		player1Health.OnDeath += () => OnPlayerDied(player1Health);
		player2Health.OnDeath += () => OnPlayerDied(player2Health);
	}

	private void OnDestroy()
	{
		player1Health.OnDeath -= () => OnPlayerDied(player1Health);
		player2Health.OnDeath -= () => OnPlayerDied(player2Health);
	}

	private void OnPlayerDied(PlayerHealth deadPlayer)
	{
		if (matchEnded) return;

		matchEnded = true;

		int winnerNumber = deadPlayer.PlayerNumber == 1 ? 2 : 1;
		StartCoroutine(ShowWinScreenAfterDelay(winnerNumber));
	}

	private IEnumerator ShowWinScreenAfterDelay(int winnerPlayer)
	{
		yield return new WaitForSeconds(winDelay);

		winnerPlayerTxt.text = $"Player {winnerPlayer} Wins!!";
		winScreenPanel.SetActive(true);
	}

	public void ReturnMainMenu()
	{
		SceneManager.LoadScene(0);
	}

	public void RestartMatch()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void QuitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}
}
