using System.Collections.Generic;
using Timers;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Root {
    public class InventoryDisplay : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler {
        public Inventory inventory;
        public RectTransform slotPanelPrefab;
        public RectTransform inventoryBackground;
        private List<RectTransform> slots = new();
        private Vector2Int maxDimensions;
        private Dictionary<InventoryItem, InventoryItemDisplay> itemToItemDisplay = new();
        [SerializeField] private float dragSmoothing;
        
        private void Start()
        {
            Generate();
            inventory.OnItemAdded += (item, position) => {
                var obj = Instantiate(item.itemState.ItemSo.InventoryItemPrefab, transform);
                itemToItemDisplay.Add(item, obj);
                obj.Initialize(this, item, item.itemState.ItemSo.InventoryItemIcon, item.Size, (Vector2)position * slotPanelPrefab.sizeDelta.x, item.rotation);
            };
            inventory.OnItemRemoved += item => {
                itemToItemDisplay.Remove(item, out var obj);
                if (obj == null) return;
                Destroy(obj.gameObject);
            };
        }

        public void Generate()
        {
            if (inventory == null) return;
            foreach (var position in inventory.GetInventorySlotPositions())
            {
                var slot = Instantiate(slotPanelPrefab, transform);
                maxDimensions.x = math.max(maxDimensions.x, position.x);
                maxDimensions.y = math.max(maxDimensions.y, position.y);
                slot.anchoredPosition += new Vector2(position.x, position.y) * slotPanelPrefab.sizeDelta.x;
                slots.Add(slot);
            }
            inventoryBackground.sizeDelta = new Vector2((maxDimensions.x + 1) * slots[0].sizeDelta.x, (maxDimensions.y + 1) * slots[0].sizeDelta.y);
        }
        
        public void OnDrop(PointerEventData eventData) {
            Debug.Log(eventData.position);
        }

        public Vector2Int MousePositionToSlotCoords(Vector2 mousePosition) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(inventoryBackground, mousePosition, null, out var relativePos);
            relativePos /= slotPanelPrefab.sizeDelta.x;
            return new Vector2Int((int)math.floor(relativePos.x), (int)math.floor(relativePos.y));
        }


        private bool draggingItem;
        private InventoryItem itemBeingDragged;
        private InventoryItemDisplay itemDisplayBeingDragged;
        private Vector2 originalRelativeDragPosition;
        private Vector2Int originalRelativeDragPositionSlot;
        private Vector2Int correctedRelativeDragPositionSlot;
        private InventoryItem.InventoryItemRotation draggingItemRotation;
        public void OnBeginDrag(PointerEventData eventData) {
            if (draggingItem) return;
            var slotPos = MousePositionToSlotCoords(eventData.position);
            Debug.Log(slotPos);
            if (!inventory.GetItem(slotPos, out itemBeingDragged)) return;

            draggingItem = true;
            itemDisplayBeingDragged = itemToItemDisplay[itemBeingDragged];
            draggingItemRotation = itemBeingDragged.rotation;

            RectTransform parentRect = (RectTransform)itemDisplayBeingDragged.transform.parent;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, null, out var localPointerPos);
            originalRelativeDragPosition = localPointerPos - (Vector2)itemDisplayBeingDragged.GetComponent<RectTransform>().localPosition;
            
            originalRelativeDragPositionSlot = MousePositionToSlotCoords(eventData.position) - itemBeingDragged._position;
            correctedRelativeDragPositionSlot = originalRelativeDragPositionSlot;
        }

        public void OnDrag(PointerEventData eventData) {
            
        }
        
        public void Update() {
            if (!draggingItem) return;
            if (GameManager.Input.Inventory.RotateRight.WasCompletedThisFrame())
                RotateDraggingItem();
            RectTransform parentRect = (RectTransform)itemDisplayBeingDragged.transform.parent;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRect, 
                    Mouse.current.position.value, 
                    null, 
                    out Vector2 localPointerPosition))
            {
                var rect = itemDisplayBeingDragged.GetComponent<RectTransform>();
                rect.localPosition = Vector3.Lerp(rect.localPosition, localPointerPosition - originalRelativeDragPosition, dragSmoothing);
            }
        }

        public void OnEndDrag(PointerEventData eventData) {
            if(!draggingItem) return;
            
            if (!UIUtility.GetFirstComponentUnderCursor(eventData, out InventoryDisplay inventoryDisplay)) {
                ReturnDraggedItem();
                return;
            }

            var inv = inventoryDisplay.inventory;
            var pivotSlot = inventoryDisplay.MousePositionToSlotCoords(eventData.position) - correctedRelativeDragPositionSlot;
            var originalRotation = itemBeingDragged.rotation;
            
            itemBeingDragged.rotation = draggingItemRotation;
            if (!inv.IsAreaFree(itemBeingDragged.RotationCorrectedSize, pivotSlot, itemBeingDragged)) {
                itemBeingDragged.rotation = originalRotation;
                ReturnDraggedItem();
                return;
            }
            
            itemBeingDragged.rotation = originalRotation;
            inventory.RemoveItem(itemBeingDragged._position, out _);
            
            itemBeingDragged.rotation = draggingItemRotation;
            inv.InsertItem(itemBeingDragged, pivotSlot);
            draggingItem = false;
        }

        private void RotateDraggingItem() {
            Vector2Int currentSize = InventoryItem.GetRotationCorrectedSize(itemBeingDragged.Size, draggingItemRotation);
            
            draggingItemRotation = draggingItemRotation.RotateRight();
            
            var rect = itemDisplayBeingDragged.GetComponent<RectTransform>();
            rect.localRotation = Quaternion.Euler(0, 0, (float)draggingItemRotation);
            
            originalRelativeDragPosition = new Vector2(originalRelativeDragPosition.y, -originalRelativeDragPosition.x);
            
            correctedRelativeDragPositionSlot = new Vector2Int(
                correctedRelativeDragPositionSlot.y,
                currentSize.x - 1 - correctedRelativeDragPositionSlot.x
            );
            
            RectTransform parentRect = (RectTransform)itemDisplayBeingDragged.transform.parent;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRect, 
                    Mouse.current.position.value, 
                    null, 
                    out Vector2 localPointerPosition))
            {
                rect.localPosition = localPointerPosition - originalRelativeDragPosition;
            }
        }

        private void ReturnDraggedItem() {
            draggingItem = false;
            itemDisplayBeingDragged.SetPosition(itemDisplayBeingDragged.originalPosition, itemDisplayBeingDragged.originalRotation);
            itemBeingDragged = null;
            itemDisplayBeingDragged = null;
        }
    }
}