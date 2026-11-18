using UnityEngine;

namespace Root
{
    [CreateAssetMenu(menuName = "Items/Weapons/Melee")]
    public class MeleeWeaponItemSo : ItemSo
    {
        [field: SerializeField] public float Range { get; private set; }
        [field: SerializeField] public float Cooldown { get; private set; }
        [field: SerializeField] public float HitTime { get; private set; }
        [field: SerializeField] public int Damage { get; private set; }
    }
}