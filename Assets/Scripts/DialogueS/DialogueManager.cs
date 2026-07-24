using NUnit.Framework.Constraints;
using Root;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public bool IsTyping = false;

    [SerializeField] private float textTypingSpeed;

    private Queue<string> _sentences;
    private string _currentSentence;
    private Coroutine _typingCoroutine;
    private DialogueSO _currentSpeaker;
    private TextMeshProUGUI _currentDisplayText;

    public event Action OnDialogueStarted;
    public event Action OnDialogueEnded;

    private List<string> _currentData = new List<string>();
    private float _currentTextDuration;
    private int _arrayIndex;
    private bool _canInterruptTyping;

    private DialogueState _states;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        GameManager.DialogueManager = this;
    }

    private void Start()
    {
        _sentences = new Queue<string>();
    }

    public void SetDialogueParameters(DialogueSO dialogue, TextMeshProUGUI text)
    {
        _currentSpeaker = dialogue;
        _currentDisplayText = text;
    }

    public void TriggerDialogue()
    {
        CheckState();
    }

    public void CheckState()
    {
        switch (_states)
        {
            case DialogueState.StartTalking:
                StartDialogue();
                break;
            case DialogueState.IsTalking:
                HandleTextSkip();
                break;
            case DialogueState.CanRepeatDialogue:
                StartDialogue();
                break;
            case DialogueState.FinishedTalking:
                break;
            default:
                break;
        }

        if (CheckDialogueState() == DialogueState.FinishedTalking)
        {
            if (_currentSpeaker.CanRepeatDialogue)
            {
                ChangeDialogueState(DialogueState.CanRepeatDialogue);
            }
        }
    }

    private void ChangeDialogueState(DialogueState state)
    {
        _states = state;
    }

    private DialogueState CheckDialogueState()
    {
        return _states;
    }

    private void HandleTextSkip()
    {
        if (IsTyping)
        {
            FinishSentenceEarly();
        }
        else
        {
            DisplayNextSentence();
        }
    }

    private void StartDialogue()
    {
        OnDialogueStarted?.Invoke();

        if (_currentDisplayText == null || _currentSpeaker == null) return;

        SetCurrentDialogue();

        _sentences.Clear();

        foreach (string sentence in _currentData)
        {
            _sentences.Enqueue(sentence);
        }

        DisplayNextSentence();

        ChangeDialogueState(DialogueState.IsTalking);
    }


    public void DisplayNextSentence()
    {
        if (_sentences.Count == 0)
        {
            EndDialogue(_currentSpeaker);
            return;
        }

        _currentSentence = _sentences.Dequeue();

        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        _typingCoroutine = StartCoroutine(TypeSentence(_currentSentence));
    }

    private void FinishSentenceEarly()
    {
        StopCoroutine(_typingCoroutine); 

        _currentDisplayText.text = IsSpeakerNameShowable();
        _currentDisplayText.text += _currentSentence;

        StartCoroutine(FinishVisual());
    }

    private IEnumerator TypeSentence(string sentence)
    {
        IsTyping = true;

       /* string prefix = "";
        if (_currentSpeaker.IsAbleToShowSpeaker && !string.IsNullOrEmpty(_currentSpeaker.SpeakerName))
        {
            prefix = $"{_currentSpeaker.SpeakerName}: ";
        }*/

        _currentDisplayText.text = IsSpeakerNameShowable();

        foreach (char letter in sentence.ToCharArray())
        {
            while (_canInterruptTyping)
            {
                yield return null;
            }

            _currentDisplayText.text += letter;

            yield return new WaitForSeconds(textTypingSpeed);
        }

        _currentTextDuration = _currentSpeaker.DialogueData[_arrayIndex].TextDuration;
        yield return new WaitForSeconds(_currentTextDuration);

        _arrayIndex++;
        IsTyping = false;

        //AutoHideText(); 
    }

    IEnumerator FinishVisual()
    {
        while (_canInterruptTyping)
        {
            yield return null;
        }

        yield return new WaitForSeconds(_currentTextDuration);

        IsTyping = false;

        //AutoHideText(); 
    }

    private void SetCurrentDialogue()
    {
        foreach (var data in _currentSpeaker.DialogueData)
        {
            foreach (string line in data.Text)
            {
                _currentData.Add(line);
            }
        }
    }

    private string IsSpeakerNameShowable()
    {
        string prefix = "";
        if (_currentSpeaker.IsAbleToShowSpeaker && !string.IsNullOrEmpty(_currentSpeaker.SpeakerName))
        {
            prefix = $"{_currentSpeaker.SpeakerName}: ";
            return prefix;
        }

        return prefix;
    }


    private void AutoHideText()
    {
        HandleTextSkip();
    }

    private void EndDialogue(DialogueSO dialogue)
    {
        _arrayIndex = 0;
        _currentDisplayText.text = "";
        _sentences.Clear();
        _currentData.Clear();

        OnDialogueEnded?.Invoke();

        ChangeDialogueState(DialogueState.FinishedTalking);
    }

    public void StopCurrentDialogue()
    {
        _canInterruptTyping = true;
    }

    public void ResumeCurrentDialogue()
    {
        _canInterruptTyping = false;
    }
}

