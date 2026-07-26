using System;
using UnityEngine;

namespace Root
{
    public class MerchantInteraction : InteractBehaviour
    {
        [SerializeField] MerchantTrigger merchantTrigger;

        private void Start()
        {
            DialogueManager.Instance.OnDialogueStarted += StartedExecutingDialogue;
            DialogueManager.Instance.OnDialogueEnded += FinishedExecutingDialogue;

            merchantTrigger._OnStoreShow?.Invoke(false);
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

        public override void StartedExecutingDialogue()
        {
            GameManager.Input.Movement.Disable();

            base.StartedExecutingDialogue();
        }

        public override void FinishedExecutingDialogue()
        {       
            GameManager.Input.Movement.Enable();

            ShowStoreItems();
            base.FinishedExecutingDialogue();
        }

       
        public void ShowStoreItems()
        {
            merchantTrigger._OnStoreShow?.Invoke(true);
        }

    }
}
