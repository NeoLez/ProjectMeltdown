using UnityEngine;

namespace Root
{
    public class PackageClimateConditionsSo : ScriptableObject
    {
        [field: SerializeField] public PackageItemGenerationDataSo PackageSettings { get; private set; }
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
