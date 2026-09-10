namespace Root {
    [System.Serializable]
    public class ItemConsumableState : ItemState {
        public int usesLeft;

        public ItemConsumableState(ItemSo itemSo) : base(itemSo) {
        }

        public override string ToString() {
            return usesLeft.ToString();
        }

        public override ItemState Clone() {
            var clone = new ItemConsumableState(ItemSo);
            clone.usesLeft = usesLeft;
            return clone;
        }
    }
}