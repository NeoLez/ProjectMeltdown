using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogues", menuName = "SO/Dialogues")]
public class DialogueSO : ScriptableObject
{
    [field: SerializeField] public string SpeakerName { get; private set; }
    [field: SerializeField] public bool IsAbleToShowSpeaker { get; private set; }
    [field: SerializeField] public bool CanRepeatDialogue { get; private set; }

    [SerializeField] private DialogueAudioInfoSO[] dialogueAudio;

    [SerializeField] private DialogueData[] dialogueData;

    public DialogueChoices[] DialogueChoices;

    //public string SpeakerName => speakerName;
    //public bool CanRepeatDialogue => canRepeatDialogue;
    public DialogueAudioInfoSO[] DialogueAudio => dialogueAudio;
    public DialogueData[] DialogueData => dialogueData;
    public bool HasChoices => DialogueChoices.Length > 0;

    public Action OnDialogueStarted;
    public Action OnDialogueEnded;

    public Action<int> OnSelectedChoice;
}

[System.Serializable]
public struct DialogueData
{
    [TextArea(3, 10)]
    public string Text;
    [Min(0)]
    [SerializeField] float textDuration;
    public bool HasChoices;
    public readonly float TextDuration => textDuration;
}

[System.Serializable]
public struct DialogueChoices
{
    public string Text;
}