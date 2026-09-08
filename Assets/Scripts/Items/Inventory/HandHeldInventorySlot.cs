using UnityEngine;

namespace Root {
    public class HandHeldInventorySlot : MonoBehaviour, IItemDragReceiver {
        private PlayerItemHolder _playerItemHolder;
        private InventoryItemDisplay _inventoryItemDisplay;
        [SerializeField] private float cellSize;
        [SerializeField] private Vector2Int minimumSize;
        [SerializeField] private RectTransform canvasTransform;

        private void OnEnable() {
            UpdateVisuals();
        }

        private void Start() {
            if(_playerItemHolder == null) _playerItemHolder = GameManager.Player.GetComponent<PlayerItemHolder>();
            _playerItemHolder.OnItemChanged += UpdateVisuals;
        }

        public void UpdateVisuals() {
            if (!gameObject.activeInHierarchy) return;
            
            if(_playerItemHolder == null) _playerItemHolder = GameManager.Player.GetComponent<PlayerItemHolder>();
            
            if(_inventoryItemDisplay != null)
                Destroy(_inventoryItemDisplay.gameObject);
            if (!_playerItemHolder.HasItem) {
                canvasTransform.sizeDelta = (Vector2)minimumSize * cellSize;
                return;
            }

            canvasTransform.sizeDelta = (Vector2)_playerItemHolder.HeldItem.ItemSo.InventorySize * cellSize;
            var item = new InventoryItem(null, _playerItemHolder.HeldItem, Vector2Int.zero, InventoryItem.InventoryItemRotation.Zero);
            _inventoryItemDisplay = Instantiate(item.itemState.ItemSo.InventoryItemPrefab, transform);
            _inventoryItemDisplay.Initialize(item, item.itemState.ItemSo.InventoryItemIcon, item.Size, Vector2.zero, item.rotation);
            _inventoryItemDisplay._inventoryItem.ReallyUglyFixForNow += Clear;
        }

        private void Clear() {
            _playerItemHolder.ForceClearHeldItem();
            UpdateVisuals();
        }

        public bool CanTakeItem(Vector2 position, Vector2Int size, InventoryItem item) {
            return item == _inventoryItemDisplay?._inventoryItem || !_playerItemHolder.HasItem ||
                   item.Inventory.TryFindFreeArea(_playerItemHolder.HeldItem.ItemSo.InventorySize, out _, out _, item);
        }

        public bool TakeItem(Vector2 position, InventoryItem.InventoryItemRotation rotation, InventoryItem item) {
            if (!_playerItemHolder.HasItem || item.Inventory.InsertItem(_playerItemHolder.HeldItem)) {
                _playerItemHolder.ForceClearHeldItem();
                _playerItemHolder.Pickup(item.itemState);
                return true;
            }
            
            return false;
        }

        public void ClearFeedback() {
        }
    }
}