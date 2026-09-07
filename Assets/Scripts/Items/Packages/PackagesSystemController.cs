using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class PackagesSystemController : MonoBehaviour
    {
        public static PackagesSystemController Instance;

        private Dictionary<string, int> _packagePricesDict = new();
        private int _packagePriceSum;
        private List<DeliveryPackageItem> _currentSpawnedPackages = new();

        [SerializeField] private PackageObjectivesUI _visuals;
        [SerializeField] private MapGeneration mapGeneration;
        [SerializeField] private float fixedSpawnTime;
        [SerializeField] private float verticalOffset;

        public Action OnDeliveryStationReached;
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
        }

        private void Update()
        {
            GetNextStationToDeliver();
        }

        public void EnablePackageGeneration(Transform instancePivot, GameObject[] packagesToDeliver)
        {
            StartCoroutine(GeneratePackages(instancePivot, packagesToDeliver));
        }

        IEnumerator GeneratePackages(Transform instancePivot, GameObject[] packagesToDeliver)
        {
            Vector3 newPos = instancePivot.position;
            for (int i = 0; i < packagesToDeliver.Length; i++)
            {
                GameObject prefab = Instantiate(packagesToDeliver[i]);
                prefab.transform.position = instancePivot.transform.position;

                DeliveryPackageItem currentPackage = prefab.GetComponent<DeliveryPackageItem>();
                _currentSpawnedPackages.Add(currentPackage);

                yield return new WaitForSeconds(fixedSpawnTime);

                newPos += Vector3.up * verticalOffset;
            }

            PackagesCheck();
        }

        public void PackagesCheck()
        {
            if (_currentSpawnedPackages.Count > 0)
            {
                foreach (DeliveryPackageItem package in _currentSpawnedPackages)
                {
                    package.InitializePackageData(package.GetSO().PackageRandomPriceGenerator(), package.GetSO().PackageDurabilityLevel);
                    _packagePricesDict.Add(package.GetSO().GenerateUniqueID(), package.GetSO().GetGeneratedNumber());
                }
            }
            _visuals.ChangeCanvas(true);
            _visuals.ChangeUi("Tenes que entregar " + _currentSpawnedPackages.Count + " paquetes a la proxima estacion");
        }

        public void RetrieveCurrentPackageData(DeliveryPackageItem package)
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

            _visuals.ChangeUi("Entregaste todos los paquetes");
            NotificationManager.Instance.ShowNotification("+ $" + _packagePriceSum);
        }

        public void SumCurrentPackages()
        {
            foreach (DeliveryPackageItem package in _currentSpawnedPackages)
            {
                _packagePriceSum += package.GetPrice();
            }
        }

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

        //el mismo controller se encarga de chequear en donde instanciar las zonas de delivery de paquetes segun x condiciones de cada paquete
        public void GetNextStationToDeliver()
        {
            if(mapGeneration.IsTrainInStation())
            {
                OnDeliveryStationReached?.Invoke(); //aca cuando llegue a la estacion, si mi info coincide, activo a la zona de delivery de todos lo que hipoteticamente tenga activos jajaj
                return;
            }
        }
    }
}
