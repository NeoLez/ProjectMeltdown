using Root.Controller;
using Root.Managers;
using TMPro;
using UnityEngine;

namespace Root
{
    public class StoreMenu : MonoBehaviour
    {
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private SelllDeposit deposit;
        [SerializeField] private StoreBuyBell storeTrigger;
        [SerializeField] private Canvas walletDisplay;
        [SerializeField] private UnityEngine.UI.Button[] storeButtons;
        [SerializeField] private UnityEngine.UI.Button returnButton;

        [Header("Buy UI")]
        [SerializeField] private Canvas storePanel;

        [Header("Sell UI")]
        [SerializeField] private Canvas priceDisplay;
        [SerializeField] private TMP_Text priceCounter;
        [SerializeField] private UnityEngine.UI.Button confirmButton;

        [SerializeField] private Animator _animator;

        private void Awake()
        {            
            storeButtons[0].onClick.AddListener(() =>
            {
                ShowDisplays();
            });

            storeButtons[1].onClick.AddListener(() => 
            {
                HideDisplays();

                deposit.LoadDisplay();
            });

            confirmButton.onClick.AddListener(deposit.ConfirmSell);
            returnButton.onClick.AddListener(CloseAllUI);

            deposit.OnPriceUpdated += RefreshSumAmount;
            deposit.OnItemUpdated += ToggleConfirmButton;
            deposit.OnItemUpdated += ToggleReturnButton;
        }


        private void Start()
        {
            mainCanvas.gameObject.SetActive(false);

            EnableSellCanvas(false);
            EnableWalletCanvas(false);
            EnableBuyCanvas(false);
        }

        public void ShowDisplays()
        {
            Open();
        }

        public void HideDisplays()
        {
            Close();
        }

        public void Open()
        {
            mainCanvas.gameObject.SetActive(true);
            EnableBuyCanvas(true);
            EnableSellCanvas(false);
            EnableWalletCanvas(true);
            MouseHandler.RequestControl(CursorLockMode.Confined, true, this);

            GameManager.Input.Movement.Disable();
            GameManager.Input.CameraMovement.Disable();
            GameManager.Input.Inventory.InventoryToggle.Disable();
            GameManager.Input.Inventory.AlternativeCloseInventory.Disable();
        }

        public void Close()
        {
            EnableBuyCanvas(false);
            EnableSellCanvas(true);
            MouseHandler.RelinquishControl(this);       
        }

        private void CloseAllUI()
        {
            if (deposit.AreUnconfirmedItems()) return;

            storeTrigger.CloseStore();
            EnableBuyCanvas(false);
            EnableSellCanvas(false);
            EnableWalletCanvas(false);

            MouseHandler.RelinquishControl(this);

            GameManager.PlayerInventoryUI.CloseInventory();
            GameManager.Input.Movement.Enable();
            GameManager.CameraController.EnableNormalMovement();
            GameManager.Input.CameraMovement.Enable();
            GameManager.Input.Inventory.InventoryToggle.Enable();

            mainCanvas.gameObject.SetActive(false);
        }

        private void ToggleConfirmButton()
        {
            confirmButton.interactable = deposit.AreUnconfirmedItems();
            storeButtons[0].interactable = !deposit.AreUnconfirmedItems();
        }
        private void ToggleReturnButton()
        {
            returnButton.interactable = !deposit.AreUnconfirmedItems();
        }

        public void EnableBuyCanvas(bool state)
        {
            _animator.Play("UIhands1");
            storePanel.enabled = state;
        }

        public void EnableWalletCanvas(bool state)
        {
            RefreshWalletAmount();
            walletDisplay.enabled = state;
        }

        public void EnableSellCanvas(bool state)
        {
            _animator.Play("UIhands2");
            priceDisplay.enabled = state;
        }


        private void RefreshSumAmount()
        {
            int amount = deposit.GetCurrentAmount();
            priceCounter.text = string.Format("{0}$", amount);
        }

        public void RefreshWalletAmount()
        {
            int amount = EconomyManager.Instance.GetMoney();
            TMP_Text moneyText =  walletDisplay.GetComponentInChildren<TextMeshProUGUI>();
            moneyText.text = string.Format("{0}$", amount);
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
