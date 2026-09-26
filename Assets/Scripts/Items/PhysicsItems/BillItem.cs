using Root.Managers;
using UnityEngine;

namespace Root {
    [RequireComponent(typeof(Poolable))]
    public class BillItem : PhysicalItem {
        [SerializeField] private Renderer renderer; 
        public override void Interact() {
            EconomyManager.Instance.AddMoney(ItemSo.BillDenomination);
            MoneyFeedback.Instance.GrabbedBill(ItemSo);
            Wallet.Instance.AddBill(ItemSo);
            
            PoolManager.ReturnObjectToPool(GetComponent<Poolable>());
        }
        
        public BillItemSo ItemSo => (BillItemSo)ItemState.ItemSo;
    }
}