using Root.Controller;
using Root.Managers;
using UnityEngine;

namespace Root
{
    public class StoreMenu : Menu.Menu 
    {
        [SerializeField] StoreManager _storeManager;
        [SerializeField] MerchantTrigger _trigger;
        [SerializeField] Canvas choiseCanvas;
        [SerializeField] UnityEngine.UI.Button[] storeButtons;

        private void Awake()
        {
            storeButtons[0].onClick.AddListener(() =>
            {
                _storeManager.GenerateStoreItems();
                _trigger.HandleStore(true);
                MouseHandler.RelinquishControl(this);
                GameManager.Input.Movement.Enable();
                GameManager.Input.CameraMovement.Enable();
                EnableCanvas(false);

                UIManager.Instance.CloseMenu(UIManager.UITypes.Store);
            });

            storeButtons[1].onClick.AddListener(() => 
            {
                _storeManager.SellItems();
                MouseHandler.RelinquishControl(this);
                EnableCanvas(false);

                UIManager.Instance.CloseMenu(UIManager.UITypes.Store);
            });
        }


        private void Start()
        {
            if(UIManager.Instance.storeMenu == null)
            {
                UIManager.Instance.storeMenu = this;
            }
        }

        public override void Open()
        {
            EnableCanvas(true);

            MouseHandler.RequestControl(CursorLockMode.Confined, true, this);

            GameManager.Input.Movement.Disable();
            GameManager.Input.CameraMovement.Disable();

            base.Open();
        }

        public override void Close()
        {
            EnableCanvas(false);
            base.Close();        
        }

        private void EnableCanvas(bool state)
        {
            choiseCanvas.enabled = state;
        }

    }
}
