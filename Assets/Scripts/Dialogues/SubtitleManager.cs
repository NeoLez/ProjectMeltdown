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
        }

        public void ShowSubtitle()
        {
            dialogueManager.Initialize(dialogue, subtitleText);

            HideOrViewCanvas(true);
        }

        public void HideSubtitle()
        {
            if (dialogueManager.IsTyping) return;

            HideOrViewCanvas(false);
        }

        private void HideOrViewCanvas(bool set)
        {
            subtitleCanvas.enabled = set;
        }
    }
}
