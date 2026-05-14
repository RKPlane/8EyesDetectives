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
		var map = inputActions.FindActionMap("UI"); // Asegúrate de que el nombre del Action Map coincida con el que tienes en tu Input Actions

		pauseAction = map.FindAction("Pause"); // Asegúrate de que el nombre de la acción coincida con el que tienes en tu Input Actions

        
	}
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
       
	}

    // Update is called once per frame
    void Update()
	{ // Verificar si se ha presionado la tecla de pausa
		if (pauseAction.WasPerformedThisFrame() && !isPaused)
		{ 
			pauseGame();
            
		} else if (pauseAction.WasPerformedThisFrame() && isPaused) {
            resumeGame();
            
        }
    }

    public void OpenMenuOptions() // Método para abrir el menú de opciones
	{
        mainMenu.SetActive(false);
		pauseMenu.SetActive(false);
		menuOptions.SetActive(true);
    }

    public void OpenMainMenu() // Método para volver al menú principal
	{
        mainMenu.SetActive(true);
        menuOptions.SetActive(false);
		pauseMenu.SetActive(false);
	}

    //Para mas adelante
    public void pauseGame() // Método para pausar el juego
	{
        Time.timeScale = 0;
		pauseMenu.SetActive(true);
		isPaused = true;
        Debug.Log("Paused");
	}

    public void resumeGame() // Método para reanudar el juego
	{
        Time.timeScale = 1;
        isPaused = false;
        Debug.Log("Resumed");
		pauseMenu.SetActive(false);
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
