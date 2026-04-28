using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 6f;
    private Rigidbody2D rb;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.8f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    [Header("Web Reference")]
    public WebController webController;

    [Header("Input")]
    public InputActionAsset inputActions;

    private InputAction moveAction;
    private InputAction jumpAction;
    private float moveInput;

    public bool bFaceRight;
    public Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        var map = inputActions.FindActionMap("Player");
        moveAction = map.FindAction("Move");
        jumpAction = map.FindAction("SpiderJump");
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>().x;
    }

    void FixedUpdate()
    {
        bool isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

        bool isSwinging = webController != null && webController.IsAnyAttached;

        animator.SetBool("isJumping", !isGrounded && !isSwinging);
        animator.SetBool("isHanging", isSwinging);

        if (!isSwinging)
        {
            rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
            animator.SetFloat("Speed", moveInput * moveInput);
        }

        if (jumpAction.IsPressed() && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
    }
}
