using UnityEngine;

namespace Root
{
    public class PlayerItemHolder : MonoBehaviour
    {
        [SerializeField] private Transform holdPoint;

        public PickupItem HeldItem { get; private set; }

        public bool HasItem => HeldItem != null;

        public void Pickup(PickupItem item)
        {
            if (HasItem)
                return;

            HeldItem = item;

            Rigidbody rb = item.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            Collider[] colliders =
                item.GetComponentsInChildren<Collider>();

            foreach (var col in colliders)
                col.enabled = false;

            item.transform.SetParent(holdPoint);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }

        public void Drop()
        {
            if (!HasItem)
                return;

            Rigidbody rb =
                HeldItem.GetComponent<Rigidbody>();

            Collider[] colliders =
                HeldItem.GetComponentsInChildren<Collider>();

            foreach (var col in colliders)
                col.enabled = true;

            HeldItem.transform.SetParent(null);

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            HeldItem = null;

        }

        public void ForceClearHeldItem()
        {
            if (HeldItem != null)
                HeldItem.transform.SetParent(null);

            HeldItem = null;
        }
    }
}