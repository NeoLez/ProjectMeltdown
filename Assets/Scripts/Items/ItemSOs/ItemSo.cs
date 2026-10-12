using System.Collections.Generic;
using NaughtyAttributes;
using Root.Factories;
using Root.Managers;
using UnityEngine;
using UnityEngine.Assertions;

namespace Root {
    /// <summary>
    /// Represents an item type. Stores all IMMUTABLE DATA that's common to all instances and handles creation of specific item instances. Must have on child class that overrides CreateState() for every ItemState type.
    /// </summary>
    [CreateAssetMenu(menuName = "Items/Base Item", fileName =  "Item")]
    public class ItemSo : ScriptableObject {
        [field: SerializeField] public PhysicalItem PhysicalItemPrefab { get; private set; }
        [field: SerializeField] public InventoryItemDisplay InventoryItemPrefab { get; private set; }
        [field: SerializeField] public Sprite InventoryItemIcon { get; private set; }
        [field: SerializeField] public GameObject HeldItemGameObject { get; private set; }
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Vector2Int InventorySize { get; private set; }

        [SerializeField, SerializeReference] private ItemState defaultItemState;

        private ItemState GetDefaultItemState() {
            return defaultItemState.Clone();;
        }

        [field: SerializeField, Expandable] public StoreItemData StoreItemData { get; private set; }

        //For setting information that should always be valid, like any item that depends on the station that it spawned on (e.g. keycards) 
        [SerializeField] private List<ItemStateInitializer> requiredInitializers = new();
        public IReadOnlyList<ItemStateInitializer> RequiredInitializers => requiredInitializers;

        
        /// <summary>
        /// Returns a default ItemState of the correct type. Should always be overriden by child classes to define the ItemState type they expect. 
        /// </summary>
        public virtual ItemState CreateUninitializedState() {
            return defaultItemState.Clone();
        }

        /// <summary>
        /// Returns an initialized ItemState of the correct type. If no initializers are present this function returns the default state.
        /// </summary>
        public ItemState CreateState(ItemSpawnContext ctx, IEnumerable<ItemStateInitializer> initializers) {
            var state = GetDefaultItemState();
            
            foreach (var i in RequiredInitializers) i.Apply(state, ctx);
            
            if (initializers != null)
                foreach (var i in initializers) i.Apply(state, ctx);
            
            return state;
        }
        
        /// <summary>
        /// Returns an initialized ItemState of the correct type. If no initializers are present this function returns the default state.
        /// </summary>
        public ItemState CreateState(ItemSpawnContext ctx, ItemStateInitializer initializer = null) {
            var state = GetDefaultItemState();
            
            foreach (var i in RequiredInitializers) i.Apply(state, ctx);
            
            if (initializer != null)
                initializer.Apply(state, ctx);
            
            return state;
        }
        
        /// <summary>
        /// Creates a new instance of the physical item handling pool allocations, etc. All physical items should be created this way.
        /// The state must already be initialized CreateState()
        /// </summary>
        /// <param name="state">The initial item's state. The PhysicalItem gets initialized using this.</param>
        public PhysicalItem CreatePhysicalItem(ItemState state) {
            var obj = PoolManager.GetObject(PhysicalItemPrefab.GetComponent<Poolable>());
            var physicalItem = obj.GetComponent<PhysicalItem>();
            physicalItem.ItemState = state;
            return physicalItem;
        }
        
        public PhysicalItem CreatePhysicalItem(ItemSpawnContext ctx, IEnumerable<ItemStateInitializer> initializers) {
            var obj = PoolManager.GetObject(PhysicalItemPrefab.GetComponent<Poolable>());
            var physicalItem = obj.GetComponent<PhysicalItem>();
            physicalItem.ItemState = CreateState(ctx, initializers);
            return physicalItem;
        }
        
        public PhysicalItem CreatePhysicalItem(ItemSpawnContext ctx, ItemStateInitializer initializer = null) {
            var obj = PoolManager.GetObject(PhysicalItemPrefab.GetComponent<Poolable>());
            var physicalItem = obj.GetComponent<PhysicalItem>();
            physicalItem.ItemState = CreateState(ctx, initializer);
            return physicalItem;
        }

        private void OnValidate() {
            Assert.IsTrue(PhysicalItemPrefab.IsStateTypeValid(defaultItemState), "ItemState doesn't match the one reported by PhysicalItem");
            Assert.IsTrue(InventoryItemPrefab.IsStateTypeValid(defaultItemState), "ItemState doesn't match the one reported by InventoryItemDisplay");
            Assert.IsTrue(StoreItemData == null || StoreItemData.item == this, "StoreItemData itemSo mismatch");
            Assert.IsTrue(defaultItemState.ItemSo == this, "Wrong ItemSo set inside the default item state");
            Assert.IsTrue(InventorySize is { x: >= 1, y: >= 1 }, "InventorySize must be 1x1 or greater");
            Assert.IsNotNull(PhysicalItemPrefab.GetComponent<ItemPoolable>(), "PhysicalItemPrefab doesn't have a Poolable component");
        }
        
        
        
        
        
#if UNITY_EDITOR
        [Button]
        [ContextMenu("Generate Default Item State")]
        private void GenerateStateInEditor() {
            UnityEditor.Undo.RecordObject(this, "Generate Item State");
            defaultItemState = CreateUninitializedState();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}