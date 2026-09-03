using Timers;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Root {
    public class InventoryItemDisplay : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image image;
        [SerializeField] private Canvas canvas;
        [SerializeField] private int initialSortingOrder;
        [SerializeField] private int draggingSortingOrder;
        [SerializeField] private float dragSizeMultiplier = 1.2f;
        [SerializeField] private float dragSmoothing = 25;
        [SerializeField] private float sizeChangeSmoothing = 25;
        private InventoryItem _inventoryItem;
        public Vector2 originalPosition;
        public Vector2 originalSize;
        public InventoryItem.InventoryItemRotation originalRotation;
        
        private float _cellSize; 
        
        public virtual void Initialize(InventoryItem item, Sprite itemIcon, Vector2Int size, Vector2 position, InventoryItem.InventoryItemRotation rotation) {
            _cellSize = rectTransform.sizeDelta.x;

            _inventoryItem = item;
            canvas.sortingOrder = initialSortingOrder;

            rectTransform.sizeDelta *= size;
            SetPosition(position, rotation);
            
            originalPosition = position;
            originalSize = size;
            originalRotation = rotation;
                        
            image.sprite = itemIcon;
        }

        private void SetPosition(Vector2 position, InventoryItem.InventoryItemRotation rotation) {
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
            float iconSize = _cellSize > 0 ? _cellSize : rectTransform.sizeDelta.x;

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

            canvas.GetComponent<RectTransform>().anchoredPosition = position + GetCurrentVisualOffset(rotation);
            rectTransform.rotation = Quaternion.Euler(0, 0, (int)rotation);
        }


        private void SetSortingOrder(bool dragging) {
            if (dragging) {
                canvas.sortingOrder = draggingSortingOrder;
                return;
            }
            canvas.sortingOrder = initialSortingOrder;
        }

        private bool _isBeingDragged;
        private InventoryItem.InventoryItemRotation _currentRotation;
        private Vector2 _targetPosition;
        private float _targetSize;

        public void OnBeginDrag(PointerEventData eventData) {
            if(_isBeingDragged) return;
            
            SetSortingOrder(true);
            _isBeingDragged = true;
            _currentRotation = originalRotation;
            _targetPosition = eventData.position;
            _targetSize = 1;
        }

        private void Update() {

            if (!_isBeingDragged) return;
            var canvasTransform = (RectTransform)canvas.transform;
            canvasTransform.anchoredPosition = Vector2.Lerp(canvasTransform.anchoredPosition, _targetPosition, dragSmoothing * Time.deltaTime);
            canvasTransform.localScale = Vector3.Lerp(canvasTransform.localScale, new Vector3(_targetSize, _targetSize, 1), sizeChangeSmoothing * Time.deltaTime);
            RotateDraggingItem((int)GameManager.Input.Inventory.RotateItem.ReadValue<Vector2>().y);
        }

        public void OnDrag(PointerEventData eventData) {
            _targetPosition = GetRelativePosition(eventData.position);
            
            Vector2 correctedScreenPos = GetBottomLeftScreenPosition(eventData);
            Vector2Int currentSize = InventoryItem.GetRotationCorrectedSize(_inventoryItem.Size, _currentRotation);

            if (!UIUtility.GetFirstComponentUnderCursor(eventData, out IItemDragReceiver receiver) || 
                !receiver.CanTakeItem(correctedScreenPos, currentSize, _inventoryItem)) 
            {
                _targetSize = dragSizeMultiplier;
            }
            else {
                _targetSize = 1;
            }
        }

        public void OnEndDrag(PointerEventData eventData) {
            _isBeingDragged = false;
    
            Vector2 correctedScreenPos = GetBottomLeftScreenPosition(eventData);
            Vector2Int currentSize = InventoryItem.GetRotationCorrectedSize(_inventoryItem.Size, _currentRotation);
            
            if (!UIUtility.GetFirstComponentUnderCursor(eventData, out IItemDragReceiver receiver)) {
                receiver = GetWorldDragReceiver();
                if (receiver == null) {
                    ReturnItem();
                    return;
                }
            }

            if (!receiver.CanTakeItem(correctedScreenPos, currentSize, _inventoryItem)) {
                ReturnItem();
                return;
            }

            _inventoryItem.Inventory.RemoveItem(_inventoryItem);
            receiver.TakeItem(correctedScreenPos, _currentRotation, _inventoryItem);
        }

        private void ReturnItem() {
            _isBeingDragged = false;
            var canvasTransform = (RectTransform)canvas.transform;
            canvasTransform.localScale = new Vector3(1, 1, 1);
            SetSortingOrder(false);
            SetPosition(originalPosition, originalRotation);
        }

        private void RotateDraggingItem(int direction) {
            direction = math.sign(direction);
            if (direction == 0) return;
    
            if (direction == 1) 
            {
                _currentRotation = _currentRotation.RotateRight();
            } 
            else if (direction == -1) 
            {
                _currentRotation = _currentRotation.RotateLeft();
            }

            rectTransform.localRotation = Quaternion.Euler(0, 0, (float)_currentRotation);
        }

        private Vector2 GetRelativePosition(Vector2 position) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform.parent, position, null, out var localPointerPos);
            return localPointerPos;
        }

        private void OnDisable() {
            ReturnItem();
        }

        private Vector2 GetCurrentVisualOffset(InventoryItem.InventoryItemRotation rotation) {
            Vector2 currentSize = InventoryItem.GetRotationCorrectedSize(_inventoryItem.Size, rotation);
            return currentSize * _cellSize / 2f;
        }
        
        private Vector2 GetBottomLeftScreenPosition(PointerEventData eventData) {
            Vector2 localPointerPos = GetRelativePosition(eventData.position);
            Vector2 localBottomLeft = localPointerPos - GetCurrentVisualOffset(_currentRotation) + Vector2.one * _cellSize / 2f;
            Transform parentTransform = canvas.transform.parent;
            Vector3 worldBottomLeft = parentTransform.TransformPoint(localBottomLeft);
            return RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, worldBottomLeft);
        }

        private IItemDragReceiver GetWorldDragReceiver() {
            var mousePosition = Pointer.current.position.value;
            Ray ray = GameManager.Camera.ScreenPointToRay(mousePosition/GameManager.GetResolutionRatio());
            if (!Physics.Raycast(ray, out var hit, GameManager.CameraController.interactDistance) ||
                !hit.collider.gameObject.TryGetComponent<IItemDragReceiver>(out var component)) {
                return null;
            }

            return component;
        }
    }
}