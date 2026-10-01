using Root.Controller;
using Root.Managers;
using System;
using TMPro;
using UnityEngine;

namespace Root
{
    public class StoreMenu : Menu.Menu 
    {
        [SerializeField] private StoreManager storeManager;
        [SerializeField] private SelllDeposit deposit;
        [SerializeField] private MerchantTrigger _trigger;

        [Header("Buy UI")]
        [SerializeField] private Canvas choiseCanvas;
        [SerializeField] private UnityEngine.UI.Button[] storeButtons;

        [Header("Sell UI")]
        [SerializeField] private Canvas priceDisplay;
        [SerializeField] private TMP_Text priceCounter;
        [SerializeField] private UnityEngine.UI.Button confirmButton;
        [SerializeField] private UnityEngine.UI.Button returnButton;

        private void Awake()
        {
            storeButtons[0].onClick.AddListener(() =>
            {
                EnableChoiceCanvas(false);

                storeManager.GenerateStoreItems();
                _trigger.CanShowItems(true);

                MouseHandler.RelinquishControl(this);
                GameManager.Input.Movement.Enable();
                GameManager.Input.CameraMovement.Enable();

                UIManager.Instance.CloseMenu(UIManager.UITypes.Store);
            });

            storeButtons[1].onClick.AddListener(() => 
            {
                EnableChoiceCanvas(false);

                deposit.LoadDisplay();
                EnableSellInventoryCanvas(true);
                GameManager.Input.Inventory.InventoryToggle.Disable();
                UIManager.Instance.CloseMenu(UIManager.UITypes.Store);
                GameManager.PlayerInventoryUI.OpenInventory(GetComponent<Inventory>());
            });

            confirmButton.onClick.AddListener(deposit.ConfirmSell);
            returnButton.onClick.AddListener(CloseAllUI);

            deposit.OnPriceUpdated += RefreshSumAmount;
            deposit.OnItemUpdated += ToggleConfirmButton;
            deposit.OnItemUpdated += ToggleReturnButton;
        }


        private void Start()
        {
            if(UIManager.Instance.storeMenu == null)
            {
                UIManager.Instance.storeMenu = this;
            }

            EnableChoiceCanvas(false);
            EnableSellInventoryCanvas(false);
        }

        public override void Open()
        {
            _trigger.CanShowItems(false);
            EnableChoiceCanvas(true);

            MouseHandler.RequestControl(CursorLockMode.Confined, true, this);

            GameManager.Input.Movement.Disable();
            GameManager.Input.CameraMovement.Disable();

            base.Open();
        }

        public override void Close()
        {
            EnableChoiceCanvas(false);

            base.Close();        
        }

        private void CloseAllUI()
        {
            if (deposit.AreUnconfirmedItems()) return;

            _trigger.IsStoreOpened = false;
            EnableSellInventoryCanvas(false);
            MouseHandler.RelinquishControl(this);

            GameManager.PlayerInventoryUI.CloseInventory();
            GameManager.Input.Movement.Enable();
            GameManager.Input.CameraMovement.Enable();
            GameManager.Input.Inventory.InventoryToggle.Enable();
        }

        private void ToggleConfirmButton()
        {
            confirmButton.interactable = deposit.AreUnconfirmedItems();
        }
        private void ToggleReturnButton()
        {
            returnButton.interactable = !deposit.AreUnconfirmedItems();
        }

        private void EnableChoiceCanvas(bool state)
        {
            choiseCanvas.enabled = state;
        }

        public void EnableSellInventoryCanvas(bool enable)
        {
            priceDisplay.enabled = enable;
        }

        private void RefreshSumAmount()
        {
            var amount = deposit.GetCurrentAmount();
            priceCounter.text = string.Format("{0}$", amount);
        }

        private void OnDestroy()
        {
            storeButtons[0].onClick.RemoveAllListeners();
            storeButtons[1].onClick.RemoveAllListeners();

            confirmButton.onClick.RemoveAllListeners();
            returnButton.onClick.RemoveAllListeners();

            deposit.OnPriceUpdated -= RefreshSumAmount;
            deposit.OnItemUpdated -= ToggleConfirmButton;
            deposit.OnItemUpdated -= ToggleReturnButton;
        }
    }
}
