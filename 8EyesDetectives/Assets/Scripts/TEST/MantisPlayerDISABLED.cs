using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MantisPlayerDISABLED : MonoBehaviour
{
    //Player
    public float speed = 5f;
    public float jumpForce = 6f;
    private Rigidbody2D rb;
    public float lerpSpeed = 0.85f;
    [SerializeField] private bool isHolding = false;
    public float throwForce = 5f;

    //Coger y lanzar objetos
    private GameObject heldObject = null;
    private float maxThrowMultiplier = 1.5f; // Máximo valor por el cual se multiplicará throwForce cuando la Mantis lance algo mientras salta
    [SerializeField] private Collider2D pickUpCollider;
    [SerializeField] private Transform carryCheck;
    private LayerMask layerDefaultObjetos;
    private LayerMask layerHeld;

    //Ground check
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.6f;
    [SerializeField] private LayerMask groundLayer;


    [Header("Input Settings")]
    public InputActionAsset inputActions; // Da acceso a todas las acciones de input definidas en el Input Action Asset
    private InputAction m_moveAction;// Se utiliza para almacenar la acción que queremos utilizar
    private InputAction m_jumpAction;// Salto
    private InputAction m_pickUpAction; // Coger objetos
    private InputAction m_throwAction; // Lanzar objetos verticalmente
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
        m_throwAction = InputSystem.actions.FindAction("Attack");

        layerDefaultObjetos = LayerMask.NameToLayer("Default");
        layerHeld = LayerMask.NameToLayer("NoCollision");
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

        // Giro de la mantis
        if (horizontalMovement > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        } else if (horizontalMovement < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

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

        // Soltar y coger objetos
        if (m_pickUpAction.WasPressedThisFrame())
        {
            if (isHolding)
            {
                Soltar();
            }
            else
            {
                // Detección del objeto cogible más cercano a la Mantis
                Collider2D[] results = new Collider2D[20];
                int colliders = pickUpCollider.Overlap(ContactFilter2D.noFilter, results);
                Collider2D closest = null;
                float minDistance = 10f;
                for (int i = 0;  i < colliders; i++)
                {
                    if (results[i].gameObject.CompareTag("Carryable"))
                    {
                        float distance = Vector2.Distance(transform.position, results[i].transform.position);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closest = results[i];
                        }
                    }
                }
                if (closest != null)
                {
                    Coger(closest);
                }
            }
        }

        // Lanzar objeto cogido
        if (m_throwAction.WasPressedThisFrame() && isHolding)
        {
            Lanzar();
        }
    }

    void Turn() //testing con el spriterenderer
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.flipX = !sr.flipX;

        bFaceRight = !bFaceRight;
    }

    public void Soltar()
    {
        isHolding = false;
        Debug.Log("unheld");
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
        objeto.GetComponent<Rigidbody2D>().linearVelocityY = throwForce + Mathf.Clamp(rb.linearVelocityY, 0, throwForce * maxThrowMultiplier); //AddForce(Vector2.up * (throwForce + rb.linearVelocityY), ForceMode2D.Impulse);
    }
}
