using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Descriptions", menuName = "Dialogues", order = 0)]
public class DialogueSO: ScriptableObject
{
    [SerializeField] private string speakerName;
    [field : SerializeField] public bool IsAbleToShowSpeaker { get; private set; }
    [SerializeField] bool canRepeatDialogue;
    [SerializeField] private DialogueData[] dialogueData;

    public string SpeakerName => speakerName;
    public bool CanRepeatDialogue => canRepeatDialogue;
    public DialogueData[] DialogueData => dialogueData;
}

[System.Serializable]
public struct DialogueData
{
    [TextArea(3, 10)]
    public string[] Text;
    [Min(0)]
    [SerializeField] float textDuration;

    public readonly float TextDuration => textDuration;
}
