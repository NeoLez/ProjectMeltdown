using UnityEngine;

namespace Root
{
    [CreateAssetMenu(menuName = "Items/Keycard")]
    public class KeycardSo : ItemSo
    {
        [field: SerializeField] public CardColor Color { get; private set; }
        
        public enum CardColor { Red, White, Blue }
    }
}