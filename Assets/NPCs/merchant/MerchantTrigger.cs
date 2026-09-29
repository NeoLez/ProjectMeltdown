using Root;
using Root.Controller;
using Root.Managers;
using System;
using UnityEngine;

namespace Root
{
    public class MerchantTrigger : MonoBehaviour
    {
        [SerializeField] GameObject _face;
        [SerializeField] StoreManager _storeManager;
        [SerializeField] Collider triggerCollider;

        private Animator _anim;
        public Action<bool> _OnStoreShow;
        bool _canTriggerOnStart;
        private void Awake()
        {
            _OnStoreShow += HandleInteraction;
        }

        void Start()
        {
            if (_storeManager.CanSpawnMultipleItems)
            {
                _canTriggerOnStart = true;
            }
            _anim = _face.GetComponent<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject != GameManager.Player.gameObject) return;

            if(_canTriggerOnStart) UIManager.Instance.OpenMenu(UIManager.UITypes.Store);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject != GameManager.Player.gameObject) return;

            CanShowItems(false);
            _canTriggerOnStart = true;
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
            triggerCollider.enabled = enable ? true : false;

            if (enable)
            {
                CanShowItems(true);
            }
            
        }

        private void OnDestroy()
        {
            _OnStoreShow -= HandleInteraction;
        }
    }

}



