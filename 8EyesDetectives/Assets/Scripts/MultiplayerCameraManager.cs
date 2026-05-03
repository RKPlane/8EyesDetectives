using Unity.Cinemachine;
using UnityEngine;

public class MultiplayerCameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineTargetGroup targetGroup;

	[SerializeField] private float defaultRadius = 1f;

	[SerializeField] private float defaultHeight = 5f;

	private void Awake()
	{
        if (targetGroup == null)
        { targetGroup= FindFirstObjectByType<CinemachineTargetGroup>(); }
	}

    public void RegisterTarget(Transform target)
    { 
        if (targetGroup == null || target == null) {
            return;
        }
        targetGroup.AddMember(target, defaultHeight, defaultRadius);
	}

    public void UnregisterTarget(Transform target) 
    {
        if (targetGroup == null || target == null) { return; }
        targetGroup.RemoveMember(target);
    }

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
