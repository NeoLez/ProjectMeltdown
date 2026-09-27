using PrimeTween;
using UnityEngine;

namespace Root
{
    public class DeliveryConfirmationButton : InteractableNormalCamera
    {
        [SerializeField] PackageDeliverPost packagePost;

        [Header("Animation Settings")]
        [SerializeField] private Vector3 maximumRotation = new Vector3(45f, 0f, 0f);
        [SerializeField] private float returnMaximumDuration = 0.5f;

        private bool _hasConfirmedInteraction;
        private bool _isAnimating;

        private void Awake()
        {
            packagePost.OnPackagesDelivered += SetConfirmationButtonStatus;
        }

        public override void Interact() 
        {
            if (_isAnimating) return;

            if (_hasConfirmedInteraction) return;

            StartLeverAnimation(maximumRotation, returnMaximumDuration);

            ConfirmDelivery();
        }

        private void StartLeverAnimation(Vector3 rotationAngle, float returnDuration)
        {
            _isAnimating = true;
            Tween.LocalRotation(
            target: transform,
            startValue: transform.localRotation,
            endValue: Quaternion.Euler(rotationAngle),
            duration: returnDuration,
            ease: Ease.OutQuad,
            cycles: 2,
            cycleMode: CycleMode.Yoyo
            ).OnComplete(() => _isAnimating = false);
        }

        private void SetConfirmationButtonStatus(bool algo)
        {
            _hasConfirmedInteraction = algo;
        }

        public void ConfirmDelivery()
        {
            if (!packagePost.DepositedPackages()) return;
            
            packagePost.CheckGoal();
        }
        private void OnDestroy()
        {
            packagePost.OnPackagesDelivered -= SetConfirmationButtonStatus;
        }
    }
}
