namespace Root
{
    [System.Serializable]
    public class KeycardState : ItemState
    {
        public int boundCode = -1;
        public KeycardSo.CardColor Color => ((KeycardSo)ItemSo).Color;

        public KeycardState(KeycardSo itemSo) : base(itemSo) { }
        

        public override string ToString() {
            return $"{((KeycardSo)ItemSo).Color} Keycard (bound: {boundCode})";
        }
    }
}