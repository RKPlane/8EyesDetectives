using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 6f;
    private Rigidbody2D rb;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.6f;
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
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        animator.SetBool("isJumping", !isGrounded);

        bool isSwinging = webController != null && webController.IsAnyAttached;
        if (!isSwinging)
        {
            rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
            animator.SetFloat("Speed", moveInput * moveInput);
        }

        if (jumpAction.IsPressed() && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetBool("isJumping", true);
        }

        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
    }
}
