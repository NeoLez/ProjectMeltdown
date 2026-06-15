using UnityEngine;

namespace Root
{
    public class PickupItem : MonoBehaviour
    {
        [SerializeField] private ItemType itemType;
        [SerializeField] private GameObject heldVisualPrefab;

        public ItemType ItemType => itemType;
        public GameObject HeldVisualPrefab => heldVisualPrefab;
    }
}