using UnityEngine;

namespace Root
{
    public class PackagesDataContainer : MonoBehaviour
    {
        [SerializeField] PackageItemSo packageData;

        public PackageItemSo GetSO() => packageData;
    }
}
