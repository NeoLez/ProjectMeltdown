using UnityEngine;

namespace Root {
    public class BrakeFluidItem : PhysicalItem {
        private static readonly int Insert = Animator.StringToHash("Insert");
        public ItemChargeSo So => itemState as ItemChargeSo;
        
        [SerializeField] private Animator animator;
        public void AnimatorOn()
        {
            animator.SetBool(Insert, true);
        }
        
        public void Consume(float damage)
        {
            if (So.currentCharge <= 0) return;
            So.currentCharge -= damage;
        }

        public float GetRepairAmountLeft() => So.currentCharge;
        
        protected override bool IsStateTypeValid(ItemState state) {
            return state is ItemChargeSo;
        }
    }
}