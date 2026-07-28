using Root;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogues", menuName = "SO/Dialogues")]
public class DialogueSO: ScriptableObject
{
    [SerializeField] private string speakerName;
    [field : SerializeField] public bool IsAbleToShowSpeaker { get; private set; }

    [SerializeField] private bool canRepeatDialogue;

    [SerializeField] private DialogueAudioInfoSO[] dialogueAudio;

    [SerializeField] private DialogueData[] dialogueData;

    public string SpeakerName => speakerName;
    public bool CanRepeatDialogue => canRepeatDialogue;
    public DialogueAudioInfoSO[] DialogueAudio => dialogueAudio;
    public DialogueData[] DialogueData => dialogueData;


    public Action OnDialogueStarted;
    public Action OnDialogueEnded;
}

[System.Serializable]
public struct DialogueData
{
    [TextArea(3, 10)]
    public string Text;
    [Min(0)]
    [SerializeField] float textDuration;

    public readonly float TextDuration => textDuration;
}
