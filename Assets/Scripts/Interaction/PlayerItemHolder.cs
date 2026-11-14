using System;
using Root.Handheld;
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

        private HandheldItem currentHeldVisual;

        private bool _canLauchItem = true;
        private void Awake()
        {
            GameManager.ItemHolder = this;
            HeldItem = null;
            GameManager.Input.Inventory.PutHeldInInventory.performed += SaveHeldItem;
            GameManager.Input.Inventory.DropItem.performed += Drop;
            GameManager.Input.Handheld.LeftClick.performed += UseItem;
        }

        private void Update()
        {
            if (!HasItem)
                crosshair.fillAmount = 0;
            else
                crosshair.fillAmount = GameManager.Input.Inventory.PutHeldInInventory.GetTimeoutCompletionPercentage();;
        }

        public bool Pickup(PhysicalItem item)
        {
            if (!WalletController.CanInteract) return false;
            if (HasItem && HeldItem.ItemSo) {
                if(GetComponent<Inventory>().InsertItem(item.ItemState)) {
                    PoolManager.ReturnObjectToPool(item.gameObject.GetComponent<Poolable>());
                    return true;
                }
                
                GameManager.AudioSystem.PlaySound(cantMoveToInventorySound, GameManager.AudioSystem.VFX);
                return false;
            }

            if(!GameManager.Train.IsStopped())
                GameManager.Train.RemoveObjectFromContainers(item.GetComponent<VisualContainer>());
            
            HeldItem = item.ItemState;

            UpdateVisuals();

            PoolManager.ReturnObjectToPool(item.gameObject.GetComponent<Poolable>());

            OnItemChanged?.Invoke();
            return true;
        }

        public bool Pickup(ItemState item)
        {
            if (!WalletController.CanInteract) return false;
            if (HasItem && HeldItem.ItemSo) {
                if(GetComponent<Inventory>().InsertItem(item)) {
                    return true;
                }

                GameManager.AudioSystem.PlaySound(cantMoveToInventorySound, GameManager.AudioSystem.VFX);
                return false;
            }
            
            HeldItem = item;

            UpdateVisuals();
            
            OnItemChanged?.Invoke();
            return true;
        }

        public void PickupOrDrop(PhysicalItem item)
        {
            if (!Pickup(item))
            {
                var itemState = item.ItemState;
                PoolManager.ReturnObjectToPool(item.gameObject.GetComponent<Poolable>());
                Drop(itemState);
            }
        }
        
        public void PickupOrDrop(ItemState item)
        {
            if (!Pickup(item))
            {
                Drop(item);
            }
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
                Destroy(currentHeldVisual.gameObject);
            
            OnItemChanged?.Invoke();
            CanLaunchItem(true);
        }
        
        public void Drop(ItemState item)
        {

            var physicalItem = item.ItemSo.CreatePhysicalItem(item);
            physicalItem.transform.parent = null;
            
            var rbItem = physicalItem.GetComponent<Rigidbody>();
            
            rbItem.AddForce(cameraPivot.forward * _throwStrenght, ForceMode.Force);
            physicalItem.transform.position = cameraPivot.position + cameraPivot.forward * dropDistance;

            if (!GameManager.Train.IsStopped())
                GameManager.Train.AddObjectToContainers(physicalItem.GetComponent<VisualContainer>());
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

        private void UseItem(InputAction.CallbackContext ctx) {
            if(currentHeldVisual != null)
                currentHeldVisual.Use(ctx);
        }

        public void CanLaunchItem(bool state)
        {
            _canLauchItem = state;
        }

        public void ForceClearHeldItem() {
            if (HeldItem == null) return;
            HeldItem = null;
            if(currentHeldVisual != null)
                Destroy(currentHeldVisual.gameObject);
            OnItemChanged?.Invoke();
        }

        public void ForceSetHeldItem(ItemState item)
        {
            HeldItem = item;
            
            UpdateVisuals();
            
            OnItemChanged?.Invoke();
        }

        private void UpdateVisuals()
        {
            if (HeldItem.ItemSo.HeldItemGameObject != null)
            {
                currentHeldVisual = Instantiate(HeldItem.ItemSo.HeldItemGameObject, holdPoint);
                currentHeldVisual.transform.localPosition = Vector3.zero;
                currentHeldVisual.transform.localRotation = Quaternion.identity;
                currentHeldVisual.state = HeldItem;
                
                if (packages.IsItemIncluded(HeldItem.ItemSo)) {
                    PackageStampGenerator.Instance.SetStampTexture(currentHeldVisual, (PackageItemState)HeldItem);
                }
            }
        }
        
        private void OnDestroy() {
            GameManager.Input.Inventory.PutHeldInInventory.performed -= SaveHeldItem;
        }
    }
}