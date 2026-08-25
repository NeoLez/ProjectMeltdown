using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Root {
    public class Inventory : MonoBehaviour {
        private class InventorySlot {
            [CanBeNull] public ItemPivotTuple InventoryItem;
            public bool IsFree => InventoryItem == null;
        }

        private class ItemPivotTuple {
            public readonly Vector2Int Pivot;
            public readonly InventoryItem InventoryItem;

            public ItemPivotTuple(Vector2Int pivot, InventoryItem inventoryItem) {
                Pivot = pivot;
                InventoryItem = inventoryItem;
            }
        }

        [field: SerializeField] public Vector2Int Size { get; private set; }
        private List<ItemPivotTuple> _items = new();
        private Dictionary<Vector2Int, InventorySlot> _slots = new();

        private void Awake() {
            for (int x = 0; x < Size.x; x++) {
                for (int y = 0; y < Size.y; y++) {
                    _slots[new Vector2Int(x, y)] = new InventorySlot();
                }
            }
        }

        public bool InsertItem(ItemState item, Vector2Int position, InventoryItem.InventoryItemRotation rotation) {
            //More extensive search for free spaces needed
            
            Vector2Int size = InventoryItem.GetRotationCorrectedSize(item.ItemSo.InventorySize, rotation);
            if (!IsAreaFree(size, position)) return false;
            
            var invItem = new InventoryItem(item, rotation);
            var itemPivotTuple = new ItemPivotTuple(position, invItem);
            
            if (!SetSlotsToItem(size, position, itemPivotTuple)) return false;
            
            _items.Add(itemPivotTuple);
            return true;
        }

        public bool ContainsItemType(ItemSo itemSo) {
            foreach (var item in _items) {
                if (item.InventoryItem.itemState.ItemSo == itemSo) return true;
            }

            return false;
        }
        
        public bool InsertItem(InventoryItem invItem, Vector2Int position) {
            Vector2Int size = invItem.Size;
            if (!IsAreaFree(size, position, invItem)) return false;
            
            var itemPivotTuple = new ItemPivotTuple(position, invItem);
            
            if (!SetSlotsToItem(size, position, itemPivotTuple)) return false;
            
            _items.Add(itemPivotTuple);
            return true;
        }

        public List<Vector2Int> GetInventorySlotPositions()
        {
            return _slots.Keys.ToList();
        }
        
        public bool RemoveItem(Vector2Int position, out InventoryItem inventoryItem) {
            inventoryItem = null;
            
            if (!_slots.TryGetValue(position, out InventorySlot slot) || slot.IsFree) return false;
            if (!SetSlotsToItem(slot.InventoryItem!.InventoryItem.Size, slot.InventoryItem.Pivot, null))
                throw new Exception("wtf");
            _items.Remove(slot.InventoryItem);

            inventoryItem = slot.InventoryItem.InventoryItem;
            return true;
        }
        
        public bool GetItem(Vector2Int position, out InventoryItem inventoryItem) {
            inventoryItem = null;
            if (!_slots.TryGetValue(position, out InventorySlot slot) || slot.IsFree) return false;

            inventoryItem = slot.InventoryItem!.InventoryItem;
            return true;
        }

        private bool SetSlotsToItem(Vector2Int size, Vector2Int position, ItemPivotTuple item) {
            for (int x = 0; x < size.x; x++) {
                for (int y = 0; y < size.y; y++) {
                    if (!_slots.TryGetValue(position + new Vector2Int(x, y), out InventorySlot slot)) return false;
                    slot.InventoryItem = item;
                }
            }

            return true;
        }
        
        private bool IsAreaFree(Vector2Int size, Vector2Int position, InventoryItem item = null) {
            for (int x = 0; x < size.x; x++) {
                for (int y = 0; y < size.y; y++) {
                    if (!_slots.TryGetValue(position + new Vector2Int(x, y), out InventorySlot slot)) return false;
                    if (slot.IsFree) continue;
                    if (slot.InventoryItem!.InventoryItem == item) continue;
                    return false;
                }
            }

            return true;
        }
    }
}