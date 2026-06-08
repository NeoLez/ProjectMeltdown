using System;
using UnityEngine;

namespace Root
{
    public class StoreItemDisplay : InteractableNormalCamera
    {
        private StoreItemData _data;
        private int _price;
        [SerializeField] public bool _purchased;
        private PriceCanvas _priceCanvas;
        public MerchantHand _storeHand;
        [SerializeField] private Transform _storeItemPivot;
        public event Action<MerchantHand, StoreItemDisplay> OnPurchased;

        public void Initialize(StoreItemData data, int price, PriceCanvas priceCanvas)
        {
            _data = data;
            _price = price;
            _priceCanvas = priceCanvas;
        }

        private void Update() {
            if (!_purchased) {
                transform.rotation = _storeHand.objectPivot.rotation;
                transform.position = _storeHand.objectPivot.transform.position + (_storeItemPivot.position - transform.position);
            }
        }

        public override void Interact()
        {   
            if (_purchased) return;

            if (!EconomyManager.Instance.SpendMoney(_price))
            {
                Debug.Log("No hay plata");
                return;
            }
            Debug.Log("Bought");
            _purchased = true;
            OnPurchased?.Invoke(_storeHand, this);
            
            transform.GetChild(0).GetComponent<Rigidbody>().isKinematic = false;
            
            if (_priceCanvas != null)
                _priceCanvas.Hide();
        }
    }
}