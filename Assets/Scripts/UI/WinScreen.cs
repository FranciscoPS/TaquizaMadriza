using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
	[SerializeField] GameObject winScreenPanel;

	public void ReturnMainMenu()
	{
		HideWinPanel();

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

	public void QuitGame()
	{
		HideWinPanel();

		Debug.Log("Quit application");

#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}

	public void RestartMatch()
	{
		HideWinPanel();
		int currentIndex = SceneManager.GetActiveScene().buildIndex;
		SceneManager.LoadScene(currentIndex);
	}

	private void HideWinPanel()
	{
		if (winScreenPanel != null && winScreenPanel.activeSelf)
			winScreenPanel.SetActive(false);
	}
}
