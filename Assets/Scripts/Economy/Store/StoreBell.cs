using System;
using UnityEngine;

namespace Root
{
    public abstract class StoreBell : InteractableNormalCamera
    {
        public bool IsStoreOpened { get; set; }
        public bool CanCloseStore {  get; set; }    

        [SerializeField] protected StoreManager storeManager;

        protected Action OnBellRanged;
        protected Action OnBellRangedAgain;

        public override void Interact()
        {
            if (!IsStoreOpened)
            {
                OnBellRanged?.Invoke();
            }
            //else
            //{
            //    OnBellRangedAgain?.Invoke();
            //}
        }

        //agregar habilitar y desabilitar interaccion
    }
}
