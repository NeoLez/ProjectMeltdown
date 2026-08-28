using Root.Controller;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Root {
    public class PlayerInventoryUI : MonoBehaviour {
        [SerializeField] InventoryDisplay playerInventory;
        [SerializeField] InventoryDisplay otherInventory;
        private bool playerInventoryInitialized;
        private bool inventoryOpen;
        private void Awake() {
            GameManager.PlayerInventoryUI = this;
            GameManager.Input.Inventory.InventoryToggle.performed += InventoryToggle;
        }

        private void InventoryToggle(InputAction.CallbackContext _) {
            if (inventoryOpen) {
                CloseInventory();
            }
            else {
                OpenPlayerInventory();
            }
        }

        public void OpenPlayerInventory() {
            if (inventoryOpen) return;
            if (!playerInventoryInitialized) {
                playerInventory.LoadInventory(GameManager.Player.GetComponent<Inventory>());
                playerInventoryInitialized = true;
            }
            playerInventory.gameObject.SetActive(true);
            MouseHandler.RequestControl(CursorLockMode.Confined, true, this);
            GameManager.Input.Movement.Disable();
            GameManager.Input.CameraMovement.Disable();
            GameManager.Input.Interaction.Disable();
            inventoryOpen = true;
        }

        public void CloseInventory() {
            playerInventory.gameObject.SetActive(false);
            otherInventory.gameObject.SetActive(false);
            MouseHandler.RelinquishControl(this);
            GameManager.Input.Movement.Enable();
            GameManager.Input.CameraMovement.Enable();
            GameManager.Input.Interaction.Enable();
            inventoryOpen = false;
        }

        public void OpenInventory(Inventory inventory) {
            if (inventoryOpen) return;
            otherInventory.LoadInventory(inventory);
            if (!playerInventoryInitialized) {
                playerInventory.LoadInventory(GameManager.Player.GetComponent<Inventory>());
                playerInventoryInitialized = true;
            }
            playerInventory.gameObject.SetActive(true);
            otherInventory.gameObject.SetActive(true);
            MouseHandler.RequestControl(CursorLockMode.Confined, true, this);
            GameManager.Input.Movement.Disable();
            GameManager.Input.CameraMovement.Disable();
            GameManager.Input.Interaction.Disable();
            inventoryOpen = true;
        }
    }
}