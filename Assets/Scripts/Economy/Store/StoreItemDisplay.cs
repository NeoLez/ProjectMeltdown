using Root.Factories;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Root
{
    public class StoreItemDisplay : MonoBehaviour
    {
        [NonSerialized] public bool _purchased = true;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image itemIcon;
        [SerializeField] private float buttonAlpha = 0.2f;

        private StoreItemData _data;
        private int _price;
        private string _name;
        private Sprite _itemIcon;
        private Button _button;
        private string label = "${0}";

        public event Action<StoreItemDisplay, StoreItemData> OnPurchased;
        public event Action OnSingleItemBought;


         private void Awake()
         {
             _button = GetComponentInChildren<Button>();
             _button.onClick.AddListener(Interaction);
         }

        public void Initialize(StoreItemData data, int price)
        {
            _data = data;
            _price = price;
            _name = data.item.ItemName;
            _itemIcon = data.item.InventoryItemIcon;

            if(_button != null)
            {
                _button.interactable = HasEnoughMoney() || _purchased;
            }

            SetDisplay();
        }

        public void RefreshItems()
        {
            if (_purchased) return;
            _button.interactable = HasEnoughMoney() || _purchased;
            SetAlpha(_purchased);
        }

        private void SetDisplay()
        {
            nameText.text = _name;
            priceText.text = string.Format(label, _price.ToString());
            itemIcon.sprite = _itemIcon;
        }

        public void Interaction()
        {
            if (!CanPurchase()) return;
            if (_purchased) return;

            NotificationManager.Instance.ShowNotification($"Compraste {_data.item.ItemName}");
            _purchased = true;
            OnPurchased?.Invoke(this, _data);
            OnSingleItemBought?.Invoke();

            SetAlpha(false);
            _button.interactable = false;
            _button.onClick.RemoveAllListeners();
        }

        private bool HasEnoughMoney()
        {
            if (EconomyManager.Instance.GetMoney() < _price)
            {
                return false;
            }
            return true;

        }

        public bool CanPurchase()
        {
            if(!EconomyManager.Instance.SpendMoney(_price))
            {
                NotificationManager.Instance.ShowNotification($"No tenes dinero suficiente");
                return false;
            }
            return true;
        }

        public void SetAlpha(bool state)
        {
            CanvasGroup alpha = GetComponentInChildren<CanvasGroup>();
            alpha.alpha = Mathf.Clamp01(buttonAlpha);
            alpha.interactable = state;
            alpha.blocksRaycasts = !state;
        }

        public void SetNotPurchased() {
            _purchased = false;
        }

    }
}