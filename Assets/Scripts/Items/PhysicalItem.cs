using UnityEngine;

namespace Root {
    public class PhysicalItem : InteractableNormalCamera {
        [SerializeField] private ItemSo defaultItemSo;
        [SerializeField] [SerializeReference] public ItemState itemState;
        
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
        
        
        [ContextMenu("Generate Starting State")]
        private void GenerateStateInEditor() {
            if (defaultItemSo == null) {
                Debug.LogWarning("You must assign an ItemSO to 'Default ItemSo' first!");
                return;
            }
            
            itemState = defaultItemSo.CreateState();

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}