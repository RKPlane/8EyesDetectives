using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WebRenderer : MonoBehaviour
{
    public WebRope rope;

    LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.enabled = false;
    }

    void LateUpdate()
    {
        if (!rope.IsBuilt)
        {
            line.enabled = false;
            return;
        }

        line.enabled = true;
        line.positionCount = rope.Segments.Count;
        for (int i = 0; i < rope.Segments.Count; i++)
            line.SetPosition(i, rope.Segments[i].position);
    }

    public void Enable() { }             // visibility is driven by rope state in LateUpdate
    public void Disable() => line.enabled = false;
}
