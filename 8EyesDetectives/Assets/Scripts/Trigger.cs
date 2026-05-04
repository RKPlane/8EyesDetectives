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

    public float fadeDuration = 0.5f;
    public float playerSeparation = 3f;
    private Image img;
    private bool triggered = false;
    private void Awake()
    {
        img = black.GetComponent<Image>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !triggered)
        {
            triggered = true;
            Debug.Log("[Trigger] Activated");
            StartCoroutine(RutinaFade());

        }

        if (collision.gameObject.CompareTag("Caja") && !triggered)
        {
            triggered = true;
            Debug.Log("[Trigger] Activated");
            StartCoroutine(RutinaFade());

        }

		if (collision.gameObject.CompareTag("Carryable") && !triggered)
		{
			triggered = true;
			Debug.Log("[Trigger] Activated");
			StartCoroutine(RutinaFade());

		}
	}

    IEnumerator RutinaFade()
    {
        //Fade a negro
        yield return Fade(0, 1);

        //Efecto que queremos ejecutar invisiblemente
        switch (tipoTrigger)
        {
            case TipoTrigger.ZonaMuerte:
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
            case TipoTrigger.Conversacion:
                Player.instance.transform.position = new Vector3(transform.position.x - playerSeparation, transform.position.y, transform.position.z);
                MantisPlayer.instance.transform.position = new Vector3(transform.position.x + playerSeparation, transform.position.y, transform.position.z);
                DialogueManager.Instance.StartConversation(conversation);
                break;
        }


        //Fade a transparente
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
}
