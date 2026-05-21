using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.InputSystem.Utilities; // quita este using si no usas TextMeshPro

/// <summary>
/// Pantalla de lobby: cada jugador presiona cualquier botón en su mando
/// para registrarse. Cuando los dos estén listos, carga la escena de juego.
///
/// Coloca este script en la escena de lobby/menú.
/// El PlayerDeviceManager debe existir (se crea automáticamente si no existe).
/// </summary>
public class LobbyDevicePicker : MonoBehaviour
{
    [Header("UI (opcional — conecta tus textos de estado)")]
    public TextMeshProUGUI statusP1;
    public TextMeshProUGUI statusP2;

    [Header("Escena a cargar cuando los 2 jugadores estén listos")]
    public string gameSceneName = "Game";

    [Header("Tiempo de espera antes de cargar (segundos)")]
    public float countdownSeconds = 2f;

    // Dispositivos confirmados por cada jugador. null = aún no se ha unido.
    readonly InputDevice[] pickedDevices = new InputDevice[2];
    bool loading;

    void OnEnable()
    {
        // Escucha cualquier acción en cualquier dispositivo.
        InputSystem.onAnyButtonPress.CallOnce(OnButtonPressed);
    }

    // Se llama cada vez que alguien pulsa un botón en cualquier dispositivo.
    void OnButtonPressed(InputControl control)
    {
        if (loading) return;

        InputDevice device = control.device;

        // Ignorar teclado/mouse si solo quieres mandos — borra este bloque si no.
        /*if (device is not Gamepad)
        {
            InputSystem.onAnyButtonPress.CallOnce(OnButtonPressed);
            return;
        }*/

        // Ignorar si este dispositivo ya está asignado.
        if (device == pickedDevices[0] || device == pickedDevices[1])
        {
            InputSystem.onAnyButtonPress.CallOnce(OnButtonPressed);
            return;
        }

        // Asignar al primer slot libre.
        for (int i = 0; i < pickedDevices.Length; i++)
        {
            if (pickedDevices[i] == null)
            {
                pickedDevices[i] = device;
                UpdateUI(i, device.displayName);
                Debug.Log($"[Lobby] Player {i + 1} → {device.displayName}");
                break;
            }
        }

        // Si los dos jugadores ya eligieron, arrancar.
        if (pickedDevices[0] != null && pickedDevices[1] != null)
        {
            StartCoroutine(CommitAndLoad());
            return;
        }

        // Seguir escuchando hasta que el segundo jugador se una.
        InputSystem.onAnyButtonPress.CallOnce(OnButtonPressed);
    }

    IEnumerator CommitAndLoad()
    {
        loading = true;

        // Asegurarse de que el singleton existe.
        if (PlayerDeviceManager.Instance == null)
        {
            var go = new GameObject("PlayerDeviceManager");
            go.AddComponent<PlayerDeviceManager>();
        }

        // Guardar la elección en el singleton para que persista.
        PlayerDeviceManager.Instance.SetDevices(pickedDevices[0], pickedDevices[1]);

        if (statusP1 != null) statusP1.text = "¡Listo!";
        if (statusP2 != null) statusP2.text = "¡Listo!";

        yield return new WaitForSeconds(countdownSeconds);
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }

    void UpdateUI(int playerIndex, string deviceName)
    {
        TextMeshProUGUI label = playerIndex == 0 ? statusP1 : statusP2;
        if (label != null)
            label.text = $"Player {playerIndex + 1}: {deviceName}\nPresiona de nuevo para confirmar";
    }
}