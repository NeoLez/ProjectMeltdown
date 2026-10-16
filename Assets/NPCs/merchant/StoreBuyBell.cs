using Root;
using Root.Controller;
using Root.Managers;
using System;
using UnityEngine;

namespace Root
{
    public class StoreBuyBell : StoreBell
    {
        [SerializeField] Animator merchantAnimator;
        [SerializeField] private StoreMenu menu;

        public Action<bool> _OnStoreShow;

        private void Awake()
        {
            OnBellRanged += OpenStore;
            OnBellRangedAgain += CloseStore;

            _OnStoreShow += HandleInteraction;
        }

        private void OpenStore()
        {
            CanShowItems(true);
            IsStoreOpened = true;
        }

        private void CloseStore()
        {
            CanShowItems(false);
            IsStoreOpened = false;
        }

        public void CanShowItems(bool show)
        {
            if (show)
            {
                merchantAnimator.SetBool("Appear", true);
                CancelInvoke(nameof(DelayedShow));
                Invoke(nameof(DelayedShow), 1f);
            }
            else 
            {
                merchantAnimator.SetBool("Appear", false);
                CancelInvoke(nameof(DelayedShow));
                storeManager.HideItems();
                menu.EnableWalletCanvas(false);
            }

        }
        public void DelayedShow()
        {
            storeManager.ShowItems();
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
            OnBellRanged = null;
        }

    }

}



