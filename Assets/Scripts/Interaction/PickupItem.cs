using UnityEngine;

namespace Root
{
    public class PickupItem : MonoBehaviour
    {
        [SerializeField] private ItemType itemType;
        [SerializeField] private GameObject heldVisualPrefab;
        private bool hasBeenPicked;

        public ItemType ItemType => itemType;
        public GameObject HeldVisualPrefab => heldVisualPrefab;

        public void HasBeenPickedUp() {
            if (!hasBeenPicked) transform.parent.parent = null;
            hasBeenPicked = true;
        }
    }
}