using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SpiderWeb : MonoBehaviour
{
    [Header("Referencias")]
    public Transform attackPoint;
    public LineRenderer lineRenderer;

    [Header("Ajustes")]
    public float webSpeed = 20f;   // velocidad de extension
    public float maxDistance = 15f;

    [Header("Input System")]
    public InputActionAsset inputActions;
    private InputAction shootAction;


    private void OnEnable()
    {
        var playerMap = inputActions.FindActionMap("Player");
        if (playerMap != null)
        {
            shootAction = playerMap.FindAction("Shoot");
            shootAction?.Enable();
        }
    }

    private void OnDisable()
    {
        shootAction?.Disable();
    }

    private void Update()
    {
        if (shootAction != null && shootAction.WasPressedThisFrame())
        {
            StartCoroutine(ShootWeb());
        }
    }



    IEnumerator ShootWeb()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;

        Vector3 startPoint = attackPoint.position;
        Vector3 direction = (mouseWorldPos - startPoint).normalized;

        RaycastHit2D hit = Physics2D.Raycast(startPoint, direction, maxDistance);

        Vector3 targetPoint;

        if (hit.collider != null)
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = startPoint + direction * maxDistance;
        }


        float distance = 0f;

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, startPoint);

        while (distance < Vector3.Distance(startPoint, targetPoint))
        {
            distance += webSpeed * Time.deltaTime;
            Vector3 currentPoint = startPoint + direction * distance;

            lineRenderer.SetPosition(1, currentPoint);

            yield return null;
        }

        // Mantener la telaraña x segundos
        yield return new WaitForSeconds(5f);

        lineRenderer.enabled = false;
    }


}




