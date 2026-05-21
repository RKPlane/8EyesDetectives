using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
