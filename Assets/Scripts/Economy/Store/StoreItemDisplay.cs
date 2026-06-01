using TMPro;
using UnityEngine;

namespace Root
{
    public class StoreItemDisplay : InteractableNormalCamera
    {
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private GameObject priceCanvas;

        [SerializeField] private float showDistance = 3f;

        private StoreItemData _data;
        private int _price;
        private bool _purchased;

        public void Initialize(StoreItemData data, int price)
        {
            _data = data;
            _price = price;

            if (priceText != null)
                priceText.text = "$" + price;
        }

        private void Update()
        {
            if (_purchased)
                return;

            if (GameManager.Player == null || priceCanvas == null)
                return;

            float distance = Vector3.Distance(
                transform.position,
                GameManager.Player.transform.position);

            priceCanvas.SetActive(distance <= showDistance);
        }

        public override void Interact()
        {
            if (_purchased)
                return;

            if (!EconomyManager.Instance.SpendMoney(_price))
            {
                Debug.Log("No hay suficiente dinero");
                return;
            }

            _purchased = true;

            if (priceCanvas != null)
                priceCanvas.SetActive(false);
        }
    }
}