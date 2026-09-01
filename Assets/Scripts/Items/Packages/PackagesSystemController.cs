using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class PackagesSystemController : MonoBehaviour
    {
        public static List<PackageController> packages = new();

        Dictionary<string, int> packagePricesDict = new();

        private void Start()
        {
            if (packages.Count > 0)
            {
                foreach (PackageController package in packages)
                {
                    package.InitializePackageData(package.GetSO().PackageRandomPriceGenerator(), package.GetSO().PackageDurabilityLevel);
                    packagePricesDict.Add(package.GetSO().PackageID, package.GetSO().GetGeneratedNumber());
                }
            }
        }

        public void CheckData(PackageController package)
        {
            if (packages.Contains(package))
            {
                if (packagePricesDict.TryGetValue(package.GetSO().PackageID, out var generatedPrice))
                {
                    package.InitializePackageData(generatedPrice, package.GetSO().PackageDurabilityLevel);
                }
            }        
        }

        //prevencion contemporanea
        private void OnDestroy()
        {
            if (packages.Count > 0)
            {
                packages.Clear();
            }
            packagePricesDict.Clear();
        }
    }
}
