using System;
using UnityEngine;

namespace Root {
    public class Button : InteractableNormalCamera {
        public GameObject onObject;
        public GameObject offObject;
        public event Action OnClicked;
        
        private float _unlockTime;
        private bool _isLocked;

        [SerializeField] private AudioClip _press;
        [SerializeField] private AudioClip _door;
        private bool IsLocked() {
            return _isLocked;
        }
        private void Update() {
            if (_isLocked && Time.time > _unlockTime)
                Unlock();
        }

        public override void Interact() {
            if (_isLocked) return;
            //Debug.Log("Interact");
            OnClicked?.Invoke();
            GameManager.AudioSystem.PlaySoundPositional(_press, transform.position, GameManager.AudioSystem.VFX, 0.5f);
            GameManager.AudioSystem.PlaySoundPositional(_door, transform.position, GameManager.AudioSystem.VFX, 0.8f);
        }

        public void Lock() {
            _unlockTime = float.MaxValue;
            _isLocked = true;
            onObject.SetActive(false);
            offObject.SetActive(true);
        }

        public void Unlock() {
            _isLocked = false;
            onObject.SetActive(true);
            offObject.SetActive(false);
        }

        public void LockForSeconds(float duration) {
            _unlockTime = Time.time + duration;
            _isLocked = true;
            onObject.SetActive(false);
            offObject.SetActive(true);
        }
    }
}