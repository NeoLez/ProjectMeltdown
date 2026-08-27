using UnityEngine;
using UnityEngine.UI;

namespace Root {
    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class InventoryItemDisplay : MonoBehaviour {
        private RectTransform rectTransform;
        private InventoryDisplay inventoryDisplay;
        private InventoryItem _inventoryItem;
        public Vector2 originalPosition;
        public Vector2 originalSize;
        public InventoryItem.InventoryItemRotation originalRotation;
        
        private float cellSize; 
        
        public virtual void Initialize(InventoryDisplay disp, InventoryItem item, Sprite itemIcon, Vector2Int size, Vector2 position, InventoryItem.InventoryItemRotation rotation) {
            rectTransform = GetComponent<RectTransform>();
            cellSize = rectTransform.sizeDelta.x; 
            
            originalPosition = position;
            originalSize = size;
            originalRotation = rotation;

            _inventoryItem = item;
            inventoryDisplay = disp;

            rectTransform.sizeDelta *= size;
            SetPosition(position, rotation);
            
            GetComponent<Image>().sprite = itemIcon;
        }

        public void SetPosition(Vector2 position, InventoryItem.InventoryItemRotation rotation) {
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
            float iconSize = cellSize > 0 ? cellSize : rectTransform.sizeDelta.x;

            switch (rotation) {
                case InventoryItem.InventoryItemRotation.Quarter:
                    position += new Vector2(originalSize.y * iconSize, 0);
                    break;
                case InventoryItem.InventoryItemRotation.Half:
                    position += new Vector2(originalSize.x * iconSize, originalSize.y * iconSize);
                    break;
                case InventoryItem.InventoryItemRotation.ThreeQuarters:
                    position += new Vector2(0, originalSize.x * iconSize); 
                    break;
            }

            rectTransform.anchoredPosition = position;
            rectTransform.rotation = Quaternion.Euler(0, 0, (int)rotation);
        }
    }
}