using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class PackagesSystemController : MonoBehaviour
    {
        public static List<PackageController> packages = new();

        private Dictionary<string, int> _packagePricesDict = new();

        private int _packagePriceSum;

        private void Start()
        {
            if (packages.Count > 0)
            {
                foreach (PackageController package in packages)
                {
                    package.InitializePackageData(package.GetSO().PackageRandomPriceGenerator(), package.GetSO().PackageDurabilityLevel);
                    _packagePricesDict.Add(package.GetSO().GenerateUniqueID(), package.GetSO().GetGeneratedNumber());
                }
            }
        }

        public void CheckData(PackageController package)
        {
            if (packages.Contains(package))
            {
                if (_packagePricesDict.TryGetValue(package.GetSO().PackageID, out var generatedPrice))
                {
                    package.InitializePackageData(generatedPrice, package.GetSO().PackageDurabilityLevel);
                }
            }        
        }

        public void CheckPackageConditions()
        {
            //chequear estado de los paquetes y que dependiend de su vida, te de un porcenataje extra de dinero más uno de base

        }

        public void SumCurrentPackages()
        {
            foreach (PackageController package in packages)
            {
               _packagePriceSum += package.GetPrice();
            }

            //injectar estos valores a una UI
        }
        //una funcion que suma los precios de todos lo paquetes para sacar un promedio
        //solo llamarla al final de completar la mision

        //prevencion contemporanea
        private void OnDestroy()
        {
            CleanReferences();
        }

        private void CleanReferences()
        {
            if (packages.Count > 0)
            {
                packages.Clear();
            }
            _packagePricesDict.Clear();
        }
    }
}
