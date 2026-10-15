using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class NpcToll : MonoBehaviour
    {
        [SerializeField] private int moneyToRequest;
        [SerializeField] private PackageRequestType requestType;

        [ShowIf("ShowPackageAmount")]
        [SerializeField] private ItemGroup packagesToRequest;
        [ShowIf("ShowPackageTypes")]
        public List<PackageItemGenerationDataSo> Packages = new();

        [SerializeField] private NPCInteraction interaction;
        [SerializeField] private TollDoor barrier;

        public int AmountToRequest => moneyToRequest;
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
                    item2.OnEffectExcuted += barrier.OpenTollBarrier;
                }
            }
        }

        private void OnDestroy()
        {
            foreach (var item in interaction.Dialogue.DialogueChoices)
            {
                List<DialogueEffectSO> effectsToSuscribe = item.effects;

                if (item.effects == null) break;
                foreach (var item2 in effectsToSuscribe)
                {
                    item2.OnEffectExcuted -= barrier.OpenTollBarrier;
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
