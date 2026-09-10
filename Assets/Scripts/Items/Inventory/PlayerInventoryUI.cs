using System;
using Root.Controller;
using Root.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Root {
    public class PlayerInventoryUI : Menu.Menu {
        [SerializeField] InventoryDisplay playerInventoryDisplay;
        [SerializeField] InventoryDisplay otherInventoryDisplay;
        [SerializeField] HandHeldInventorySlot handHeldInventorySlot;
        private Inventory _otherInventory;
        private bool playerInventoryInitialized;
        private bool inventoryOpen;
        private void Awake() {
            GameManager.PlayerInventoryUI = this;
            GameManager.Input.Inventory.InventoryToggle.performed += InventoryToggle;
        }

        private void InventoryToggle(InputAction.CallbackContext _) {
            if (inventoryOpen) {
                UIManager.Instance.CloseMenu(UIManager.UITypes.Inventory);
            }
            else {
                OpenInventory();
            }
        }

        public void CloseInventory() {
            if (!inventoryOpen) return;
            UIManager.Instance.CloseMenu(UIManager.UITypes.Inventory);
        }

        public void OpenInventory(Inventory inventory = null) {
            _otherInventory = inventory;
            if (inventoryOpen) return;
            UIManager.Instance.OpenMenu(UIManager.UITypes.Inventory);
        }

        public override void Open() {
            base.Open();
            ShowDisplays();
        }

        public override void Close() {
            base.Close();
            HideDisplays();
        }

        private void ShowDisplays() {
            if (_otherInventory != null) {
                otherInventoryDisplay.LoadInventory(_otherInventory);
                otherInventoryDisplay.gameObject.SetActive(true);
            }
            
            handHeldInventorySlot.gameObject.SetActive(true);
            handHeldInventorySlot.UpdateVisuals();
            
            if (!playerInventoryInitialized) {
                playerInventoryDisplay.LoadInventory(GameManager.Player.GetComponent<Inventory>());
                playerInventoryInitialized = true;
            }
            playerInventoryDisplay.gameObject.SetActive(true);
            
            MouseHandler.RequestControl(CursorLockMode.Confined, true, this);
            GameManager.Input.Movement.Disable();
            GameManager.Input.CameraMovement.Disable();
            GameManager.Input.Interaction.Disable();
            inventoryOpen = true;
        }

        private void HideDisplays() {
            playerInventoryDisplay.gameObject.SetActive(false);
            handHeldInventorySlot.gameObject.SetActive(false);
            otherInventoryDisplay.gameObject.SetActive(false);
            MouseHandler.RelinquishControl(this);
            GameManager.Input.Movement.Enable();
            GameManager.Input.CameraMovement.Enable();
            GameManager.Input.Interaction.Enable();
            inventoryOpen = false;
        }

        private void OnDestroy() {
            GameManager.Input.Inventory.InventoryToggle.performed -= InventoryToggle;
        }
    }
}