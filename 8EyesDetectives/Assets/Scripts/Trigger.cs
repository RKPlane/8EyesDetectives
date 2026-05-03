using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Trigger : MonoBehaviour
{
    [SerializeField] private GameObject black;
    [SerializeField] private DialogueConversation conversation;

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
            Debug.Log("triggered");
            triggered = true;
            StartCoroutine(RutinaFade());
        }
    }

    IEnumerator RutinaFade()
    {
        //Fade a negro
        yield return Fade(0, 1);

        //Efecto que queremos ejecutar invisiblemente
        Player.instance.transform.position = new Vector3(transform.position.x - playerSeparation, transform.position.y, transform.position.z);
        MantisPlayer.instance.transform.position = new Vector3(transform.position.x + playerSeparation, transform.position.y, transform.position.z);
        DialogueManager.Instance.StartConversation(conversation);

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
