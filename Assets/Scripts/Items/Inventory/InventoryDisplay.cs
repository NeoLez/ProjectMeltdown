using System.Collections.Generic;
using Timers;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Root {
    public class InventoryDisplay : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler {
        public Inventory inventory;
        public RectTransform slotPanelPrefab;
        public RectTransform inventoryBackground;
        private List<RectTransform> slots = new();
        private Vector2Int maxDimensions;
        private Dictionary<InventoryItem, InventoryItemDisplay> itemToItemDisplay = new();
        
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
        public void OnBeginDrag(PointerEventData eventData) {
            var slotPos = MousePositionToSlotCoords(eventData.position);
            Debug.Log(slotPos);
            if (!inventory.GetItem(slotPos, out itemBeingDragged)) return;

            draggingItem = true;
            itemDisplayBeingDragged = itemToItemDisplay[itemBeingDragged];
            Debug.Log(itemDisplayBeingDragged.name);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(itemDisplayBeingDragged.GetComponent<RectTransform>(), eventData.position, null, out originalRelativeDragPosition);

            originalRelativeDragPositionSlot = MousePositionToSlotCoords(eventData.position) - itemBeingDragged._position;
        }

        public void OnDrag(PointerEventData eventData) {
            if (!draggingItem) return;
            Debug.Log(eventData.scrollDelta);
            RectTransform parentRect = (RectTransform)itemDisplayBeingDragged.transform.parent;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRect, 
                    eventData.position, 
                    eventData.pressEventCamera, 
                    out Vector2 localPointerPosition))
            {
                itemDisplayBeingDragged.GetComponent<RectTransform>().localPosition = localPointerPosition - originalRelativeDragPosition;
            }
        }

        public void OnEndDrag(PointerEventData eventData) {
            if(!draggingItem) return;
            
            if (!UIUtility.GetFirstComponentUnderCursor(eventData, out InventoryDisplay inventoryDisplay)) {
                ReturnDraggedItem();
                return;
            }

            var inv = inventoryDisplay.inventory;
            var pivotSlot = inventoryDisplay.MousePositionToSlotCoords(eventData.position) - originalRelativeDragPositionSlot;
            if (!inv.IsAreaFree(itemBeingDragged.RotationCorrectedSize, pivotSlot, itemBeingDragged)) {
                ReturnDraggedItem();
                return;
            }

            inventory.RemoveItem(itemBeingDragged._position, out _);
            inv.InsertItem(itemBeingDragged, pivotSlot);
            draggingItem = false;
        }

        private void ReturnDraggedItem() {
            draggingItem = false;
            itemDisplayBeingDragged.SetPosition(itemDisplayBeingDragged.originalPosition, itemDisplayBeingDragged.originalRotation);
            itemBeingDragged = null;
            itemDisplayBeingDragged = null;
        }
    }
}