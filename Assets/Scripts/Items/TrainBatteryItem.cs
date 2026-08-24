using UnityEngine;

namespace Root {
    public class TrainBatteryItem : PhysicalItem {
        private static readonly int Insert = Animator.StringToHash("Insert");
        public ItemChargeState State => itemState as ItemChargeState;
        [SerializeField] private float visualsScale;
        [SerializeField] private Animator animator;

        public void AnimatorOn()
        {
            animator.SetBool(Insert, true);
        }
    }
}