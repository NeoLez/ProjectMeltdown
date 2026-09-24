using UnityEngine;

namespace Root
{
    [CreateAssetMenu(menuName = "Items/Package Item", fileName = "Package")]
    public class PackageItemGenerationDataSo : ScriptableObject
    {
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public TypeOfPackage TypeOfPackage { get; private set; }

        [Header("Price Settings")]
        [field: SerializeField] public int MaxPriceValue { get; private set; }
        [Min(0)]
        [field: SerializeField] public int MinPriceValue { get; private set; }

        [Header("Durability Settings")]
        [field: SerializeField] public int MaxDurability { get; private set; }

        [Min(0)]
        [field: SerializeField] public int MinDurability { get; private set; }

    }
    public enum TypeOfPackage
    {
        None,
        Food,
        Supply
    }

}
