using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class PackagesSystemController : MonoBehaviour
    {
        public static PackagesSystemController Instance;

        //[SerializeField] private GameObject[] availablePackages;
        [SerializeField] private PackageStampGenerator packageStampGenerator;

        [SerializeField] private PackageObjectivesUI _visuals;
        [SerializeField] private MapGeneration mapGeneration;
        [SerializeField] private float fixedSpawnTime;
        [SerializeField] private float verticalOffset;

        private int _packagePriceSum;
        private List<DeliveryPackageItem> _currentSpawnedPackages = new();

        private Coroutine _packageGenerationRoutine;
        private MissionObjectiveSO deliveryMissions; //TODO-Improve this and expand
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void EnablePackageGeneration(NPCInteraction perpetrator, Transform instancePivot, int amount)
        {
            if (_packageGenerationRoutine != null)
            {
                StopCoroutine(_packageGenerationRoutine);
                _packageGenerationRoutine = null;
            }

            _packageGenerationRoutine = StartCoroutine(GeneratePackages(perpetrator, instancePivot, amount));
        }

        private IEnumerator GeneratePackages(NPCInteraction perpetrator, Transform instancePivot, int amountToSpawn)
        {
            Vector3 newPos = instancePivot.position;
            GameObject[] availablePackages = perpetrator.Mission.AvailablePackages;
            for (int i = 0; i < amountToSpawn; i++)
            {
                int randomIndex = Random.Range(0, availablePackages.Length);
                PhysicalItem item = availablePackages[randomIndex].GetComponent<PhysicalItem>();
                GameObject prefab = item.ItemState.ItemSo.CreatePhysicalItem().gameObject;

                prefab.transform.parent = instancePivot.parent;
                prefab.transform.position = instancePivot.transform.position;

                DeliveryPackageItem currentPackage = prefab.GetComponent<DeliveryPackageItem>();
                _currentSpawnedPackages.Add(currentPackage);
                InitializePackges(currentPackage);

                yield return new WaitForSeconds(fixedSpawnTime);

                newPos += Vector3.up * verticalOffset;
            }

            _visuals.ActivateNotification();
            _visuals.SetNewObjective(perpetrator.Mission);

            MissionsManager.Instance.RegisterMission(perpetrator.Mission, _currentSpawnedPackages); 
            deliveryMissions = perpetrator.Mission;
        }

        private void InitializePackges(DeliveryPackageItem package)
        {
            if (_currentSpawnedPackages.Count > 0)
            {
                package.InitializePackageData();
            }
        }

        public void CheckPackageConditions(bool objectiveReached, List<PackageItemState> depositedPackages = null)
        {
            if(objectiveReached)
            {
                EconomyManager.Instance.AddMoney(_packagePriceSum); //TODO-Add more variants to the result
            }
            else
            {
               int finalAmount = GetAverageSumFromDeposited(depositedPackages);
               EconomyManager.Instance.AddMoney(finalAmount);
            }
            UpdateFeedback();

            MissionsManager.Instance.FinishMission(deliveryMissions);
        }

        private void UpdateFeedback()
        {
            _visuals.ClearCurrentObjective();
            NotificationManager.Instance.ShowNotification("+ $" + _packagePriceSum);

            _packagePriceSum = 0;
        }

        public void SumCurrentDeposited(int amount)
        {
            _packagePriceSum = amount;
        }

        public int GetAverageSumFromDeposited(List<PackageItemState> depositedPackages)
        {
            int depositedCount = 0;
            int totalDepositedAmount = 0;

            for (int i = 0; i < depositedPackages.Count; i++)
            {
                int packagePrice = depositedPackages[i].price;

                _packagePriceSum -= packagePrice;

                totalDepositedAmount += packagePrice;
                depositedCount++;

                if (_packagePriceSum <= 0)
                {
                    float averageRewardPerPackage = totalDepositedAmount / depositedCount;

                    _packagePriceSum = (int)averageRewardPerPackage / _currentSpawnedPackages.Count;

                    /*Debug.Log($"Paquetes procesados: {depositedCount}");
                    Debug.Log($"Monto total depositado: {totalDepositedAmount}");
                    Debug.Log($"Promedio a entregar: {averageRewardPerPackage}");*/
                    break;
                }
            }
            return _packagePriceSum;
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
