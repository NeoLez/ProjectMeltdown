using System;
using UnityEngine;

namespace Root {
    public class Button : Interactable {
        public GameObject onObject;
        public GameObject offObject;
        public event Action OnClicked;

        public bool Locked { get; private set; }
        
        public override void Interact(bool state) {
            if (state && !Locked) {
                OnClicked?.Invoke();
            }
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