using UnityEngine;

public class Paralax: MonoBehaviour
{
    [Header("Players")]
    public Transform player1;
    public Transform player2;

    [Header("Parallax")]
    [Range(0f, 1f)]
    public float parallaxX = 0.5f;

    [Range(0f, 1f)]
    public float parallaxY = 0.5f;

    private Vector3 lastCenterPosition;

    void Start()
    {
        if (player1 == null || player2 == null)
        {
            Debug.LogError("Faltan referencias de players.");
            enabled = false;
            return;
        }

        lastCenterPosition = GetCenterPosition();
    }

    void LateUpdate()
    {
        Vector3 currentCenter = GetCenterPosition();

        // Movimiento entre frames
        Vector3 deltaMovement = currentCenter - lastCenterPosition;

        // Aplicar parallax
        transform.position += new Vector3(
            deltaMovement.x * parallaxX,
            deltaMovement.y * parallaxY,
            0f
        );

        // Guardar centro actual
        lastCenterPosition = currentCenter;
    }

    Vector3 GetCenterPosition()
    {
        return (player1.position + player2.position) / 2f;
    }
}
