using System;
using PrimeTween;
using UnityEngine;

namespace Root {
    public class EmergencyStopButton : Interactable {
        public int usesLeft;
        public bool isBraking;
        public event Action OnBrake;
        
        public float brakeSpeed;

        public Transform buttonObject;
        public float buttonTravelDistance;
        public float buttonPressTime;
        [SerializeField] private EmergencyStopButtonCover cover;
        
        
        private Easing coverEasing;
        
        private bool isAnimating = false;
        [SerializeField] DiscSlot _discSlot;
        [SerializeField] private AudioClip stopSound;
        [SerializeField] private AudioClip interactSound;
        [SerializeField] private TriggerAnimationByInteract _brakeDoorAnim;
        private void Awake() {
            coverEasing = Easing.Bounce(0.5f);
        }

        public override void StartInteraction() {
            if (isAnimating) return;
            if (interactSound != null) GameManager.AudioSystem.PlaySoundPositional(interactSound, transform.position, GameManager.AudioSystem.VFX);

            if (_brakeDoorAnim.IsObjectOpen || IsSpent() || isBraking || GameManager.Train.IsStopped() || _discSlot.GetBrakeDisc() == null)
            {
                LowerAndRaiseButton();
                return;
            }

            LowerButtonAndCloseCover();
            isBraking = true;
            usesLeft--;
            GameManager.AudioSystem.PlaySound(stopSound, GameManager.AudioSystem.VFX);
            _discSlot.GetBrakeDisc().SetDiscUsage();
            OnBrake?.Invoke();
        }

        public override void EndInteraction()
        {
            
        }
        

        public void LowerButton() {
            isAnimating = true;
            Tween.LocalPosition(buttonObject, buttonObject.localPosition, buttonObject.localPosition + Vector3.up * buttonTravelDistance, buttonPressTime, coverEasing).OnComplete(() => isAnimating = false);
        }
        
        public void RaiseButton() {
            isAnimating = true;
            Tween.LocalPosition(buttonObject, buttonObject.localPosition, buttonObject.localPosition - Vector3.up * buttonTravelDistance, buttonPressTime, coverEasing).OnComplete(() => isAnimating = false);
        }

        public void LowerButtonAndCloseCover() {
            isAnimating = true;
            Tween.LocalPosition(buttonObject, buttonObject.localPosition, buttonObject.localPosition + Vector3.up * buttonTravelDistance, buttonPressTime, coverEasing).OnComplete(() => cover.CloseCover());
        }
        
        public void LowerAndRaiseButton() {
            isAnimating = true;
            Tween.LocalPosition(buttonObject, buttonObject.localPosition, buttonObject.localPosition + Vector3.up * buttonTravelDistance, buttonPressTime, coverEasing).OnComplete(() => RaiseButton());
        }
        
        public bool IsBraking() {
            return isBraking;
        }
        
        public void FinishBraking() {
            RaiseButton();
            isBraking = false;
        }

        public int Repair(int amount) {
            /*int maxRepairs = maxUses - usesLeft;
            if (amount > maxRepairs) {
                usesLeft = maxUses;
                return amount - maxRepairs;
            }
            
            usesLeft += amount;*/
            usesLeft = amount;
            return 0;
        }

        public bool IsSpent() {
            return usesLeft <= 0;
        }

    }
}