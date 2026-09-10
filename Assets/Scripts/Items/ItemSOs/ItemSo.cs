using Root.Managers;
using UnityEngine;
using UnityEngine.Assertions;

namespace Root {
    [CreateAssetMenu(menuName = "Items/Base Item", fileName =  "Item")]
    public class ItemSo : ScriptableObject {
        [field: SerializeField] public PhysicalItem PhysicalItemPrefab { get; private set; }
        [field: SerializeField] public InventoryItemDisplay InventoryItemPrefab { get; private set; }
        [field: SerializeField] public Sprite InventoryItemIcon { get; private set; }
        [field: SerializeField] public GameObject HeldItemGameObject { get; private set; }
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Vector2Int InventorySize { get; private set; }

        public virtual ItemState CreateState() {
            return new ItemState(this);
        }

        public PhysicalItem CreatePhysicalItem() {
            var obj = PoolManager.GetObject(PhysicalItemPrefab.GetComponent<Poolable>());
            var physicalItem = obj.GetComponent<PhysicalItem>();
            physicalItem.itemState = PhysicalItemPrefab.itemState.Clone();
            return physicalItem;
        }

        private void OnValidate() {
            Assert.IsTrue(InventorySize is { x: >= 1, y: >= 1 }, "InventorySize must be 1x1 or greater");
            Assert.IsNotNull(PhysicalItemPrefab.GetComponent<ItemPoolable>(), "PhysicalItemPrefab doesn't have a Poolable component");
        }
    }
}