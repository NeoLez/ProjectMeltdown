using Root;
using System;
using UnityEngine;

namespace Root
{
    public class MerchantTrigger : MonoBehaviour
    {
        [SerializeField] GameObject _face;
        [SerializeField] StoreManager _storeManager;
        [SerializeField] Collider triggerCollider;


        Animator _anim;
        public Action<bool> _OnStoreShow;

        private void Awake()
        {
            _OnStoreShow += HandleInteraction;
        }

        private void OnDestroy()
        {
            _OnStoreShow -= HandleInteraction;
        }

        void Start()
        {
            _anim = _face.GetComponent<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject != GameManager.Player.gameObject) return;
            HandleStore(true);
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject != GameManager.Player.gameObject) return;
            HandleStore(false);
        }

        public void DelayedShow()
        {
            _storeManager.ShowItems();
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

        private void HandleInteraction(bool enable)
        {
            triggerCollider.enabled = enable ? true : false;

            if(enable) HandleStore(true);
        }
    }

}



