using PrimeTween;
using UnityEngine;

namespace Root
{
    public class DeliveryConfirmationButton : InteractableNormalCamera
    {
        [SerializeField] PackageDeliverPost packagePost;

        [Header("Animation Settings")]
        [SerializeField] private Vector3 maximumRotation = new Vector3(45f, 0f, 0f);
        [SerializeField] private float returnDuration = 0.5f;

        private bool _hasConfirmedInteraction;
        private bool isAnimating;
        private void Awake()
        {
            packagePost.OnPackagesDelivered += SetConfirmationButtonStatus;
        }

        public override void Interact() 
        {
            if (packagePost.DepositedPackages() <= 0) return;
            if (isAnimating) return;

            isAnimating = true;
            Tween.Rotation(
            target: transform,
            endValue: Quaternion.Euler(maximumRotation),
            duration: returnDuration,
            ease: Ease.OutQuad,
            cycles: 2,
            cycleMode: CycleMode.Yoyo
            ).OnComplete(() => isAnimating = false);

            ConfirmDelivery();
        }

        private void SetConfirmationButtonStatus(bool algo)
        {
            _hasConfirmedInteraction = algo;
        }

        public void ConfirmDelivery()
        {
            if (_hasConfirmedInteraction) return;
             //aca chequear si la mision fue activada ademas ocualquiera
            packagePost.CheckGoal();
        }
        private void OnDestroy()
        {
            packagePost.OnPackagesDelivered -= SetConfirmationButtonStatus;
        }
    }
}
