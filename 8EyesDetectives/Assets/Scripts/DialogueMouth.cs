using UnityEngine;

public class DialogueMouth : MonoBehaviour
{
    public Characters character;
    public Transform mouthPoint;

    private void Start()
    {
        //Registrar la boca y asociarla al personaje en DialogueManager
        DialogueManager.Instance.LoadMouth(character, mouthPoint);
    }
}