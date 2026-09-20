using UnityEngine;
using UnityEngine.Serialization;

namespace Root {
    public class PhysicalItem : InteractableNormalCamera {
        
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
        [SerializeField] [SerializeReference] private ItemState itemState;
        
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


        public void VisualOnly(bool state) {
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
        }
        
        public virtual void StateUpdate() {}

        protected virtual bool IsStateTypeValid(ItemState state) {
            return true;
        }

        protected virtual void Initialize() {
            
        }
    }
}