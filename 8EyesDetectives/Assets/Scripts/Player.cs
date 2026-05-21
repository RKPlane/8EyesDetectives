using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    static public Player instance;
    public float speed = 5f;
    public float jumpForce = 6f;

    private Rigidbody2D rb;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.8f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    [Header("Web Reference")]
    public WebController webController;

    public bool control = true;
    public Animator animator;

    private Vector2 moveInput;
    private bool tryingJump;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // ACTION: Move
    public void OnMove(InputValue value)
    {
        if (!control)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = value.Get<Vector2>();
    }

    // ACTION: SpiderJump
    public void OnSpiderJump(InputValue value)
    {
        if (!control) return;

        tryingJump = value.isPressed;
    }

    // ACTION: Reset
    public void OnReset(InputValue value)
    {
        if (value.isPressed)
        {
            RestartCurrentScene();
        }
    }

    void FixedUpdate()
    {
        bool isGrounded = Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0f,
            groundLayer);

        bool isSwinging =
            webController != null &&
            webController.IsAnyAttached;

        animator.SetBool("isJumping", !isGrounded && !isSwinging);
        animator.SetBool("isHanging", isSwinging);

        if (!isSwinging)
        {
            rb.linearVelocity =
                new Vector2(moveInput.x * speed, rb.linearVelocity.y);

            animator.SetFloat("Speed", moveInput.x * moveInput.x);
        }

        if (!control)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (tryingJump && isGrounded && !isSwinging)
        {
            rb.linearVelocity =
                new Vector2(rb.linearVelocity.x, jumpForce);

            tryingJump = false;
        }

        if (moveInput.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    public void ForceStop()
    {
        moveInput = Vector2.zero;
        tryingJump = false;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }



    public void RestartCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}

