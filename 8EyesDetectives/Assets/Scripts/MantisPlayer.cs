using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class MantisPlayer : MonoBehaviour
{
    static public MantisPlayer instance;

    // ── Movement & physics ────────────────────────────────────────────────
    public float speed = 5f;
    public float jumpForce = 6f;
    public float lerpSpeed = 0.85f;
    public float throwForce = 11f;
    private Rigidbody2D rb;

    // ── Input ─────────────────────────────────────────────────────────────
    [Header("Input")]
    private bool tryingJump = false;

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
    private Vector2 moveInput;
    private bool pickUpPressed;
    private bool throwPressed;
    private bool cutWebPressed;
    public bool control = false;

    void Awake()
    {
        instance = this;

        rb = GetComponent<Rigidbody2D>();
        layerDefaultObjetos = LayerMask.NameToLayer("Default");
        layerHeld = LayerMask.NameToLayer("NoCollision");
    }

    public void OnMove(InputValue value)
    {
        if (!control) return;

        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (!control) return;

        tryingJump = value.isPressed;
    }

    public void OnPickUp(InputValue value)
    {
        if (!control) return;

        if (value.isPressed)
            pickUpPressed = true;
    }

    public void OnThrow(InputValue value)
    {
        if (!control) return;

        if (value.isPressed)
            throwPressed = true;
    }

    public void OnCutWeb(InputValue value)
    {
        if (!control) return;

        if (value.isPressed)
            cutWebPressed = true;
    }

    void Update()
    {
        if (!control)
        {
            moveInput = Vector2.zero;
            tryingJump = false;
            return;
        }

        if (pickUpPressed)
        {
            pickUpPressed = false;

            if (isHolding) Soltar();
            else TryPickUp();
        }

        if (throwPressed)
        {
            throwPressed = false;

            if (isHolding)
                Lanzar();
        }

        if (cutWebPressed)
        {
            cutWebPressed = false;
            TryCutWeb();
        }
    }

    void FixedUpdate()
    {
        bool isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
        animator.SetBool("isJumping", !isGrounded);

        rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);
        animator.SetFloat("Speed", moveInput.x * moveInput.x);

        if (tryingJump && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (moveInput.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < 0) transform.localScale = new Vector3(-1, 1, 1);

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
        objeto.GetComponent<Rigidbody2D>().linearVelocityY = throwForce + Mathf.Clamp(rb.linearVelocityY, 0, throwForce * maxThrowMultiplier);
    }
}
