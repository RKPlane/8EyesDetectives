using UnityEngine;

// Stamped onto every rope segment by WebRope.Build so the Mantis
// can find the owning rope from any collider hit on the Web layer.
public class WebSegment : MonoBehaviour
{
    public WebRope rope;
}
