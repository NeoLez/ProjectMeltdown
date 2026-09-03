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
            GameManager.Input.Inventory.Disable();
            GameManager.Input.Movement.Disable();
            GameManager.Input.CameraMovement.Disable();
            GameManager.Input.Interaction.Wallet.Disable();
            GameManager.Input.Menu.Pause.Disable();
            GameManager.Input.Interaction.Wallet.Disable();
            if (GameManager.Wallet.IsOpened) {
                GameManager.Wallet.ToggleWallet();
            }
            dialogueManager.Initialize(dialogue, subtitleText);

            HideOrViewCanvas(true);
        }

        public void HideSubtitle()
        {
            GameManager.Input.Inventory.Enable();
            GameManager.Input.Movement.Enable();
            GameManager.Input.CameraMovement.Enable();
            GameManager.Input.Interaction.Wallet.Enable();
            GameManager.Input.Menu.Pause.Enable();
            GameManager.Input.Interaction.Wallet.Enable();
            if (dialogueManager.IsTyping) return;

            HideOrViewCanvas(false);
        }

        private void HideOrViewCanvas(bool set)
        {
            subtitleCanvas.enabled = set;
        }
    }
}
