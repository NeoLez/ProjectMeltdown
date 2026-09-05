using Root.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public bool IsTyping = false;

    [Header("Dialogue Config")]
    [SerializeField] private float textTypingSpeed;
    [SerializeField] private bool isAudioPredictable;
    [SerializeField] private TextMeshProUGUI skipText;

    [Header("Dialogue Options")]
    [SerializeField] private Button[] choiceOptions;
    private TextMeshProUGUI[] _choicesText;

    private Queue<string> _sentences;
    private string _currentSentence;
    private Coroutine _typingCoroutine;
    private DialogueSO _currentSpeaker;
    private DialogueAudioInfoSO _currentSpeakerDialogueAudio;
    private TextMeshProUGUI _currentDisplayText;

    public event Action OnDialogueStarted;
    public event Action OnDialogueEnded;

    private List<string> _currentData = new List<string>();
    private float _currentTextDuration;
    private int _arrayIndex;
    private bool _canInterruptTyping;

    private DialogueState _states;
    private AudioSource _audioSource;
    private DialogueSO _oldspeaker;
    private Dictionary<string, DialogueAudioInfoSO> _audioInfoDictorary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        GameManager.DialogueManager = this;

        _audioSource = this.gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        _sentences = new Queue<string>();

        GameManager.AudioSystem.AssignOutputMixerGroup(_audioSource, GameManager.AudioSystem.VFX);

        EnableTextSkip(false);

        EnableDisableChoices(false);
    }

    public void Initialize(DialogueSO dialogue, TextMeshProUGUI text, bool hasChoisingSystem)
    {
        if (hasChoisingSystem) InitialiceChoices();

        _currentSpeaker = dialogue;
        _currentDisplayText = text;

        InitializeAudioInfo();
    }

    private void InitialiceChoices()
    {
        _choicesText = new TextMeshProUGUI[choiceOptions.Length];

        int index = 0;
        foreach (var choice in choiceOptions)
        {
            int currentIndex = index;

            _choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            choiceOptions[index].onClick.AddListener(()=>StopDialogue(currentIndex));
            index++;
        }
    }

    private void EnableDisableChoices(bool state)
    {
        for (int i = 0; i < choiceOptions.Length; i++)
        {
            choiceOptions[i].gameObject.SetActive(state);
        }
    }

    private void InitializeAudioInfo()
    {
        if (_currentSpeaker.DialogueAudio.Length < 0) return;

        _audioInfoDictorary = new Dictionary<string, DialogueAudioInfoSO>();

        foreach (DialogueAudioInfoSO audio in _currentSpeaker.DialogueAudio)
        {
            _audioInfoDictorary.Add(audio.id, audio);
        }

    }

    private void SetCurrentSpeakerAudio(string id)
    {
        DialogueAudioInfoSO audioInfo = null;

        _audioInfoDictorary.TryGetValue(id, out audioInfo);

        if (audioInfo != null)
        {
            _currentSpeakerDialogueAudio = audioInfo;
        }
        else
        {
            Debug.LogWarning("Failed to find audio info for id: " + id);
        }
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
                //nueva funcion que chequea si estoy interactuando con otro npc asi triggerear dialogo de nuevo
                CheckContinueDialoging();
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

    private void CheckContinueDialoging()
    {
        OnDialogueStarted?.Invoke();
        //necesito dispara el evento para obtener los datos
        if (_oldspeaker != _currentSpeaker)
        {
            ChangeDialogueState(DialogueState.StartTalking);
            CheckState();
        }
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

        _currentSpeaker.OnDialogueStarted?.Invoke();
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
            if(_currentSpeaker.HasChoices)
            {
                DisplayChoices();
            }
            else
            {
                EndDialogue(_currentSpeaker);
            }
            return;
        }

        _currentSentence = _sentences.Dequeue();

        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);


        SetCurrentSpeakerAudio(_currentSpeaker.DialogueAudio[0].id);
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
        EnableTextSkip(false);
        _currentDisplayText.text = IsSpeakerNameShowable();

        foreach (char letter in sentence.ToCharArray())
        {
            while (_canInterruptTyping)
            {
                yield return null;
            }

            PlayDialogueSound(letter, letter);
            _currentDisplayText.text += letter;

            yield return new WaitForSeconds(textTypingSpeed);
        }

        _currentTextDuration = _currentSpeaker.DialogueData[_arrayIndex].TextDuration;
        yield return new WaitForSeconds(_currentTextDuration);

        _arrayIndex++;
        IsTyping = false;

        EnableTextSkip(true);
        //AutoHideText(); 
    }

    private void PlayDialogueSound(int currentDisplayedCharacterCount, char currentCharacter)
    { 

        AudioClip[] dialogueTypingSoundClips = _currentSpeakerDialogueAudio.dialogueTypingSounds;
        int frequencyLevel = _currentSpeakerDialogueAudio.frecuencyLevel;
        float minPitch = _currentSpeakerDialogueAudio.minPitch;
        float maxPitch = _currentSpeakerDialogueAudio.maxPitch;

        if (currentDisplayedCharacterCount % _currentSpeakerDialogueAudio.frecuencyLevel == 0)
        {
            AudioClip clip = null;
            if (isAudioPredictable)
            {
                int hashCode = currentCharacter.GetHashCode();
                int predictableIndex = hashCode % _currentSpeakerDialogueAudio.dialogueTypingSounds.Length;
                clip = _currentSpeakerDialogueAudio.dialogueTypingSounds[predictableIndex];

                int minPitchInt = (int)(minPitch * 100);
                int maxPitchInt = (int)(maxPitch * 100);
                int pitchRange = maxPitchInt - minPitchInt;

                if (pitchRange != 0)
                {
                    int preedictablePitch = (hashCode % pitchRange) + minPitchInt;
                    float predictablePitch2 = preedictablePitch / 100f;
                    _audioSource.pitch = predictablePitch2;
                }
                else
                {
                    _audioSource.pitch = minPitch;
                }
            }
            else
            {
                int randomIndex = UnityEngine.Random.Range(0, _currentSpeakerDialogueAudio.dialogueTypingSounds.Length);
                clip = _currentSpeakerDialogueAudio.dialogueTypingSounds[randomIndex];

                _audioSource.pitch = UnityEngine.Random.Range(_currentSpeakerDialogueAudio.minPitch, _currentSpeakerDialogueAudio.maxPitch);
            }
            _audioSource.PlayOneShot(clip);

        }

    }

    IEnumerator FinishVisual()
    {
        while (_canInterruptTyping)
        {
            yield return null;
        }

        yield return new WaitForSeconds(_currentTextDuration);

        IsTyping = false;

        EnableTextSkip(true);
        //AutoHideText(); 
    }

    private void SetCurrentDialogue()
    {
        foreach (var data in _currentSpeaker.DialogueData)
        {
            _currentData.Add(data.Text);
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

    private void EnableTextSkip(bool enable)
    {
        skipText.enabled = enable;
    }

    private void AutoHideText()
    {
        HandleTextSkip();
    }

    private void EndDialogue(DialogueSO dialogue)
    {
        MouseHandler.RelinquishControl(this);

        _arrayIndex = 0;
        _currentDisplayText.text = "";
        _sentences.Clear();
        _currentData.Clear();

        _currentSpeaker.OnDialogueEnded?.Invoke();
        OnDialogueEnded?.Invoke();
        ChangeDialogueState(DialogueState.FinishedTalking);

        _oldspeaker = _currentSpeaker;
    }


    public void StopCurrentDialogue()
    {
        _canInterruptTyping = true;
    }

    public void ResumeCurrentDialogue()
    {
        _canInterruptTyping = false;
    }

    private void DisplayChoices()
    {
        MouseHandler.RequestControl(CursorLockMode.Confined, true, this);

        List<DialogueChoices> currentChoices=new();

        for (int i = 0; i < _currentSpeaker.DialogueChoices.Length; i++)
        {
            currentChoices.Add(_currentSpeaker.DialogueChoices[i]);
        }

        if (currentChoices.Count > choiceOptions.Length) return;

        int index = 0;
        foreach (var choice in currentChoices)
        {
            choiceOptions[index].gameObject.SetActive(true);
            _choicesText[index].text = choice.Text;
            index++;
        }

        /*for (int i = index; i < choiceOptions.Length; i++) //disable tha remaining dialogue options
        {
            choiceOptions[i].gameObject.SetActive(false);
        }*/
    }
    private void StopDialogue(int seletecButtonIndex)
    {
        _currentSpeaker.OnSelectedChoice?.Invoke(seletecButtonIndex);

        EnableDisableChoices(false);
        EndDialogue(_currentSpeaker);
    }

}

public enum DialogueState
{
    StartTalking,
    IsTalking,
    CanRepeatDialogue,
    FinishedTalking
}

