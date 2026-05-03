using UnityEngine;

public class Puerta : MonoBehaviour
{
    [SerializeField] private bool open = false;
    private SpriteRenderer sr;
    private Collider2D col;
    [SerializeField] private int ID = -1; //-1 si cualquier llave sirve, cualquier otra ID para llaves específicas
    [SerializeField] private Sprite closedDoor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    public void Open()
    {
        if (!open)
        {
            open = true;
            if (closedDoor != null)
                sr.sprite = closedDoor;
            else
                sr.enabled = false;

            if (col != null) col.enabled = false;
        }
    }

    public void Close()
    {
        open = false;
        sr.enabled = true;
        if (col != null) col.enabled = true;
    }

    public int GetID()
    {
        return ID;
    }
}
