using UnityEngine;

public class MoveableWeightPlatform : MonoBehaviour
{
    public bool weighted = false;
    public bool unweighted = false;
    public float weight;

    [SerializeField] private GameObject target;

    Vector3 origin;
    Vector3 distance;
    Vector3 previousPosition;

    private Rigidbody2D playerRb = null;

    private void Awake()
    {
        origin = transform.position;
    }

    void Start()
    {
        distance = transform.position - target.transform.position;
        previousPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (weighted)
        {
            Move();
        }
        if (unweighted)
        {
            MoveBack();
        }

        // Move
        if (playerRb != null)
        {
            Vector2 delta = (Vector2)transform.position - (Vector2)previousPosition;
            if (delta != Vector2.zero) { playerRb.position += delta; }
        }

        previousPosition = transform.position;
    }

    public void Move()
    {
        Vector3 targetPosition = target.transform.position - distance;
        Vector3 lerpTarget = Vector3.Lerp(origin, targetPosition, weight);
        transform.position = Vector3.Lerp(transform.position, lerpTarget, 2f * Time.fixedDeltaTime);
    }

    public void MoveBack()
    {
        transform.position = Vector2.Lerp(transform.position, origin, Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerRb = null;
        }
    }
}