using UnityEngine;

namespace Root {
    public class InventoryItem{
        public ItemState itemState;
        public InventoryItemRotation rotation;
        private Vector2Int _size;
        public Vector2Int Size => GetRotationCorrectedSize(_size, rotation);

        public InventoryItem(ItemState item, InventoryItemRotation itemRotation) {
            itemState = item;
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