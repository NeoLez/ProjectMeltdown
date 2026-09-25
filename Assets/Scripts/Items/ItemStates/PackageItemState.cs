using UnityEngine;

namespace Root {
    [System.Serializable]
    public class PackageItemState : ItemState {
        public int price;
        public float durability;
        public RenderTexture stampTexture;
        public TypeOfPackage typeOfPackage;
        public bool canBeDelivered = true;
        public PackageItemState(PackageItemSo itemSo) : base(itemSo) { }
        
        public override string ToString() {
            return $"MaxCharge: {price}, CurrentCharge: {durability}";
        }

        public override ItemState Clone() {
            var clone = new PackageItemState(ItemSo as PackageItemSo) {
                price = price,
                durability = durability,
                typeOfPackage = typeOfPackage,
                canBeDelivered = canBeDelivered
                //TODO: Should the texture also be cloned?
            };
            return clone;
        }
    }
}