using UnityEngine;

namespace Root
{
    [CreateAssetMenu(menuName = "Items/Package Item", fileName = "Package")]
    public class PackageItemSo : ScriptableObject
    {
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public ClimateConditions PackageAffectConditions { get; private set; }
        [field: SerializeField] public float AmountOfLife { get; private set; }
        [field: SerializeField] public float DamageMultiplier { get; private set; }
        [field: SerializeField] public float TimeVariable { get; private set; } //cambiar el nombre, esta horrible
        [field: SerializeField] public int PackageValue { get; private set; }
    }

    public enum ClimateConditions
    {
        Humid,
        Hot,
        Cold
    }
}
