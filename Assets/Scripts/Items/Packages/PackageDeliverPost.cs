using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Root
{
    public class PackageDeliverPost : InteractableNormalCamera, IItemDragReceiver
    {
        [SerializeField] private int amountOfPackagesToDeliver;
        [SerializeField] private Transform dropPivot;
        [SerializeField] private Animator animator;
        [SerializeField] private TMP_Text priceCounter;
        public Transform DropPivot => dropPivot;

        private string _format = "{0}$";

        private List<int> _depositedPackages = new();
        private int _currentPackageSum;

        private int _animStateOpen = Animator.StringToHash("OpenDepositDoor");
        private int _animStateClose = Animator.StringToHash("CloseDepositDoor");

        private bool _isAnimating;
        private bool _hasCompletedGoal;

        private void Start()
        {
            RefreshSumAmount(0);
        }

        public void MissionsCheck(List<MissionObjectiveSO> activeMissions)
        {
            //aca registro en mis variable locales todo su info
            //cuando termino de entregar, reemplazo los valores de esas variables por la "sigueinte mision"
        }

        public void DepositPackage(DeliveryPackageItem packageController)
        {
            if (_hasCompletedGoal) return;
            if (_isAnimating) return;
            
            StartCoroutine(TriggerDepositAnims());

            _depositedPackages.Add(1);
            RefreshSumAmount(packageController.GetPrice());
            Destroy(packageController.gameObject, 0.5f);

            CheckGoal();
        }
        
        public void DepositPackage(PackageItemState itemState)
        {
            if (_hasCompletedGoal) return;
            if (_isAnimating) return;
            
            StartCoroutine(TriggerDepositAnims());

            _depositedPackages.Add(1);
            RefreshSumAmount(itemState.price);

            CheckGoal();
        }
        
        private void CheckGoal()
        {
            if (amountOfPackagesToDeliver == _depositedPackages.Count)
            {
                PackagesSystemController.Instance.CheckPackageConditions();
                _hasCompletedGoal = true;
                return;
            }
        }

        private IEnumerator TriggerDepositAnims()
        {
            _isAnimating = true;
            animator.SetTrigger(_animStateOpen);
            yield return new WaitForSeconds(1);
            animator.SetTrigger(_animStateClose);
            _isAnimating = false;
        }

        private void RefreshSumAmount(int amount)
        {
            _currentPackageSum += amount;
            priceCounter.text = string.Format(_format, _currentPackageSum);

            PackagesSystemController.Instance.SumCurrentDeposited(_currentPackageSum);
        }

        public bool HasReachedDepositGoal()
        {
            return _hasCompletedGoal;
        }

        private void OnDestroy()
        {
            _depositedPackages.Clear();
        }

        public override void Interact()
        {
            PlayerItemHolder holder = GameManager.Player.GetComponent<PlayerItemHolder>();

            if (holder == null) return;

            if (!holder.HasItem) return;
            var itemState = holder.HeldItem as PackageItemState;
            if (itemState == null) return;
            
            DepositPackage(itemState);
            holder.ForceClearHeldItem();
        }

        public bool CanTakeItem(Vector2 position, Vector2Int size, InventoryItem item) {
            return item.itemState is PackageItemState && !_isAnimating && !_hasCompletedGoal;
        }

        public bool TakeItem(Vector2 position, InventoryItem.InventoryItemRotation rotation, InventoryItem item) {
            if(!CanTakeItem(position, InventoryItem.GetRotationCorrectedSize(item.Size, rotation), item)) return false;
            
            DepositPackage(item.itemState as PackageItemState);
            return true;
        }

        public void ClearFeedback() {
        }
    }
}
