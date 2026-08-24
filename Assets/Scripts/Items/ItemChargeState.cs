using UnityEngine.Serialization;

namespace Root {
    [System.Serializable]
    public class ItemChargeState : ItemState {
        [FormerlySerializedAs("MaxCharge")] public float maxCharge;
        [FormerlySerializedAs("CurrentCharge")] public float currentCharge;

        public ItemChargeState(ItemSo itemSo) : base(itemSo) { }
        public override string ToString() {
            return $"MaxCharge: {maxCharge}, CurrentCharge: {currentCharge}";
        }
    }
}