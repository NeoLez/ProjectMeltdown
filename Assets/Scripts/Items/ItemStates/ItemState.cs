using UnityEngine;

namespace Root {
    /// <summary>
    ///Represents an instance of an item. It stores all the PERSISTENT data each INDIVIDUAL item holds. It is referenced by all other forms of the item (such as the PhysicalItem and the InventoryItem).
    /// </summary>
    [System.Serializable]
    public class ItemState {
        [field: SerializeField] public ItemSo ItemSo { get; protected set; }

        public ItemState(ItemSo itemSo) {
            ItemSo = itemSo;
        }

        public virtual ItemState Clone() {
            return new ItemState(ItemSo);
        }
    }
}