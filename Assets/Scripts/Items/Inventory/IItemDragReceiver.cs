using UnityEngine;

namespace Root {
    public interface IItemDragReceiver {
        public bool CanTakeItem(Vector2 position, Vector2Int size, InventoryItem item);
        public bool TakeItem(Vector2 position, InventoryItem.InventoryItemRotation rotation, InventoryItem item);
        public void ClearFeedback();
    }
}