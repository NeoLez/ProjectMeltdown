using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Root
{
    public class MissionsManager : MonoBehaviour
    {
        public static MissionsManager Instance;

        private Dictionary<string, MissionObjectiveSO> deliveryMissions = new(); //TODO-Porbably make it more specific
        private Dictionary<string, MissionData> activeMissionData = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

        }

        public void RegisterMission(MissionObjectiveSO activeMission, List<DeliveryPackageItem> packageTypes)
        {
            if (!deliveryMissions.ContainsKey(activeMission.Id))
            {
                deliveryMissions.Add(activeMission.Id, activeMission);
                RegisterPackage(activeMission, packageTypes);
            }
        }

        private void RegisterPackage(MissionObjectiveSO activeMission, List<DeliveryPackageItem> spawnedPackages)
        {
            List<TypeOfPackage> spawnedTypes = new();
            for (int i = 0; i < spawnedPackages.Count; i++)
            {
                spawnedTypes.Add(spawnedPackages[i].GetTypeOfPackage());
            }

            activeMissionData[activeMission.Id] = new MissionData(activeMission.AmountOfPackages, spawnedTypes);
        }

        public void FinishMission(MissionObjectiveSO activeMission)
        {
            if (deliveryMissions.ContainsKey(activeMission.Id))
            {
                deliveryMissions.Remove(activeMission.Id);
                UnregisterPackages(activeMission.Id);
            }
        }
        private void UnregisterPackages(string currentMissionData)
        {
            activeMissionData.Remove(currentMissionData);
            //Debug.Log(deliveryMissions.Count);
            //Debug.Log(activeMissionData.Count);
        }

        public bool VerifyDeliveryConditions(string missionId, int currentDepositedAmount, List<TypeOfPackage> packages)
        {
            if (!activeMissionData.TryGetValue(missionId, out MissionData data))
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
