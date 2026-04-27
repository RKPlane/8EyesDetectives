using UnityEngine;

public class MultiplayerAutoRegister : MonoBehaviour
{
    private MultiplayerCameraManager cameraManager;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        cameraManager = FindFirstObjectByType<MultiplayerCameraManager>();

        if (cameraManager != null)
        {
            cameraManager.RegisterTarget(transform);
        }

	}

    private void OnDestroy()
    {
        if (cameraManager != null)
        {
            cameraManager.UnregisterTarget(transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
