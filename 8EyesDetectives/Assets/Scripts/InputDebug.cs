using UnityEngine;
using UnityEngine.InputSystem;

public class InputDebug : MonoBehaviour
{
    void Start()
    {
        Debug.Log("===== DEVICES CONNECTED =====");

        foreach (InputDevice device in InputSystem.devices)
        {
            Debug.Log(
                $"Name: {device.displayName} | " +
                $"Type: {device.GetType().Name} | " +
                $"ID: {device.deviceId}"
            );
        }

        Debug.Log("===== PLAYER INPUTS =====");

        PlayerInput[] players = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);

        for (int i = 0; i < players.Length; i++)
        {
            PlayerInput p = players[i];

            string deviceNames = "";

            foreach (InputDevice d in p.devices)
            {
                deviceNames += d.displayName + " ";
            }

            Debug.Log(
                $"PlayerIndex: {p.playerIndex} | " +
                $"CurrentControlScheme: {p.currentControlScheme} | " +
                $"Devices: {deviceNames}"
            );
        }
    }

    void Update()
    {
        // Detectar mandos conectados/desconectados en tiempo real
        foreach (Gamepad gamepad in Gamepad.all)
        {
            if (gamepad.startButton.wasPressedThisFrame)
            {
                Debug.Log(
                    $"START PRESSED -> " +
                    $"{gamepad.displayName} | " +
                    $"ID: {gamepad.deviceId}"
                );
            }
        }
    }

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        Debug.Log(
            $"DEVICE CHANGE -> {device.displayName} | {change}"
        );
    }
}