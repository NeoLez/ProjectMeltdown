using UnityEngine;

namespace Root
{
    [CreateAssetMenu(menuName = "Items/Keycard")]
    public class KeycardSo : ItemSo
    {
        [field: SerializeField] public KeycardState.CardColor Color { get; private set; }

        public override ItemState CreateState()
        {
            return new KeycardState(this);
        }
    }
}