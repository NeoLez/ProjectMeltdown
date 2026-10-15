using NUnit.Framework;
using Root.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class NpcToll : MonoBehaviour
    {
        [SerializeField] private int moneyToRequest;
        [SerializeField] private ItemGroup packagesToRequest;
        [SerializeField] private PackageRequestType requestType;

        public List<PackageItemGenerationDataSo> Packages = new();
        public int AmountToRequest => moneyToRequest;
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
