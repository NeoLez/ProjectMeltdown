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
    }

    public enum PackageRequestType
    {
        Amount,
        TypeOfPackages
    }
}
