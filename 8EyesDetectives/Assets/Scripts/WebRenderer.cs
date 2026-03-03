using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WebRenderer : MonoBehaviour
{
    public WebConnection connection;

    private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.enabled = false;
    }

    private void LateUpdate()
    {
        if (!connection.IsAttached) return;

        line.SetPosition(0, transform.position);
        line.SetPosition(1, connection.AnchorPoint);
    }

    public void Enable() => line.enabled = true;
    public void Disable() => line.enabled = false;
}
