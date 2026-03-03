using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class WebConnection : MonoBehaviour
{
    [Header("Input")]
    public InputActionAsset inputActions;

    private InputAction reelInAction;
    private InputAction reelOutAction;

    [Header("Reel Settings")]
    public float reelSpeed = 8f;
    public float minLength = 1f;

    private DistanceJoint2D joint;

    public Vector2 AnchorPoint { get; private set; }
    public bool IsAttached => joint != null;

    private void Awake()
    {
        reelInAction = inputActions.FindAction("ReelIn");
        reelOutAction = inputActions.FindAction("ReelOut");
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void Attach(Vector2 point)
    {
        if (joint != null)
            Destroy(joint);

        AnchorPoint = point;

        joint = gameObject.AddComponent<DistanceJoint2D>();
        joint.autoConfigureDistance = false;
        joint.connectedAnchor = AnchorPoint;
        joint.distance = Vector2.Distance(transform.position, AnchorPoint);
        joint.enableCollision = true;
    }

    public void Detach()
    {
        if (joint != null)
            Destroy(joint);
    }

    private void Update()
    {
        if (!IsAttached) return;

        if (reelInAction.IsPressed())
            joint.distance = Mathf.Max(
                minLength,
                joint.distance - reelSpeed * Time.deltaTime
            );

        if (reelOutAction.IsPressed())
            joint.distance += reelSpeed * Time.deltaTime;
    }
}
