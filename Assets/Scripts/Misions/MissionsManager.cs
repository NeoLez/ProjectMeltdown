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

        //private List<MissionObjectiveSO> missionsToDeliver = new();
        private MissionObjectiveSO missionsToDeliver;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }


        public void RegisterMission(MissionObjectiveSO activeMission, List<PackageItemState> packageTypes)
        {
            if (!_deliveryMissions.ContainsKey(activeMission.Id))
            {
                _deliveryMissions.Add(activeMission.Id, activeMission);
                RegisterPackage(activeMission, packageTypes);
                missionsToDeliver = activeMission;
                if (!_packagesToDeliver.ContainsKey(activeMission.Id))
                {
                    _packagesToDeliver.Add(activeMission.Id, new List<PackageItemState>());

                    foreach (var packageType in packageTypes)
                    {
                        _packagesToDeliver[activeMission.Id].Add(packageType);
                    }
                }
            }
        }

        private void RegisterPackage(MissionObjectiveSO activeMission, List<PackageItemState> spawnedPackages)
        {
            List<TypeOfPackage> spawnedTypes = new();
            for (int i = 0; i < spawnedPackages.Count; i++) {
                spawnedTypes.Add(spawnedPackages[i].typeOfPackage);
            }

            _activeMissionData[activeMission.Id] = new MissionData(activeMission.AmountOfPackages, spawnedTypes);
        }

        public void FinishMission(MissionObjectiveSO activeMission)
        {
            if (_deliveryMissions.ContainsKey(activeMission.Id))
            {
                RegisterUndepositedPackages();

                _deliveryMissions.Remove(activeMission.Id);
                _activeMissionData.Remove(activeMission.Id);

                missionsToDeliver = null;
            }
        }

        private void RegisterUndepositedPackages()
        {
            if (_packagesToDeliver.Count > 0)
            {
                foreach (var package in _packagesToDeliver)
                {
                    foreach (var item in package.Value)
                    {
                        item.canBeDelivered = false;

                        remaingPackages.Add(item);
                        //Debug.Log(item.ToString());
                    }
                }
            }
        }

        public void DeleteDepositedPackage(string activeMission, List<PackageItemState> stateToRemove)
        {
            if (_packagesToDeliver.TryGetValue(activeMission, out var packageList))
            {
                foreach (var item in stateToRemove)
                {
                    packageList.Remove(item);
                }
            }
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


        /* public List<MissionObjectiveSO> EnlistedMissions()
         {
             if (missionsToDeliver.Count == 0) return null;

             foreach (var item in missionsToDeliver)
             {
                 if (_deliveryMissions.ContainsKey(item.Id))
                 {
                     missionsToDeliver.Remove(item); //evitar duplicados
                 }
             }

             return missionsToDeliver;
         }*/

        public MissionObjectiveSO SingleMission()
        {
            if (missionsToDeliver == null) return null;
            
            return missionsToDeliver;
        }

        public bool AreMissionsActive()
        {
            return _deliveryMissions.Count > 0;
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
