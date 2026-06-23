using System;
using UnityEngine;

namespace Root {
    public class PanelButton : Interactable {
        public GameObject onObject;
        public GameObject offObject;
        public event Action OnClicked;
        [SerializeField] private AudioClip sound;

        [field: SerializeField] private bool Locked { get; set; }
        
        public override void StartInteraction() {
            if (!Locked) {
                OnClicked?.Invoke();
                if (sound != null) AudioSource.PlayClipAtPoint(sound, transform.position);
            }
        }

        public override void EndInteraction()
        {
            
        }

        public void Lock() {
            Locked = true;
            if (onObject == null || offObject == null) return;
            onObject.SetActive(false);
            offObject.SetActive(true);
        }

        public void Unlock() {
            Locked = false;
            if (onObject == null || offObject == null) return;
            onObject.SetActive(true);
            offObject.SetActive(false);
        }
        
    }
}