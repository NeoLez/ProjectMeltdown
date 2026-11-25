using UnityEngine.InputSystem;

namespace Root.Handheld
{
    public class HealingHandheld : HandheldItem
    {
        public override void Use(InputAction.CallbackContext ctx)
        {
            base.Use(ctx);
            var itemSo = (HealingItemSo)state.ItemSo;
            var health = GameManager.Player.GetComponent<HealthControl>();
            if (!health.CanCure()) return;
            health.Heal(itemSo.HealAmount);
            health.Cure(itemSo.InfectionHealAmount);
            GameManager.ItemHolder.ForceClearHeldItem();
        }
    }
}