using PrimeTween;
using System;
using UnityEngine;

namespace Root {
    public class EmergencyStopButton : Interactable {
        public int maxUses;
        public int usesLeft;
        public bool isBraking;
        
        public float brakeSpeed;

        public Transform buttonObject;
        public float buttonTravelDistance;
        public float buttonPressTime;
        
        public Transform coverObject;
        public float coverRotationAngle;
        public float coverRotationTime;
        private Easing coverEasing;
        
        private bool isAnimating = false;
        private bool isCoverDown = true;
        [SerializeField] DiscSlot _discSlot;
        [SerializeField] private AudioClip stopSound;
        [SerializeField] private AudioClip interactSound;
        private void Awake() {
            coverEasing = Easing.Bounce(0.5f);
        }

        public override void StartInteraction() {
            if (isAnimating) return;
            if (interactSound != null) AudioSource.PlayClipAtPoint(interactSound, transform.position);
            if (isCoverDown) {
                OpenCover();
                return;
            }

            if (IsSpent() || isBraking || GameManager.Train.IsStopped() || _discSlot.Disc == null)
            {
                Debug.Log("EmergenciaNO");
                LowerAndRaiseButton();
                return;
            }

            LowerButtonAndCloseCover();
            isBraking = true;
            usesLeft--;
            GameManager.AudioSystem.PlaySound(stopSound);
            Debug.Log("Emergencia");
            _discSlot.Disc.SetDiscUsage();
        }

        public override void EndInteraction()
        {
            
        }

        public void OpenCover() {
            isAnimating = true;
            isCoverDown = false;
            Tween.LocalEulerAngles(coverObject, coverObject.localEulerAngles, coverObject.localEulerAngles + new Vector3(coverRotationAngle, 0, 0), coverRotationTime, coverEasing).OnComplete(() => isAnimating = false);
        }
        
        public void CloseCover() {
            isAnimating = true;
            isCoverDown = true;
            Tween.LocalEulerAngles(coverObject, coverObject.localEulerAngles, coverObject.localEulerAngles - new Vector3(coverRotationAngle, 0, 0), coverRotationTime, coverEasing).OnComplete(() => isAnimating = false);
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
            Tween.LocalPosition(buttonObject, buttonObject.localPosition, buttonObject.localPosition + Vector3.up * buttonTravelDistance, buttonPressTime, coverEasing).OnComplete(() => CloseCover());
        }
        
        public void LowerAndRaiseButton() {
            isAnimating = true;
            Tween.LocalPosition(buttonObject, buttonObject.localPosition, buttonObject.localPosition + Vector3.up * buttonTravelDistance, buttonPressTime, coverEasing).OnComplete(() => RaiseButton());
        }
        
        public bool IsBreaking() {
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