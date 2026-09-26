using System;
using System.Collections;
using System.Collections.Generic;
using Root.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

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

        private IEnumerator GeneratePackages(NPCInteraction perpetrator, Transform instancePivot, int amountToSpawn) {
            var startingNode = perpetrator.Section.Node;
            var destinationNode = FindDestinationNode(startingNode);
            
            Vector3 newPos = instancePivot.position;
            //TODO: Why make this an array of gameobjects? Maybe use ItemSo instead since all it's being used for is getting a default item state.
            GameObject[] availablePackages = perpetrator.Mission.AvailablePackages;
            for (int i = 0; i < amountToSpawn; i++)
            {
                int randomIndex = Random.Range(0, availablePackages.Length);
                var item = availablePackages[randomIndex].GetComponent<DeliveryPackageItem>();
                var state = (PackageItemState)item.State.Clone();
                state.Initialize(destinationNode);
                //Doing this so that the package gets initialized AFTER it's assigned it's valid state.
                GameObject prefab = item.ItemState.ItemSo.CreatePhysicalItem(state).gameObject;

                prefab.transform.parent = instancePivot.parent;
                prefab.transform.position = instancePivot.transform.position;

                DeliveryPackageItem currentPackage = prefab.GetComponent<DeliveryPackageItem>();
                _currentSpawnedPackages.Add(currentPackage);

                yield return new WaitForSeconds(fixedSpawnTime);

                newPos += Vector3.up * verticalOffset;
            }

            _visuals.ActivateNotification();
            _visuals.SetNewObjective(perpetrator.Mission);

            MissionsManager.Instance.RegisterMission(perpetrator.Mission, _currentSpawnedPackages); 
            deliveryMissions = perpetrator.Mission;
        }
        
        /// <summary>
        /// Finds the closest node to the starting one. If there are multiple at the same distance, it chooses one of those at random.
        /// </summary>
        /// <param name="startingNode"></param>
        private MapPointsGen.Node FindDestinationNode(MapPointsGen.Node startingNode) {
            var destinationNodes = startingNode.GetNodesThatMatch((node, _) => node.feature == MapPointsGen.Feature.STATION, 10);
            if (destinationNodes.Count == 0) {
                //TODO: What happens if no matching node is found? This should be done before even letting you accept a mission.
                return startingNode;
            }

            var potentialDestinations = new List<MapPointsGen.Node>();
            var minDepth = destinationNodes[0].Item1.dist;
            for (int i = 0; i < destinationNodes.Count; i++) {
                if (destinationNodes[i].Item1.dist <= minDepth)
                    potentialDestinations.Add(destinationNodes[i].Item1);
                else break;
            }

            if (potentialDestinations.Count == 1) return potentialDestinations[0];
            return potentialDestinations[Random.Range(0, potentialDestinations.Count)];
        }

        public int CheckPackageConditions(bool objectiveReached, List<PackageItemState> depositedPackages = null) {
            int finalSum = objectiveReached ? _packagePriceSum : GetAverageSumFromDeposited(depositedPackages); //TODO-Add more variants to the result
            
            UpdateFeedback();

            MissionsManager.Instance.FinishMission(deliveryMissions);
            return finalSum;
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
