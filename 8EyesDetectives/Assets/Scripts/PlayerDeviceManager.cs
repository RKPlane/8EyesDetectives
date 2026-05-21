using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerDeviceManager : MonoBehaviour
{
    public static PlayerDeviceManager Instance { get; private set; }

    // Dispositivos asignados permanentemente a cada jugador.
    // [0] = Player1, [1] = Player2.
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

    // ── Asignación inicial ────────────────────────────────────────────────────

    void AssignDevicesOnce()
    {
        var gamepads = Gamepad.all;

        if (gamepads.Count >= 2)
        {
            // Dos mandos conectados: asignación directa por orden de conexión.
            assignedDevices[0] = gamepads[0];
            assignedDevices[1] = gamepads[1];
            Debug.Log($"[Devices] P1={gamepads[0].displayName}  P2={gamepads[1].displayName}");
        }
        else if (gamepads.Count == 1)
        {
            // Solo un mando: P1 lo usa, P2 queda sin dispositivo (teclado/mouse si quieres).
            assignedDevices[0] = gamepads[0];
            assignedDevices[1] = Keyboard.current; // cambia esto si P2 usa otro esquema
            Debug.LogWarning("[Devices] Solo 1 mando. P1=Gamepad, P2=Keyboard.");
        }
        else
        {
            Debug.LogWarning("[Devices] No hay mandos conectados.");
        }
    }

    // ── Repairing al cargar escena ────────────────────────────────────────────

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Pequeño delay para que los PlayerInput ya hayan hecho su Awake.
        StartCoroutine(ReparentDevicesNextFrame());
    }

    System.Collections.IEnumerator ReparentDevicesNextFrame()
    {
        yield return null; // espera un frame a que los jugadores se instancien

        var players = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);

        foreach (var pi in players)
        {
            int idx = pi.playerIndex; // 0 = P1, 1 = P2
            if (idx < 0 || idx >= assignedDevices.Length) continue;

            InputDevice device = assignedDevices[idx];
            if (device == null) continue;

            // Fuerza el dispositivo correcto, ignorando el pairing automático.
            pi.SwitchCurrentControlScheme(GetSchemeForDevice(device), device);
            Debug.Log($"[Devices] {pi.gameObject.name} → {device.displayName}");
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    static string GetSchemeForDevice(InputDevice device)
    {
        // Ajusta los nombres al control scheme de tu InputActionAsset.
        if (device is Gamepad) return "Gamepad";
        if (device is Keyboard) return "Keyboard&Mouse";
        return "Gamepad";
    }

    /// Devuelve el dispositivo asignado a un jugador (útil desde otros scripts).
    public InputDevice GetDevice(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= assignedDevices.Length) return null;
        return assignedDevices[playerIndex];
    }

    /// Llamado desde LobbyDevicePicker cuando los jugadores eligen su mando.
    /// Sobreescribe la asignación automática inicial.
    public void SetDevices(InputDevice p1, InputDevice p2)
    {
        assignedDevices[0] = p1;
        assignedDevices[1] = p2;
        Debug.Log($"[Devices] Fijados — P1={p1?.displayName}  P2={p2?.displayName}");
    }
}   