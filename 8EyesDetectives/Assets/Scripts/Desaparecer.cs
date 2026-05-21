using UnityEngine;
using UnityEngine.UI;

public class Desaparecer : MonoBehaviour
{
    Image imagen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DialogueManager.Instance.FreezeAll();
        imagen = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        imagen.color = new Color(imagen.color.r, imagen.color.g, imagen.color.b, imagen.color.a - 0.2f * Time.deltaTime);

        if (imagen.color.a <= 0)
        {
            DialogueManager.Instance.UnfreezeAll();
            Destroy(gameObject);
        }
    }

}
