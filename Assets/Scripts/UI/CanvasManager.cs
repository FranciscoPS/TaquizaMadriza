using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
	[Header("Canvas Panels")]
	[SerializeField] private GameObject mainMenuPanel;
	[SerializeField] private GameObject controlsPanel;
	[SerializeField] private GameObject TipsPanel;

    private void Awake()
	{
		ShowPanel(mainMenuPanel);
	}

	public void ShowControls()
	{
		ShowPanel(controlsPanel);
	}

	public void ShowMainMenu()
	{
		ShowPanel(mainMenuPanel);
	}

	public void ShowTips()
	{
		ShowPanel(TipsPanel); 
	}

	private void ShowPanel(GameObject panelToShow)
	{
		if (mainMenuPanel == null || controlsPanel == null)
		{
			Debug.LogError("Panels no asignados en el Inspector.");
			return;
		}

		mainMenuPanel.SetActive(false);
		controlsPanel.SetActive(false);
		TipsPanel.SetActive(false);

        panelToShow.SetActive(true);
	}
	public void LoadNextScene()
	{
		int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
		SceneManager.LoadScene(currentSceneIndex + 1);
	}

	public void QuitGame()
	{
		Debug.Log("Quit application");

#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}
}
