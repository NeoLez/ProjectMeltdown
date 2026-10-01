using Root;
using Root.Controller;
using Root.Managers;
using System;
using UnityEngine;

namespace Root
{
    public class MerchantTrigger : MonoBehaviour //TODO-Erease this script and migrate everything to MerchantInteraction o StoreManager
    {
        public bool IsStoreOpened { get; set; }

        [SerializeField] private GameObject _face;
        [SerializeField] private StoreManager _storeManager;
        [SerializeField] private Collider triggerCollider;
        [SerializeField] private MerchantInteraction interaction;

        private Animator _anim;
        public Action<bool> _OnStoreShow;

        private void Awake()
        {
            _OnStoreShow += HandleInteraction;
            interaction.OnInteractionContinue += Interact;
        }

        void Start()
        {
            _anim = _face.GetComponent<Animator>();
        }

        public void Interact()
        {
            if (IsStoreOpened) return;

            IsStoreOpened = true;
            UIManager.Instance.OpenMenu(UIManager.UITypes.Store);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsStoreOpened) return;
            CanShowItems(false);
            IsStoreOpened = false;
        }

        public void CanShowItems(bool show)
        {
            if (show)
            {
                _anim.SetBool("Appear", true);
                CancelInvoke(nameof(DelayedShow));
                Invoke(nameof(DelayedShow), 1f);
            }
            else 
            {
                _anim.SetBool("Appear", false);
                CancelInvoke(nameof(DelayedShow));
                _storeManager.HideItems();
            }

        }
        public void DelayedShow()
        {
            _storeManager.ShowItems();
        }

        private void HandleInteraction(bool enable)
        {
            if (enable)
            {
                CanShowItems(true);
            }
            
        }

        private void OnDestroy()
        {
            _OnStoreShow -= HandleInteraction;
            interaction.OnInteractionContinue -= Interact;
        }

    }

}



