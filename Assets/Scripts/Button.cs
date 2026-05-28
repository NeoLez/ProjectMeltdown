using System;
using UnityEngine;

namespace Root {
    public class Button : InteractableNormalCamera {
        public GameObject onObject;
        public GameObject offObject;
        public event Action OnClicked;

        [field: SerializeField] private bool Locked { get; set; }
        
        public override void Interact() {
            OnClicked?.Invoke();
        }

        public void Lock() {
            Locked = true;
            onObject.SetActive(false);
            offObject.SetActive(true);
        }

        public void Unlock() {
            Locked = false;
            onObject.SetActive(true);
            offObject.SetActive(false);
        }
        
    }
}