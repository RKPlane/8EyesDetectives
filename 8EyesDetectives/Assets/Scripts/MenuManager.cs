using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject menuOptions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
