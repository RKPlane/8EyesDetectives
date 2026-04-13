using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string targetScene;

    public void LoadScene()
    {
        SceneManager.LoadScene(targetScene);
    }
}