using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject menuOptions;
	public InputActionAsset inputActions;
	private InputAction pauseAction;
    private bool isPaused = false;



	private void Awake()
	{
		var map = inputActions.FindActionMap("UI");

		pauseAction = map.FindAction("Pause");

	}
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (pauseAction.IsPressed() && !isPaused){
            pauseGame();
        } else if (pauseAction.IsPressed() && isPaused) {
            resumeGame();
        }
    }

    public void OpenMenuOptions()
    {
        mainMenu.SetActive(false);
        menuOptions.SetActive(true);
    }

    public void OpenMainMenu()
    {
        mainMenu.SetActive(true);
        menuOptions.SetActive(false);
    }

    //Para mas adelante
    public void pauseGame()
    {
        Time.timeScale = 0;
        OpenMainMenu();
        isPaused = true;
        Debug.Log("Paused");
	}

    public void resumeGame()
    {
        Time.timeScale = 1;
        OpenMainMenu();
        isPaused = false;
        Debug.Log("Resumed");
	}
	public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Nivel0");
    }

    public void ContinueGame()
    {
		SceneManager.LoadScene("WebTest");
	}
}
