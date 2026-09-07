using System;
using UnityEngine;

namespace Root
{
    public class MerchantInteraction : InteractBehaviour
    {
        [SerializeField] MerchantTrigger merchantTrigger;

        private void Start()
        {
            merchantTrigger._OnStoreShow?.Invoke(false);
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

        public override void FinishedExecutingDialogue()
        {       
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
