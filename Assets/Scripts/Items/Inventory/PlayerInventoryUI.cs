using System.Collections.Generic;
using Root.Controller;
using Root.Managers;
using Timers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Root {
    public class PlayerInventoryUI : Menu.Menu {
        public static PlayerInventoryUI Instance { get; private set; }
        [SerializeField] InventoryDisplay playerInventoryDisplay;
        [SerializeField] InventoryDisplay otherInventoryDisplay;
        [SerializeField] HandHeldInventorySlot handHeldInventorySlot;
        [SerializeField] private ItemActionButton itemActionButtonPrefab;
        [SerializeField] private RectTransform itemActionsTitlePrefab;
        private Inventory _otherInventory;
        private bool playerInventoryInitialized;
        private bool inventoryOpen;
        private void Awake() {
            GameManager.PlayerInventoryUI = this;
            GameManager.Input.Inventory.InventoryToggle.performed += InventoryToggle;
            GameManager.Input.Inventory.AlternativeCloseInventory.performed += InventoryClose;
            Instance = this;
        }

        private void InventoryToggle(InputAction.CallbackContext _) {
            if (inventoryOpen) {
                UIManager.Instance.CloseMenu(UIManager.UITypes.Inventory);
            }
            else {
                OpenInventory();
            }
        }

        private void InventoryClose(InputAction.CallbackContext val) {
            
            if(inventoryOpen)
                InventoryToggle(val);
        }

        public void CloseInventory() {
            if (!inventoryOpen) return;
            UIManager.Instance.CloseMenu(UIManager.UITypes.Inventory);
        }

        public void OpenInventory(Inventory inventory = null) {
            if (inventoryOpen) return;
            _otherInventory = inventory;
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
            GameManager.Input.Inventory.AlternativeCloseInventory.Enable();
            inventoryOpen = true;
        }

        private void HideDisplays() {
            playerInventoryDisplay.gameObject.SetActive(false);
            handHeldInventorySlot.gameObject.SetActive(false);
            otherInventoryDisplay.gameObject.SetActive(false);
            ClearOptionsDialogue();
            MouseHandler.RelinquishControl(this);
            GameManager.Input.Movement.Enable();
            GameManager.Input.CameraMovement.Enable();
            GameManager.Input.Interaction.Enable();
            GameManager.Input.Inventory.AlternativeCloseInventory.Disable();
            inventoryOpen = false;
        }

        public bool IsQuickTransferPossible(Inventory inventory, out Inventory destination) {
            destination = null;
            if (_otherInventory == null) return false;
            
            if (inventory == playerInventoryDisplay.inventory) {
                destination = _otherInventory;
                return true;
            }
            
            if (inventory == _otherInventory) {
                destination = playerInventoryDisplay.inventory;
                return true;
            }

            return false;
        }

        private readonly List<ItemActionButton> _actionButtons = new();
        private RectTransform _titleGameObject;
        private Component _component;
        public void DisplayOptionsDialogue(List<ItemAction> actions, ItemSo itemSo, PointerEventData eventData, Component component) {
            ClearOptionsDialogue();

            _component = component;
            if (!UIUtility.ScreenToCanvasPosition(GetComponent<Canvas>(), GetComponent<RectTransform>(),
                    eventData.position, out Vector2 offset)) return;
            offset = GetOptionsDialogueAnchorPoint(actions.Count, offset);
            actions.Reverse();
            foreach (var action in actions) {
                var button = Instantiate(itemActionButtonPrefab, transform);
                button.SetAction(action);
                var rectTransform = button.GetComponent<RectTransform>();
                var pos = offset;
                rectTransform.anchoredPosition = pos;

                offset.y += rectTransform.sizeDelta.y;
                _actionButtons.Add(button);
            }
            
            _titleGameObject = Instantiate(itemActionsTitlePrefab, transform);
            _titleGameObject.GetComponent<RectTransform>().anchoredPosition = offset;
            _titleGameObject.GetComponentInChildren<TMP_Text>().text = itemSo.ItemName;
        }

        private Vector2 GetOptionsDialogueAnchorPoint(int amount, Vector2 clickPosition) {
            var rect = itemActionButtonPrefab.GetComponent<RectTransform>();
            var titleRect = itemActionsTitlePrefab.GetComponent<RectTransform>();
            Vector2 buttonSize = rect.sizeDelta;
            Vector2 totalSize = GetComponent<RectTransform>().sizeDelta;
            Vector2 bounds = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y * amount + titleRect.sizeDelta.y);

            if (bounds.x + clickPosition.x > totalSize.x / 2)
                clickPosition.x -= bounds.x;
            if (bounds.y + clickPosition.y > totalSize.y / 2)
                clickPosition.y -= bounds.y;
            
            
            return clickPosition;
        }

        public void ClearOptionsDialogue(Component component = null) {
            if (component != null && component != _component) return;
            
            foreach (var button in _actionButtons) {
                Destroy(button.gameObject);
            }
            _actionButtons.Clear();
            
            if (_titleGameObject != null)
                Destroy(_titleGameObject.gameObject);
        }

        private void OnDestroy() {
            GameManager.Input.Inventory.InventoryToggle.performed -= InventoryToggle;
            GameManager.Input.Inventory.AlternativeCloseInventory.performed -= InventoryClose;
        }
    }
}