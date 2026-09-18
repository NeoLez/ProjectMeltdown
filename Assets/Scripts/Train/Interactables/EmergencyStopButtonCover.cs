using PrimeTween;
using UnityEngine;

namespace Root {
    public class EmergencyStopButtonCover : Interactable {
        public float coverRotationAngle;
        public float coverRotationTime;
        private bool _isCoverDown = true;
        private bool _isAnimating;
        private Easing _coverEasing;

        private void Awake() {
            _coverEasing = Easing.Bounce(0.5f);
        }

        public override void StartInteraction() {
            if (_isAnimating) return;
            if(_isCoverDown)
                OpenCover();
            else
                CloseCover();
        }

        public override void EndInteraction() {
            
        }
        
        private void OpenCover() {
            _isAnimating = true;
            _isCoverDown = false;
            Tween.LocalRotation(transform, transform.rotation, Quaternion.Euler(new Vector3(coverRotationAngle, 0, 0)), coverRotationTime, _coverEasing).OnComplete(() => _isAnimating = false);
        }
        
        public void CloseCover() {
            _isAnimating = true;
            _isCoverDown = true;
            Tween.LocalRotation(transform, transform.rotation, Quaternion.identity, coverRotationTime, _coverEasing).OnComplete(() => _isAnimating = false);
        }
    }
}