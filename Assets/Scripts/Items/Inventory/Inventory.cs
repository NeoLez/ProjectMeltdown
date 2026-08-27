using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;using Root;
using UnityEngine;

namespace Root {
    public class Inventory : MonoBehaviour {
        [SerializeField] private ItemSo itemSoToAdd;
        [SerializeField] private InventoryItem.InventoryItemRotation rotation;
        [SerializeField] private bool find;
        [SerializeField] private int amount;
        [SerializeField] private Vector2Int pos;

        [ContextMenu("AddItem")]
        private void AddItem() {
            for (int i=0; i<amount; i++)
                if (find)
                    Debug.Log(InsertItem(itemSoToAdd.CreateState()));
                else
                    Debug.Log(InsertItem(itemSoToAdd.CreateState(), pos, rotation));
        }
        
        [ContextMenu("RemoveItem")]
        private void RemoveItem() {
            RemoveItem(pos, out InventoryItem invItem);
        }
        
        private class InventorySlot {
            [CanBeNull] public InventoryItem InventoryItem;
            public bool IsFree => InventoryItem == null;
        }

        public event Action OnRedraw;
        public event Action<InventoryItem, Vector2Int> OnItemAdded;
        public event Action<InventoryItem> OnItemRemoved;
        
        [field: SerializeField] public Vector2Int Size { get; private set; }
        private HashSet<InventoryItem> _items = new();
        private Dictionary<Vector2Int, InventorySlot> _slots = new();

        private void Awake() {
            for (int x = 0; x < Size.x; x++) {
                for (int y = 0; y < Size.y; y++) {
                    _slots[new Vector2Int(x, y)] = new InventorySlot();
                }
            }
        }

        public bool InsertItem(ItemState item, Vector2Int position, InventoryItem.InventoryItemRotation rotation) {
            
            Vector2Int size = InventoryItem.GetRotationCorrectedSize(item.ItemSo.InventorySize, rotation);
            if (!IsAreaFree(size, position)) return false;
            
            var invItem = new InventoryItem(this, item, position, rotation);
            
            if (!SetSlotsToItem(size, position, invItem)) return false;
            
            _items.Add(invItem);
            OnItemAdded?.Invoke(invItem, position);
            return true;
        }
        
        public bool InsertItem(ItemState item) {
            if (TryFindFreeArea(item.ItemSo.InventorySize, out Vector2Int foundPosition, out InventoryItem.InventoryItemRotation foundRotation)) {
                return InsertItem(item, foundPosition, foundRotation);
            }

            return false;
        }

        public bool ContainsItemType(ItemSo itemSo) {
            foreach (var item in _items) {
                if (item.itemState.ItemSo == itemSo) return true;
            }

            return false;
        }
        
        public bool InsertItem(InventoryItem invItem, Vector2Int position) {
            Vector2Int size = invItem.RotationCorrectedSize;
            if (!IsAreaFree(size, position, invItem)) return false;

            if (!SetSlotsToItem(size, position, invItem)) return false;

            invItem._position = position;
            _items.Add(invItem);
            OnItemAdded?.Invoke(invItem, position);
            return true;
        }

        public List<Vector2Int> GetInventorySlotPositions()
        {
            return _slots.Keys.ToList();
        }
        
        public bool RemoveItem(Vector2Int position, out InventoryItem inventoryItem) {
            inventoryItem = null;
            
            if (!_slots.TryGetValue(position, out InventorySlot slot) || slot.IsFree) return false;
            inventoryItem = slot.InventoryItem;
            OnItemRemoved?.Invoke(inventoryItem);
            if (!SetSlotsToItem(slot.InventoryItem!.RotationCorrectedSize, slot.InventoryItem._position, null))
                return false;
            _items.Remove(slot.InventoryItem);
            
            return true;
        }
        
        public bool GetItem(Vector2Int position, out InventoryItem inventoryItem) {
            inventoryItem = null;
            if (!_slots.TryGetValue(position, out InventorySlot slot) || slot.IsFree) return false;

            inventoryItem = slot.InventoryItem;
            return true;
        }

        private bool SetSlotsToItem(Vector2Int size, Vector2Int position, [CanBeNull] InventoryItem item) {
            for (int x = 0; x < size.x; x++) {
                for (int y = 0; y < size.y; y++) {
                    if (!_slots.TryGetValue(position + new Vector2Int(x, y), out InventorySlot slot)) return false;
                    slot.InventoryItem = item;
                }
            }

            return true;
        }
        
        public bool IsAreaFree(Vector2Int size, Vector2Int position, InventoryItem item = null) {
            for (int x = 0; x < size.x; x++) {
                for (int y = 0; y < size.y; y++) {
                    if (!_slots.TryGetValue(position + new Vector2Int(x, y), out InventorySlot slot)) return false;
                    if (slot.IsFree) continue;
                    if (slot.InventoryItem == item) continue;
                    return false;
                }
            }

            return true;
        }
        
        private bool TryFindFreeArea(Vector2Int baseSize, out Vector2Int position, out InventoryItem.InventoryItemRotation rotation) {
            position = Vector2Int.zero;
            rotation = InventoryItem.InventoryItemRotation.Zero;

            //Skip checking Quarter if the item is a square
            InventoryItem.InventoryItemRotation[] rotationsToCheck = 
                baseSize.x == baseSize.y ? new[] { InventoryItem.InventoryItemRotation.Zero } 
                    : new[] { InventoryItem.InventoryItemRotation.Zero, InventoryItem.InventoryItemRotation.Quarter };
            
            
            foreach (var currentRotation in rotationsToCheck) {
                Vector2Int rotatedSize = InventoryItem.GetRotationCorrectedSize(baseSize, currentRotation);
                
                for (int y = 0; y <= Size.y - rotatedSize.y; y++) {
                    for (int x = 0; x <= Size.x - rotatedSize.x; x++) {
                        Vector2Int currentPosition = new Vector2Int(x, y);
                        
                        if (IsAreaFree(rotatedSize, currentPosition)) {
                            position = currentPosition;
                            rotation = currentRotation;
                            return true; 
                        }
                    }
                }
            }

            return false;
        }
    }
}