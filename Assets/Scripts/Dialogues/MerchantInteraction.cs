using UnityEngine;

namespace Root
{
    public class MerchantInteraction : InteractBehaviour
    {
        [SerializeField] StoreBuyBell storeBell;
        [SerializeField] Collider dialogueTrigger;
        [SerializeField] StoreManager storeManager;
        private void Start()
        {
            if (HasDialogue() || !storeManager.CanSpawnMultipleItems)
            {
                storeBell._OnStoreShow?.Invoke(false);
            }         
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


            TriggerDialogue();
        }

        void TriggerDialogue()
        {
            if (Dialogue == null) return;

            DialogueManager.Instance.StartConversation(Dialogue);
        }

        public override void FinishedExecutingDialogue()
        {
            if (!Dialogue.CanRepeatDialogue)
            {
                hasBeenTriggeredOnce = true;
                HandleInteraction(false);
            }

            base.FinishedExecutingDialogue();

            ShowStoreItems();
        }

        private void ShowStoreItems()
        {
            storeBell._OnStoreShow?.Invoke(true);
        }

        private void HandleInteraction(bool enable)
        {
            dialogueTrigger.enabled = enable ? true : false;
        }
    }
}
