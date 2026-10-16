using UnityEngine.InputSystem;

namespace Root.Handheld {
    public class BillHandheld : HandheldItem {
        public override void Use(InputAction.CallbackContext ctx) {
            base.Use(ctx);
            var itemSo = (BillItemSo)state.ItemSo;
            EconomyManager.Instance.AddMoney(itemSo.BillDenomination);
            MoneyFeedback.Instance.GrabbedBill(itemSo);
            Wallet.Instance.AddBill(itemSo);
            GameManager.ItemHolder.ForceClearHeldItem();
        }
    }
}