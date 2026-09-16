using System;
using System.Collections.Generic;
using Root.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Root
{
    public class PlayerItemHolder : MonoBehaviour
    {
        [SerializeField] private Transform holdPoint;
        [SerializeField] private float dropDistance = 1.5f;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private AudioClip cantMoveToInventorySound;
        [SerializeField] private Image crosshair;
        [SerializeField] private float _throwStrenght = 999f;

        public event Action OnItemChanged;
        public ItemState HeldItem { get; private set; }

        public bool HasItem => HeldItem != null;

        private GameObject currentHeldVisual;

        private Dictionary<string, PackageData> _currentPackageData = new();

        private void Awake()
        {
            HeldItem = null;
            GameManager.Input.Inventory.PutHeldInInventory.performed += SaveHeldItem;
        }

        private void Update()
        {
            if (!HasItem)
            {
                crosshair.fillAmount = 0;
                return;
            }
            if (Input.GetKeyUp(KeyCode.R)) { crosshair.fillAmount = 0; }
            else if (Input.GetKey(KeyCode.R))
            {
                crosshair.fillAmount += 3f * Time.deltaTime;
            }
        }

        public void Pickup(PhysicalItem item)
        {
            if (HasItem && HeldItem.ItemSo)
                Drop();

            if(!GameManager.Train.IsStopped())
                GameManager.Train.RemoveObjectFromContainers(item.GetComponent<VisualContainer>());
            
            HeldItem = item.itemState;
            if (item.TryGetComponent(out StoreItemDisplay itemDisplay)) itemDisplay.OnInteraction?.Invoke();

            if (item.TryGetComponent(out DeliveryPackageItem deliveryPackage))
            {
                _currentPackageData.Add(deliveryPackage.PackageData.Id, deliveryPackage.PackageData);
            }
            
            if (item.itemState.ItemSo.HeldItemGameObject == null) return;
            currentHeldVisual = Instantiate(item.itemState.ItemSo.HeldItemGameObject, holdPoint);
            currentHeldVisual.transform.localPosition = Vector3.zero;
            currentHeldVisual.transform.localRotation = Quaternion.identity;
            
            PoolManager.ReturnObjectToPool(item.gameObject.GetComponent<Poolable>());
            
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
            physicalItem.transform.parent = null;
            Debug.Log("wtf", physicalItem);
            var rbItem = physicalItem.GetComponent<Rigidbody>();
            var deliveryPackage = physicalItem.GetComponent<DeliveryPackageItem>();

            if (deliveryPackage && _currentPackageData.TryGetValue(deliveryPackage.PackageData.Id, out PackageData data))
            {
                deliveryPackage.SetPackageData(data);
                _currentPackageData.Remove(deliveryPackage.PackageData.Id);
            }

            if (CheckIfDeliveryPostNearby(out var deliveryPost) && deliveryPackage)
            {
                deliveryPost.DepositPackage(deliveryPackage);
            }
            else
            {
                rbItem.AddForce(cameraPivot.forward * _throwStrenght, ForceMode.Force);
                physicalItem.transform.position =
                cameraPivot.position +
                cameraPivot.forward * dropDistance;
            }

            if(!GameManager.Train.IsStopped())
                GameManager.Train.AddObjectToContainers(physicalItem.GetComponent<VisualContainer>());
            
            HeldItem = null;
            if (currentHeldVisual != null)
                Destroy(currentHeldVisual);
            
            OnItemChanged?.Invoke();

        }

        public bool CheckIfDeliveryPostNearby(out PackageDeliverPost packagePost) 
        {
            packagePost = null;
            return Physics.Raycast(cameraPivot.position, cameraPivot.forward, out var hit, 7f) && hit.collider.gameObject.TryGetComponent(out packagePost) && !packagePost.HasReachedDepositGoal();
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
            _currentPackageData.Clear();
        }
    }
}