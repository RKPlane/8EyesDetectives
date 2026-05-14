using UnityEngine;

[RequireComponent(typeof(WebRenderer))]
public class WebAnchor : MonoBehaviour
{
    [Header("Puntos de anclaje")]
    [Tooltip("Point A")]
    public Transform anchorA;

    [Tooltip("Point B")]
    public Transform anchorB;

    [Header("Rope")]
    public WebRope rope;

    void Awake()
    {
        if (rope == null)
        {
            Debug.LogError("Falta asignar la WebRope en el Inspector.", this);
            return;
        }
        if (anchorA == null || anchorB == null)
        {
            Debug.LogError("Faltan anchorA o anchorB.", this);
            return;
        }

        rope.BuildStatic(anchorA.position, anchorB.position);

        // Activa el renderer si hay uno asignado
        GetComponent<WebRenderer>()?.Enable();
    }

#if UNITY_EDITOR
    // Dibuja un preview en la escena para facilitar el placement
    void OnDrawGizmos()
    {
        if (anchorA == null || anchorB == null) return;
        Gizmos.color = new Color(0.9f, 0.9f, 0.4f, 0.8f);
        Gizmos.DrawLine(anchorA.position, anchorB.position);
        Gizmos.DrawWireSphere(anchorA.position, 0.15f);
        Gizmos.DrawWireSphere(anchorB.position, 0.15f);
    }
#endif
}