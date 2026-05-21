using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class WebController : MonoBehaviour
{
    [Header("Input")]
    private Player player;

    [Header("Aim")]
    [Tooltip("Arrastra aquí el componente AimCursor del hijo cursor.")]
    public AimCursor aimCursor;

    [Header("Web Settings")]
    public LayerMask grappleLayer;

    [Tooltip("Layers que bloquean el disparo (Floor, paredes, etc.). " +
             "Configura a 'Floor' en el Inspector.")]
    public LayerMask obstacleLayer;

    public float maxDistance = 20f;
    public float stickRadius = 1.5f;
    public float swingForce = 5f;

    [Header("Webs (3 slots — assign in Inspector)")]
    public WebRope[] ropes = new WebRope[3];
    public WebRenderer[] renderers = new WebRenderer[3];

    Rigidbody2D rb;
    private float moveInput;
    private bool shootPressed;
    private bool detachPressed;

    int attachedIndex = -1;

    public bool IsAnyAttached =>
        attachedIndex >= 0 &&
        attachedIndex < ropes.Length &&
        ropes[attachedIndex] != null &&
        ropes[attachedIndex].IsPlayerAttached;

    void Awake()
    {
            rb = GetComponent<Rigidbody2D>();
            player = GetComponent<Player>();

    }

    public void OnMove(InputValue value)
    {
        if (player != null && player.control)
            moveInput = value.Get<Vector2>().x;
    }

    public void OnShootWeb(InputValue value)
    {
        if (player != null && player.control && value.isPressed)
            shootPressed = true;
    }

    public void OnDetachWeb(InputValue value)
    {
        if (player != null && player.control && value.isPressed)
            detachPressed = true;
    }

    void Update()
    {
        if (attachedIndex >= 0 &&
            (ropes[attachedIndex] == null || !ropes[attachedIndex].IsPlayerAttached))
        {
            attachedIndex = -1;
        }

        if (shootPressed)
        {
            shootPressed = false;
            TryAttach();
        }

        if (detachPressed)
        {
            detachPressed = false;
            TryDetach();
        }
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

        // Dirección: desde AimCursor si está asignado, si no fallback al ratón
        Vector2 direction;
        if (aimCursor != null)
        {
            direction = aimCursor.AimDirection;
        }
        else
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            direction = (mousePos - (Vector2)transform.position).normalized;
        }

        // Primero comprobamos si hay un obstáculo (Floor u otros) antes del punto de grapple.
        // Si el primer hit es un obstáculo y NO es grappleable, bloqueamos el disparo.
        LayerMask combinedMask = grappleLayer | obstacleLayer;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, combinedMask);

        if (hit.collider == null)
        {
            Debug.Log("[Web] Raycast missed");
            return;
        }

        // Si lo que se golpea primero está en obstacleLayer pero NO en grappleLayer, bloqueado
        int hitLayerMask = 1 << hit.collider.gameObject.layer;
        bool isGrappleable = (hitLayerMask & grappleLayer.value) != 0;

        if (!isGrappleable)
        {
            Debug.Log($"[Web] Bloqueado por obstáculo: {hit.collider.gameObject.name}");
            return;
        }

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
        rb.AddForce(tangent * moveInput * swingForce);
    }
}