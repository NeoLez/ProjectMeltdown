using UnityEngine;

namespace Root
{
    public class PackageClimateConditionsSO : ScriptableObject
    {
        [field: SerializeField] public PackageItemSo PackageSettings { get; private set; }
        [field: SerializeField] public ClimateConditions PackageAffectConditions { get; private set; }
        [field: SerializeField] public float DamageCooldown { get; private set; } //cambiar el nombre, esta horrible

    }
    public enum ClimateConditions
    {
        Humid,
        Hot,
        Cold
    }
}
