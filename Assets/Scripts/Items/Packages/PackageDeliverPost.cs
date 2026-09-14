using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Root
{
    public class PackageDeliverPost : MonoBehaviour
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
    }
}
