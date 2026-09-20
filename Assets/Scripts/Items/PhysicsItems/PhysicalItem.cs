using Root.Managers;
using Timers;
using UnityEngine;

namespace Root {
    [RequireComponent(typeof(BoundingBoxTracker))]
    public class PhysicalItem : InteractableNormalCamera {
        [Header("Editor Only Data")]
#if UNITY_EDITOR
        [SerializeField] private ItemSo defaultItemSo;
        [ContextMenu("Generate Starting State")]
        private void GenerateStateInEditor() {
            if (defaultItemSo == null) {
                Debug.LogWarning("You must assign an ItemSO to 'Default ItemSo' first!");
                return;
            }

            var state = defaultItemSo.CreateState();
            if (!IsStateTypeValid(state)) {
                Debug.LogError("Unexpected State Type'" + state.GetType() + "'. Are you assigning the right ItemSO?");
                return;
            }
            ItemState = defaultItemSo.CreateState();
            
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
        
        [Header("Physical Item Data")]
        [SerializeField] [SerializeReference] private ItemState itemState;
        private BoundingBoxTracker _boundingBoxTracker;

        protected virtual void Awake() {
            _boundingBoxTracker = GetComponent<BoundingBoxTracker>();
            _boundingBoxTracker.OnMapSectionRemoved += () => {
                PoolManager.ReturnObjectToPool(GetComponent<Poolable>());
            };
        }

        public ItemState ItemState {
            get => itemState;
            set {
                itemState = value;
                Initialize();
            }
        }

        public override void Interact() {
            PlayerItemHolder holder = GameManager.Player.GetComponent<PlayerItemHolder>();

            if (holder == null)
                return;

            holder.Pickup(this);
        }


        /// <summary>
        /// If <paramref name="state"/> is true, will disable all the object's behaviours, physics and colliders except for visual ones to allow for use in, for instance, animations. Setting it to false will undo this change.
        /// </summary>
        /// <param name="state"></param>
        public virtual void VisualOnly(bool state) {
            Rigidbody rb = GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = state;
                rb.useGravity = !state;
                rb.constraints = state ? RigidbodyConstraints.FreezeRotation : RigidbodyConstraints.None;
            }

            Collider[] colliders = GetComponentsInChildren<Collider>();

            foreach (var col in colliders)
                col.enabled = !state;
            
            _boundingBoxTracker.enabled = !state;
        }
        
        /// <summary>
        /// Returns whether the input <paramref name="state"/> is valid. This function should be overriden by child classes to ensure that the results are valid
        /// </summary>
        /// <param name="state"></param>
        protected virtual bool IsStateTypeValid(ItemState state) {
            return true;
        }

        /// <summary>
        /// Runs every time the ItemState is set, most of the time it's only at the beginning of the PhysicalItem's lifespan.
        /// </summary>
        protected virtual void Initialize() {
            
        }
    }
}