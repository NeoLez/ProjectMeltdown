using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Root
{
    public class PackageDeliverPost : MonoBehaviour
    {
        [SerializeField] private int amountOfPackagesToDeliver;
        [SerializeField] private Transform dropPivot;

        [SerializeField] private TMP_Text priceCounter;
        private string _format = "{0}$";
        public Transform DropPivot => dropPivot;

        private Dictionary<string, DeliveryPackageItem> _depositedPackages = new();
        private int _currentSum;

        private int _currentPackageSum;

        private void Start()
        {
            RefreshSumAmount(0);
        }

        private void CheckGoal()
        {
            if (amountOfPackagesToDeliver == _currentSum)
            {
                PackagesSystemController.Instance.CheckPackageConditions();
                return;
            }
        }

        /* private void OnTriggerEnter(Collider other)
         {
             if (other.TryGetComponent(out DeliveryPackageItem packageController))
             {
                 if (IsPackageDeposited(packageController))
                 {
                     _depositedPackages.Add(packageController.GetSO().PackageID, packageController);
                     _currentSum++;
                     Destroy(packageController.gameObject, 0.5f);

                     CheckGoal();
                 }

             }
         }*/

        public void DepositPackage(DeliveryPackageItem packageController)
        {
            if (IsPackageDeposited(packageController))
            {
                _depositedPackages.Add(packageController.GetSO().PackageID, packageController);
                _currentSum++;

                RefreshSumAmount(packageController.GetPrice());
                Destroy(packageController.gameObject, 0.5f);

                CheckGoal();
            }
        }

        private void RefreshSumAmount(int amount)
        {
            _currentPackageSum += amount;
            priceCounter.text = string.Format(_format, _currentPackageSum);
        }

        private bool IsPackageDeposited(DeliveryPackageItem packageController)
        {
            if (!_depositedPackages.TryGetValue(packageController.GetSO().PackageID, out var generatedPrice))
            {
                return true;
            }
            return false;
        }

        private void OnDestroy()
        {
            _depositedPackages.Clear();
        }
    }
}
