using UnityEngine;
using UnityEngine.UI;

public class PanatallaScript : MonoBehaviour
{
	public Toggle pantalla;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		if (Screen.fullScreen)
		{
			pantalla.isOn = true;
		}
		else
		{
			pantalla.isOn = false;
		}
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void CambiaPantalla(bool pantallaCompleta)
	{
		Screen.fullScreen = pantallaCompleta;
	}
}
