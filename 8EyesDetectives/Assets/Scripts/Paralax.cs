using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Camera")]
    public Transform cam;

    [Header("Parallax")]
    [Range(0f, 1f)]
    public float parallaxX = 0.2f;

    [Range(0f, 1f)]
    private float parallaxY = 0f;

    private Vector3 lastCamPosition;

    void Start()
    {
        if (cam == null)
            cam = Camera.main.transform;

        lastCamPosition = cam.position;
    }

    void LateUpdate()
    {
        Vector3 camMovement = cam.position - lastCamPosition;

        // Aplicar solo parallax horizontal estable
        transform.position += new Vector3(
            camMovement.x * parallaxX,
            camMovement.y * parallaxY,
            0f
        );

        lastCamPosition = cam.position;
    }
}