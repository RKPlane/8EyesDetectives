using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerDeviceManager : MonoBehaviour
{
    public static PlayerDeviceManager Instance { get; private set; }

    // [0] = Spider (Player.cs), [1] = Mantis (MantisPlayer.cs)
    readonly InputDevice[] assignedDevices = new InputDevice[2];

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        AssignDevicesOnce();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //Asignacion inicial

    void AssignDevicesOnce()
    {
        var gamepads = Gamepad.all;

        if (gamepads.Count >= 2)
        {
            assignedDevices[0] = gamepads[0];
            assignedDevices[1] = gamepads[1];
            Debug.Log($"[Devices] Spider={gamepads[0].displayName}  Mantis={gamepads[1].displayName}");
        }
        else if (gamepads.Count == 1)
        {
            assignedDevices[0] = gamepads[0];
            assignedDevices[1] = Keyboard.current;
            Debug.LogWarning("[Devices] Solo 1 mando. Spider=Gamepad, Mantis=Keyboard.");
        }
        else
        {
            Debug.LogWarning("[Devices] No hay mandos conectados.");
        }
    }

    //Validacion entre escenas

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ReparentDevicesNextFrame());
    }

    System.Collections.IEnumerator ReparentDevicesNextFrame()
    {
        yield return null;

        //Validacion
        var spiderInput = FindPlayerInputWithComponent<Player>();
        var mantisInput = FindPlayerInputWithComponent<MantisPlayer>();

        ApplyDevice(spiderInput, assignedDevices[0], "Spider");
        ApplyDevice(mantisInput, assignedDevices[1], "Mantis");
    }

    static void ApplyDevice(PlayerInput pi, InputDevice device, string roleName)
    {
        if (pi == null || device == null) return;

        //validacion
        foreach (var d in pi.devices)
            if (d == device) return;

        pi.SwitchCurrentControlScheme(GetSchemeForDevice(device), device);
        Debug.Log($"[Devices] {roleName} ({pi.gameObject.name}) → {device.displayName}");
    }

    //Helpers

    //Fix
    static PlayerInput FindPlayerInputWithComponent<T>() where T : Component
    {
        var component = Object.FindFirstObjectByType<T>();
        if (component == null) return null;
        return component.GetComponentInParent<PlayerInput>()
            ?? component.GetComponent<PlayerInput>();
    }

    static string GetSchemeForDevice(InputDevice device)
    {
        if (device is Gamepad) return "Gamepad";
        if (device is Keyboard) return "Keyboard&Mouse";
        return "Gamepad";
    }

    public InputDevice GetDevice(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= assignedDevices.Length) return null;
        return assignedDevices[playerIndex];
    }

    //Script
    public void SetDevices(InputDevice p1, InputDevice p2)
    {
        assignedDevices[0] = p1;
        assignedDevices[1] = p2;
        Debug.Log($"[Devices] Fijados — Spider={p1?.displayName}  Mantis={p2?.displayName}");
    }
}