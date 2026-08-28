using UnityEngine;

namespace Root {
    [RequireComponent(typeof(Inventory))]
    public class CrateInventory : InteractableNormalCamera {
        public override void Interact() {
            GameManager.PlayerInventoryUI.OpenInventory(GetComponent<Inventory>());
        }
    }
}