using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public enum DialogueType
    {
        Talk,
        Lore
    }

    public static DialogueManager Instance;

    [Header("Settings")]
    public float letterSpeed = 0.1f;
    private float letterTimer = 0.0f;

    [Header("Input")]
    public InputActionAsset inputActions;
    private InputAction m_nextAction;

    [Header("Prefabs")]
    public GameObject dialoguePrefabL;
    public GameObject dialoguePrefabR;

    private Dictionary<Characters, Transform> characterData = new Dictionary<Characters, Transform>();

    public bool running = false;

    private string currentMsg = string.Empty;
    private DialogueConversation currentConversation = null;
    private int currentLine = 0;
    private int currentChar = 0;
    private string currentLineText = string.Empty;

    private GameObject currentInstance = null;
    private TextMeshPro tmp;

    private Player playerSpider;
    private MantisPlayer playerMantis;

    private bool dialogueActive = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        m_nextAction = InputSystem.actions.FindAction("Next");
    }

    void Start()
    {
        playerSpider = FindFirstObjectByType<Player>();
        playerMantis = FindFirstObjectByType<MantisPlayer>();
    }

    void Update()
    {
        if (!running) return;

        if (currentLineText.Equals(currentMsg))
        {
            if (m_nextAction != null && m_nextAction.IsPressed())
                NextLine();
        }
        else
        {
            letterTimer += Time.deltaTime;

            if (letterTimer > letterSpeed)
            {
                AddLetter();
                letterTimer = 0f;
            }
        }
    }

    void LateUpdate()
    {
        if (running)
            UpdatePosition();
    }

    private void AddLetter()
    {
        if (currentLineText == null || currentChar >= currentLineText.Length)
            return;

        string nextLetter = currentLineText.Substring(currentChar, 1);

        if (nextLetter != " ")
        {
            currentMsg += nextLetter;
            currentChar++;
        }
        else
        {
            currentMsg += nextLetter;
            currentChar++;
        }

        if (tmp != null)
            tmp.text = currentMsg;
    }

    // ───────────────────────────────────────────────
    // START / NEXT / END
    // ───────────────────────────────────────────────

    public void StartConversation(DialogueConversation conversation)
    {
        if (currentConversation != null) return;

        currentConversation = conversation;
        running = true;

        SetDialogueState(true);

        currentInstance = Instantiate(CheckSide());
        tmp = currentInstance.GetComponentInChildren<TextMeshPro>();

        currentLine = 0;
        currentChar = 0;
        currentMsg = string.Empty;

        currentLineText = currentConversation.lines[currentLine].text;
    }

    public void NextLine()
    {
        currentChar = 0;
        currentMsg = string.Empty;
        letterTimer = 0f;

        currentLine++;

        if (currentLine >= currentConversation.lines.Count)
        {
            EndConversation();
            return;
        }

        currentLineText = currentConversation.lines[currentLine].text;

        if (tmp != null)
            tmp.text = "";
    }

    public void EndConversation()
    {
        running = false;

        SetDialogueState(false);

        Unfreeze(playerSpider);
        Unfreeze(playerMantis);

        currentLine = 0;
        currentChar = 0;
        currentConversation = null;

        if (currentInstance != null)
            Destroy(currentInstance);
    }

    // ───────────────────────────────────────────────
    // PLAYER FREEZE SYSTEM
    // ───────────────────────────────────────────────

    private void SetDialogueState(bool active)
    {
        dialogueActive = active;

        if (playerSpider != null)
        {
            playerSpider.control = !active;
            Freeze(playerSpider.GetComponent<Rigidbody2D>());
        }

        if (playerMantis != null)
        {
            playerMantis.control = !active;
            Freeze(playerMantis.GetComponent<Rigidbody2D>());
        }
    }

    public void FreezeAll()
    {
        Freeze(playerSpider);
        Freeze(playerMantis);
    }

    private void Unfreeze(MonoBehaviour player)
    {
        if (player == null) return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private void Freeze(MonoBehaviour player)
    {
        if (player == null) return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        Player p1 = player as Player;
        if (p1 != null)
            p1.ForceStop();

        MantisPlayer p2 = player as MantisPlayer;
        if (p2 != null)
            p2.ForceStop();
    }

    private void Freeze(Rigidbody2D rb)
    {
        if (rb == null) return;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    // ───────────────────────────────────────────────
    // POSITIONING
    // ───────────────────────────────────────────────

    private void UpdatePosition()
    {
        if (currentConversation == null || currentInstance == null) return;

        var personaje = currentConversation.lines[currentLine].characterName;

        if (characterData.ContainsKey(personaje))
        {
            currentInstance.transform.position = characterData[personaje].position;
        }

        if (Camera.main != null)
            currentInstance.transform.forward = Camera.main.transform.forward;
    }

    private GameObject CheckSide()
    {
        return dialoguePrefabR;
    }

    // ───────────────────────────────────────────────
    // EXTERNAL API
    // ───────────────────────────────────────────────

    public void LoadMouth(Characters character, Transform mouth)
    {
        characterData[character] = mouth;
    }
}
