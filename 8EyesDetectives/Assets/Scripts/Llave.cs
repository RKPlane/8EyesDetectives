using UnityEngine;

public class Llave : MonoBehaviour
{
    public int ID = -1; //-1 si vale solo para puertas genéricas, cualquier otra ID para puertas específicas

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Puerta"))
        {
            //Obtener componente e ID de la puerta
            Puerta puerta = collision.gameObject.GetComponent<Puerta>();
            int puertaID = puerta.GetID();
            //Abrir puerta si se cumplen las condiciones
            if (puertaID == -1 || ID == puertaID)
            {
                puerta.Open();
                Destroy(gameObject);
            }

        }
    }
}
