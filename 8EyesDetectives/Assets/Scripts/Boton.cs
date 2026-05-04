using UnityEngine;

public class Boton : MonoBehaviour
{
    enum TipoBoton
    {
        Puerta,
        Plataforma,
        Rotable
    }

    // Customizable
    [SerializeField] private bool permanent = true; // El botón se queda presionado?
    [SerializeField] private TipoBoton tipoBoton;
    [SerializeField] private GameObject objetoEnlazado;
    [SerializeField] private Sprite botonPresionado;
    private Sprite botonDefault;

    // Funcionamiento interno
    public bool on = false;

    private SpriteRenderer sr;
    private Collider2D col;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        botonDefault = sr.sprite;
    }

    // Activación del botón
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.CompareTag("Player") && !on) || collision.gameObject.CompareTag("Caja"))
        {
            // Efecto del botón
            switch (tipoBoton)
            {
                case TipoBoton.Plataforma:
                    if (collision.gameObject.CompareTag("Player"))
                    {
						objetoEnlazado.GetComponent<MoveableWeightPlatform>().weight = 0.125f;
                    }
                    else
                    {
						objetoEnlazado.GetComponent<MoveableWeightPlatform>().weight = 0.25f;
					}
					objetoEnlazado.GetComponent<MoveableWeightPlatform>().weighted = true;
					objetoEnlazado.GetComponent<MoveableWeightPlatform>().unweighted = false;
					break;
                case TipoBoton.Puerta:
                    objetoEnlazado.GetComponent<Puerta>().Open();
                    break;
                case TipoBoton.Rotable:
					objetoEnlazado.GetComponent<RotablePlatform>().rotar = true;
					break;
            }

            // Cambio interno en el botón
            on = true;
            sr.sprite = botonPresionado;

        }
    }

    // Desactivación del botón
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!permanent)
        {
            if (((collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Caja")) && on && ThingsAtButton() == 0))
            {
                // Efecto del botón
                switch (tipoBoton)
                {
                    case TipoBoton.Plataforma:
						objetoEnlazado.GetComponent<MoveableWeightPlatform>().unweighted = true;
						objetoEnlazado.GetComponent<MoveableWeightPlatform>().weighted = false;
						break;
                    case TipoBoton.Puerta:
                        objetoEnlazado.GetComponent<Puerta>().Close();
                        break;
					case TipoBoton.Rotable:
						objetoEnlazado.GetComponent<RotablePlatform>().rotar = false;
						break;
				}

                // Cambio interno en el botón
                on = false;
                sr.sprite = botonDefault;
            }
        }
    }

    private int ThingsAtButton()
    {
        // Comprobación de Players dentro del botón
        Collider2D[] results = new Collider2D[30];
        int colliders = col.Overlap(ContactFilter2D.noFilter, results);
        int playerAmount = 0;
        for (int i = 0; i < colliders; i++)
        {
            if (results[i].gameObject.CompareTag("Player") || results[i].gameObject.CompareTag("Caja"))
            {
                playerAmount++;
            }
        }
        Debug.Log(playerAmount);
        return playerAmount;
    }
}
