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
        bool algo;

        void Start()
        {
            if (_storeManager.CanSpawnMultipleItems)
            {
                algo = true;
            }
            _anim = _face.GetComponent<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject != GameManager.Player.gameObject) return;
            if(algo) UIManager.Instance.OpenMenu(UIManager.UITypes.Store);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject != GameManager.Player.gameObject) return;
            HandleStore(false);
            algo = true;
        }

        public void HandleStore(bool show)
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
                HandleStore(true);
            }
            
        }

        private void OnDestroy()
        {
            _OnStoreShow -= HandleInteraction;
        }
    }

}



