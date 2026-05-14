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

    [Header("Webs (3 slots — assign in Inspector)")]
    public WebRope[] ropes = new WebRope[3];
    public WebRenderer[] renderers = new WebRenderer[3];

    Rigidbody2D rb;
    InputAction shootAction, detachAction, moveAction;
    Vector2 moveInput;

    int attachedIndex = -1;

    // FIX
    public bool IsAnyAttached =>
        attachedIndex >= 0 &&
        attachedIndex < ropes.Length &&
        ropes[attachedIndex] != null &&
        ropes[attachedIndex].IsPlayerAttached;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        var map = inputActions.FindActionMap("Player");
        shootAction = map.FindAction("ShootWeb");
        detachAction = map.FindAction("DetachWeb");
        moveAction = map.FindAction("Move");
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

        // Si la rope fue cortada por la mantis, limpia el índice.
        if (attachedIndex >= 0 &&
            (ropes[attachedIndex] == null || !ropes[attachedIndex].IsPlayerAttached))
        {
            attachedIndex = -1;
        }

        if (shootAction.WasPressedThisFrame()) TryAttach();
        if (detachAction.WasPressedThisFrame()) TryDetach();
    }

    void FixedUpdate()
    {
        if (IsAnyAttached)
            ApplySwingForce(ropes[attachedIndex]);
    }

    void TryAttach()
    {
        if (IsAnyAttached) { Debug.Log("[Web] Already attached, ignoring shoot"); return; }

        int slot = -1;
        for (int i = 0; i < ropes.Length; i++)
        {
            string state = ropes[i] == null ? "NULL" : ropes[i].IsBuilt ? "built" : "free";
            Debug.Log($"[Web] Slot {i}: {state}");
            if (ropes[i] != null && !ropes[i].IsBuilt) { slot = i; break; }
        }

        if (slot < 0) { Debug.Log("[Web] No free slot — all 3 webs exist"); return; }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = (mousePos - (Vector2)transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grappleLayer);
        if (hit.collider == null) { Debug.Log("[Web] Raycast missed"); return; }

        ropes[slot].Build(hit.point, rb);
        renderers[slot]?.Enable();
        attachedIndex = slot;
        Debug.Log($"[Web] Attached to slot {slot}");
    }

    void TryDetach()
    {
        Debug.Log($"[Web] TryDetach — attachedIndex={attachedIndex}, IsAnyAttached={IsAnyAttached}");
        if (!IsAnyAttached) return;
        ropes[attachedIndex].DetachAndStick(grappleLayer, stickRadius);
        attachedIndex = -1;
    }

    void ApplySwingForce(WebRope rope)
    {
        if (rope == null || !rope.IsPlayerAttached) return;
        Vector2 toAnchor = (Vector2)transform.position - rope.AnchorPoint;
        Vector2 tangent = Vector2.Perpendicular(toAnchor.normalized);
        rb.AddForce(tangent * moveInput.x * swingForce);
    }
}
