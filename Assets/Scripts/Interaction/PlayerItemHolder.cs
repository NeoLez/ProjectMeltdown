using UnityEngine;

namespace Root
{
    public class PlayerItemHolder : MonoBehaviour
    {
        [SerializeField] private Transform holdPoint;
        [SerializeField] private float dropDistance = 1.5f;

        private Transform oldParent;
        private GameObject currentHeldVisual;

        [field: SerializeField] public PickupItem HeldItem { get; private set; }

        public bool HasItem => HeldItem != null;

        public void Pickup(PickupItem item)
        {
            if (HasItem)
                return;

            HeldItem = item;

            VisualContainer visualContainer = item.GetComponent<VisualContainer>();

            if (visualContainer != null)
            {
                visualContainer.visuals.layer = LayerMask.NameToLayer("GrabObject");
                visualContainer.visuals.SetActive(false);
            }

            Rigidbody rb = item.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            Collider[] colliders = item.GetComponentsInChildren<Collider>();

            foreach (var col in colliders)
                col.enabled = false;

            oldParent = item.transform.parent;

            if (item.HeldVisualPrefab != null)
            {
                currentHeldVisual = Instantiate(
                    item.HeldVisualPrefab,
                    holdPoint
                );

                currentHeldVisual.transform.localPosition = Vector3.zero;
                currentHeldVisual.transform.localRotation = Quaternion.identity;
            }
        }

        public void Drop()
        {
            if (!HasItem)
                return;

            if (currentHeldVisual != null)
                Destroy(currentHeldVisual);

            HeldItem.transform.position =
                GameManager.Camera.transform.position +
                GameManager.Camera.transform.forward * dropDistance;

            HeldItem.transform.SetParent(oldParent);

            VisualContainer visualContainer =
                HeldItem.GetComponent<VisualContainer>();

            if (visualContainer != null)
            {
                visualContainer.visuals.SetActive(true);
                visualContainer.visuals.layer =
                    LayerMask.NameToLayer("NotGrabbedObject");
            }

            Collider[] colliders =
                HeldItem.GetComponentsInChildren<Collider>();

            foreach (var col in colliders)
                col.enabled = true;

            Rigidbody rb = HeldItem.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            HeldItem = null;
            currentHeldVisual = null;
        }

        public void ForceClearHeldItem()
        {
            if (currentHeldVisual != null)
                Destroy(currentHeldVisual);

            if (HeldItem != null)
            {
                VisualContainer visualContainer =
                    HeldItem.GetComponent<VisualContainer>();

                if (visualContainer != null)
                {
                    visualContainer.visuals.SetActive(true);
                    visualContainer.visuals.layer =
                        LayerMask.NameToLayer("NotGrabbedObject");
                }

                HeldItem.transform.SetParent(oldParent);
            }

            HeldItem = null;
            currentHeldVisual = null;
        }
    }
}