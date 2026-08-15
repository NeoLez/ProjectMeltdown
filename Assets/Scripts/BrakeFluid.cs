using System.ComponentModel;
using UnityEngine;

namespace Root
{
    public class BrakeFluid : MonoBehaviour
    {
        [Min(0)]
        public float RepairAmount;

        [SerializeField] Animator _animator;

        private float _repairAmountLeft;
        private void Start()
        {
            _repairAmountLeft = RepairAmount;
        }
        public void AnimatorOn()
        {
            Debug.Log("startAnimationFluid");
            _animator.SetBool("Insert", true);
        }

        public void Consume(float damage)
        {
            if (_repairAmountLeft <= 0) return;
            _repairAmountLeft -= damage;
        }

        public float GetRepairAmountLeft() => _repairAmountLeft;
        
    }
}