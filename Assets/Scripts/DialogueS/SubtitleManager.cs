using System.Linq;
using TMPro;
using UnityEngine;

namespace Root
{
    public class SubtitleManager : MonoBehaviour
    {
        public static SubtitleManager Instance;

        [SerializeField] Canvas subtitleCanvas;
        [SerializeField] TextMeshProUGUI subtitleText;

        [SerializeField] bool isAbleToShowSpeaker;
        private DialogueManager dialogueManager;
        private DialogueSO dialogue;

        string speakerNameText;
        string dialogueText;
        private int _arrayIndex;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            HideOrViewCanvas(false);

            dialogueManager = DialogueManager.Instance;

            dialogueManager.OnDialogueStarted += ShowSubtitle;
            dialogueManager.OnDialogueEnded += HideSubtitle;
        }

        private void OnDestroy()
        {
            dialogueManager.OnDialogueStarted -= ShowSubtitle;
            dialogueManager.OnDialogueEnded -= HideSubtitle;
        }

        public void SetTextValues(DialogueSO data) 
        {
            dialogue = data;
            speakerNameText = data.SpeakerName;
        }

        public void ShowSubtitle()
        {
            dialogueManager.SetDialogueParameters(dialogue, subtitleText);

            HideOrViewCanvas(true);
        }

        public void HideSubtitle()
        {
            if (dialogueManager.IsTyping) return;
            _arrayIndex=0;
            HideOrViewCanvas(false);
        }

        private void HideOrViewCanvas(bool set)
        {
            subtitleCanvas.enabled = set;
        }
    }
}
