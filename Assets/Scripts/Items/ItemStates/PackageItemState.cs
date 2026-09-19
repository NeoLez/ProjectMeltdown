namespace Root {
    [System.Serializable]
    public class PackageItemState : ItemState {
        public int price;
        public float durability;
        
        public PackageItemState(PackageItemSo itemSo) : base(itemSo) { }
        
        public override string ToString() {
            return $"MaxCharge: {price}, CurrentCharge: {durability}";
        }

        public override ItemState Clone() {
            var clone = new PackageItemState(ItemSo as PackageItemSo) {
                price = price,
                durability = durability
            };
            return clone;
        }
    }
}