using System.Collections;
using UnityEngine;

namespace Root
{
    public class MerchantInteraction : InteractBehaviour
    {
        [SerializeField] private StoreBuyBell storeBell;
        [SerializeField] private Collider dialogueTrigger;
        [SerializeField] private StoreManager storeManager;
        [SerializeField] private float _delay = 4f;
        [SerializeField] private Animator merchantAnimator;

        public bool WasAnimationTriggered { get; private set; }

        private void Start()
        {
            if (HasDialogue() || !storeManager.CanSpawnMultipleItems)
            {
                storeBell._OnStoreShow?.Invoke(false);
            }         
        }

        public override void StartedExecutingDialogue()
        {
            base.StartedExecutingDialogue();
        }

        public override void ExecuteDialogue()
        {
            if (!gameObject.activeInHierarchy) return;

            if (hasBeenTriggeredOnce) return;

            if (!WasAnimationTriggered)
                StartCoroutine(TriggerAnimationAndDialogue());
            else
                TriggerDialogue();
        }

        private IEnumerator TriggerAnimationAndDialogue()
        {
            if (merchantAnimator == null) yield break;
            if (Dialogue == null) yield break;

            GameManager.Input.Movement.Disable();
            IsAnimationPlaying = true;
            merchantAnimator.Play("2_StartShop");
            yield return new WaitForSeconds(_delay);

            WasAnimationTriggered = true;
            IsAnimationPlaying = false;
            TriggerDialogue();
        }

        private void TriggerDialogue()
        {
            if (Dialogue == null) return;

            DialogueManager.Instance.StartConversation(Dialogue);
        }

        public override void FinishedExecutingDialogue()
        {
            if (!Dialogue.CanRepeatDialogue)
            {
                hasBeenTriggeredOnce = true;
            }

            base.FinishedExecutingDialogue();

            ShowStoreItems();
        }

        private void ShowStoreItems()
        {
            storeBell._OnStoreShow?.Invoke(true);
        }

    }
}
