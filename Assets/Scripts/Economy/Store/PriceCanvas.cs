using TMPro;
using UnityEngine;

namespace Root
{
    public class PriceCanvas : MonoBehaviour
    {
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private float showDistance = 3f;

        private StoreItemDisplay itemToDisplay;
        private Canvas _canvas;
        //private bool _hidden;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;
        }

        public void Initialize(int price, StoreItemDisplay item)
        {
            if (priceText != null)
                priceText.text = "$" + price;

            if (item != null)
            {
                itemToDisplay = item;
                itemToDisplay.OnShowPrice += Show;
                itemToDisplay.OnHidePrice += Hide;
            }
        }

        public void Show()
        {
            //_hidden = false;
            _canvas.enabled = true;
        }

        public void Hide()
        {
            //_hidden = true;
            _canvas.enabled = false;
        }

        private void OnDestroy()
        {
            itemToDisplay.OnShowPrice -= Show;
            itemToDisplay.OnHidePrice -= Hide;
        }

        /* private void Update()
         {
             if (_hidden) return;
             if (GameManager.Player == null) return;

             float distance = Vector3.Distance(
                 transform.position,
                 GameManager.Player.transform.position);

             _canvas.enabled = distance <= showDistance;
         }*/
    }
}