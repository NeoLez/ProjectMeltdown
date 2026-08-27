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
        
        public virtual void Initialize(InventoryDisplay disp, InventoryItem item, Sprite itemIcon, Vector2Int size, Vector2 position, InventoryItem.InventoryItemRotation rotation) {
            originalPosition = position;
            originalSize = size;
            originalRotation = rotation;

            _inventoryItem = item;
            inventoryDisplay = disp;

            SetPosition(position, rotation);
            
            rectTransform.sizeDelta *= size;
            GetComponent<Image>().sprite = itemIcon;
        }

        public void SetPosition(Vector2 position, InventoryItem.InventoryItemRotation rotation) {
            rectTransform = GetComponent<RectTransform>();
            var iconSize = rectTransform.sizeDelta.x;
            switch (rotation) {
                case InventoryItem.InventoryItemRotation.Quarter:
                    position += new Vector2(originalSize.y * iconSize, 0);
                    break;
                case InventoryItem.InventoryItemRotation.Half:
                    position += new Vector2(originalSize.x * iconSize, originalSize.y * iconSize);
                    break;
                case InventoryItem.InventoryItemRotation.ThreeQuarters:
                    position += new Vector2(0, originalSize.y * iconSize);
                    break;
            }

            rectTransform.anchoredPosition = position;
            rectTransform.rotation = Quaternion.Euler(0, 0, (int)rotation);
        }
    }
}