using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class PackagesSystemController : MonoBehaviour
    {
        public static PackagesSystemController Instance;

        [SerializeField] private GameObject[] availablePackages;
        [SerializeField] private PackageStampGenerator packageStampGenerator;

        [SerializeField] private PackageObjectivesUI _visuals;
        [SerializeField] private MapGeneration mapGeneration;
        [SerializeField] private float fixedSpawnTime;
        [SerializeField] private float verticalOffset;

        private int _packagePriceSum;
        private List<DeliveryPackageItem> _currentSpawnedPackages = new();

        public Action OnDeliveryStationReached;

        private PlayerInputActions _input;
        private Coroutine _packageGenerationRoutine;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Update()
        {
            GetNextStationToDeliver();
        }

        public void EnablePackageGeneration(NPCInteraction perpetrator, Transform instancePivot, int amount)
        {
            if(_packageGenerationRoutine!=null)
            {
                StopCoroutine(_packageGenerationRoutine);
                _packageGenerationRoutine = null;
            }

            _packageGenerationRoutine = StartCoroutine(GeneratePackages(perpetrator, instancePivot, amount));

            packageStampGenerator.EnableCanvas(true);
        }

        private IEnumerator GeneratePackages(NPCInteraction perpetrator, Transform instancePivot, int amountToSpawn)
        {
            Vector3 newPos = instancePivot.position;
            for (int i = 0; i < amountToSpawn; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, availablePackages.Length);
                PhysicalItem item = availablePackages[randomIndex].GetComponent<PhysicalItem>();
                GameObject prefab = item.ItemState.ItemSo.CreatePhysicalItem().gameObject;

                prefab.transform.parent = instancePivot.parent;
                prefab.transform.position = instancePivot.transform.position;

                DeliveryPackageItem currentPackage = prefab.GetComponent<DeliveryPackageItem>();
                _currentSpawnedPackages.Add(currentPackage);

                yield return new WaitForSeconds(fixedSpawnTime);

                newPos += Vector3.up * verticalOffset;

                InitializePackges(currentPackage);
                
                yield return new WaitForSeconds(0.1f);
            }

            packageStampGenerator.EnableCanvas(false);

            _visuals.ActivateNotification();
            _visuals.SetNewObjective(perpetrator.Mission);

            //MissionsManager.Instance.RegisterMission(perpetrator.Mission);
        }

        private void InitializePackges(DeliveryPackageItem package)
        {
            if (_currentSpawnedPackages.Count > 0)
            {
                package.InitializePackageData();
            }
        }

        public void CheckPackageConditions()
        {
            EconomyManager.Instance.AddMoney(_packagePriceSum);

            _visuals.ClearCurrentObjective();
            NotificationManager.Instance.ShowNotification("+ $" + _packagePriceSum);

            _packagePriceSum = 0;
        }

        public void SumCurrentDeposited(int amount)
        {
            _packagePriceSum = amount;
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
        private void OnDestroy()
        {
            if (_currentSpawnedPackages.Count > 0)
            {
                _currentSpawnedPackages.Clear();
            }

            if (_packageGenerationRoutine != null)
            {
                StopCoroutine(_packageGenerationRoutine);
                _packageGenerationRoutine = null;
            }
        }
    }
}
