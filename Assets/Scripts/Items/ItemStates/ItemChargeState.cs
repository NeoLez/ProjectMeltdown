namespace Root {
    [System.Serializable]
    public class ItemChargeState : ItemState {
        public float maxCharge;
        public float currentCharge;

        public ItemChargeState(ItemChargeStateSo itemSo) : base(itemSo) { }
        public override string ToString() {
            return $"MaxCharge: {maxCharge}, CurrentCharge: {currentCharge}";
        }
    }
}