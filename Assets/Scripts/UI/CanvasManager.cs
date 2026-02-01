using UnityEngine;
using UnityEngine.SceneManagement;
using TaquizaMadriza.Audio;

public class CanvasManager : MonoBehaviour
{
	[Header("Canvas Panels")]
	[SerializeField] private GameObject mainMenuPanel;
	[SerializeField] private GameObject controlsPanel;

	private void Awake()
	{
		ShowPanel(mainMenuPanel);
	}

	private void Start()
	{
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlayIntroVoice();
		}
	}

	public void ShowControls()
	{
		PlayButtonSound();
		ShowPanel(controlsPanel);
	}

	public void ShowMainMenu()
	{
		PlayButtonSound();
		ShowPanel(mainMenuPanel);
	}

	private void ShowPanel(GameObject panelToShow)
	{
		if (mainMenuPanel == null || controlsPanel == null)
		{
			return;
		}

		mainMenuPanel.SetActive(false);
		controlsPanel.SetActive(false);

		panelToShow.SetActive(true);
	}
	
	public void LoadNextScene()
	{
		PlayButtonSound();
		int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
		SceneManager.LoadScene(currentSceneIndex + 1);
	}

	public void QuitGame()
	{
		PlayButtonSound();

#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}
	
	private void PlayButtonSound()
	{
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlayButtonSound();
		}
	}
}
