using TaquizaMadriza.Combat;
using TaquizaMadriza.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WinScreen : MonoBehaviour
{
	[Header("UI")]
	[SerializeField] private GameObject winScreenPanel;
	[SerializeField] private TextMeshProUGUI winnerPlayerTxt;

	[Header("Credits")]
	[SerializeField] private GameObject creditsPanel;
    [SerializeField] private float creditsDelay = 6f;

    [Header("Players")]
	[SerializeField] private PlayerHealth player1Health;
	[SerializeField] private PlayerHealth player2Health;

	[Header("Settings")]
	[SerializeField] private float winDelay = 3f;

	[Header("Slow Motion")]
	[SerializeField, Range(0.05f, 1f)]
	private float slowMotionScale = 0.25f;

	[Header("Winner Text Pulse")]
	[SerializeField] private float pulseScaleAmount = 0.08f;
	[SerializeField] private float pulseSpeed = 2f;

	[SerializeField]
	private float slowMotionDuration = 3f;

	private bool matchEnded = false;
	private Coroutine pulseCoroutine;
	private Vector3 baseTextScale;
    private Coroutine creditsCoroutine;
    private bool creditsShown = false;

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

		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlayKOVoice();
		}

		int winnerNumber = deadPlayer.PlayerNumber == 1 ? 2 : 1;
		StartCoroutine(WinSequence(winnerNumber));
	}

	private IEnumerator ShowWinScreenAfterDelay(int winnerPlayer)
	{
		yield return new WaitForSeconds(winDelay);

		winnerPlayerTxt.text = $"Player {winnerPlayer} Wins!!";
		winScreenPanel.SetActive(true);
	}

	private IEnumerator WinSequence(int winnerPlayer)
	{
		Time.timeScale = slowMotionScale;
		Time.fixedDeltaTime = 0.02f * Time.timeScale;

		yield return new WaitForSecondsRealtime(slowMotionDuration);

		Time.timeScale = 1f;
		Time.fixedDeltaTime = 0.02f;

		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlayPlayerWinsVoice(winnerPlayer);
		}

		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.PlayGameOverMusic();
		}

        winnerPlayerTxt.text = $"Player {winnerPlayer} Wins!!";
        winScreenPanel.SetActive(true);

        baseTextScale = winnerPlayerTxt.transform.localScale;
        pulseCoroutine = StartCoroutine(PulseWinnerText());

        creditsCoroutine = StartCoroutine(AutoShowCredits());
    }
    private void HideWinPanel()
    {
        if (creditsCoroutine != null)
        {
            StopCoroutine(creditsCoroutine);
            creditsCoroutine = null;
        }

        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }

        winnerPlayerTxt.transform.localScale = baseTextScale;

        if (winScreenPanel.activeSelf)
            winScreenPanel.SetActive(false);
    }

    private IEnumerator PulseWinnerText()
	{
		float timer = 0f;

		while (true)
		{
			timer += Time.unscaledDeltaTime * pulseSpeed;

			float scaleOffset = Mathf.Sin(timer) * pulseScaleAmount;
			winnerPlayerTxt.transform.localScale = baseTextScale * (1f + scaleOffset);

			yield return null;
		}
	}

    private IEnumerator AutoShowCredits()
    {
        yield return new WaitForSecondsRealtime(creditsDelay);

        ShowCredits();
    }

    public void ShowCredits()
    {
        if (creditsShown) return;

        creditsShown = true;

        creditsPanel.SetActive(true);
    }

    public void ReturnMainMenu()
	{
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlayButtonSound();
		}
		SceneManager.LoadScene(0);
	}

	public void RestartMatch()
	{
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlayButtonSound();
		}
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void QuitGame()
	{
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlayButtonSound();
		}
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}
}
