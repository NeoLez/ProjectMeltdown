using UnityEngine;

namespace Root {
    public class InventoryItem{
        public ItemState itemState;
        public Inventory Inventory;
        public InventoryItemRotation rotation;
        private Vector2Int _size;
        public Vector2Int _position;
        public Vector2Int RotationCorrectedSize => GetRotationCorrectedSize(_size, rotation);
        public Vector2Int Size => _size;

        public InventoryItem(Inventory inventory,ItemState item, Vector2Int position, InventoryItemRotation itemRotation) {
            Inventory = inventory;
            itemState = item;
            _position = position;
            rotation = itemRotation;
            _size = item.ItemSo.InventorySize;
        }
        
        public enum InventoryItemRotation {
            Zero = 0,
            Quarter = 90,
            Half = 180,
            ThreeQuarters = 270,
        }
        
        public static Vector2Int GetRotationCorrectedSize(Vector2Int size, InventoryItemRotation rotation) {
            if (rotation is InventoryItemRotation.Zero or InventoryItemRotation.Half) return size;
            
            return new Vector2Int(size.y, size.x); 
        }
    }
}