using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //Player
    public float speed = 5f;
    public float jumpForce = 6f;
    private Rigidbody2D rb;

    //Ground check
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.6f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Web Reference")]
    public WebController webController;

    private float moveInput;   // -1, 0, or 1


    [Header("Input Settings")]
    public InputActionAsset inputActions; // Da acceso a todas las acciones de input definidas en el Input Action Asset
    private InputAction m_moveAction;// Se utiliza para almacenar la accion que queremos utilizar
    private InputAction m_jumpAction;// Salto
    //private Vector2 moveInput;
    private bool jumpPressed;

    // ?? Key bindings (reassignable from the Inspector) ????????????????????
    [Header("Key Bindings")]
    public Key moveLeftKey = Key.A;
    public Key moveRightKey = Key.D;
    public Key jumpKey = Key.W;

    //Sprite
    public bool bFaceRight;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //m_moveAction = InputSystem.actions.FindAction("Move1");
        //m_jumpAction = InputSystem.actions.FindAction("Jump");
    }

    //Fisicas
    /*
    void FixedUpdate()
    {
        //Ground Check
        bool isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );

        //Movement
        float horizontalMovement = moveInput.x;
        bool isSwinging = webController != null && webController.IsAnyAttached;
        if (!isSwinging)
            rb.linearVelocity = new Vector2(horizontalMovement * speed, rb.linearVelocity.y);
        //characterAnimator.SetFloat("MovementSpeed", Mathf.Abs(horizontalMovement));

        //SALTO
        if (m_jumpAction.IsPressed() && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        /* TURN
         
         if (horizontalMovement < 0 && bFaceRight ||
            horizontalMovement > 0 && !bFaceRight)
        {
            Turn();
        }
    }*/

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        var kb = Keyboard.current;
        moveInput = 0f;
        if (kb[moveLeftKey].isPressed) moveInput = -1f;
        if (kb[moveRightKey].isPressed) moveInput = 1f;
    }

    void FixedUpdate()
    {
        var kb = Keyboard.current;

        // ?? Ground check ??????????????????????????????????????????????????
        bool isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );

        // ?? Horizontal movement ???????????????????????????????????????????
        bool isSwinging = webController != null && webController.IsAnyAttached;
        if (!isSwinging)
            rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        // ?? Jump ??????????????????????????????????????????????????????????
        if (kb[jumpKey].isPressed && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        // ?? Flip sprite ???????????????????????????????????????????????????
        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    void Turn() // kept for reference
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.flipX = !sr.flipX;
        bFaceRight = !bFaceRight;
    }
}
