using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerSwingController : MonoBehaviour
{
    [Header("Input")]
    public InputActionAsset inputActions;

    private InputAction moveAction;

    [Header("Swing Settings")]
    public float swingForce = 15f;
    public WebConnection connection;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        moveAction = inputActions.FindAction("Move");
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (!connection.IsAttached) return;

        Vector2 toAnchor = (Vector2)transform.position - connection.AnchorPoint;
        Vector2 tangent = Vector2.Perpendicular(toAnchor.normalized);

        rb.AddForce(tangent * moveInput.x * swingForce);
    }
}

/*
 | Input      | Action     |
| ---------- | ---------- |
| Mouse Left | Shoot web  |
| Release    | Detach     |
| A / D      | Pump swing |
| W          | Reel in    |
| S          | Reel out   |
 */