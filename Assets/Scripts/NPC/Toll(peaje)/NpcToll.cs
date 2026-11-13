using System;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class NpcToll : MonoBehaviour
    {
        [SerializeField] private Animator tollAnimator;
        [SerializeField] private float moneyToRequest;
        [SerializeField] private PackageRequestType requestType;

        [ShowIf("ShowPackageAmount")]
        [SerializeField] private ItemGroup packagesToRequest;
        [ShowIf("ShowPackageTypes")]
        public List<PackageItemGenerationDataSo> Packages = new();

        [SerializeField] private NPCInteraction interaction;
        [SerializeField] private TollDoor barrier;
        [SerializeField] private TrainPathWaypoint pathWaypointToUnlock;
        [SerializeField] private float unlockPathDelay;

        public float AmountToRequest => moneyToRequest;
        public ItemGroup PackagesToRequest => packagesToRequest;

        public PackageRequestType RequestType => requestType;

        public Animator animator;

        public bool ShowPackageAmount()
        {
            return requestType == PackageRequestType.Amount;
        }

        public bool ShowPackageTypes()
        {
            return requestType == PackageRequestType.TypeOfPackages;
        }

        private void Start()
        {
            foreach (var item in interaction.Dialogue.DialogueChoices)
            {
                List<DialogueEffectSO> effectsToSuscribe = item.effects;

                if (item.effects == null) break;
                foreach (var item2 in effectsToSuscribe)
                {
                    item2.OnEffectExcuted += SubscribeFunction;
                }
            }
        }

        private void SubscribeFunction()
        {
            barrier.OpenTollBarrier();
            Invoke(nameof(UnlockPath), unlockPathDelay);
            tollAnimator.Play("PeajeGood");
        }

        private void UnlockPath()
        {
            pathWaypointToUnlock.stop = false;
        }

        private void OnDestroy()
        {
            foreach (var item in interaction.Dialogue.DialogueChoices)
            {
                List<DialogueEffectSO> effectsToSuscribe = item.effects;

                if (item.effects == null) break;
                foreach (var item2 in effectsToSuscribe)
                {
                    item2.OnEffectExcuted -= SubscribeFunction;
                }
            }
        }
    }

    public enum PackageRequestType
    {
        Amount,
        TypeOfPackages
    }
}
