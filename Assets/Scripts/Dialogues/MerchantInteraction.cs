using System;
using UnityEngine;

namespace Root
{
    public class MerchantInteraction : InteractBehaviour
    {
        [SerializeField] MerchantTrigger merchantTrigger;

        private void Start()
        {
            Dialogue.OnDialogueStarted += StartedExecutingDialogue;
            Dialogue.OnDialogueEnded += FinishedExecutingDialogue;

            merchantTrigger._OnStoreShow?.Invoke(false);
        }

        private void OnDestroy()
        {
            Dialogue.OnDialogueStarted -= StartedExecutingDialogue;
            Dialogue.OnDialogueEnded -= FinishedExecutingDialogue;
        }

        public override void ExecuteDialogue()
        {
            if (!gameObject.activeInHierarchy) return;

            if (hasBeenTriggeredOnce) return;

            SubtitleManager.Instance.SetTextValues(Dialogue);

            TriggerDialogue();
        }

        void TriggerDialogue()
        {
            if (Dialogue != null)
            {
                DialogueManager.Instance.TriggerDialogue();
            }
        }

        public override void StartedExecutingDialogue()
        {
            //GameManager.Input.Movement.Disable();

            base.StartedExecutingDialogue();
        }

        public override void FinishedExecutingDialogue()
        {       
            //GameManager.Input.Movement.Enable();

            if(!Dialogue.CanRepeatDialogue) hasBeenTriggeredOnce = true;

            ShowStoreItems();
            base.FinishedExecutingDialogue();
        }

       
        public void ShowStoreItems()
        {
            merchantTrigger._OnStoreShow?.Invoke(true);
        }

    }
}
