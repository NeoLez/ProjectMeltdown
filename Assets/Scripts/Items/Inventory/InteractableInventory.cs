using UnityEngine;

namespace Root {
    [RequireComponent(typeof(Inventory))]
    public class InteractableInventory : InteractableNormalCamera {

        private MeshFilter _mesh;
        [SerializeField] private Mesh[] _meshList;

        private void Start()
        {
            _mesh = GetComponentInChildren<MeshFilter>();
        }
        public override void Interact() {
            GameManager.PlayerInventoryUI.OpenInventory(GetComponent<Inventory>());
            VisualFeedback();
        }
        private void VisualFeedback()
        {
            if (_meshList.Length > 0)
            {
                _mesh.mesh = _meshList[1];
            }
            
        }
    }
}