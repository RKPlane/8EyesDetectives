using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Trigger : MonoBehaviour
{
    [SerializeField] private GameObject black;
    [SerializeField] private DialogueConversation conversation;
    enum TipoTrigger
    {
        Conversacion,
        ZonaMuerte
    }
    [SerializeField] private TipoTrigger tipoTrigger = TipoTrigger.ZonaMuerte;

    public int playerRequirement = 1;
    public float fadeDuration = 0.5f;
    public float playerSeparation = 3f;
    private Image img;
    private bool triggered = false;
    private SpriteRenderer sr;
    private Collider2D col;
    private void Awake()
    {

        img = black.GetComponent<Image>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!triggered)
        {
            if (collision.gameObject.CompareTag("Player") && PlayersAtTrigger() >= playerRequirement)
            {
                Debug.Log(PlayersAtTrigger());
                triggered = true;
                Debug.Log("[Trigger] Activated");
                StartCoroutine(RutinaFade());
            }

            if (collision.gameObject.CompareTag("Caja"))
            {
                triggered = true;
                Debug.Log("[Trigger] Activated");
                StartCoroutine(RutinaFade());
            }

            if (collision.gameObject.CompareTag("Carryable"))
            {
                triggered = true;
                Debug.Log("[Trigger] Activated");
                StartCoroutine(RutinaFade());

            }
        }
	}

    IEnumerator RutinaFade()
    {
        yield return Fade(0, 1);

        Player playerSpider = FindFirstObjectByType<Player>();
        MantisPlayer playerMantis = FindFirstObjectByType<MantisPlayer>();

        switch (tipoTrigger)
        {
            case TipoTrigger.ZonaMuerte:
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;

            case TipoTrigger.Conversacion:

                if (playerSpider != null && playerMantis != null)
                {
                    playerSpider.transform.position =
                        new Vector3(transform.position.x - playerSeparation, transform.position.y-1f, 0);

                    playerMantis.transform.position =
                        new Vector3(transform.position.x + playerSeparation, transform.position.y-1f, 0);

                    playerMantis.transform.localScale = new Vector3(-1, 1, 1);

                    if (DialogueManager.Instance != null)
                        DialogueManager.Instance.StartConversation(conversation);
                }
                else
                {
                    Debug.LogError("Players no encontrados en RutinaFade()");
                }

                break;
        }

        yield return Fade(1, 0);
    }

    IEnumerator Fade(float start, float end)
    {
        float t = 0;
        Color c = img.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(start, end, t / fadeDuration);
            img.color = c;
            yield return null;
        }

        c.a = end;
        img.color = c;
    }
    private int PlayersAtTrigger()
    {
        // Comprobación de Players dentro del Trigger
        Collider2D[] results = new Collider2D[30];
        int colliders = col.Overlap(ContactFilter2D.noFilter, results);
        int playerAmount = 0;
        for (int i = 0; i < colliders; i++)
        {
            if (results[i].gameObject.CompareTag("Player"))
            {
                playerAmount++;
            }
        }
        Debug.Log(playerAmount);
        return playerAmount;
    }
}
