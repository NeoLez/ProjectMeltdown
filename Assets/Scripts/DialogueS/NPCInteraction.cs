using System;
using UnityEngine;

namespace Root
{
    public class NPCInteraction : InteractBehaviour
    {
        [SerializeField] DialogueSO dialogue;        

        private void Start()
        {
            DialogueManager.Instance.OnDialogueStarted += StartedExecutingDialogue;
            DialogueManager.Instance.OnDialogueEnded += FinishedExecutingDialogue;
        }

        private void OnDestroy()
        {
            DialogueManager.Instance.OnDialogueStarted -= StartedExecutingDialogue;
            DialogueManager.Instance.OnDialogueEnded -= FinishedExecutingDialogue;
        }

        public override void ExecuteDialogue()
        {
            if (!gameObject.activeInHierarchy) return;

            SubtitleManager.Instance.SetTextValues(dialogue);

            TriggerDialogue();
        }

        void TriggerDialogue()
        {
            if (dialogue != null)
            {
                DialogueManager.Instance.TriggerDialogue();
            }
        }

        public virtual void StartedExecutingDialogue(){ }

        public virtual void FinishedExecutingDialogue(){

        }

    }

    public enum DialogueState
    {
        StartTalking,
        IsTalking,
        CanRepeatDialogue, 
        FinishedTalking
    }
}
