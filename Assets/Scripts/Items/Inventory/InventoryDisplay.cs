using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Root {
    public class InventoryDisplay : MonoBehaviour, IItemDragReceiver {
        public Inventory inventory;
        public RectTransform slotPanelPrefab;
        public RectTransform inventoryBackground;
        private readonly List<RectTransform> _slots = new();
        private Vector2Int _maxDimensions;
        private readonly Dictionary<InventoryItem, InventoryItemDisplay> _itemToItemDisplay = new();
        [SerializeField] private float dragSmoothing;

        private void Awake() {
            GameManager.Input.Inventory.DropItemModifier.performed += HandleMoveToHand;
        }

        private void HandleMoveToHand(InputAction.CallbackContext _) {
            if (!gameObject.activeInHierarchy || inventory == null) return;
            var playerItemHolder = GameManager.Player.GetComponent<PlayerItemHolder>();
            if (playerItemHolder.HasItem) return;
            if (!inventory.RemoveItem(MousePositionToSlotCoords(Pointer.current.position.value), out var item)) return;

            playerItemHolder.Pickup(item.itemState);
        }
        
        public void LoadInventory(Inventory inv) {
            if (inventory != null) {
                inventory.OnItemAdded -= HandleOnItemAdded;
                inventory.OnItemRemoved -= HandleOnItemRemoved;
            }

            inventory = inv;
            inventory.OnItemAdded += HandleOnItemAdded;
            inventory.OnItemRemoved += HandleOnItemRemoved;

            Refresh();
        }
        
        private void HandleOnItemAdded(InventoryItem item, Vector2Int position) {
            var obj = Instantiate(item.itemState.ItemSo.InventoryItemPrefab, transform);
            _itemToItemDisplay.Add(item, obj);
            obj.Initialize(item, item.itemState.ItemSo.InventoryItemIcon, item.Size, (Vector2)position * slotPanelPrefab.sizeDelta.x, item.rotation);
        }
        
        private void HandleOnItemRemoved(InventoryItem item) {
            _itemToItemDisplay.Remove(item, out var obj);
            Destroy(obj.gameObject);
        }

        public void Refresh() {
            _maxDimensions = Vector2Int.zero;
            if (inventory == null) return;
            
            foreach (var slot in _slots) {
                Destroy(slot.gameObject);
            }
            _slots.Clear();
            foreach (var displayItem in _itemToItemDisplay.Values) {
                Destroy(displayItem.gameObject);
            }
            _itemToItemDisplay.Clear();

            Generate();
            
            foreach (var item in inventory.GetItems()) {
                HandleOnItemAdded(item, item._position);
            }
        }

        private void Generate()
        {
            foreach (var position in inventory.GetInventorySlotPositions())
            {
                var slot = Instantiate(slotPanelPrefab, transform);
                _maxDimensions.x = math.max(_maxDimensions.x, position.x);
                _maxDimensions.y = math.max(_maxDimensions.y, position.y);
                slot.anchoredPosition += new Vector2(position.x, position.y) * slotPanelPrefab.sizeDelta.x;
                _slots.Add(slot);
            }
            inventoryBackground.sizeDelta = new Vector2((_maxDimensions.x + 1) * _slots[0].sizeDelta.x, (_maxDimensions.y + 1) * _slots[0].sizeDelta.y);
        }

        private Vector2Int MousePositionToSlotCoords(Vector2 mousePosition) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(inventoryBackground, mousePosition, null, out var relativePos);
            relativePos /= slotPanelPrefab.sizeDelta.x;
            return new Vector2Int((int)math.floor(relativePos.x), (int)math.floor(relativePos.y));
        }

        public bool CanTakeItem(Vector2 position, Vector2Int size, InventoryItem item) {
            return inventory.IsAreaFree(size, MousePositionToSlotCoords(position), item);
        }

        public bool TakeItem(Vector2 position, InventoryItem.InventoryItemRotation rotation, InventoryItem item) {
            return inventory.InsertItem(item.itemState, MousePositionToSlotCoords(position), rotation);
        }
    }
}