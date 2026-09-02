using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class PackagesSystemController : MonoBehaviour
    {
        public static PackagesSystemController Instance;

        private Dictionary<string, int> _packagePricesDict = new();
        private int _packagePriceSum;
        private List<PackageController> _currentSpawnedPackages = new();

        [SerializeField] private Transform instancePivot;
        [SerializeField] private GameObject[] packagesToDeliver;
        [SerializeField] PackageTrigger _visuals;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            _visuals.ChangeCanvas(false);
            GeneratePackages();
        }

        public void GeneratePackages()
        {
            for (int i = 0; i < packagesToDeliver.Length; i++)
            {
                GameObject prefab = Instantiate(packagesToDeliver[i], instancePivot, true);

                PackageController currentPackage = prefab.GetComponent<PackageController>();
                _currentSpawnedPackages.Add(currentPackage);
            }

            PackagesCheck();
        }

        public void PackagesCheck()
        {
            if (_currentSpawnedPackages.Count > 0)
            {
                foreach (PackageController package in _currentSpawnedPackages)
                {
                    package.InitializePackageData(package.GetSO().PackageRandomPriceGenerator(), package.GetSO().PackageDurabilityLevel);
                    _packagePricesDict.Add(package.GetSO().GenerateUniqueID(), package.GetSO().GetGeneratedNumber());
                }
            }
            _visuals.ChangeCanvas(true);
            //Debug.Log("Tiene que llevar " + _currentSpawnedPackages.Count);
            _visuals.ChangeUi("Tiene que llevar " + _currentSpawnedPackages.Count);
        }

        public void RetrieveCurrentPackageData(PackageController package)
        {
            if (_packagePricesDict.TryGetValue(package.GetSO().PackageID, out var generatedPrice))
            {
                package.InitializePackageData(generatedPrice, package.GetSO().PackageDurabilityLevel); 
            }
          
        }

        public void CheckPackageConditions()
        {
            SumCurrentPackages();

            EconomyManager.Instance.AddMoney(_packagePriceSum);

            _visuals.ChangeUi("You've delivered all packages");
            _visuals.ChangeCanvas(false);
        }
        private void EvaluatePackageConditions()
        {            
            //condiciones--> si tenes todos suma el promedio de los 3. Si estan todos con la vida mayor a tanto, se suma tanto; plantear tres casos?
            //chequear estado de los paquetes y que dependiend de su vida, te de un porcenataje extra de dinero más uno de base
        }
        public void SumCurrentPackages()
        {
            foreach (PackageController package in _currentSpawnedPackages)
            {
                _packagePriceSum += package.GetPrice();
            }

            //injectar estos valores a una UI
        }

        //prevencion contemporanea
        private void OnDestroy()
        {
            CleanReferences();
        }

        private void CleanReferences()
        {
            if (_currentSpawnedPackages.Count > 0)
            {
                _currentSpawnedPackages.Clear();
            }
            _packagePricesDict.Clear();
        }
    }
}
