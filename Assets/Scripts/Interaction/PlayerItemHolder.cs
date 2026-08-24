using UnityEngine;

namespace Root
{
    public class PlayerItemHolder : MonoBehaviour
    {
        [SerializeField] private Transform holdPoint;
        [SerializeField] private float dropDistance = 1.5f;
        [SerializeField] private Transform cameraPivot;

        private void Awake() {
            HeldItem = null;
        }

        private GameObject currentHeldVisual;

        public ItemState HeldItem { get; private set; }

        public bool HasItem => HeldItem != null;

        public void Pickup(PhysicalItem item)
        {
            if (HasItem && HeldItem.ItemSo)
                Drop();

            HeldItem = item.itemState;
            Debug.Log(HeldItem);
            Destroy(item.gameObject);
            
            if (item.itemState.ItemSo.HeldItemGameObject == null) return;
            currentHeldVisual = Instantiate(item.itemState.ItemSo.HeldItemGameObject, holdPoint);
            currentHeldVisual.transform.localPosition = Vector3.zero;
            currentHeldVisual.transform.localRotation = Quaternion.identity;
        }
        

        public void Drop()
        {
            Debug.Log("Drop");
            if (!HasItem)
                return;
            
            Debug.Log(HeldItem);
            
            var physicalItem = HeldItem.ItemSo.CreatePhysicalItem();
            physicalItem.itemState = HeldItem;
            
            physicalItem.transform.position =
                cameraPivot.position +
                cameraPivot.forward * dropDistance;

            HeldItem = null;
            if (currentHeldVisual != null)
                Destroy(currentHeldVisual);
        }

        public void ForceClearHeldItem()
        {
            HeldItem = null;
            if (currentHeldVisual != null)
                Destroy(currentHeldVisual);
        }
    }
}