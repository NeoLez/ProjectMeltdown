using UnityEngine;

namespace Root
{
    [CreateAssetMenu(menuName = "Items/Package Item", fileName = "Package")]
    public class PackageItemSo : ScriptableObject
    {
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public TypeOfPackage TypeOfPackage { get; private set; }

        public float PackageDurabilityLevel { get; private set; }

        [Header("Price Settings")]
        [field: SerializeField] public int MaxPriceValue { get; private set; }
        [Min(0)]
        [field: SerializeField] public int MinPriceValue { get; private set; }

        [Header("Duraility Settings")]
        [field: SerializeField] public float MaxDurability { get; private set; }

        [SerializeField] string packageID;
        public string PackageID => packageID;

        private int generatedValue;
        public int PackageRandomPriceGenerator()
        {
            return generatedValue = Random.Range(MinPriceValue, MaxPriceValue);
        }

        public int GetGeneratedNumber()
        {
            return generatedValue;
        }

    }

    public enum TypeOfPackage
    {
        Food,
        Supply
        //me qude sin ideas
    }

}
