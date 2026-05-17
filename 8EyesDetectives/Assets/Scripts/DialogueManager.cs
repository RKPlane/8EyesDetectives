using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public enum DialogueType
    {
        Talk,
        Lore
    }

    //Referencia global al sistema de diálogos
    public static DialogueManager Instance;

    //Variables configurables
    public float letterSpeed = 0.1f; //Cada cuanto tiempo (en segundos) se suma una letra al mensaje actual
    private float letterTimer = 0.0f; //Contador interno del tiempo que pasa
    //public DialogueConversation prueba; //TESTEO

    //Input y prefabs
    public InputActionAsset inputActions;
    private InputAction m_nextAction;
    public GameObject dialoguePrefabL;
    public GameObject dialoguePrefabR;

    //Diccionario que asocia cada ID de personaje a su "boca"
    private Dictionary<Characters, Transform> characterData = new Dictionary<Characters, Transform>();

    //Control
    public bool running = false;

    //Funcionamiento interno
    private string currentMsg = string.Empty; //String que se dibuja en pantalla
    private DialogueConversation currentConversation = null; //Conversación en curso
    private int currentLine = 0; //Índice de la línea que estamos mostrando desde la lista de DialogueConversation actual
    private int currentChar = 0; //Siguiente letra a sumar para avanzar en el dibujado de toda la línea actual
    private string currentLineText = string.Empty; //El texto completo de la línea que se quiere dibujar

    private GameObject currentInstance = null; //Instancia del prefab actual
    private TextMeshPro tmp;

    void Awake()
    {
        //Guardar referencia global a DialogueManager
        if (Instance == null)
        {
            Instance = this;
        }
        m_nextAction = InputSystem.actions.FindAction("Next");
    }

    void Update()
    {
        if (running)
        {
            if (currentLineText.Equals(currentMsg))
            {
                //Avanzar de línea al presionar el botón de "Next"
                if (m_nextAction.IsPressed())
                {
                    NextLine();
                }
            }
            else
            {
                //Ir sumando letras al mensaje en pantalla
                letterTimer += Time.deltaTime;
                if (letterTimer > letterSpeed)
                {
                    AddLetter();
                    letterTimer = 0.0f;
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (running)
        {
            UpdatePosition();
        }
    }

    private void AddLetter()
    {
        //Saltarse espacios para no interrumpir ritmo del diálogo
        var letterSearch = true;
        var nextLetter = string.Empty;
        while (letterSearch)
        {
            nextLetter = currentLineText.Substring(currentChar, 1);
            if (nextLetter != " ")
            {
                letterSearch = false;
            }
            else
            {
                currentMsg += nextLetter;
                currentChar++;
            }
        }
        //Sumar la letra
        currentMsg += nextLetter;
        tmp.text = currentMsg;
        currentChar++;
    }

    #region Funciones públicas de control
    public void StartConversation(DialogueConversation conversation)
    {
        if (currentConversation == null)
        {
            TogglePlayerControl();
            running = true;
            currentInstance = Instantiate(CheckSide()); //Instanciamos el prefab de diálogo con TMP y SpriteRenderer
            tmp = currentInstance.GetComponentInChildren<TextMeshPro>();

            currentConversation = conversation; //Cargamos la conversación 
            currentLineText = currentConversation.lines[currentLine].text; //Cargamos el texto de la primera línea
        }
    }

    public void NextLine()
    {
        tmp.text = string.Empty; //Vaciar mensaje en pantalla
        currentChar = 0; //Reiniciar contador de letras
        currentLine++; //Avanzar el contador de línea
        currentMsg = string.Empty; //Limpiamos el string que se muestra en pantalla
        //Si llegamos al final de la lista de la conversación actual, la terminamos
        if (currentLine == currentConversation.lines.Count)
        {
            EndConversation();
        }
        else
        {
            //Si no estamos en el final, cargamos siguiente diálogo
            currentLineText = currentConversation.lines[currentLine].text; //Cargamos el texto de la línea actual
        }
    }

    public void EndConversation()
    {
        TogglePlayerControl();
        running = false;
        currentLine = 0;
        currentConversation = null;
        Destroy(currentInstance);
    }
    #endregion

    public void LoadMouth(Characters character, Transform mouth)
    {
        characterData[character] = mouth;
    }

    private void UpdatePosition()
    {
        var personaje = currentConversation.lines[currentLine].characterName;
        if (characterData.ContainsKey(personaje))
        {
            currentInstance.transform.position = characterData[personaje].position;
        }
        currentInstance.transform.forward = Camera.main.transform.forward;
    }

    private GameObject CheckSide()
    {
        //Lógica que decide si el dialogo debe tirarse a la izquierda o a la derecha (depende de la cámara)
        return dialoguePrefabR;
    }

    private void TogglePlayerControl()
    {
        MantisPlayer.instance.control = !MantisPlayer.instance.control;
        Player.instance.control = !Player.instance.control;
    }

}
