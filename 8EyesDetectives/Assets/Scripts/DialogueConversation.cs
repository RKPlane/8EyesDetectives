using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueConversation", menuName = "Scriptable Objects/DialogueConversation")]
public class DialogueConversation : ScriptableObject
{
    public List<DialogueLine> lines = new List<DialogueLine>();
}