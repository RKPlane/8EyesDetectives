using UnityEngine;

public class UpBox : MonoBehaviour
{
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			collision.transform.SetParent(this.transform);
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			if (gameObject.activeInHierarchy && collision.gameObject.activeInHierarchy)
			{
				collision.transform.SetParent(null);
			}
		}
	}
}
