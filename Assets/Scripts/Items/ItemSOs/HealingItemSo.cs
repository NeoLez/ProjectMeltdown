using UnityEngine;

namespace Root
{
    [CreateAssetMenu(menuName = "Items/Healing")]
    public class HealingItemSo : ItemSo
    {
        [field: SerializeField] public float HealAmount { get; private set; }
        [field: SerializeField] public float InfectionHealAmount { get; private set; }
        [field: SerializeField] public float UseTime { get; private set; }
    }
}