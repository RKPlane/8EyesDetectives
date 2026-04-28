using UnityEngine;
using UnityEngine.InputSystem;

public class MantisPlayer : MonoBehaviour
{
    // ── Movement & physics ────────────────────────────────────────────────
    public float speed = 5f;
    public float jumpForce = 6f;
    public float lerpSpeed = 0.85f;
    public float throwForce = 5f;
    private Rigidbody2D rb;

    // ── Input ─────────────────────────────────────────────────────────────
    [Header("Input")]
    public InputActionAsset inputActions;

    // ── Cut web ───────────────────────────────────────────────────────────
    [Header("Cut Web")]
    public float cutRadius = 1.5f;
    public LayerMask webLayer;

    // ── Pick-up / carry ───────────────────────────────────────────────────
    [SerializeField] private bool isHolding = false;
    private GameObject heldObject = null;
    private float maxThrowMultiplier = 1.5f;
    [SerializeField] private Collider2D pickUpCollider;
    [SerializeField] private Transform carryCheck;
    public Collider2D carryCollider;
    private int layerDefaultObjetos;
    private int layerHeld;

    // ── Ground check ──────────────────────────────────────────────────────
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(2f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    // ── Sprite ────────────────────────────────────────────────────────────
    public bool bFaceRight;

    // ── Animator ──────────────────────────────────────────────────────────
    public Animator animator;

    // ── Internal state ────────────────────────────────────────────────────
    private float moveInput;
    private InputAction moveAction, jumpAction, pickUpAction, throwAction, cutWebAction;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        layerDefaultObjetos = LayerMask.NameToLayer("Default");
        layerHeld = LayerMask.NameToLayer("NoCollision");

        var map = inputActions.FindActionMap("Mantis");
        moveAction = map.FindAction("Move");
        jumpAction = map.FindAction("Jump");
        pickUpAction = map.FindAction("PickUp");
        throwAction = map.FindAction("Throw");
        cutWebAction = map.FindAction("CutWeb");
    }

    void OnEnable() => inputActions.FindActionMap("Mantis").Enable();
    void OnDisable() => inputActions.FindActionMap("Mantis").Disable();

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>().x;

        if (pickUpAction.WasPressedThisFrame())
        {
            if (isHolding) Soltar();
            else TryPickUp();
        }

        if (throwAction.WasPressedThisFrame() && isHolding)
            Lanzar();

        if (cutWebAction.WasPressedThisFrame())
            TryCutWeb();
    }

    void FixedUpdate()
    {
        bool isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
        animator.SetBool("isJumping", !isGrounded);

        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
        animator.SetFloat("Speed", moveInput * moveInput);

        if (jumpAction.IsPressed() && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);

        if (isHolding)
            heldObject.transform.position = Vector2.Lerp(
                heldObject.transform.position,
                carryCheck.position,
                lerpSpeed);
    }

    // ── Pick-up helpers ───────────────────────────────────────────────────

    void TryPickUp()
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

    // ── Cut web ───────────────────────────────────────────────────────────

    void TryCutWeb()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, cutRadius, webLayer);
        if (hit == null) return;

        WebSegment seg = hit.GetComponent<WebSegment>();
        if (seg != null) seg.rope.Clear();
    }

    // ── Carry helpers ─────────────────────────────────────────────────────

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
