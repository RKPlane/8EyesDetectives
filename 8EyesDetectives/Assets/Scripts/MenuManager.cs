using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject menuOptions;
	public GameObject pauseMenu;
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
        if (pauseAction.WasPerformedThisFrame() && !isPaused){
            pauseGame();
        } else if (pauseAction.WasPerformedThisFrame() && isPaused) {
            resumeGame();
        }
    }

    public void OpenMenuOptions()
    {
        mainMenu.SetActive(false);
        menuOptions.SetActive(true);
		pauseMenu.SetActive(false);
	}

    public void OpenMainMenu()
    {
        mainMenu.SetActive(true);
        menuOptions.SetActive(false);
		pauseMenu.SetActive(false);
	}

    //Para mas adelante
    public void pauseGame()
    {
        Time.timeScale = 0;
		pauseMenu.SetActive(true);
		isPaused = true;
        Debug.Log("Paused");
	}

    public void resumeGame()
    {
        Time.timeScale = 1;
		pauseMenu.SetActive(false);
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

}
