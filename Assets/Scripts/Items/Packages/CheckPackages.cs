using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class CheckPackages : MonoBehaviour
    {
        [SerializeField] private int amountOfPackagesToDeliver;
        private List<PackageController> packages;
        private int _currentSum;

        private void CheckGoal()
        {
            if (amountOfPackagesToDeliver == _currentSum)
            {
                //Ui de victoria
            }
        }
        //un trigger que se encargue de chequear la cantidad de paquetes, su estado y darte dinero
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PackageController packageController))
            {
                if (!packages.Contains(packageController)) //crear una lista para que no se sumen si los vuelvo a dropear 
                {
                    packages.Add(packageController);
                    _currentSum++;

                    CheckGoal();
                }
                
            }
        }
    }
}
