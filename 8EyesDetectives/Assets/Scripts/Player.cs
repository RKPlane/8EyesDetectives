using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    [Header("Input")]
    public InputActionAsset inputActions;
    private bool tryingJump = false;

    private InputAction moveAction;
    private InputAction jumpAction;
	private InputAction resetAction;
	private float moveInput;

    public bool control = false;
    public bool bFaceRight;
    public Animator animator;

    void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        var map = inputActions.FindActionMap("Player");
        moveAction = map.FindAction("Move");
        jumpAction = map.FindAction("SpiderJump");
        resetAction = map.FindAction("Reset");
	}

    void Update()
    {
        if (control)
        {
            moveInput = moveAction.ReadValue<Vector2>().x;
            tryingJump = jumpAction.IsPressed();
        } else
        {
            //Si el control se quita mientras moveInput tiene un valor, el valor nunca volvería a cero, por eso hace falta esta línea
            moveInput = 0f;
            tryingJump = false;
        }

		if (resetAction.WasPressedThisFrame()) 
		{
			RestartCurrentScene();
		}

	}

    void FixedUpdate()
    {
        bool isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

        bool isSwinging = webController != null && webController.IsAnyAttached;

        animator.SetBool("isJumping", !isGrounded && !isSwinging);

        if (!isSwinging)
        {
            animator.SetBool("isHanging", false);

        } else
        {
            animator.SetBool("isHanging", true);

        }

        if (!isSwinging)
        {
            rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
            animator.SetFloat("Speed", moveInput * moveInput);
        }

        if (tryingJump && isGrounded && !isSwinging)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

	public void RestartCurrentScene()
	{
		Scene currentScene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(currentScene.name);
	}

}

