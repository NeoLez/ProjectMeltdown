using UnityEngine;

namespace Root.Managers {
    public class ItemPoolable : Poolable {
        public override void TurnOff() {
            base.TurnOff();
            var item = GetComponent<PhysicalItem>();
            item.VisualOnly(false);
            item.itemState = null;
            var visualContainer = GetComponent<VisualContainer>();
            visualContainer.goal = null;
            var rigidbody =  GetComponent<Rigidbody>();
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.rotation = Quaternion.identity;
            rigidbody.transform.rotation = Quaternion.identity;
            if(item.TryGetComponent(out StoreItemDisplay storeDisplay))
            {
                storeDisplay._storeHand = null;
                storeDisplay._purchased = true;
            }
        }
    }
}