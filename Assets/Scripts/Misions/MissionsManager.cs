using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Root
{
    public class MissionsManager : MonoBehaviour
    {
        public static MissionsManager Instance;

        private Dictionary<string, MissionObjectiveSO> _deliveryMissions = new(); //TODO-Porbably make it more specific
        private Dictionary<string, MissionData> _activeMissionData = new();

        private Dictionary<string, List<PackageItemState>> _packagesToDeliver = new(); //para tener un registro de los depositados

        private List<PackageItemState> remaingPackages = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }


        public void RegisterMission(MissionObjectiveSO activeMission, List<DeliveryPackageItem> packageTypes)
        {
            if (!_deliveryMissions.ContainsKey(activeMission.Id))
            {
                _deliveryMissions.Add(activeMission.Id, activeMission);
                RegisterPackage(activeMission, packageTypes);

             /*   foreach (var packageType in packageTypes)
                {
                    if(!_packagesToDeliver.ContainsKey(activeMission.Id))
                        _packagesToDeliver.Add(activeMission.Id, packageType.State); //fix this
                    
                }*/
            }
        }

        private void RegisterPackage(MissionObjectiveSO activeMission, List<DeliveryPackageItem> spawnedPackages)
        {
            List<TypeOfPackage> spawnedTypes = new();
            for (int i = 0; i < spawnedPackages.Count; i++)
            {
                spawnedTypes.Add(spawnedPackages[i].GetTypeOfPackage());
            }

            _activeMissionData[activeMission.Id] = new MissionData(activeMission.AmountOfPackages, spawnedTypes);
        }

        public void FinishMission(MissionObjectiveSO activeMission)
        {
            if (_deliveryMissions.ContainsKey(activeMission.Id))
            {
                Algo();
                _deliveryMissions.Remove(activeMission.Id);
                UnregisterPackages(activeMission.Id);
            }
        }

        private void Algo()
        {
           /* if (_packagesToDeliver.Count > 0)
            {
                foreach (var item in _packagesToDeliver)
                {
                    item.Value.canBeDelivered = false;

                    remaingPackages.Add(item.Value);
                    Debug.Log(item.ToString());
                }
            }*/
        }
        private void UnregisterPackages(string currentMissionData)
        {
            _activeMissionData.Remove(currentMissionData);
            //Debug.Log(deliveryMissions.Count);
            //Debug.Log(activeMissionData.Count);
        }
        public void DeleteDepositedPackage(string activeMission, PackageItemState state) //el id aca deja de existir cuidado
        {
           // _packagesToDeliver.Remove(activeMission, out state);
        }

        public bool VerifyDeliveryConditions(string missionId, int currentDepositedAmount, List<TypeOfPackage> packages)
        {
            if (!_activeMissionData.TryGetValue(missionId, out MissionData data))
                return false;

            if (data.packages == null || packages == null)
                return false;

            bool arePackagesEqual = data.packages.Count == packages.Count && !data.packages.Except(packages).Any();

            return data.amount == currentDepositedAmount && arePackagesEqual;
        }

        private class MissionData
        {
            public int amount;
            public List<TypeOfPackage> packages = new();

            public MissionData(int amount, List<TypeOfPackage> package)
            {
                this.amount = amount;
                this.packages = new List<TypeOfPackage>(package);
            }
        }
    }
}
