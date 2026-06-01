using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.InputSystem.Utilities;

public class LobbyDevicePicker : MonoBehaviour
{
    [Header("UI (opcional)")]
    public TextMeshProUGUI statusP1;   // Spider
    public TextMeshProUGUI statusP2;   // Mantis

    [Header("Escena a cargar cuando los 2 jugadores estén listos")]
    public string gameSceneName = "Game";

    [Header("Tiempo de espera antes de cargar (segundos)")]
    public float countdownSeconds = 2f;

    // [0] = Spider, [1] = Mantis
    readonly InputDevice[] pickedDevices = new InputDevice[2];

    bool loading;

    void OnEnable()
    {
        InputSystem.onAnyButtonPress.CallOnce(OnButtonPressed);
    }

    void OnButtonPressed(InputControl control)
    {
        if (loading) return;

        InputDevice device = control.device;

        // Solo mandos
        if (device is not Gamepad)
        {
            InputSystem.onAnyButtonPress.CallOnce(OnButtonPressed);
            return;
        }

        //VALIDACION SI SPAM
        if (device == pickedDevices[0] || device == pickedDevices[1])
        {
            InputSystem.onAnyButtonPress.CallOnce(OnButtonPressed);
            return;
        }

        //PRIMER PLAYER Y SEGUNDO PLAYER ASIGNAR
        for (int i = 0; i < pickedDevices.Length; i++)
        {
            if (pickedDevices[i] == null)
            {
                pickedDevices[i] = device;
                string roleName = i == 0 ? "Spider" : "Mantis";
                UpdateUI(i, device.displayName, roleName);
                Debug.Log($"[Lobby] {roleName} → {device.displayName}");
                break;
            }
        }

        //START
        if (pickedDevices[0] != null && pickedDevices[1] != null)
        {
            StartCoroutine(CommitAndLoad());
            return;
        }

        //VALIDACION AL OTRO PLAYER
        InputSystem.onAnyButtonPress.CallOnce(OnButtonPressed);
    }

    IEnumerator CommitAndLoad()
    {
        loading = true;

        //Validacion
        if (PlayerDeviceManager.Instance == null)
        {
            var go = new GameObject("PlayerDeviceManager");
            go.AddComponent<PlayerDeviceManager>();
        }

        // Guardar: [0]=Spider, [1]=Mantis.
        PlayerDeviceManager.Instance.SetDevices(pickedDevices[0], pickedDevices[1]);

        if (statusP1 != null) statusP1.text = "Spider: ¡Listo!";
        if (statusP2 != null) statusP2.text = "Mantis: ¡Listo!";

        yield return new WaitForSeconds(countdownSeconds);

        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }

    void UpdateUI(int slotIndex, string deviceName, string roleName)
    {
        TextMeshProUGUI label = slotIndex == 0 ? statusP1 : statusP2;
        if (label != null)
            label.text = $"{roleName}: {deviceName}";
    }
}