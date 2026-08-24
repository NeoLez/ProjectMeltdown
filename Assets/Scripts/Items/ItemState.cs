using UnityEngine;

namespace Root {
    [System.Serializable]
    public class ItemState {
        [field: SerializeField] public ItemSo ItemSo { get; protected set; }

        public ItemState(ItemSo itemSo) {
            ItemSo = itemSo;
        }
    }
}