using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Puerta : MonoBehaviour
{
    [SerializeField] private bool open = false;

    private SpriteRenderer sr;
    [SerializeField] private Sprite closedDoor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Open()
    {
        if (!open) {
            if (closedDoor != null)
            {
               sr.sprite = closedDoor;
            } else
            {
                sr.enabled = false;
            }

            gameObject.layer = LayerMask.NameToLayer("NoCollision");
        }
    }

    public void Close()
    {
        // Por implementar
    }
}
