using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Root
{
    public class PlayerItemHolder : MonoBehaviour
    {
        [SerializeField] private Transform holdPoint;
        [SerializeField] private float dropDistance = 1.5f;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private AudioClip cantMoveToInventorySound;
        public event Action OnItemChanged;
        
        private void Awake() {
            HeldItem = null;
            GameManager.Input.Inventory.PutHeldInInventory.performed += SaveHeldItem;
        }

        private GameObject currentHeldVisual;

        public ItemState HeldItem { get; private set; }

        public bool HasItem => HeldItem != null;

        private PackageData _currentPackage = null;
        public void Pickup(PhysicalItem item)
        {
            if (HasItem && HeldItem.ItemSo)
                Drop();

            if(!GameManager.Train.IsStopped())
                GameManager.Train.RemoveObjectFromContainers(item.GetComponent<VisualContainer>());
            
            HeldItem = item.itemState;
            if (item.TryGetComponent(out StoreItemDisplay itemDisplay)) itemDisplay.OnInteraction?.Invoke();

            if (item.TryGetComponent(out DeliveryPackageItem deliveryPackage)) _currentPackage = deliveryPackage.PackageData;

            Destroy(item.gameObject);
            
            if (item.itemState.ItemSo.HeldItemGameObject == null) return;
            currentHeldVisual = Instantiate(item.itemState.ItemSo.HeldItemGameObject, holdPoint);
            currentHeldVisual.transform.localPosition = Vector3.zero;
            currentHeldVisual.transform.localRotation = Quaternion.identity;
            
            OnItemChanged?.Invoke();
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
            
            OnItemChanged?.Invoke();
        }
        

        public void Drop()
        {
            if (!HasItem)
                return;

            var physicalItem = HeldItem.ItemSo.CreatePhysicalItem();
            physicalItem.itemState = HeldItem;

            var deliveryPackage = physicalItem.GetComponent<DeliveryPackageItem>();

            if (deliveryPackage) deliveryPackage.SetPackageData(_currentPackage);

            if (CheckIfDeliveryPostNearby(out var deliveryPost) && deliveryPackage)
            {
                deliveryPost.DepositPackage(deliveryPackage);
            }
            else
            {
                physicalItem.transform.position =
                cameraPivot.position +
                cameraPivot.forward * dropDistance;
            }

            if(!GameManager.Train.IsStopped())
                GameManager.Train.AddObjectToContainers(physicalItem.GetComponent<VisualContainer>());
            
            HeldItem = null;
            _currentPackage = null;
            if (currentHeldVisual != null)
                Destroy(currentHeldVisual);
            
            OnItemChanged?.Invoke();

        }

        public bool CheckIfDeliveryPostNearby(out PackageDeliverPost packagePost) 
        {
            packagePost = null;
            return Physics.Raycast(cameraPivot.position, cameraPivot.forward, out var hit, 7f) && hit.collider.gameObject.TryGetComponent(out packagePost);
        }

        private void SaveHeldItem(InputAction.CallbackContext _) {
            if (!HasItem) return;
            if(!GetComponent<Inventory>().InsertItem(HeldItem)) {
                GameManager.AudioSystem.PlaySound(cantMoveToInventorySound, GameManager.AudioSystem.VFX);
                return;
            }
            OnItemChanged?.Invoke();
            ForceClearHeldItem();
        }

        public void ForceClearHeldItem() {
            if (HeldItem == null) return;
            HeldItem = null;
            Destroy(currentHeldVisual);
            OnItemChanged?.Invoke();
        }

        private void OnDestroy() {
            GameManager.Input.Inventory.PutHeldInInventory.performed -= SaveHeldItem;
        }
    }
}