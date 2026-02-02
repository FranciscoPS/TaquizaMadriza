using UnityEngine;
using UnityEngine.SceneManagement;
using TaquizaMadriza.Audio;

public class PausdeMenu : MonoBehaviour
{
	[SerializeField] private GameObject pauseMenuPanel;

	private void Awake()
	{
		pauseMenuPanel.SetActive(false);
		Time.timeScale = 1f;
		AudioListener.pause = false;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	private void Update()
	{
		if (pauseMenuPanel == null) return;

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (pauseMenuPanel.activeSelf)
				ResumeMatch();
			else
				Pause();
		}
	}

	private void Pause()
	{
		pauseMenuPanel.SetActive(true);
		Time.timeScale = 0f;
		AudioListener.pause = true;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public void ResumeMatch()
	{
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlayButtonSound();
		}
		
		if (pauseMenuPanel != null)
			pauseMenuPanel.SetActive(false);

		Time.timeScale = 1f;
		AudioListener.pause = false;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public void QuitMatch()
	{
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlayButtonSound();
		}
		
		ResumeMatch();

		int currentIndex = SceneManager.GetActiveScene().buildIndex;
		int previousIndex = currentIndex - 1;

		if (previousIndex >= 0 && previousIndex < SceneManager.sceneCountInBuildSettings)
		{
			SceneManager.LoadScene(previousIndex);
		}
		else
		{
			SceneManager.LoadScene(0);
		}
	}
}
