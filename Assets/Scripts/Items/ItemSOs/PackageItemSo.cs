using UnityEngine;

namespace Root {
    [CreateAssetMenu(menuName = "Items/Package")]
    public class PackageItemSo : ItemSo {
        public override ItemState CreateState() {
            return new PackageItemState(this);
        }
    }
}