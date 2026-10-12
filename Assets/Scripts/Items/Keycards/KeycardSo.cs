using UnityEngine;

namespace Root
{
    [CreateAssetMenu(menuName = "Items/Keycard")]
    public class KeycardSo : ItemSo
    {
        [field: SerializeField] public CardColor Color { get; private set; }

        public override ItemState CreateUninitializedState()
        {
            return new KeycardState(this);
        }
        
        public enum CardColor { Red, White, Blue }
    }
}