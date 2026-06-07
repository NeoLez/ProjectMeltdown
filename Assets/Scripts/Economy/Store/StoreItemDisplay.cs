using UnityEngine;

namespace Root
{
    public class StoreItemDisplay : InteractableNormalCamera
    {
        private StoreItemData _data;
        private int _price;
        private bool _purchased;
        private PriceCanvas _priceCanvas;

        public bool IsPurchased => _purchased;

        public void Initialize(StoreItemData data, int price, PriceCanvas priceCanvas)
        {
            _data = data;
            _price = price;
            _priceCanvas = priceCanvas;
        }

        public override void Interact()
        {
            if (_purchased) return;

            if (!EconomyManager.Instance.SpendMoney(_price))
            {
                Debug.Log("No hay plata");
                return;
            }

            _purchased = true;

            if (_priceCanvas != null)
                _priceCanvas.Hide();
        }
    }
}