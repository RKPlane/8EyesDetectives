using UnityEngine;
using UnityEngine.InputSystem;

public class MantisPlayer : MonoBehaviour
{
    //Player
    public float speed = 5f;
    public float jumpForce = 6f;
    private Rigidbody2D rb;
    public float lerpSpeed = 0.85f;
    private bool isHolding = false;
    private GameObject heldObject = null;

    //Ground check
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform carryCheck;
    [SerializeField] private float groundRadius = 0.6f;
    [SerializeField] private LayerMask groundLayer;


    [Header("Input Settings")]
    public InputActionAsset inputActions; // Da acceso a todas las acciones de input definidas en el Input Action Asset
    private InputAction m_moveAction;// Se utiliza para almacenar la acción que queremos utilizar
    private InputAction m_jumpAction;// Salto
    private InputAction m_pickUpAction;
    public Collider2D carryCollider;
    private Vector2 moveInput;
    private bool jumpPressed;

    //Sprite
    public bool bFaceRight;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        m_moveAction = InputSystem.actions.FindAction("Move");
        m_jumpAction = InputSystem.actions.FindAction("Jump");
        m_pickUpAction = InputSystem.actions.FindAction("Interact");
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

        if (isHolding)
        {
            heldObject.transform.position = Vector2.Lerp(heldObject.transform.position, carryCheck.transform.position, lerpSpeed);
        }
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (m_pickUpAction.IsPressed())
        {
            if (collision.gameObject.CompareTag("Carryable") && !isHolding)
            {
                isHolding = true;
                heldObject = collision.gameObject;
                heldObject.transform.parent = carryCollider.transform;
            }
        }
    }
}
