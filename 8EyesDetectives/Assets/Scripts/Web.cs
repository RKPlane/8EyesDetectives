using UnityEngine;

public class Web : MonoBehaviour
{
    //Sonido para mas tarde
    /*
    [Header("Sonido al impactar")]
    public AudioClip hitSound;
    private AudioSource audioSource;*/

    private void Start()
    {
        //Sonido para mas tarde
        /*
         audioSource = GetComponent<AudioSource>(); 
         */

        //Validacion hasta que hayan paredes o rigid bodys para que no explote el engine de balas infinitas por el mapa
        //Destroy(gameObject, 2f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*if (collision.gameObject.CompareTag("Target"))
        {
            //Sonido para mas tarde
            
            if (audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }

            //Cambiar color del objetivo a rojo
            SpriteRenderer renderer = collision.gameObject.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = Color.red;
            }

            //Destruir objetivo
            Destroy(collision.gameObject, 0.3f);
        }*/

        // Destruir bala
        //Destroy(gameObject, 0.1f);
    }
}
