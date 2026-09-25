using System;
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
        [SerializeField] private ItemGroup packages;

        public event Action OnItemChanged;
        public ItemState HeldItem { get; private set; }

        public bool HasItem => HeldItem != null;

        private GameObject currentHeldVisual;

        private bool _canLauchItem = true;
        private void Awake()
        {
            GameManager.ItemHolder = this;
            HeldItem = null;
            GameManager.Input.Inventory.PutHeldInInventory.performed += SaveHeldItem;
            GameManager.Input.Inventory.DropItem.performed += Drop;
        }

        private void Update()
        {
            if (!HasItem)
            {
                crosshair.fillAmount = 0;
                return;
            }
            if (GameManager.Input.Inventory.PutHeldInInventory.WasReleasedThisFrame()) { crosshair.fillAmount = 0; }
            else if (GameManager.Input.Inventory.PutHeldInInventory.IsPressed())
            {
                crosshair.fillAmount += Time.deltaTime / 0.6f;
            }
        }

        public void Pickup(PhysicalItem item)
        {
            if (!WalletController.CanInteract) return;
            if (HasItem && HeldItem.ItemSo)
                Drop();

            if(!GameManager.Train.IsStopped())
                GameManager.Train.RemoveObjectFromContainers(item.GetComponent<VisualContainer>());
            
            HeldItem = item.ItemState;
            if (item.TryGetComponent(out StoreItemDisplay itemDisplay)) itemDisplay.OnInteraction?.Invoke();

            if (item.ItemState.ItemSo.HeldItemGameObject == null) return;
            currentHeldVisual = Instantiate(item.ItemState.ItemSo.HeldItemGameObject, holdPoint);
            currentHeldVisual.transform.localPosition = Vector3.zero;
            currentHeldVisual.transform.localRotation = Quaternion.identity;

            if (item.TryGetComponent(out DeliveryPackageItem packageItem)) PackageStampGenerator.Instance.SetStampTexture(currentHeldVisual, packageItem.State);

            PoolManager.ReturnObjectToPool(item.gameObject.GetComponent<Poolable>());

            OnItemChanged?.Invoke();
        }

        public void Pickup(ItemState item)
        {
            if (!WalletController.CanInteract) return;
            if (HasItem && HeldItem.ItemSo)
                Drop();
            
            HeldItem = item;

            if (item.ItemSo.HeldItemGameObject == null) return;
            currentHeldVisual = Instantiate(item.ItemSo.HeldItemGameObject, holdPoint);
            currentHeldVisual.transform.localPosition = Vector3.zero;
            currentHeldVisual.transform.localRotation = Quaternion.identity;

            if (packages.IsItemIncluded(item.ItemSo)) {
                PackageStampGenerator.Instance.SetStampTexture(currentHeldVisual, (PackageItemState)item);
            }
            
            OnItemChanged?.Invoke();
        }


        public void Drop()
        {
            if (!HasItem)
                return;

            var physicalItem = HeldItem.ItemSo.CreatePhysicalItem(HeldItem);
            physicalItem.transform.parent = null;
            
            var rbItem = physicalItem.GetComponent<Rigidbody>();

            if(_canLauchItem)
            {
                rbItem.AddForce(cameraPivot.forward * _throwStrenght, ForceMode.Force);
                physicalItem.transform.position =
                cameraPivot.position +
                cameraPivot.forward * dropDistance;
            }

            if (!GameManager.Train.IsStopped())
                GameManager.Train.AddObjectToContainers(physicalItem.GetComponent<VisualContainer>());
            
            HeldItem = null;
            if (currentHeldVisual != null)
                Destroy(currentHeldVisual);
            
            OnItemChanged?.Invoke();
            CanLaunchItem(true);
        }

        private void Drop(InputAction.CallbackContext _) {
            Drop();
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

        public void CanLaunchItem(bool state)
        {
            _canLauchItem = state;
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