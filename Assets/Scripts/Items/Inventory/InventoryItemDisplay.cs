using UnityEngine;
using UnityEngine.UI;

namespace Root {
    public class InventoryItemDisplay : MonoBehaviour {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image image;
        [SerializeField] private Canvas canvas;
        [SerializeField] private int initialSortingOrder;
        [SerializeField] private int draggingSortingOrder;
        private InventoryDisplay inventoryDisplay;
        private InventoryItem _inventoryItem;
        public Vector2 originalPosition;
        public Vector2 originalSize;
        public InventoryItem.InventoryItemRotation originalRotation;
        
        private float cellSize; 
        
        public virtual void Initialize(InventoryDisplay disp, InventoryItem item, Sprite itemIcon, Vector2Int size, Vector2 position, InventoryItem.InventoryItemRotation rotation) {
            cellSize = rectTransform.sizeDelta.x; 
            
            originalPosition = position;
            originalSize = size;
            originalRotation = rotation;

            _inventoryItem = item;
            inventoryDisplay = disp;
            canvas.sortingOrder = initialSortingOrder;

            rectTransform.sizeDelta *= size;
            SetPosition(position, rotation);
            
            image.sprite = itemIcon;
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

            canvas.GetComponent<RectTransform>().anchoredPosition = position;
            rectTransform.rotation = Quaternion.Euler(0, 0, (int)rotation);
        }

        public void SetSortingOrder(bool dragging) {
            if (dragging) {
                canvas.sortingOrder = draggingSortingOrder;
                return;
            }
            canvas.sortingOrder = initialSortingOrder;
        }
    }
}