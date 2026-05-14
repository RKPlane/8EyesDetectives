using System.Collections.Generic;
using UnityEngine;

public class WebRope : MonoBehaviour
{
    [Header("Segment Settings")]
    public GameObject segmentPrefab;
    public int segmentCount = 16;

    [Header("Collision")]
    public float colliderRadiusMultiplier = 0.6f;

    //FIX
    [Header("Catenary")]
    [Range(0f, 0.5f)]
    public float catenarySag = 0.15f;

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

            // FIX
            Vector2 pos = CatenaryPoint(anchorPoint, player.position, t, catenarySag);

            GameObject go = Instantiate(segmentPrefab, pos, Quaternion.identity);
            go.layer = LayerMask.NameToLayer("Web");

            WebSegment seg = go.AddComponent<WebSegment>();
            seg.rope = this;

            CircleCollider2D col = go.GetComponent<CircleCollider2D>();
            if (col == null) col = go.AddComponent<CircleCollider2D>();
            col.radius = colliderRadius;

            Rigidbody2D segRb = go.GetComponent<Rigidbody2D>();

            // FIX
            segRb.simulated = false;

            segments.Add(segRb);
        }

        // Primer segmento anclado al mundo
        HingeJoint2D anchor = segments[0].gameObject.AddComponent<HingeJoint2D>();
        anchor.autoConfigureConnectedAnchor = false;
        anchor.connectedAnchor = anchorPoint;

        // Cadena de segmentos
        for (int i = 1; i < segmentCount; i++)
        {
            HingeJoint2D joint = segments[i].gameObject.AddComponent<HingeJoint2D>();
            joint.connectedBody = segments[i - 1];
            joint.autoConfigureConnectedAnchor = true;
        }

        // Joint del jugador
        playerJoint = player.gameObject.AddComponent<HingeJoint2D>();
        playerJoint.connectedBody = segments[segmentCount - 1];
        playerJoint.autoConfigureConnectedAnchor = true;

        //Joints slack
        foreach (var seg in segments)
            seg.simulated = true;

        IsPlayerAttached = true;
    }

    //FIX
    static Vector2 CatenaryPoint(Vector2 from, Vector2 to, float t, float sagAmount)
    {
        Vector2 linear = Vector2.Lerp(from, to, t);
        float ropeLength = Vector2.Distance(from, to);
        float sag = 4f * sagAmount * ropeLength * t * (1f - t);
        return linear + Vector2.down * sag;
    }

    //TEMPORARY FIX
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
                pin.connectedAnchor = pinPoint; // world space si no hay rigidbody
            }
        }
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

    // CUT
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

    // Webs estaticas
    public void BuildStatic(Vector2 pointA, Vector2 pointB)
    {
        Clear();
        AnchorPoint = pointA;
        IsPlayerAttached = false;

        float ropeLength = Vector2.Distance(pointA, pointB);
        float segmentSpacing = ropeLength / (segmentCount - 1);
        float colliderRadius = segmentSpacing * colliderRadiusMultiplier;

        for (int i = 0; i < segmentCount; i++)
        {
            float t = (float)i / (segmentCount - 1);
            Vector2 pos = CatenaryPoint(pointA, pointB, t, catenarySag);
            GameObject go = Instantiate(segmentPrefab, pos, Quaternion.identity);
            go.layer = LayerMask.NameToLayer("Web");

            WebSegment seg = go.AddComponent<WebSegment>();
            seg.rope = this;

            CircleCollider2D col = go.GetComponent<CircleCollider2D>();
            if (col == null) col = go.AddComponent<CircleCollider2D>();
            col.radius = colliderRadius;

            segments.Add(go.GetComponent<Rigidbody2D>());
        }

        HingeJoint2D anchorA = segments[0].gameObject.AddComponent<HingeJoint2D>();
        anchorA.autoConfigureConnectedAnchor = false;
        anchorA.connectedAnchor = pointA;

        for (int i = 1; i < segmentCount; i++)
        {
            HingeJoint2D joint = segments[i].gameObject.AddComponent<HingeJoint2D>();
            joint.connectedBody = segments[i - 1];
            joint.autoConfigureConnectedAnchor = true;
        }

        HingeJoint2D anchorB = segments[segmentCount - 1].gameObject.AddComponent<HingeJoint2D>();
        anchorB.autoConfigureConnectedAnchor = false;
        anchorB.connectedAnchor = pointB;
    }
}
