using UnityEngine;

public class Puerta : MonoBehaviour
{
    [SerializeField] private bool open = false;
    private SpriteRenderer sr;
    private Collider2D col;
    [SerializeField] private int ID = -1; //-1 si cualquier llave sirve, cualquier otra ID para llaves específicas
    [SerializeField] private Sprite openDoor;
    private Sprite closedDoor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        closedDoor = sr.sprite;
        col = GetComponent<Collider2D>();
    }

    public void Open()
    {
        if (!open)
        {
            open = true;
            if (openDoor != null)
                sr.sprite = openDoor;
            else
                sr.enabled = false;

            if (col != null) col.enabled = false;
        }
    }

    public void Close()
    {
        open = false;
        sr.sprite = closedDoor;
        if (col != null) col.enabled = true;
    }

    public int GetID()
    {
        return ID;
    }
}
