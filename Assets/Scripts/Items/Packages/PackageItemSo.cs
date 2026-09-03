using UnityEngine;

namespace Root
{
    [CreateAssetMenu(menuName = "Items/Package Item", fileName = "Package")]
    public class PackageItemSo : ScriptableObject
    {
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public TypeOfPackage TypeOfPackage { get; private set; }

        [Header("Price Settings")]
        [field: SerializeField] public int MaxPriceValue { get; private set; }
        [Min(0)]
        [field: SerializeField] public int MinPriceValue { get; private set; }

        [Header("Duraility Settings")]
        [field: SerializeField] public float PackageDurabilityLevel { get; private set; }
        [field: SerializeField] public float MaxDurability { get; private set; }

        public string PackageID => _packageID;

        private string _packageID;
        private int _generatedValue;
        public int PackageRandomPriceGenerator()
        {
            return _generatedValue = Random.Range(MinPriceValue, MaxPriceValue);
        }

        public int GetGeneratedNumber()
        {
            return _generatedValue;
        }

        const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789";
        public string GenerateUniqueID()
        {
            int charAmount = Random.Range(0, 8);
            for (int i = 0; i < charAmount; i++)
            {
                _packageID += glyphs[Random.Range(0, glyphs.Length)];
            }

            return _packageID;
        }

    }

    public enum TypeOfPackage
    {
        Food,
        Supply
    }

}
