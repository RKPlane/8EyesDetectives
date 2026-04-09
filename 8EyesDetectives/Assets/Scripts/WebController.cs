using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class WebController : MonoBehaviour
{
    [Header("Input")]
    public InputActionAsset inputActions;

    [Header("Web Settings")]
    public LayerMask grappleLayer;
    public float maxDistance = 20f;
    public float stickRadius = 1.5f;
    public float swingForce = 5f;

    [Header("Left Web")]
    public WebRope ropeLeft;
    public WebRenderer rendererLeft;

    [Header("Right Web")]
    public WebRope ropeRight;
    public WebRenderer rendererRight;

    Rigidbody2D rb;
    InputAction shootLeft, shootRight, detachLeft, detachRight, moveAction;
    Vector2 moveInput;

    public bool IsLeftAttached  => ropeLeft  != null && ropeLeft.IsPlayerAttached;
    public bool IsRightAttached => ropeRight != null && ropeRight.IsPlayerAttached;
    public bool IsAnyAttached   => IsLeftAttached || IsRightAttached;

    void Awake()
    {
        rb          = GetComponent<Rigidbody2D>();
        shootLeft   = inputActions.FindAction("ShootWebLeft");
        shootRight  = inputActions.FindAction("ShootWebRight");
        detachLeft  = inputActions.FindAction("DetachWebLeft");
        detachRight = inputActions.FindAction("DetachWebRight");
        moveAction  = inputActions.FindAction("Move");
    }

    void OnEnable()  => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

        if (shootLeft.WasPressedThisFrame())   TryAttach(ropeLeft,  rendererLeft);
        if (shootRight.WasPressedThisFrame())  TryAttach(ropeRight, rendererRight);
        if (detachLeft.WasPressedThisFrame())  DetachAndStick(ropeLeft);
        if (detachRight.WasPressedThisFrame()) DetachAndStick(ropeRight);
    }

    void FixedUpdate()
    {
        ApplySwingForce(ropeLeft);
        ApplySwingForce(ropeRight);
    }

    void TryAttach(WebRope rope, WebRenderer renderer)
    {
        Vector2 mousePos  = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = (mousePos - (Vector2)transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grappleLayer);
        if (hit.collider == null) return;

        rope.Build(hit.point, rb);
        renderer.Enable();
    }

    void DetachAndStick(WebRope rope)
    {
        rope.DetachAndStick(grappleLayer, stickRadius);
    }

    void ApplySwingForce(WebRope rope)
    {
        if (rope == null || !rope.IsPlayerAttached) return;

        Vector2 toAnchor = (Vector2)transform.position - rope.AnchorPoint;
        Vector2 tangent  = Vector2.Perpendicular(toAnchor.normalized);
        rb.AddForce(tangent * moveInput.x * swingForce);
    }
}
