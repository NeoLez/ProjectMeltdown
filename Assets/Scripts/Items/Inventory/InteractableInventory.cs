using UnityEngine;

namespace Root {
    [RequireComponent(typeof(Inventory))]
    public class InteractableInventory : InteractableNormalCamera {
        public override void Interact() {
            GameManager.PlayerInventoryUI.OpenInventory(GetComponent<Inventory>());
        }
    }
}