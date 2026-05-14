using UnityEngine;
using UnityEngine.UI;

public class ScriptVolume : MonoBehaviour
{
    public Slider volumeSlider;
    public float sliderValue;
    public Image Mute;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.5f);
        AudioListener.volume = volumeSlider.value;
        Silencio();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CambiaSlider(float valor) //Este método se llama cada vez que el valor del slider cambia
	{
		sliderValue = volumeSlider.value;
		AudioListener.volume = sliderValue;
		PlayerPrefs.SetFloat("Volume", sliderValue);
		Silencio();
	}

	public void Silencio()
	{
		if (AudioListener.volume == 0)
		{
			Mute.enabled = true;
		}
		else
		{
			Mute.enabled = false;
		}
	}
}
