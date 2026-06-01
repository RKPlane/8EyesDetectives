using UnityEngine;
using UnityEngine.InputSystem;

public class AimCursor : MonoBehaviour
{
    [Header("Referencias")]
    public InputActionAsset inputActions;

    [Header("Mando")]
    public float stickSpeed = 12f;
    public float maxRadius = 8f;
    public float minRadius = 0.5f;

    [Header("Visuals")]
    public SpriteRenderer cursorSprite;

    [Header("Solo Spider")]
    public bool onlySpider = true;

    public Player player;
    public PlayerInput playerInput;

    // Output
    public Vector2 AimWorldPos { get; private set; }
    public Vector2 AimDirection { get; private set; }

    private InputAction aimAction;

    private Vector2 localCursorPos;
    private bool usingGamepad;

    //fix
    private Gamepad MyGamepad
    {
        get
        {
            if (playerInput == null) return null;
            foreach (var device in playerInput.devices)
                if (device is Gamepad g) return g;
            return null;
        }
    }

    void Awake()
    {
        var map = inputActions.FindActionMap("Player");
        aimAction = map.FindAction("Aim");

        if (cursorSprite == null)
            cursorSprite = GetComponentInChildren<SpriteRenderer>();

        player = GetComponentInParent<Player>();
        playerInput = GetComponentInParent<PlayerInput>();

        localCursorPos = Vector2.right * minRadius;

        //validacion de spider
        if (onlySpider && player != null && player.webController == null)
        {
            enabled = false;
            gameObject.SetActive(false);
            return;
        }
    }

    // Start()

    void Update()
    {
        if (transform.parent == null) return;

        DetectDevice();

        if (usingGamepad)
            UpdateGamepadCursor();
        else
            UpdateMouseCursor();

        Vector2 delta = AimWorldPos - (Vector2)transform.parent.position;
        AimDirection = delta.sqrMagnitude > 0.0001f ? delta.normalized : Vector2.right;

        transform.position = AimWorldPos;
    }

    void DetectDevice()
    {
        Gamepad pad = MyGamepad;

        if (pad != null && pad.rightStick.ReadValue().magnitude > 0.1f)
        {
            usingGamepad = true;
            Cursor.visible = false;
            return;
        }

        if (Mouse.current != null && Mouse.current.delta.ReadValue().magnitude > 0.5f)
        {
            usingGamepad = false;
            Cursor.visible = true;
        }
    }

    void UpdateMouseCursor()
    {
        if (Mouse.current == null || Camera.main == null) return;

        Vector3 screenPos = Mouse.current.position.ReadValue();
        screenPos.z = Mathf.Abs(Camera.main.transform.position.z);

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        Vector2 playerPos = transform.parent.position;
        Vector2 delta = worldPos - playerPos;

        if (delta.magnitude > maxRadius)
            worldPos = playerPos + delta.normalized * maxRadius;

        if (delta.magnitude < minRadius)
            worldPos = playerPos + (delta.sqrMagnitude > 0.0001f ? delta.normalized : Vector2.right) * minRadius;

        AimWorldPos = worldPos;
    }

    void UpdateGamepadCursor()
    {
        Gamepad pad = MyGamepad;
        if (pad == null) return;

        Vector2 stick = pad.rightStick.ReadValue();

        float mag = stick.magnitude;

        if (mag < 0.15f)
            stick = Vector2.zero;
        else
            stick = stick.normalized * ((mag - 0.15f) / 0.85f);

        localCursorPos += stick * stickSpeed * Time.deltaTime;

        if (localCursorPos.magnitude > maxRadius)
            localCursorPos = localCursorPos.normalized * maxRadius;

        if (localCursorPos.magnitude < minRadius)
            localCursorPos = localCursorPos.normalized * minRadius;

        AimWorldPos = (Vector2)transform.parent.position + localCursorPos;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!Application.isPlaying || transform.parent == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.parent.position, AimWorldPos);
        Gizmos.DrawWireSphere(AimWorldPos, 0.1f);
    }
#endif
}