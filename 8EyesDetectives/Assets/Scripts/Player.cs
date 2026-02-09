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


    [Header("Input Settings")]
    public InputActionAsset inputActions; // Da acceso a todas las acciones de input definidas en el Input Action Asset
    private InputAction m_moveAction;// Se utiliza para almacenar la acción que queremos utilizar
    private InputAction m_jumpAction;// Salto
    private Vector2 moveInput;
    private bool jumpPressed;

    //Sprite
    public bool bFaceRight;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        m_moveAction = InputSystem.actions.FindAction("Move");
        m_jumpAction = InputSystem.actions.FindAction("Jump");
    }

    //Fisicas

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
        }*/
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = m_moveAction.ReadValue<Vector2>();

    }

    void Turn() //testing con el spriterenderer
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.flipX = !sr.flipX;

        bFaceRight = !bFaceRight;
    }
}
