using System.Collections.Generic;
using UnityEngine;

//Enum de personajes
public enum Characters
{
    Spider,
    Mantis,
    Jefe
}

//Estructura para cada línea de diálogo
[System.Serializable]
public class DialogueLine
{
    //Constructor
    public DialogueLine(Characters character, string text)
    {
        this.characterName = character;
        this.text = text;
    }

    //Atributos
    public Characters characterName;

    [TextArea(3, 5)] //Para que en el editor salga mejor
    public string text;
}