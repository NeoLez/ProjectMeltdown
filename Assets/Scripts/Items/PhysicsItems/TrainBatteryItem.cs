using UnityEngine;

namespace Root {
    public class TrainBatteryItem : PhysicalItem {
        private static readonly int Insert = Animator.StringToHash("Insert");
        public ItemChargeState State => ItemState as ItemChargeState;
        [SerializeField] private Animator animator;

        public void AnimatorOn()
        {
            animator.SetBool(Insert, true);
        }

        protected override bool IsStateTypeValid(ItemState state) {
            return state is ItemChargeState;
        }
    }
}