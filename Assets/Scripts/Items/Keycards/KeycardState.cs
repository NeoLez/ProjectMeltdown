using UnityEngine;

namespace Root
{
    [System.Serializable]
    public class KeycardState : ItemState
    {
        public enum CardColor { Red, White, Blue }

        public CardColor color;
        public int boundCode = -1;

        public KeycardState(KeycardSo itemSo) : base(itemSo)
        {
            color = itemSo.Color;
        }

        public override string ToString()
        {
            return $"{color} Keycard (bound: {boundCode})";
        }

        public override ItemState Clone()
        {
            var clone = new KeycardState(ItemSo as KeycardSo)
            {
                color = color,
                boundCode = boundCode
            };
            return clone;
        }
    }
}