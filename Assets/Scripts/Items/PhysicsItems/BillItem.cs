using Root.Managers;
using UnityEngine;

namespace Root {
    [RequireComponent(typeof(Poolable))]
    public class BillItem : PhysicalItem {
        [SerializeField] private Renderer renderer; 
        public override void Interact() {
            EconomyManager.Instance.AddMoney(ItemSo.BillDenomination);
            PoolManager.ReturnObjectToPool(GetComponent<Poolable>());
        }
        
        private BillItemSo ItemSo  => (BillItemSo)ItemState.ItemSo;
    }
}