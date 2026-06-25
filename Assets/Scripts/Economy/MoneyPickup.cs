using UnityEngine;

namespace Root
{
    public class MoneyPickup : InteractableNormalCamera
    {
        [SerializeField] private int amount = 50;

        public override void Interact()
        {
            EconomyManager.Instance.AddMoney(amount);
            MoneyFeedback.Instance.GrabbedBill();
            Destroy(gameObject);
        }
    }
}