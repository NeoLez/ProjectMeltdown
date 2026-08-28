using UnityEngine;

namespace Root {
    public class BrakeFluidItem : PhysicalItem {
        private static readonly int Insert = Animator.StringToHash("Insert");
        public ItemChargeState State => itemState as ItemChargeState;
        
        [SerializeField] private Animator animator;
        public void AnimatorOn()
        {
            animator.SetBool(Insert, true);
        }
        
        public void Consume(float damage)
        {
            if (State.currentCharge <= 0) return;
            State.currentCharge -= damage;
        }

        public float GetRepairAmountLeft() => State.currentCharge;
        
        protected override bool IsStateTypeValid(ItemState state) {
            return state is ItemChargeState;
        }
    }
}