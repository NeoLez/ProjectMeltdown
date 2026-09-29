using System;
using UnityEngine;

namespace Root
{
    public class StoreItemDisplay : InteractableNormalCamera
    {
        [NonSerialized] public bool _purchased = true;
        [SerializeField] private Transform _storeItemPivot;
        private StoreItemData _data;
        private int _price;
        private PriceCanvas _priceCanvas;
        public MerchantHand _storeHand;

        public event Action<MerchantHand, StoreItemDisplay> OnPurchased;

        public event Action OnSingleItemBought;
        public Action OnShowPrice;
        public Action OnHidePrice;

        private void Awake()
        {
            OnInteraction += Interaction;
        }

        private void OnDestroy()
        {
            OnInteraction -= Interaction;
        }

        public void Initialize(StoreItemData data, int price, PriceCanvas priceCanvas)
        {
            _data = data;
            _price = price;
            _priceCanvas = priceCanvas;
        }

        private void Update() {
            if (!_purchased && _storeHand != null) {
                transform.rotation = _storeHand.objectPivot.rotation;
                transform.position = _storeHand.objectPivot.transform.position + (_storeItemPivot.position - transform.position);
            }
        }

        public void Interaction()
        {   
            if (_purchased) return;

            NotificationManager.Instance.ShowNotification($"You bought {_data.item.ItemName}");
            _purchased = true;
            OnPurchased?.Invoke(_storeHand, this);
            OnSingleItemBought?.Invoke();

            transform.GetComponent<Rigidbody>().isKinematic = false;
            
            if (_priceCanvas != null)
                _priceCanvas.Hide();
        }

        public bool CanPurchase()
        {
            if(!EconomyManager.Instance.SpendMoney(_price))
            {
                NotificationManager.Instance.ShowNotification("You don't have enough money");
                return false;
            }
            return true;
        }

        public void SetNotPurchased() {
            _purchased = false;
        }

        public override void Interact()
        {
            throw new NotImplementedException();
        }
    }
}