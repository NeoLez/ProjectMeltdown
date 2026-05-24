using UnityEngine;

namespace Root
{
    public class MoneyPickup : Interactable
    {
        [SerializeField] private int amount = 50;

        public override void Interact(bool state)
        {
            if (!state)
                return;

            EconomyManager.Instance.AddMoney(amount);

            Destroy(gameObject);
        }
    }
}