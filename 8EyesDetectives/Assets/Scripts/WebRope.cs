using System.Collections.Generic;
using UnityEngine;

public class WebRope : MonoBehaviour
{
    [Header("Segment Settings")]
    public GameObject segmentPrefab;
    public int segmentCount = 16;

    [Header("Collision")]
    [Tooltip("Multiplier of segment spacing used as collider radius. Keep above 0.5 to avoid gaps.")]
    public float colliderRadiusMultiplier = 0.6f;

    readonly List<Rigidbody2D> segments = new List<Rigidbody2D>();
    Rigidbody2D playerRb;
    HingeJoint2D playerJoint;

    public IReadOnlyList<Rigidbody2D> Segments => segments;
    public bool IsBuilt => segments.Count > 0;
    public bool IsPlayerAttached { get; private set; }
    public Vector2 AnchorPoint { get; private set; }

    public void Build(Vector2 anchorPoint, Rigidbody2D player)
    {
        Clear();
        playerRb = player;
        AnchorPoint = anchorPoint;

        float ropeLength = Vector2.Distance(anchorPoint, player.position);
        float segmentSpacing = ropeLength / (segmentCount - 1);
        float colliderRadius = segmentSpacing * colliderRadiusMultiplier;

        for (int i = 0; i < segmentCount; i++)
        {
            float t = (float)i / (segmentCount - 1);
            Vector2 pos = Vector2.Lerp(anchorPoint, player.position, t);
            GameObject go = Instantiate(segmentPrefab, pos, Quaternion.identity);

            CircleCollider2D col = go.GetComponent<CircleCollider2D>();
            if (col == null) col = go.AddComponent<CircleCollider2D>();
            col.radius = colliderRadius;

            segments.Add(go.GetComponent<Rigidbody2D>());
        }

        //Primer segmento se ancla al mundo
        HingeJoint2D anchor = segments[0].gameObject.AddComponent<HingeJoint2D>();
        anchor.autoConfigureConnectedAnchor = false;
        anchor.connectedAnchor = anchorPoint;

        //Cadena de segmentos
        for (int i = 1; i < segmentCount; i++)
        {
            HingeJoint2D joint = segments[i].gameObject.AddComponent<HingeJoint2D>();
            joint.connectedBody = segments[i - 1];
            joint.autoConfigureConnectedAnchor = true;
        }

        //El ultimo segmento va al Player
        playerJoint = player.gameObject.AddComponent<HingeJoint2D>();
        playerJoint.connectedBody = segments[segmentCount - 1];
        playerJoint.autoConfigureConnectedAnchor = true;

        IsPlayerAttached = true;
    }

    //Despega al player y ancla el segmento al radio especificado mas cercano
    public void DetachAndStick(LayerMask grappleLayer, float stickRadius)
    {
        if (!IsBuilt) return;

        RemovePlayerJoint();
        IsPlayerAttached = false;
        playerRb = null;

        Rigidbody2D lastSeg = segments[segments.Count - 1];
        Collider2D hit = Physics2D.OverlapCircle(lastSeg.position, stickRadius, grappleLayer);

        if (hit != null)
        {
            Vector2 pinPoint = hit.ClosestPoint(lastSeg.position);

            HingeJoint2D pin = lastSeg.gameObject.AddComponent<HingeJoint2D>();
            pin.autoConfigureConnectedAnchor = false;

            Rigidbody2D surfaceRb = hit.attachedRigidbody;
            if (surfaceRb != null)
            {
                pin.connectedBody = surfaceRb;
                pin.connectedAnchor = surfaceRb.transform.InverseTransformPoint(pinPoint);
            }
            else
            {
                pin.connectedAnchor = pinPoint; // world space SI NO HAY rigidbody
            }
        }
        // si falla solo se desplega del ancla
    }

    public void Clear()
    {
        RemovePlayerJoint();
        foreach (var seg in segments)
            if (seg != null) Destroy(seg.gameObject);
        segments.Clear();
        playerRb = null;
        IsPlayerAttached = false;
    }

    // METODO FUTURO
    public void Cut(int segmentIndex)
    {
        if (!IsBuilt || segmentIndex < 0 || segmentIndex >= segments.Count) return;

        RemovePlayerJoint();
        IsPlayerAttached = false;

        for (int i = segmentIndex; i < segments.Count; i++)
            if (segments[i] != null) Destroy(segments[i].gameObject);

        segments.RemoveRange(segmentIndex, segments.Count - segmentIndex);
    }

    void RemovePlayerJoint()
    {
        if (playerJoint != null)
        {
            Destroy(playerJoint);
            playerJoint = null;
        }
        playerRb = null;
    }
}
