using System;
using UnityEngine;

namespace Root {
    public class Button : Interactable {
        public GameObject onObject;
        public GameObject offObject;
        public event Action OnClicked;

        [field: SerializeField] private bool Locked { get; set; }
        
        public override void Interact(bool state) {
            if (state && !Locked) {
                OnClicked?.Invoke();
            }
        }

        public void Lock() {
            Debug.Log("aaaa");
            Locked = true;
            onObject.SetActive(false);
            offObject.SetActive(true);
        }

        public void Unlock() {
            Debug.Log("bbbb");
            Locked = false;
            onObject.SetActive(true);
            offObject.SetActive(false);
        }
        
    }
}