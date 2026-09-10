using System;
using UnityEngine;

namespace Root
{
    public class MerchantInteraction : InteractBehaviour
    {
        [SerializeField] MerchantTrigger merchantTrigger;
        [SerializeField] Collider dialogueTrigger;
        [SerializeField] StoreManager storeManager;
        private void Start()
        {
            merchantTrigger._OnStoreShow?.Invoke(false);
        }

        public override void StartedExecutingDialogue()
        {
            HandleInteraction(true);

            base.StartedExecutingDialogue();
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
            if (!Dialogue.CanRepeatDialogue)
            {
                hasBeenTriggeredOnce = true;
                HandleInteraction(false);
            }
            ShowStoreItems();

            base.FinishedExecutingDialogue();
        }
        
        private void ShowStoreItems()
        {
            merchantTrigger._OnStoreShow?.Invoke(true);
        }

        private void HandleInteraction(bool enable)
        {
            dialogueTrigger.enabled = enable ? true : false;
        }
    }
}
