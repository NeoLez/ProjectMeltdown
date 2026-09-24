using UnityEngine;

namespace Root
{
    public class DeliveryConfirmationButton : InteractableNormalCamera
    {
        [SerializeField] PackageDeliverPost packagePost;

        private bool _hasConfirmedInteraction;

        private void Awake()
        {
            packagePost.OnPackagesDelivered += SetConfirmationButtonStatus;
        }

        public override void Interact() 
        {
            ConfirmDelivery();
        }

        private void SetConfirmationButtonStatus(bool algo)
        {
            _hasConfirmedInteraction = algo;
        }

        public void ConfirmDelivery()
        {
            if (_hasConfirmedInteraction) return;
            if (packagePost.DepositedPackages() <= 0) return;
             //aca chequear si la mision fue activada ademas ocualquiera
            packagePost.CheckGoal();
        }
        private void OnDestroy()
        {
            packagePost.OnPackagesDelivered -= SetConfirmationButtonStatus;
        }
    }
}
