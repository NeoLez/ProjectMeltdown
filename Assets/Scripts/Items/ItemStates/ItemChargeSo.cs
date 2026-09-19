namespace Root {
    [System.Serializable]
    public class ItemChargeSo : ItemState {
        public float maxCharge;
        public float currentCharge;

        public ItemChargeSo(ItemChargeStateSo itemSo) : base(itemSo) { }
        public override string ToString() {
            return $"MaxCharge: {maxCharge}, CurrentCharge: {currentCharge}";
        }

        public override ItemState Clone() {
            var clone = new ItemChargeSo(ItemSo as ItemChargeStateSo) {
                maxCharge = maxCharge,
                currentCharge = currentCharge
            };
            return clone;
        }
    }
}