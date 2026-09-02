using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class CheckPackages : MonoBehaviour
    {
        [SerializeField] private int amountOfPackagesToDeliver;
        private List<PackageController> packages = new();
        private int _currentSum;

        private void CheckGoal()
        {
            if (amountOfPackagesToDeliver == _currentSum)
            {
                //Ui de victoria
                PackagesSystemController.Instance.CheckPackageConditions();
                return;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PackageController packageController))
            {
                if (!packages.Contains(packageController))
                {
                    packages.Add(packageController);
                    _currentSum++;
                    //hacer que no puedas agarrarlos mas una vez puestos?

                    CheckGoal();
                }
                
            }
        }
    }
}
