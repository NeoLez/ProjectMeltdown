using UnityEngine;

namespace Root
{
    public class PickupItem : MonoBehaviour
    {
        [SerializeField] private ItemType itemType;

        public ItemType ItemType => itemType;
    }
}