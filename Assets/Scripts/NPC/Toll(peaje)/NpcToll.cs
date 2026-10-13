using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class NpcToll : MonoBehaviour
    {
        [SerializeField] private int amountToRequest;
        [SerializeField] private ItemGroup packagesToRequest;
        [SerializeField] private PackageRequestType requestType;

        public List<PackageItemGenerationDataSo> Packages = new();
        public int AmountToRequest => amountToRequest;
        public ItemGroup PackagesToRequest => packagesToRequest;

        public PackageRequestType RequestType => requestType;

        public Animator animator;

    }

    public enum PackageRequestType
    {
        Amount,
        TypeOfPackages
    }
}
