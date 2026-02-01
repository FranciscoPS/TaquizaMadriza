using UnityEngine;
using UnityEngine.SceneManagement;

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
		if (pauseMenuPanel != null)
			pauseMenuPanel.SetActive(false);

		Time.timeScale = 1f;
		AudioListener.pause = false;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public void QuitMatch()
	{
		ResumeMatch();

		int currentIndex = SceneManager.GetActiveScene().buildIndex;
		int previousIndex = currentIndex - 1;

		if (previousIndex >= 0 && previousIndex < SceneManager.sceneCountInBuildSettings)
		{
			SceneManager.LoadScene(previousIndex);
		}
		else
		{
			Debug.LogWarning("No hay escena anterior válida en Build Settings. Cargando escena 0.");
			SceneManager.LoadScene(0);
		}
	}
}
