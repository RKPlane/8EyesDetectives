using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Interactuable : MonoBehaviour
{
    enum Efecto
    {
        Transicion,
        Palanca,
        Conversacion,
        Lore
    }

    public InputActionAsset inputActions; // Da acceso a todas las acciones de input definidas en el Input Action Asset
    private InputAction m_useAction;// Se utiliza para almacenar la acción que queremos utilizar
    private Collider2D col;

    [SerializeField] private Efecto efecto;
    [SerializeField] private TextMeshPro tmp;
    [SerializeField] private string escena; // Transicion
    [SerializeField] private bool on = false; // Palanca

    private bool interactuable = false;
    [SerializeField] private float lerpSpeed = 0.05f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        col = GetComponent<Collider2D>();
        m_useAction = InputSystem.actions.FindAction("Interact");
    }

    // Update is called once per frame
    void Update()
    {
        if (interactuable)
        {
            // Visibilidad texto ayuda
            tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, Mathf.Lerp(tmp.color.a, 1, lerpSpeed));

            // Detectar Input
            if (m_useAction.WasPressedThisFrame())
            {
                // Efecto del botón
                switch (efecto)
                {
                    case Efecto.Transicion:
                        SceneManager.LoadScene(escena);
                        break;
                    case Efecto.Palanca:
                        on = !on;
                        // Por implementar
                        break;
                    case Efecto.Lore:
                        // Por implementar
                        break;
                }
            }
        } else
        {
            // Visibilidad texto ayuda
            tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, Mathf.Lerp(tmp.color.a, 0, lerpSpeed * 2f));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Marcar como interactuable
            interactuable = true;

        }
    }

    // Desactivación del interactuable
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if  (PlayersAtInteractable() == 0)
            {
                // Desmarcar como interactuable si no hay ningún jugador
                interactuable = false;
            }
        }
    }

    private int PlayersAtInteractable()
    {
        // Comprobación de Players dentro del interactuable
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
