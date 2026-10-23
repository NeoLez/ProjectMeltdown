using System.Collections;
using UnityEngine;

namespace Root
{
    public class MerchantInteraction : InteractBehaviour
    {
        [SerializeField] StoreBuyBell storeBell;
        [SerializeField] Collider dialogueTrigger;
        [SerializeField] StoreManager storeManager;
        [SerializeField] float _delay = 4f;
        Animator _anim;
        [SerializeField] GameObject _merchant;
        int _animState = 0;
        private void Start()
        {
            _anim = _merchant.GetComponent<Animator>();
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
            if (_animState == 2) TriggerDialogue();
            else if (_animState == 0) StartCoroutine(TriggerAnimationAndDialogue());
            
        }
        IEnumerator TriggerAnimationAndDialogue()
        {
            _animState = 1;
            if (Dialogue == null)  yield break;
            _anim.Play("2_StartShop");

            DialogueManager.Instance.PoorSafeNet();
            yield return new WaitForSeconds(_delay);
            _animState = 2;
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
