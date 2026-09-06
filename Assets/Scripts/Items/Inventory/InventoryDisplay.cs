using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Root {
    public class InventoryDisplay : MonoBehaviour, IItemDragReceiver {
        public Inventory inventory;
        public RectTransform slotPanelPrefab;
        public RectTransform inventoryBackground;
        private readonly Dictionary<Vector2Int,RectTransform> _slots = new();
        private Vector2Int _maxDimensions;
        private readonly Dictionary<InventoryItem, InventoryItemDisplay> _itemToItemDisplay = new();
        [SerializeField] private float dragSmoothing;
        private float _cellSize;
        private float _initialSlotOpacity;
        [SerializeField] private Color slotFreeColor = Color.green;
        [SerializeField] private Color slotObstructedColor = Color.red;

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
                Destroy(slot.Value.gameObject);
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

        private void Generate() {
            var slotPositions = inventory.GetInventorySlotPositions();
            
            foreach (var position in slotPositions)
            {
                var slot = Instantiate(slotPanelPrefab, transform);
                _maxDimensions.x = math.max(_maxDimensions.x, position.x);
                _maxDimensions.y = math.max(_maxDimensions.y, position.y);
                slot.anchoredPosition += new Vector2(position.x, position.y) * slotPanelPrefab.sizeDelta.x;
                _slots.Add(position, slot);
            }

            var s = _slots.Values.First();
            _cellSize = s.sizeDelta.x;
            _initialSlotOpacity = s.GetComponent<Image>().color.a;
            slotFreeColor.a = _initialSlotOpacity;
            slotObstructedColor.a = _initialSlotOpacity;
            inventoryBackground.sizeDelta = new Vector2((_maxDimensions.x + 1) * _cellSize, (_maxDimensions.y + 1) * _cellSize);
        }

        private Vector2Int MousePositionToSlotCoords(Vector2 mousePosition) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(inventoryBackground, mousePosition, null, out var relativePos);
            relativePos /= slotPanelPrefab.sizeDelta.x;
            return new Vector2Int((int)math.floor(relativePos.x), (int)math.floor(relativePos.y));
        }

        private readonly HashSet<Vector2Int> _positionsChanged = new();

        public bool CanTakeItem(Vector2 position, Vector2Int size, InventoryItem item) {
            ClearFeedback();
            var positionSlotCoords = MousePositionToSlotCoords(position);
            for (int x = 0; x < size.x; x++) {
                for (int y = 0; y < size.y; y++) {
                    var pos = new Vector2Int(x + positionSlotCoords.x, y + positionSlotCoords.y);
                    if (pos.x < 0 || pos.x >= inventory.Size.x || pos.y < 0 || pos.y >= inventory.Size.y) continue;
                    _slots[pos].GetComponent<Image>().color = slotFreeColor;
                    _positionsChanged.Add(pos);
                }
            }

            if (inventory.IsAreaFree(size, MousePositionToSlotCoords(position), out var overlaps, item)) {
                return true;
            }

            foreach (var overlap in overlaps) {
                _slots[overlap].GetComponent<Image>().color = slotObstructedColor;
                _positionsChanged.Add(overlap);
            }
            return false;
        }

        public bool TakeItem(Vector2 position, InventoryItem.InventoryItemRotation rotation, InventoryItem item) {
            return inventory.InsertItem(item.itemState, MousePositionToSlotCoords(position), rotation);
        }

        public void ClearFeedback() {
            var color = Color.white;
            color.a = _initialSlotOpacity;
            foreach (var pos in _positionsChanged) {
                _slots[pos].GetComponent<Image>().color = color;
            }
        }
    }
}