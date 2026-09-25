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
        [field: SerializeField, SerializeReference] public ItemState DefaultItemState { get; private set; }

        /// <summary>
        /// Returns a zero initialized ItemState of the correct type. Should be overriden by child classes to define the ItemState type they expect. 
        /// </summary>
        public virtual ItemState CreateState() {
            return new ItemState(this);
        }

        /// <summary>
        /// Creates a new instance of the physical item handling pool allocations, etc. All physical items should be created this way.
        /// If an initial state is not specified, it is the default one set in the Physical Item prefab
        /// </summary>
        public PhysicalItem CreatePhysicalItem() {
            var obj = PoolManager.GetObject(PhysicalItemPrefab.GetComponent<Poolable>());
            var physicalItem = obj.GetComponent<PhysicalItem>();
            physicalItem.ItemState = PhysicalItemPrefab.ItemState.Clone();
            return physicalItem;
        }
        
        /// <inheritdoc cref="CreatePhysicalItem()"/>
        /// <param name="state">The initial item's state.</param>
        public PhysicalItem CreatePhysicalItem(ItemState state) {
            var obj = CreatePhysicalItem();
            obj.ItemState = state;
            return obj;
        }

        private void OnValidate() {
            Assert.IsTrue(InventorySize is { x: >= 1, y: >= 1 }, "InventorySize must be 1x1 or greater");
            Assert.IsNotNull(PhysicalItemPrefab.GetComponent<ItemPoolable>(), "PhysicalItemPrefab doesn't have a Poolable component");
        }
        
        
        
        
        
#if UNITY_EDITOR
        [NaughtyAttributes.Button]
        [ContextMenu("Generate Default Item State")]
        private void GenerateStateInEditor() {
            UnityEditor.Undo.RecordObject(this, "Generate Item State");
            DefaultItemState = CreateState();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}