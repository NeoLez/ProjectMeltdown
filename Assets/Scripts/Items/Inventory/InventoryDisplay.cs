using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Root {
    public class InventoryDisplay : MonoBehaviour, IDropHandler {
        public Inventory inventory;
        public RectTransform slotPanelPrefab;

        private void Start()
        {
            Generate();
        }

        public void Generate()
        {
            if (inventory == null) return;
            foreach (var position in inventory.GetInventorySlotPositions())
            {
                var slot = Instantiate(slotPanelPrefab, transform);
                slot.anchoredPosition += new Vector2(position.x, position.y) * slotPanelPrefab.sizeDelta.x;
            }
        }
        
        public void OnDrop(PointerEventData eventData) {
            Debug.Log(eventData.position);
        }
    }
}