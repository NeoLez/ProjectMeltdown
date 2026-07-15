using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace Root {
    public class ForkDecisionSwitch : PanelButton {
        [SerializeField] private bool right = false;
        
        public Transform lightObject;
        public Transform rotationOn;
        public Transform rotationOff;
        public float rotationTime;
        private bool isAnimating;
        
        private void Awake() {
            OnClicked += Toggle;
        }

        public void Toggle() {
            //if (sound != null) AudioSource.PlayClipAtPoint(sound, transform.position);
            if (sound != null) GameManager.AudioSystem.PlaySoundPositional(sound, transform.position, GameManager.AudioSystem.VFX);
            if (right) TurnOff();
            else TurnOn();
        }

        public void TurnOff()
        {
            if (isAnimating) return;
            isAnimating = true;
            Tween.LocalRotation(lightObject, rotationOff.localRotation, rotationTime).OnComplete(() => isAnimating = false);
                
            right = false;
        }

        public void TurnOn()
        {
            if (isAnimating) return;
            isAnimating = true;
            Tween.LocalRotation(lightObject, rotationOn.localRotation, rotationTime).OnComplete(() => isAnimating = false);
            
            right = true;
        }

        public bool GetDirection() {
            return right;
        }
    }
}