using Root;
using Root.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public bool IsTyping = false;

    [Header("Dialogue Config")]
    [SerializeField] private float textTypingSpeed;
    [SerializeField] private bool isAudioPredictable;
    [SerializeField] private TextMeshProUGUI skipText;
    [SerializeField] private SubtitleManager subtitleManager;

    [Header("Dialogue Options")]
    [SerializeField] private HorizontalLayoutGroup choicesLayout;
    [SerializeField] private Button choicePrefab;
    private Button[] choiceOptions;
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
    private bool _hasChosenOption;

    private DialogueState _states;
    private AudioSource _audioSource;
    private DialogueSO _oldspeaker;
    private Dictionary<string, DialogueAudioInfoSO> _audioInfoDictorary;

    private DialogueExecutionContext _currentContext;
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
        //_sentences = new Queue<string>();

        GameManager.AudioSystem.AssignOutputMixerGroup(_audioSource, GameManager.AudioSystem.VFX);

        EnableTextSkip(false);
    }
    private void EnableDisableChoices(bool state)
    {
        for (int i = 0; i < choiceOptions.Length; i++)
        {
            choiceOptions[i].gameObject.SetActive(state);
        }
    }

    #region Initializations
    public void Initialize(DialogueSO dialogue, TextMeshProUGUI text, bool hasChoisingSystem)
    {
        _currentSpeaker = dialogue;
        _currentDisplayText = text;

        InitializeAudioInfo();
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

    #endregion

    public void StartConversation(DialogueSO dialogue, InteractBehaviour npcGameObject = null)
    {
        _currentContext = new DialogueExecutionContext(GameManager.Player.gameObject, npcGameObject);

        if (_currentSpeaker == null || _currentSpeaker != dialogue)
        {
            Initialize(dialogue, subtitleManager.SubtitleText, dialogue.HasChoices);
        }

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
                //Works to trigger the dialogue once more if you're interacting with a different NPC
                CheckContinueDialoging();
                break;
            default:
                break;
        }

        if (CheckDialogueState() == DialogueState.FinishedTalking)
        {
            if (_oldspeaker.CanRepeatDialogue)
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
        OnDialogueStarted?.Invoke(); //mandatory to re-obtaing the dialogue data from the NPC

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
            if (!_hasChosenOption) DisplayNextSentence();
        }
    }

    private void StartDialogue()
    {
        OnDialogueStarted?.Invoke();

        if (_currentDisplayText == null || _currentSpeaker == null) return;

        _currentSpeaker.OnDialogueStarted?.Invoke();
        SetCurrentDialogue();

        DisplayNextSentence();

        ChangeDialogueState(DialogueState.IsTalking);
    }

    public void DisplayNextSentence()
    {
        if (_arrayIndex >= _currentData.Count)
        {
            EndDialogue(_currentSpeaker);
            return;
        }

        if (_currentSpeaker.endDialogueLine.Length > 0)
        {
            if (_currentSpeaker.endDialogueLine.Length > _arrayIndex && _currentSpeaker.endDialogueLine[_arrayIndex])
            {
                EndDialogue(_currentSpeaker);
                return;
            }
        }

        _currentSentence = _currentData[_arrayIndex];

        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);

        SetCurrentSpeakerAudio(_currentSpeaker.DialogueAudio[0].id);
        _typingCoroutine = StartCoroutine(TypeSentence(_currentSentence));
    }

    private void FinishSentenceEarly()
    {
        StopCoroutine(_typingCoroutine);

        _currentDisplayText.text = IsSpeakerNameShowable();
        _currentDisplayText.text += _currentSentence;

        if (_currentSpeaker.DialogueData[_arrayIndex].HasChoices) DisplayChoices();
        _arrayIndex++;

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

        if (_currentSpeaker.DialogueData[_arrayIndex].HasChoices) DisplayChoices();

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

    private IEnumerator FinishVisual()
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

        subtitleManager.SetTextValues(_currentSpeaker);
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
        skipText.enabled = _hasChosenOption ? !enable : enable;
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
        _currentData.Clear();

        _currentSpeaker.OnDialogueEnded?.Invoke();
        OnDialogueEnded?.Invoke();
        ChangeDialogueState(DialogueState.FinishedTalking);
        if (_currentSpeaker.DialogueData[_arrayIndex].HasChoices) ClearChoices();
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


    #region Choices

    private void DisplayChoices()
    {
        _hasChosenOption = true;
        MouseHandler.RequestControl(CursorLockMode.Confined, true, this);


        choiceOptions = new Button[_currentSpeaker.DialogueChoices.Length];

        _choicesText = new TextMeshProUGUI[choiceOptions.Length];

        List<DialogueChoices> currentChoices = new();

        for (int i = 0; i < _currentSpeaker.DialogueChoices.Length; i++)
        {
            choiceOptions[i] = Instantiate(choicePrefab, choicesLayout.transform);

            int currentIndex = i;

            currentChoices.Add(_currentSpeaker.DialogueChoices[i]);
            int nextIdex = _currentSpeaker.DialogueChoices[i].NextDialogueIndex;

            if (nextIdex > _currentData.Count)
            {
                Debug.LogError("Dialogue index is greater than current dialogue length");
            }

            _choicesText[i] = choiceOptions[i].GetComponentInChildren<TextMeshProUGUI>();

            choiceOptions[i].onClick.AddListener(() => StopDialogue(currentIndex, nextIdex));
        }

        if (currentChoices.Count > choiceOptions.Length) return;

        int index = 0;
        foreach (var choice in currentChoices)
        {
            //choiceOptions[index].gameObject.SetActive(true);
            _choicesText[index].text = choice.Text;

            EnableChoises(index);

            TollInformationMenu toll = choiceOptions[index].GetComponent<TollInformationMenu>();
            toll.Initialize(choice.ChoiceDescription);

            index++;
        }

        for (int i = index; i < choiceOptions.Length; i++) //disable tha remaining dialogue options
        {
            choiceOptions[i].gameObject.SetActive(false);
        }

    }
    private void EnableChoises(int index)
    {
        if (_currentSpeaker.DialogueChoices.Length == 0) return;

        DialogueChoices selectedChoice = _currentSpeaker.DialogueChoices[index];

        if (selectedChoice.effects.Count == 0) //Force an a button with no effects to always be true
        {
            choiceOptions[index].GetComponent<TollInformationMenu>().EnableChoise(true);
        }

        for (int i = 0; i < selectedChoice.effects.Count; i++)
        {
            DialogueEffectSO effect = selectedChoice.effects[i];

            bool context = effect.CanExecute(_currentContext);

            choiceOptions[index].GetComponent<TollInformationMenu>().
                EnableChoise(context);
        }

    }

    public void SelectOption(int index)
    {
        DialogueChoices selectedChoice = _currentSpeaker.DialogueChoices[index];

        for (int i = 0; i < selectedChoice.effects.Count; i++)
        {
            //int effectIndex = i;
            DialogueEffectSO effect = selectedChoice.effects[i];

            effect.Execute(_currentContext);
        }
    }

    private void StopDialogue(int seletecButtonIndex, int nextLineIndex)
    {
        _hasChosenOption = false;

        SelectOption(seletecButtonIndex);

        EnableDisableChoices(false);

        _arrayIndex = nextLineIndex;

        DisplayNextLine();
    }

    private void DisplayNextLine()
    {
        _currentSentence = _currentData[_arrayIndex];

        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);

        SetCurrentSpeakerAudio(_currentSpeaker.DialogueAudio[0].id);
        _typingCoroutine = StartCoroutine(TypeSentence(_currentSentence));
    }

    private void ClearChoices()
    {
        if (choiceOptions.Length == 0) return;

        for (int i = 0; i < choiceOptions.Length; i++)
        {
            choiceOptions[i].onClick.RemoveAllListeners();
            choiceOptions[i].GetComponent<TollInformationMenu>().ClearPanelInformation();
            Destroy(choiceOptions[i].gameObject);
        }
    }

    #endregion
}

public enum DialogueState
{
    StartTalking,
    IsTalking,
    CanRepeatDialogue,
    FinishedTalking
}

