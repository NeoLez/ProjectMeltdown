using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Root
{
    public class PackageDeliverPost : InteractableNormalCamera, IItemDragReceiver
    {
        public bool HasCompletedGoal { get; private set; }

        [SerializeField] MissionObjectiveSO thisIshorrible;
        [SerializeField] private Transform dropPivot;
        [SerializeField] private Animator animator;
        [SerializeField] private TMP_Text priceCounter;
        [SerializeField] private TMP_Text textNotifier;
        public Transform DropPivot => dropPivot;

        private string _format = "{0}$";

        private List<PackageItemState> _depositedPackages = new();
        private int _currentPackageSum;

        private int _animStateOpen = Animator.StringToHash("OpenDepositDoor");
        private int _animStateClose = Animator.StringToHash("CloseDepositDoor");

        private bool _isAnimating;

        public Action<bool> OnPackagesDelivered;
        public List<TypeOfPackage> _packagesType = new();
        private int _amount;

        private void Start()
        {
            RefreshSumAmount(0);
        }

        public void MissionsCheck(List<MissionObjectiveSO> activeMissions)
        {
            //aca registro en mis variable locales todo su info
            //cuando termino de entregar, reemplazo los valores de esas variables por la "sigueinte mision"
        }

        public void DepositPackage(PackageItemState itemState)
        {
            if (HasCompletedGoal) return;
            if (_isAnimating) return;

            StartCoroutine(TriggerDepositAnims());

            RefreshSumAmount(itemState.price);
            if (!_depositedPackages.Contains(itemState))
            {
                _depositedPackages.Add(itemState);
            }
            _packagesType.Add(itemState.typeOfPackage);
            _amount++;
        }

        public void CheckGoal()
        {
            if(MissionsManager.Instance.VerifyDeliveryConditions(thisIshorrible.Id, _amount, _packagesType))
            {
                PackagesSystemController.Instance.CheckPackageConditions(true);
                HasCompletedGoal = true;
            }
            else
            {
                PackagesSystemController.Instance.CheckPackageConditions(false, _depositedPackages);
            }

            StartCoroutine(UpdateTextRoutine());

            OnPackagesDelivered?.Invoke(true); //si yo tengo otros paquetes que entregar, lo pongo en false asi puedo volver a presionar el boton
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


        private IEnumerator UpdateTextRoutine() 
        {
            priceCounter.enabled = false;
            textNotifier.text = "Paquetes depositados"; //TODO-Change to Localization
            yield return new WaitForSeconds(2f);
            ResetPostStatus();
        }

        private void ResetPostStatus()
        {
            _depositedPackages.Clear();

            _currentPackageSum = 0;
            RefreshSumAmount(0);
            priceCounter.enabled = true;
            textNotifier.text = "Monto Total: "; //TODO-Change to Localization
        }

        public int DepositedPackages()
        {
            return _depositedPackages.Count;
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

        public bool CanTakeItem(Vector2 position, Vector2Int size, InventoryItem item)
        {
            return item.itemState is PackageItemState && !_isAnimating && !HasCompletedGoal;
        }

        public bool TakeItem(Vector2 position, InventoryItem.InventoryItemRotation rotation, InventoryItem item)
        {
            if (!CanTakeItem(position, InventoryItem.GetRotationCorrectedSize(item.Size, rotation), item)) return false;

            DepositPackage(item.itemState as PackageItemState);
            return true;
        }

        public void ClearFeedback()
        {
        }
    }
}
