using UnityEngine;

namespace Root
{
    public class BrakeFluid : MonoBehaviour
    {
        public float repairAmount;
        public float repairAmountLeft;
        [SerializeField] Animator _animator;

        private void Start()
        {
            repairAmountLeft = repairAmount;
        }
        public void AnimatorOn()
        {
            Debug.Log("startAnimationFluid");
            _animator.SetBool("Insert", true);
        }

        public void Consume(float damage)
        {
            if (repairAmountLeft <= 0) return;
            repairAmountLeft -= damage;
        }
    }
}