using UnityEngine;
using UnityEngine.InputSystem;

public class WebShooter : MonoBehaviour
{
    [Header("Input")]
    public InputActionAsset inputActions;

    private InputAction shootWebAction;

    [Header("References")]
    public LayerMask grappleLayer;
    public float maxDistance = 20f;
    public WebConnection webConnection;
    public WebRenderer webRenderer;

    private void Awake()
    {
        shootWebAction = inputActions.FindAction("ShootWeb");
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
        if (shootWebAction.WasPressedThisFrame())
            TryAttach();

        if (shootWebAction.WasReleasedThisFrame())
            Detach();
    }

    void TryAttach()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );

        Vector2 direction = (mousePos - (Vector2)transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            direction,
            maxDistance,
            grappleLayer
        );

        if (hit.collider == null) return;

        webConnection.Attach(hit.point);
        webRenderer.Enable();
    }

    void Detach()
    {
        webConnection.Detach();
        webRenderer.Disable();
    }
}