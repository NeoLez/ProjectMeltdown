using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class PackageDeliverPost : MonoBehaviour
    {
        [SerializeField] private int amountOfPackagesToDeliver;
        [SerializeField] private Transform dropPivot;
        public Transform DropPivot => dropPivot;

        private Dictionary<string, PackageController> _depositedPackages = new();
        private int _currentSum;

        private void CheckGoal()
        {
            if (amountOfPackagesToDeliver == _currentSum)
            {
                PackagesSystemController.Instance.CheckPackageConditions();
                return;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PackageController packageController))
            {
                if (CheckDepositedPackages(packageController))
                {
                    _depositedPackages.Add(packageController.GetSO().PackageID, packageController);
                    _currentSum++;

                    CheckGoal();
                }
                
            }
        }

        private bool CheckDepositedPackages(PackageController packageController)
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
