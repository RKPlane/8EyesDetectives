using UnityEngine;

public class Boton : MonoBehaviour
{
    enum TipoBoton
    {
        Puerta,
        Plataforma
    }

    // Customizable
    [SerializeField] private bool permanent = true; // El botón se queda presionado?
    [SerializeField] private TipoBoton tipoBoton;
    [SerializeField] private GameObject objetoEnlazado;

    // Funcionamiento interno
    public bool on = false;

    private SpriteRenderer sr;
    private Collider2D col;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Activación del botón
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !on)
        {
            // Efecto del botón
            switch (tipoBoton)
            {
                case TipoBoton.Plataforma:

                    break;
                case TipoBoton.Puerta:
                    objetoEnlazado.GetComponent<Puerta>().Open();
                    break;
            }

            // Cambio interno en el botón
            on = true;
            sr.color = Color.green;
        }
    }

    // Desactivación del botón
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!permanent)
        {
            if (collision.gameObject.CompareTag("Player") && on && PlayersAtButton() == 0)
            {
                // Efecto del botón
                switch (tipoBoton)
                {
                    case TipoBoton.Plataforma:

                        break;
                    case TipoBoton.Puerta:
                        objetoEnlazado.GetComponent<Puerta>().Close();
                        break;
                }

                // Cambio interno en el botón
                on = false;
                sr.color = Color.purple;
            }
        }
    }

    private int PlayersAtButton()
    {
        // Comprobación de Players dentro del botón
        Collider2D[] results = new Collider2D[2];
        int colliders = col.Overlap(ContactFilter2D.noFilter, results);
        int playerAmount = 0;
        for (int i = 0; i < colliders; i++)
        {
            if (results[i].gameObject.CompareTag("Player"))
            {
                playerAmount++;
            }
        }
        return playerAmount;
    }
}
