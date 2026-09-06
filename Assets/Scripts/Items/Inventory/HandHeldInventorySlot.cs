using UnityEngine;

namespace Root {
    [RequireComponent(typeof(Canvas))]
    public class HandHeldInventorySlot : MonoBehaviour, IItemDragReceiver {
        private PlayerItemHolder _playerItemHolder;
        
        private void Start() {
            _playerItemHolder = GameManager.Player.GetComponent<PlayerItemHolder>();
        }

        public bool CanTakeItem(Vector2 position, Vector2Int size, InventoryItem item) {
            return !_playerItemHolder.HasItem ||
                   item.Inventory.TryFindFreeArea(_playerItemHolder.HeldItem.ItemSo.InventorySize, out _, out _, item);
        }

        public bool TakeItem(Vector2 position, InventoryItem.InventoryItemRotation rotation, InventoryItem item) {
            if (!_playerItemHolder.HasItem) {
                _playerItemHolder.Pickup(item.itemState);
                return true;
            }

            return item.Inventory.InsertItem(_playerItemHolder.HeldItem);
        }

        public void ClearFeedback() {
        }
    }
}