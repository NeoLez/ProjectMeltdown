using UnityEngine;
using UnityEngine.InputSystem;

namespace Root
{
    public class PlayerItemHolder : MonoBehaviour
    {
        [SerializeField] private Transform holdPoint;
        [SerializeField] private float dropDistance = 1.5f;
        [SerializeField] private Transform cameraPivot;

        private void Awake() {
            HeldItem = null;
            GameManager.Input.Inventory.PutHeldInInventory.performed += SaveHeldItem;
        }

        private GameObject currentHeldVisual;

        public ItemState HeldItem { get; private set; }

        public bool HasItem => HeldItem != null;

        public void Pickup(PhysicalItem item)
        {
            if (HasItem && HeldItem.ItemSo)
                Drop();

            if(!GameManager.Train.IsStopped())
                GameManager.Train.RemoveObjectFromContainers(item.GetComponent<VisualContainer>());
            
            HeldItem = item.itemState;
            Destroy(item.gameObject);
            
            if (item.itemState.ItemSo.HeldItemGameObject == null) return;
            currentHeldVisual = Instantiate(item.itemState.ItemSo.HeldItemGameObject, holdPoint);
            currentHeldVisual.transform.localPosition = Vector3.zero;
            currentHeldVisual.transform.localRotation = Quaternion.identity;
        }
        
        public void Pickup(ItemState item)
        {
            if (HasItem && HeldItem.ItemSo)
                Drop();
            
            HeldItem = item;
            
            if (item.ItemSo.HeldItemGameObject == null) return;
            currentHeldVisual = Instantiate(item.ItemSo.HeldItemGameObject, holdPoint);
            currentHeldVisual.transform.localPosition = Vector3.zero;
            currentHeldVisual.transform.localRotation = Quaternion.identity;
        }
        

        public void Drop()
        {
            if (!HasItem)
                return;
            
            var physicalItem = HeldItem.ItemSo.CreatePhysicalItem();
            physicalItem.itemState = HeldItem;
            
            physicalItem.transform.position =
                cameraPivot.position +
                cameraPivot.forward * dropDistance;

            if(!GameManager.Train.IsStopped())
                GameManager.Train.AddObjectToContainers(physicalItem.GetComponent<VisualContainer>());
            
            HeldItem = null;
            if (currentHeldVisual != null)
                Destroy(currentHeldVisual);
        }

        private void SaveHeldItem(InputAction.CallbackContext _) {
            if (!HasItem) return;
            if(GetComponent<Inventory>().InsertItem(HeldItem))
                ForceClearHeldItem();
        }

        public void ForceClearHeldItem()
        {
            HeldItem = null;
            if (currentHeldVisual != null)
                Destroy(currentHeldVisual);
        }
    }
}