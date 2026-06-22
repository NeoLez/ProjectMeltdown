using UnityEngine;

namespace Root
{
    public class BrakeFluid : MonoBehaviour
    {
        public float repairAmount;
        [SerializeField] Animator _animator;
        public void AnimatorOn()
        {
            Debug.Log("startAnimationFluid");
            _animator.SetBool("Insert", true);
        }
    }
}