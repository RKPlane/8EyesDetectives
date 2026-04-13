using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Two players on one keyboard — no InputActionAsset needed for MantisPlayer.
///
/// Player 1 (Spider / WASD player) — no changes needed there.
/// Player 2 (Mantis) uses arrow keys by default:
///
///   Move        ← → Arrow keys
///   Jump        Numpad 0  (or reassign jumpKey below)
///   Pick up     Numpad 1
///   Throw       Numpad 2
///
/// To reassign, change the Key fields in the Inspector.
/// </summary>
public class MantisPlayer : MonoBehaviour
{
    // ── Movement & physics ────────────────────────────────────────────────
    public float speed = 5f;
    public float jumpForce = 6f;
    public float lerpSpeed = 0.85f;
    public float throwForce = 5f;
    private Rigidbody2D rb;

    // ── Key bindings (reassignable from the Inspector) ────────────────────
    [Header("Key Bindings")]
    public Key moveLeftKey = Key.LeftArrow;
    public Key moveRightKey = Key.RightArrow;
    public Key jumpKey = Key.Numpad0;
    public Key pickUpKey = Key.Numpad1;
    public Key throwKey = Key.Numpad2;

    // ── Pick-up / carry ───────────────────────────────────────────────────
    [SerializeField] private bool isHolding = false;
    private GameObject heldObject = null;
    private float maxThrowMultiplier = 1.5f;
    [SerializeField] private Collider2D pickUpCollider;
    [SerializeField] private Transform carryCheck;
    public Collider2D carryCollider;
    private LayerMask layerDefaultObjetos;
    private LayerMask layerHeld;

    // ── Ground check ──────────────────────────────────────────────────────
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.6f;
    [SerializeField] private LayerMask groundLayer;

    // ── Sprite ────────────────────────────────────────────────────────────
    public bool bFaceRight;

    // ── Internal state ────────────────────────────────────────────────────
    private float moveInput;   // -1, 0, or 1

    // ─────────────────────────────────────────────────────────────────────

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        layerDefaultObjetos = LayerMask.NameToLayer("Default");
        layerHeld = LayerMask.NameToLayer("NoCollision");
    }

    void Update()
    {
        // ── Read movement ─────────────────────────────────────────────────
        var kb = Keyboard.current;
        moveInput = 0f;
        if (kb[moveLeftKey].isPressed) moveInput = -1f;
        if (kb[moveRightKey].isPressed) moveInput = 1f;

        // ── Pick up / drop ────────────────────────────────────────────────
        if (kb[pickUpKey].wasPressedThisFrame)
        {
            if (isHolding)
            {
                Soltar();
            }
            else
            {
                Collider2D[] results = new Collider2D[20];
                int count = pickUpCollider.Overlap(ContactFilter2D.noFilter, results);
                Collider2D closest = null;
                float minDistance = 10f;

                for (int i = 0; i < count; i++)
                {
                    if (results[i].gameObject.CompareTag("Carryable"))
                    {
                        float d = Vector2.Distance(transform.position, results[i].transform.position);
                        if (d < minDistance) { minDistance = d; closest = results[i]; }
                    }
                }

                if (closest != null) Coger(closest);
            }
        }

        // ── Throw ─────────────────────────────────────────────────────────
        if (kb[throwKey].wasPressedThisFrame && isHolding)
            Lanzar();
    }

    void FixedUpdate()
    {
        // ── Ground check ──────────────────────────────────────────────────
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        // ── Horizontal movement ───────────────────────────────────────────
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        // ── Jump ──────────────────────────────────────────────────────────
        if (Keyboard.current[jumpKey].isPressed && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        // ── Flip sprite ───────────────────────────────────────────────────
        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);

        // ── Carry lerp ────────────────────────────────────────────────────
        if (isHolding)
            heldObject.transform.position = Vector2.Lerp(
                heldObject.transform.position,
                carryCheck.position,
                lerpSpeed);
    }

    // ── Pick-up helpers ───────────────────────────────────────────────────

    public void Soltar()
    {
        isHolding = false;
        heldObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        heldObject.gameObject.layer = layerDefaultObjetos;
        heldObject.transform.parent = null;
        heldObject = null;
    }

    public void Coger(Collider2D objeto)
    {
        isHolding = true;
        heldObject = objeto.gameObject;
        heldObject.transform.parent = carryCollider.transform;
        objeto.attachedRigidbody.bodyType = RigidbodyType2D.Kinematic;
        objeto.gameObject.layer = layerHeld;
    }

    private void Lanzar()
    {
        GameObject objeto = heldObject;
        Soltar();
        objeto.GetComponent<Rigidbody2D>().linearVelocityY =
            throwForce + Mathf.Clamp(rb.linearVelocityY, 0, throwForce * maxThrowMultiplier);
    }
}
