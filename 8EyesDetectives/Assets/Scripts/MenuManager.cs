using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject menuOptions;
    public GameObject pauseMenu;

    public InputActionAsset inputActions;
    private InputAction pauseAction;

    private bool isPaused = false;
    public Button firstButton;

    void Awake()
    {
        var map = inputActions.FindActionMap("UI");

        pauseAction = map.FindAction("Pause");

        firstButton.Select();

        map.Enable();
    }

    void OnEnable()
    {
        pauseAction.Enable();
        pauseAction.performed += OnPausePressed;
    }

    void OnDisable()
    {
        pauseAction.performed -= OnPausePressed;
        pauseAction.Disable();
    }

    private void OnPausePressed(InputAction.CallbackContext ctx)
    {
        if (!isPaused)
            PauseGame();
        else
            ResumeGame();
    }

    public void PauseGame()
    {
        if (isPaused) return;

        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        isPaused = true;
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        isPaused = false;
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

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Tutorial");
    }
}
