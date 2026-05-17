using UnityEngine;

[RequireComponent(typeof(WebRenderer))]
public class WebAnchor : MonoBehaviour
{
    [Header("Puntos de anclaje")]
    [Tooltip("Extremo A de la web")]
    public Transform anchorA;

    [Tooltip("Extremo B de la web")]
    public Transform anchorB;

    [Header("Rigidbodies dinámicos (opcional)")]
    [Tooltip("SI EL POINTB ESTA PEGADO A ALGO PONLE EL RIGIDBODY DEL OBJETO TARGET AQUI SI NO DEJALO VACIO")]
    public Rigidbody2D rbA;

    [Tooltip("SI EL POINTB ESTA PEGADO B ALGO PONLE EL RIGIDBODY DEL OBJETO TARGET AQUI SI NO DEJALO VACIO")]
    public Rigidbody2D rbB;

    [Header("Referencias")]
    public WebRope rope;

    void Awake()
    {
        if (rope == null)
        {
            Debug.LogError("[WebAnchor] Falta asignar la WebRope en el Inspector.", this);
            return;
        }
        if (anchorA == null || anchorB == null)
        {
            Debug.LogError("[WebAnchor] Faltan anchorA o anchorB.", this);
            return;
        }

        rope.BuildStatic(anchorA.position, anchorB.position, rbA, rbB);

        // Activa el renderer si hay uno asignado
        GetComponent<WebRenderer>()?.Enable();
    }

#if UNITY_EDITOR
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